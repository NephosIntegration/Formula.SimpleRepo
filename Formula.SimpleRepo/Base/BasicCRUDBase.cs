using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Formula.SimpleRepo;

public abstract class BasicCRUDBase<TModel, TConstraintsModel>
    : BasicQueryBase<TModel, TConstraintsModel>, IBasicCRUD<TModel>
    where TModel : class
    where TConstraintsModel : new()
{
    public BasicCRUDBase(IConfiguration config) : base(config)
    {
    }

    public virtual Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return BasicSimpleCRUD.DeleteAsync<TModel>(_connection, id, transaction, commandTimeout);
    }

    public virtual Task<int> DeleteAsync(TModel entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return BasicSimpleCRUD.DeleteAsync<TModel>(_connection, entityToDelete, transaction, commandTimeout);
    }

    public virtual Task<int> DeleteListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return BasicSimpleCRUD.DeleteListAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
    }

    public virtual Task<int> DeleteListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return BasicSimpleCRUD.DeleteListAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
    }

    public virtual Task<TKey> InsertAsync<TKey>(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return BasicSimpleCRUD.InsertAsync<TKey, TModel>(_connection, entityToInsert, transaction, commandTimeout);
    }

    public virtual Task<int?> InsertAsync(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return BasicSimpleCRUD.InsertAsync<TModel>(_connection, entityToInsert, transaction, commandTimeout);
    }

    public virtual int Update(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return BasicSimpleCRUD.Update<TModel>(_connection, entityToUpdate, transaction, commandTimeout);
    }

    public virtual Task<int> UpdateAsync(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
    {
        return BasicSimpleCRUD.UpdateAsync<TModel>(_connection, entityToUpdate, transaction, commandTimeout, token);
    }

}