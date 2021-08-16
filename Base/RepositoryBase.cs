using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

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

        protected BasicCRUDBase<TModel, TConstraintsModel> _basicCRUD = null;
        public new IBasicCRUD<TModel> Basic
        {
            get
            {
                if (_basicCRUD == null)
                {
                    _basicCRUD = new BasicCRUD<TModel, TConstraintsModel>(_config);
                }
                return _basicCRUD;
            }
        }


        public virtual Task<int?> InsertAsync(TModel entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Basic.InsertAsync(entityToInsert, transaction, commandTimeout);
        }

        public virtual Task<int> UpdateAsync(TModel entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, CancellationToken? token = null)
        {
            return Basic.UpdateAsync(entityToUpdate, transaction, commandTimeout);
        }

        public virtual Task<int> DeleteAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Basic.DeleteAsync(id, transaction, commandTimeout);
        }
    }
}