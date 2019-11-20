using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Formula.SimpleRepo
{
    public interface IRepository<T> : IBuilder
    {
        int Delete(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        int Delete(T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteAsync(T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null);
        int DeleteList(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        int DeleteList(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> DeleteListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        T Get(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<T> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<T> GetList();
        IEnumerable<T> GetList(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<T> GetList(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<T>> GetListAsync();
        Task<IEnumerable<T>> GetListAsync(string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<T>> GetListAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<T> GetListPaged(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<T>> GetListPagedAsync(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        int? Insert<TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        TKey Insert<TKey, TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<TKey> InsertAsync<TKey, TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int?> InsertAsync<TEntity>(TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);
        int RecordCount(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        int RecordCount(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        int Update<TEntity>(TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> UpdateAsync<TEntity>(TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null);
    }
}