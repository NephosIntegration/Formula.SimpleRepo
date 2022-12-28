using System.Collections.Generic;

namespace Formula.SimpleRepo;

public interface IBindable
{
    string Sql { get; set; }
    Dictionary<string, object> Parameters { get; set; }
}
