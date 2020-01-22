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
        : BuilderBase<TConstraintsModel>, IRepository<TModel> 
        where TModel : class
        where TConstraintsModel : new()
    {
        protected readonly IConfiguration _config;
        protected String _connectionName;
        protected IDbConnection _connection;

        public RepositoryBase(IConfiguration config)
        {
            this._config = config;
            this._connectionName = ConnectionDetails.GetConnectionName<TModel>();
            this._connection = new SqlConnection(GetConnectionString());
        }

        protected virtual String GetConnectionString()
        {
            return _config.GetValue<String>($"ConnectionStrings:{_connectionName}");
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

        public virtual TModel Get(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.Get<TModel>(_connection, id, transaction, commandTimeout);
        }

        public virtual Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetAsync<TModel>(_connection, id, transaction, commandTimeout);
        }

        public virtual IEnumerable<TModel> GetList()
        {
			return SimpleCRUD.GetList<TModel>(_connection);
        }

        public virtual IEnumerable<TModel> GetList(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetList<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual IEnumerable<TModel> GetList(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetList<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync()
        {
			return SimpleCRUD.GetListAsync<TModel>(_connection);
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public virtual IEnumerable<TModel> GetListPaged(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListPaged<TModel>(_connection, pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.GetListPagedAsync<TModel>(_connection, pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
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

        public virtual int RecordCount(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCount<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual int RecordCount(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCount<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public virtual Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCountAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
			return SimpleCRUD.RecordCountAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
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