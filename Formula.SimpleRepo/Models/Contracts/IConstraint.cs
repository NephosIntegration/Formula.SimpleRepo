using System;
using System.Collections.Generic;
using Dapper;

namespace Formula.SimpleRepo;

public interface IConstraint
{
    string Column { get; set; }
    string DatabaseColumnName { get; set; }
    object Value { get; set; }
    TypeCode DataType { get; set; }
    bool Nullable { get; set; }
    Comparison Comparison { get; set; }

    Dictionary<string, object> Bind(SqlBuilder builder);
}
