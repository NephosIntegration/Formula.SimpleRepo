using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Formula.SimpleRepo
{
    public interface IBasicCRUD<T> : IBasicQuery<T>
    {
        int Delete(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        int Delete(T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteAsync(T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null);
        int DeleteList(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        int DeleteList(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        int? Insert(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        TKey Insert<TKey>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<TKey> InsertAsync<TKey>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int?> InsertAsync(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        int Update(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> UpdateAsync(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null);
    }
}