using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Formula.SimpleRepo;

public abstract class RepositoryBase<TModel, TConstraintsModel>
    : ReadOnlyRepositoryBase<TModel, TConstraintsModel>, IRepository<TModel>
    where TModel : class
    where TConstraintsModel : new()
{
    public RepositoryBase(IConfiguration config) : base(config)
    {
    }

    protected BasicCRUDBase<TModel, TConstraintsModel> _basicCRUD = null;

    public new IBasicCRUD<TModel> Basic
    {
        get
        {
            if (_basicCRUD == null)
            {
                _basicCRUD = new BasicCRUD<TModel, TConstraintsModel>(_config, (query) => LogQuery(query));
            }
            return _basicCRUD;
        }
    }

    public new IReadOnlyRepository<TModel> ApplyScopedConstraints()
    {
        base.ApplyScopedConstraints();
        return this;
    }

    public new IRepository<TModel> RemoveScopedConstraints()
    {
        base.RemoveScopedConstraints();
        return this;
    }

    /// <summary>
    /// Produce a list of constraints based on the populated fields of the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected QueryFacts Inspect(TModel entity)
    {
        var facts = new QueryFacts();

        facts.IdFields = GetPopulatedIdFields(entity);
        facts.IdConstraints = GetConstraints(facts.IdFields);

        // To allow for scoped constraints in the outside repository to be applied, we need to supply all
        // possible constraints for any decision making that may be done in the implemented repository
        facts.ScopedConstraints = ScopedConstraints(facts.IdConstraints);

        facts.AllConstraints = MergeConstraints(facts.IdConstraints, facts.ScopedConstraints);

        // But the only "predicate" we put on our update statement
        // is based on the scoped constraints and the id fields
        // The id fields will be added in the actual Basic.UpdateAsync call
        facts.ScopedBindings = ConstraintsToBindable(facts.ScopedConstraints);

        // We need to get a list of parameters representing the entity values
        var obj = JObject.FromObject(entity);
        
        // Convert obj to a dictionary of key/value pairs
        var entityValueParams = obj.Properties()
            .Where(x => x.Value.Type != JTokenType.Null)
            .ToDictionary(x => x.Name, x => x.Value.ToObject<object>());

        // combine entity value parameters with the scoped binding parameters overwriting any duplicate keys (preserving what the scope would apply the value as)
        facts.SanitizedValues = facts.ScopedBindings.Parameters.Concat(entityValueParams)
            .GroupBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.First().Value);

        return facts;
    }

    /// <summary>
    /// Update the properties of an object based on a dictionary of key/value pairs
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public TModel UpdateModelProperties(TModel obj, Dictionary<string, object> values)
    {
        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (values.ContainsKey(property.Name))
            {
                var value = values[property.Name];
                if (value != null)
                {
                    property.SetValue(obj, value);
                }
            }
        }

        return obj;
    }    

    public virtual Task<int?> InsertAsync(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        if (_applyScopedConstraints)
        {
            var facts = Inspect(entityToInsert);
            UpdateModelProperties(entityToInsert, facts.SanitizedValues);
        }

        return Basic.InsertAsync(entityToInsert, transaction, commandTimeout);
    }

    public virtual Task<int> UpdateAsync(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
    {
        if (_applyScopedConstraints)
        {
            var facts = Inspect(entityToUpdate);
            UpdateModelProperties(entityToUpdate, facts.SanitizedValues);
            return Basic.UpdateAsync(entityToUpdate, facts.ScopedBindings.Sql, entityToUpdate, transaction, commandTimeout);
        }
        else
        {
            return Basic.UpdateAsync(entityToUpdate, transaction, commandTimeout);
        }
    }

    public virtual Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        if (_applyScopedConstraints)
        {
            var fields = GetPopulatedIdFields(id);
            var bindable = Where(fields);
            return Basic.DeleteListAsync(bindable.Sql, bindable.Parameters, transaction, commandTimeout);
        }
        else
        {
            return Basic.DeleteAsync(id, transaction, commandTimeout);
        }
    }
}
