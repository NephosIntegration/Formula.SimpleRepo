using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Formula.SimpleRepo
{
    public abstract class RepositoryBase<TModel, TConstraintsModel> 
        : ReadOnlyRepositoryBase<TModel, TConstraintsModel>, IRepository<TModel> 
        where TModel : class
        where TConstraintsModel : new()
    {
        public RepositoryBase(IConfiguration config) : base(config)
        {
        }

        protected BasicCRUDBase<TModel, TConstraintsModel>  _basicCRUD = null;
        public new IBasicCRUD<TModel> Basic
        {
            get
            {
                if (this._basicCRUD == null)
                {
                    this._basicCRUD = new BasicCRUD<TModel, TConstraintsModel>(this._config);
                }
                return _basicCRUD;
            }
        }

        public virtual int? Insert(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.Insert(entityToInsert, transaction, commandTimeout);
        }

        public virtual Task<int?> InsertAsync(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.InsertAsync(entityToInsert, transaction, commandTimeout);
        }

        public virtual int Update(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.Update(entityToUpdate, transaction, commandTimeout);
        }

        public virtual Task<int> UpdateAsync(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
        {
            return this.Basic.UpdateAsync(entityToUpdate, transaction, commandTimeout);
        }

        public virtual int Delete(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.Delete(id, transaction, commandTimeout);
        }

        public virtual Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return this.Basic.DeleteAsync(id, transaction, commandTimeout);
        }
    }
}