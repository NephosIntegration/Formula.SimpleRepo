using Microsoft.Extensions.Configuration;

namespace Formula.SimpleRepo;

public class BasicCRUD<TModel, TConstraintsModel>
    : BasicCRUDBase<TModel, TConstraintsModel>
    where TModel : class
    where TConstraintsModel : new()
{
    public BasicCRUD(IConfiguration config) : base(config)
    {
    }
}