using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Formula.SimpleRepo
{
    public interface IReadOnlyRepository<TModel> : IBuilder
    {
        IBasicQuery<TModel> Basic { get; }
        List<String> GetIdFields();
        Hashtable GetPopulatedIdFields(object value);
        IEnumerable<TModel> Get(List<Constraint> finalConstraints, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<TModel>> GetAsync(List<Constraint> finalConstraints, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<TModel> Get(Hashtable constraints, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<TModel>> GetAsync(Hashtable constraints, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<TModel> Get(JObject json, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<TModel>> GetAsync(JObject json, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<TModel> Get(String json, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<TModel>> GetAsync(String json, IDbTransaction transaction = null, int? commandTimeout = null);
        TModel Get(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<TModel> Get(IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<TModel>> GetAsync(IDbTransaction transaction = null, int? commandTimeout = null);
    }
}