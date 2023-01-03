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

    public virtual Task<int> UpdateAsync(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
    {
        if (_applyScopedConstraints)
        {
            var obj = JObject.FromObject(entityToUpdate);
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());
            var scopedBindings = Where((List<Constraint>)null);
            foreach (var kvp in scopedBindings.Parameters)
            {
                parameters[kvp.Key] = kvp.Value;
            }            
            return Basic.UpdateAsync(entityToUpdate, scopedBindings.Sql, parameters, transaction, commandTimeout);
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
