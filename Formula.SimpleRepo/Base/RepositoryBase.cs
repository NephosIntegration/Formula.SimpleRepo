using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
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

    public virtual Task<int?> InsertAsync(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Basic.InsertAsync(entityToInsert, transaction, commandTimeout);
    }

    /// <summary>
    /// Produce a list of constraints based on the populated fields of the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected EntityParts Inspect(TModel entity)
    {
        var parts = new EntityParts();

        parts.JSONObject = JObject.FromObject(entity);
        parts.Params = JsonConvert.DeserializeObject<Dictionary<string, object>>(parts.JSONObject.ToString());
        parts.IdFields = GetPopulatedIdFields(entity);
        parts.IdConstraints = GetConstraints(parts.IdFields);

        // To allow for scoped constraints in the outside repository to be applied, we need to supply all
        // possible constraints for any decision making that may be done in the implemented repository
        parts.ScopedConstraints = ScopedConstraints(parts.IdConstraints);

        parts.AllConstraints = MergeConstraints(parts.IdConstraints, parts.ScopedConstraints);

        return parts;
    }

    /// <summary>
    /// We can't allow items to be updated that don't match the scoped scoped values
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected EntityParts Sanitize(TModel entity)
    {
        var parts = Inspect(entity);

        // Allow the repository implementation to do any transformations it needs to do
        var transformed = Where(parts.AllConstraints);

        // Update the parameters with the transformed and scope applied values
        foreach (var kvp in transformed.Parameters)
        {
            parts.Params[kvp.Key] = kvp.Value;
        }

        return parts;
    }

    public virtual Task<int> UpdateAsync(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
    {
        if (_applyScopedConstraints)
        {
            // Sanitize the parameters
            var sanitized = Sanitize(entityToUpdate);

            // But the only "predicate" we put on our update statement
            // is based on the scoped constraints and the id fields
            // The id fields will be added in the actual Basic.UpdateAsync call
            var bindings = ConstraintsToBindable(sanitized.ScopedConstraints);

            return Basic.UpdateAsync(entityToUpdate, bindings.Sql, sanitized.Params, transaction, commandTimeout);
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
