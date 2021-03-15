using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            this._connection = ConnectionDetails.GetConnection<TModel>(GetConnectionString());
        }

        protected BasicQueryBase<TModel, TConstraintsModel> _basicQuery = null;
        public IBasicQuery<TModel> Basic
        {
            get
            {
                if (this._basicQuery == null)
                {
                    this._basicQuery = new BasicQuery<TModel, TConstraintsModel>(this._config);
                }
                return _basicQuery;
            }
        }

        public List<String> GetIdFields()
        {
            var type = typeof(TModel);
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
            var properties = tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            return properties.Select(i => i.Name).ToList();
        }

        public Hashtable GetPopulatedIdFields(object value)
        {
            var output = new Hashtable();

            var fields = this.GetIdFields();
            foreach (var field in fields)
            {
                output.Add(field, value);
            }

            return output;
        }

        protected virtual String GetConnectionString()
        {
            return _config.GetValue<String>($"ConnectionStrings:{_connectionName}");
        }

        protected IEnumerable<TModel> Get(Bindable bindable, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.GetList(bindable.Sql, bindable.Parameters, transaction, commandTimeout);
        }

        protected Task<IEnumerable<TModel>> GetAsync(Bindable bindable, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.GetListAsync(bindable.Sql, bindable.Parameters, transaction, commandTimeout);
        }

        protected Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, Bindable bindable, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.GetListPagedAsync(pageNumber, rowsPerPage, bindable.Sql, orderby, bindable.Parameters, transaction, commandTimeout);
        }

        public IEnumerable<TModel> Get(List<Constraint> finalConstraints, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(finalConstraints);
            return this.Get(results, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetAsync(List<Constraint> finalConstraints, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(finalConstraints);
            return this.GetAsync(results, transaction, commandTimeout);
        }

        public IEnumerable<TModel> Get(Hashtable constraints, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(constraints);
            return this.Get(results, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetAsync(Hashtable constraints, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(constraints);
            return this.GetAsync(results, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, Hashtable constraints, string orderBy, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(constraints);
            return this.GetListPagedAsync(pageNumber, rowsPerPage, results, orderBy, parameters, transaction, commandTimeout);
        }
        public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, List<Constraint> constraints, string orderBy, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(constraints);
            return this.GetListPagedAsync(pageNumber, rowsPerPage, results, orderBy, parameters, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, JObject constraints, string orderBy, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(constraints);
            return this.GetListPagedAsync(pageNumber, rowsPerPage, results, orderBy, parameters, transaction, commandTimeout);
        }
        public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, string json, string orderBy, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            return this.GetListPagedAsync(pageNumber, rowsPerPage, obj, orderBy, parameters, transaction, commandTimeout);
        }

        public IEnumerable<TModel> Get(JObject json, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(json);
            return this.Get(results, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetAsync(JObject json, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var results = this.Where(json);
            return this.GetAsync(results, transaction, commandTimeout);
        }

        public IEnumerable<TModel> Get(String json, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            return this.Get(obj, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetAsync(String json, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            return this.GetAsync(obj, transaction, commandTimeout);
        }

        public TModel Get(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var fields = this.GetPopulatedIdFields(id);
            var bindable = this.Where(fields);
            var results = this.Get(bindable, transaction, commandTimeout);
            return results.FirstOrDefault();
        }

        public async Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var fields = this.GetPopulatedIdFields(id);
            var bindable = this.Where(fields);
            var results = await this.GetAsync(bindable, transaction, commandTimeout);
            return results.FirstOrDefault();
        }

        public IEnumerable<TModel> Get(IDbTransaction transaction = null, int? commandTimeout = null)
        {
            List<Constraint> nothing = null;
            return this.Get(nothing, transaction, commandTimeout);
        }

        public Task<IEnumerable<TModel>> GetAsync(IDbTransaction transaction = null, int? commandTimeout = null)
        {
            List<Constraint> nothing = null;
            return this.GetAsync(nothing, transaction, commandTimeout);
        }
    }
}