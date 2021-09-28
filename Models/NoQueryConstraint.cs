using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Formula.SimpleRepo
{
    public class NoQueryConstraint : Constraint
    {
        // Logic scope only, no impact on the database
        public override Dictionary<string, object> Bind(Dapper.SqlBuilder builder)
        {
            var parameters = new Dictionary<string, object>();

            parameters.Add("", null);

            return parameters;
        }
    }
}