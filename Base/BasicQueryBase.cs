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
        protected readonly string _connectionName;
        protected readonly IDbConnection _connection;
        protected readonly SimpleCRUD.Dialect _dialect;

        protected SimpleCRUD _simpleCRUD = null;

        public SimpleCRUD BasicSimpleCRUD
        {
            get
            {
                if (_simpleCRUD == null)
                {
                    _simpleCRUD = new SimpleCRUD(_dialect);
                }
                return _simpleCRUD;
            }
        }

        public BasicQueryBase(IConfiguration config)
        {
            _config = config;
            _connectionName = ConnectionDetails.GetConnectionName<TModel>();
            _connection = ConnectionDetails.GetConnection<TModel>(GetConnectionString());
            _dialect = ConnectionDetails.GetDialect<TModel>();
        }

        protected virtual string GetConnectionString()
        {
            return _config.GetValue<string>($"ConnectionStrings:{_connectionName}");
        }

        public virtual Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return BasicSimpleCRUD.GetAsync<TModel>(_connection, id, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync()
        {
            return GetListAsync(new { });
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return BasicSimpleCRUD.GetListAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return BasicSimpleCRUD.GetListAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }

        public virtual Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return BasicSimpleCRUD.GetListPagedAsync<TModel>(_connection, pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
        }


        public virtual Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return BasicSimpleCRUD.RecordCountAsync<TModel>(_connection, conditions, parameters, transaction, commandTimeout);
        }

        public Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return BasicSimpleCRUD.RecordCountAsync<TModel>(_connection, whereConditions, transaction, commandTimeout);
        }
    }
}