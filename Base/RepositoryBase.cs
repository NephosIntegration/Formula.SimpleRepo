using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using System.Threading;

namespace Formula.SimpleRepo
{
    public abstract class RepositoryBase<TModel, TConstraintsModel> 
        : BuilderBase<TConstraintsModel>, IRepository<TModel> 
        where TModel : class
        where TConstraintsModel : new()
    {
        protected IDbConnection _connection;

        public RepositoryBase(IDbConnection connection)
        {
            this._connection = connection;
        }

        public int Delete(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.Delete<TModel>(_connection, id, transaction, commandTimeout);
        }

        public int Delete(TModel entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.Delete<TModel>(_connection, entityToDelete, transaction, commandTimeout);
        }

        public Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteAsync<TModel>(_connection, id, transaction, commandTimeout);
        }

        public Task<int> DeleteAsync(TModel entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteAsync<TModel>(_connection, entityToDelete, transaction, commandTimeout);
        }

        public int DeleteList(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteList<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public int DeleteList(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteList<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public Task<int> DeleteListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteListAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public Task<int> DeleteListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.DeleteListAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public TModel Get(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Get<TModel>(_connection, id, transaction, commandTimeout);
        }

        public Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetAsync<TModel>(_connection, id, transaction, commandTimeout);
        }

        public IEnumerable<TModel> GetList()
        {
			return SimpleCRUD.GetList<TModel>(_connection);
        }

        public IEnumerable<TModel> GetList(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetList<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public IEnumerable<TModel> GetList(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetList<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetListAsync()
        {
			return SimpleCRUD.GetListAsync<TModel>(_connection);
        }

        public Task<IEnumerable<TModel>> GetListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public IEnumerable<TModel> GetListPaged(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListPaged<TModel>(_connection, pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListPagedAsync<TModel>(_connection, pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
        }

        public int? Insert<TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Insert<TEntity>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public TKey Insert<TKey, TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Insert<TKey, TEntity>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public Task<TKey> InsertAsync<TKey, TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.InsertAsync<TKey, TEntity>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public Task<int?> InsertAsync<TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.InsertAsync<TEntity>(_connection, entityToInsert, transaction, commandTimeout);
        }

        public int RecordCount(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCount<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public int RecordCount(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCount<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCountAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCountAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public int Update<TEntity>(TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Update<TEntity>(_connection, entityToUpdate, transaction, commandTimeout);
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
        {
			return SimpleCRUD.UpdateAsync<TEntity>(_connection, entityToUpdate, transaction, commandTimeout, token);
        }

    }
}