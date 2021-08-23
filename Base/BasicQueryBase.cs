using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Formula.SimpleRepo
{
    public abstract class BasicQueryBase<TModel, TConstraintsModel>
        : IBasicQuery<TModel>
        where TModel : class
        where TConstraintsModel : new()
    {
        protected readonly IConfiguration _config;
        protected string _connectionName;
        protected IDbConnection _connection;

        public BasicQueryBase(IConfiguration config)
        {
            _config = config;
            _connectionName = ConnectionDetails.GetConnectionName<TModel>();
            _connection = ConnectionDetails.GetConnection<TModel>(GetConnectionString());
        }

        protected virtual string GetConnectionString()
        {
            return _config.GetValue<string>($"ConnectionStrings:{_connectionName}");
        }

        public virtual Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.GetAsync<TModel>(_connection, id, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync()
        {
            return GetListAsync(new { });
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.GetListAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.GetListAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.GetListPagedAsync<TModel>(_connection, pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
        }


        public virtual Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.RecordCountAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return SimpleCRUD.RecordCountAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }
    }
}