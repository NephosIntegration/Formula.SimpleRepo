using System.Collections.Generic;

namespace Formula.SimpleRepo;

public class QueryLogger
{
    public delegate void LogQueryDelegate(string query);

    public static LogQueryDelegate DefaultLogQuery = (query) =>
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debug.WriteLine(query);
        }
    };
}
