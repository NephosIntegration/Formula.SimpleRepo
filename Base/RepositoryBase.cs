using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Formula.SimpleRepo
{
    public abstract class RepositoryBase<TModel, TConstraintsModel> 
        : ReadOnlyRepositoryBase<TModel, TConstraintsModel>, IRepository<TModel> 
        where TModel : class
        where TConstraintsModel : new()
    {
        public RepositoryBase(IConfiguration config) : base(config)
        {
        }

        public virtual int Delete(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.Delete<TModel>(_connection, id, transaction, commandTimeout);
        }

        public virtual int Delete(TModel entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.Delete<TModel>(_connection, entityToDelete, transaction, commandTimeout);
        }

        public virtual Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteAsync<TModel>(_connection, id, transaction, commandTimeout);
        }

        public virtual Task<int> DeleteAsync(TModel entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteAsync<TModel>(_connection, entityToDelete, transaction, commandTimeout);
        }

        public virtual int DeleteList(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteList<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual int DeleteList(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteList<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public virtual Task<int> DeleteListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteListAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual Task<int> DeleteListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteListAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public virtual int? Insert(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Insert<TModel>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public virtual TKey Insert<TKey>(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Insert<TKey, TModel>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public virtual Task<TKey> InsertAsync<TKey>(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.InsertAsync<TKey, TModel>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public virtual Task<int?> InsertAsync(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.InsertAsync<TModel>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public virtual int Update(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Update<TModel>(_connection, entityToUpdate, transaction, commandTimeout);
        }

        public virtual Task<int> UpdateAsync(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
        {
			return SimpleCRUD.UpdateAsync<TModel>(_connection, entityToUpdate, transaction, commandTimeout, token);
        }

    }
}