using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Formula.SimpleRepo;

public class QueryLogger
{
    public delegate void LogQueryDelegate(string query);

    public static LogQueryDelegate DefaultLogQuery = (query) =>
    {
        if (Debugger.IsAttached)
        {
            Debug.WriteLine(query);
        }
    };

    // Function to log the dictionary of parameters
    public static void Log(Dictionary<string, object> parameters)
    {
        if (Debugger.IsAttached && (parameters?.Count ?? 0) > 0)
        {
            Debug.WriteLine("==================");
            Debug.WriteLine("*** Parameters:");
            foreach (var p in parameters)
            {
                Debug.WriteLine($"@{p.Key} = {p.Value?.ToString() ?? "null"}");
            }
            Debug.WriteLine("==================");
        }
    }

    public static void Log(object obj)
    {
        if (Debugger.IsAttached && obj != null)
        {
            Debug.WriteLine("==================");
            Debug.WriteLine("*** Parameters:");
            try
            {
                var properties = obj.GetType().GetProperties();
                if ((properties?.Length ?? 0) > 0)
                {
                    foreach (var p in properties)
                    {
                        var value = "UNKNOWN";
                        try
                        {
                            value = p.GetValue(obj)?.ToString() ?? "null";
                        }
                        catch(Exception ex)
                        {
                            value = $"ERROR: {ex.Message}";
                        }
                        Debug.WriteLine($"@{p.Name} = {value}");
                    }
                }
                else
                {
                    Debug.WriteLine(obj.ToString());
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }
            Debug.WriteLine("==================");
        }
    }
}
