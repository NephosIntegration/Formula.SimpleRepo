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
    public abstract class ReadOnlyRepositoryBase<TModel, TConstraintsModel> 
        : BuilderBase<TConstraintsModel>, IReadOnlyRepository<TModel> 
        where TModel : class
        where TConstraintsModel : new()
    {
        protected readonly IConfiguration _config;
        protected String _connectionName;
        protected IDbConnection _connection;

        public ReadOnlyRepositoryBase(IConfiguration config)
        {
            this._config = config;
            this._connectionName = ConnectionDetails.GetConnectionName<TModel>();
            this._connection = new SqlConnection(GetConnectionString());
        }

        protected virtual String GetConnectionString()
        {
            return _config.GetValue<String>($"ConnectionStrings:{_connectionName}");
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
    }
}