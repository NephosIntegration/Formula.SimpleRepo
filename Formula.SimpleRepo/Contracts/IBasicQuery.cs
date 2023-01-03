using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Formula.SimpleRepo;

public interface IBasicQuery<T>
{
    Task<T> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<IEnumerable<T>> GetListAsync();
    Task<IEnumerable<T>> GetListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<IEnumerable<T>> GetListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<IEnumerable<T>> GetListPagedAsync(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
}