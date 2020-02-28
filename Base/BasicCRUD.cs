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
    public class BasicCRUD<TModel, TConstraintsModel> 
        : BasicCRUDBase<TModel, TConstraintsModel>
        where TModel : class
        where TConstraintsModel : new()
    {
        public BasicCRUD(IConfiguration config) : base(config)
        {
        }
    }
}