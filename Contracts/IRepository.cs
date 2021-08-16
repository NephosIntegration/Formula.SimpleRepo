using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Formula.SimpleRepo
{
    public interface IRepository<T> : IReadOnlyRepository<T>
    {
        new IBasicCRUD<T> Basic { get; }

        Task<int?> InsertAsync(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> UpdateAsync(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null);

        Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}