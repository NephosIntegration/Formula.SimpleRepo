using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Formula.SimpleRepo;

public interface IReadOnlyRepository<TModel> : IBuilder
{
    IBasicQuery<TModel> Basic { get; }
    List<string> GetIdFields();
    Hashtable GetPopulatedIdFields(object value);
    Task<IEnumerable<TModel>> GetAsync(List<Constraint> finalConstraints, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<IEnumerable<TModel>> GetAsync(Hashtable constraints, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<IEnumerable<TModel>> GetAsync(JObject json, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<IEnumerable<TModel>> GetAsync(string json, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<TModel> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<IEnumerable<TModel>> GetAsync(IDbTransaction transaction = null, int? commandTimeout = null);
    void ClearParameters();
    void AddParameter(string name, object value);
}