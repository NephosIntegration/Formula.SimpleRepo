using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Formula.SimpleRepo;

public interface IBasicCRUD<T> : IBasicQuery<T>
{
    Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<int> DeleteAsync(T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<int> DeleteListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<int> DeleteListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<TKey> InsertAsync<TKey>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<int?> InsertAsync(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
    Task<int> UpdateAsync(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null);
    Task<int> UpdateAsync(T entityToUpdate, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
}