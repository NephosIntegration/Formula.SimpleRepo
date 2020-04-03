using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;

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