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
            Trace.WriteLine(query); 
        }
    };
    // Function to log the dictionary of parameters
    public static void Log(Dictionary<string, object> parameters)
    {
        if (Debugger.IsAttached && (parameters?.Count ?? 0) > 0)
        {
            Trace.WriteLine("==================");
            Trace.WriteLine("*** Parameters:");
            foreach (var p in parameters)
            {
                Trace.WriteLine($"@{p.Key} = {p.Value?.ToString() ?? "null"}");
            }
            Trace.WriteLine("==================");
        }
    }

    public static void Log(object obj)
    {
        if (Debugger.IsAttached && obj != null)
        {
            Trace.WriteLine("==================");
            Trace.WriteLine("*** Parameters:");
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
                        catch (Exception ex)
                        {
                            value = $"ERROR: {ex.Message}";
                        }
                        Trace.WriteLine($"@{p.Name} = {value}");
                    }
                }
                else
                {
                    Trace.WriteLine(obj.ToString());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"ERROR: {ex.Message}");
            }
            Trace.WriteLine("==================");
        }
    }
}
