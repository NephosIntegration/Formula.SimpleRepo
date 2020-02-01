using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Formula.SimpleRepo
{
    public interface IReadOnlyRepository<T> : IBuilder
    {
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
        int RecordCount(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        int RecordCount(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> RecordCountAsync(string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<int> RecordCountAsync(object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}