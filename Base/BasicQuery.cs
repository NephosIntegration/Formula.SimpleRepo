using Microsoft.Extensions.Configuration;

namespace Formula.SimpleRepo
{
    public class BasicQuery<TModel, TConstraintsModel>
        : BasicQueryBase<TModel, TConstraintsModel>
        where TModel : class
        where TConstraintsModel : new()
    {
        public BasicQuery(IConfiguration config) : base(config)
        {
        }
    }
}