using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Formula.SimpleRepo;

public abstract class ReadOnlyRepositoryBase<TModel, TConstraintsModel>
    : BuilderBase<TConstraintsModel>, IReadOnlyRepository<TModel>
    where TModel : class
    where TConstraintsModel : new()
{
    protected readonly IConfiguration _config;
    protected readonly string _connectionName;
    protected readonly IDbConnection _connection;
    protected readonly SimpleCRUD.Dialect _dialect;

    public ReadOnlyRepositoryBase(IConfiguration config)
    {
        _config = config;
        _connectionName = ConnectionDetails.GetConnectionName<TModel>();
        _connection = ConnectionDetails.GetConnection<TModel>(GetConnectionString());
        _dialect = ConnectionDetails.GetDialect<TModel>();
    }

    protected BasicQueryBase<TModel, TConstraintsModel> _basicQuery = null;
    public IBasicQuery<TModel> Basic
    {
        get
        {
            if (_basicQuery == null)
            {
                _basicQuery = new BasicQuery<TModel, TConstraintsModel>(_config, (query) => LogQuery(query));
            }
            return _basicQuery;
        }
    }

    public virtual void LogQuery(string query)
    {
        QueryLogger.DefaultLogQuery(query);
    }

    public new IReadOnlyRepository<TModel> ApplyScopedConstraints()
    {
        base.ApplyScopedConstraints();
        return this;
    }

    public new IReadOnlyRepository<TModel> RemoveScopedConstraints()
    {
        base.RemoveScopedConstraints();
        return this;
    }

    public List<string> GetIdFields()
    {
        var type = typeof(TModel);
        var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
        var properties = tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        return properties.Select(i => i.Name).ToList();
    }

    protected bool IsNativeType(object o)
    {
        return o.GetType().IsPrimitive ||
               new Type[] {
                    typeof(Decimal),
                    typeof(String),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
               }.Contains(o.GetType()) ||
               Convert.GetTypeCode(o) != TypeCode.Object;
    }

    protected object GetPropertyValue(object o, string propertyName)
    {
        if (IsNativeType(o))
        {
            return o;
        }
        else
        {
            return o.GetType().GetProperty(propertyName).GetValue(o, null);
        }
    }

    public Hashtable GetPopulatedIdFields(object value)
    {
        var output = new Hashtable();

        var fields = GetIdFields();
        foreach (var field in fields)
        {
            output.Add(field, GetPropertyValue(value, field));
        }

        return output;
    }

    protected virtual string GetConnectionString()
    {
        return _config.GetValue<string>($"ConnectionStrings:{_connectionName}");
    }


    protected Task<IEnumerable<TModel>> GetAsync(Bindable bindable, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Basic.GetListAsync(bindable.Sql, bindable.Parameters, transaction, commandTimeout);
    }

    protected Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, Bindable bindable, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Basic.GetListPagedAsync(pageNumber, rowsPerPage, bindable.Sql, orderby, bindable.Parameters, transaction, commandTimeout);
    }


    public Task<IEnumerable<TModel>> GetAsync(List<Constraint> finalConstraints, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var bindable = Where(finalConstraints);
        return GetAsync(bindable, transaction, commandTimeout);
    }

    public Task<IEnumerable<TModel>> GetAsync(Hashtable constraints, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var bindable = Where(constraints);
        return GetAsync(bindable, transaction, commandTimeout);
    }

    public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, Hashtable constraints = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var bindable = Where(constraints ?? new Hashtable());
        return GetListPagedAsync(pageNumber, rowsPerPage, bindable, orderBy, constraints, transaction, commandTimeout);
    }
    public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, List<Constraint> constraints, string orderBy, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var bindable = Where(constraints);
        return GetListPagedAsync(pageNumber, rowsPerPage, bindable, orderBy, parameters, transaction, commandTimeout);
    }

    public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, JObject constraints, string orderBy, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var bindable = Where(constraints);
        return GetListPagedAsync(pageNumber, rowsPerPage, bindable, orderBy, parameters, transaction, commandTimeout);
    }
    public Task<IEnumerable<TModel>> GetListPagedAsync(int pageNumber, int rowsPerPage, string json, string orderBy, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var obj = JsonConvert.DeserializeObject<JObject>(json);
        return GetListPagedAsync(pageNumber, rowsPerPage, obj, orderBy, parameters, transaction, commandTimeout);
    }

    public Task<int> RecordCountAsync(Hashtable constraints, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var bindable = Where(constraints);
        return Basic.RecordCountAsync(bindable.Sql, bindable.Parameters, transaction, commandTimeout);
    }

    public Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Basic.RecordCountAsync(conditions, parameters, transaction, commandTimeout);
    }

    public Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Basic.RecordCountAsync(whereConditions, transaction, commandTimeout);
    }

    public Task<IEnumerable<TModel>> GetAsync(JObject json, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var results = Where(json);
        return GetAsync(results, transaction, commandTimeout);
    }

    public Task<IEnumerable<TModel>> GetAsync(string json, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var obj = JsonConvert.DeserializeObject<JObject>(json);
        return GetAsync(obj, transaction, commandTimeout);
    }

    public async Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var fields = GetPopulatedIdFields(id);
        var bindable = Where(fields);
        var results = await GetAsync(bindable, transaction, commandTimeout);
        return results.FirstOrDefault();
    }

    public Task<IEnumerable<TModel>> GetAsync(IDbTransaction transaction = null, int? commandTimeout = null)
    {
        List<Constraint> nothing = null;
        return GetAsync(nothing, transaction, commandTimeout);
    }
}
