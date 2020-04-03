using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Formula.SimpleRepo
{
    public interface IRepository<T> : IReadOnlyRepository<T>
    {
        new IBasicCRUD<T> Basic { get; }

        int? Insert(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int?> InsertAsync(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);

        int Update(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> UpdateAsync(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null);

        int Delete(object id, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}