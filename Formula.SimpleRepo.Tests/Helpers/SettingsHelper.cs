using Microsoft.Extensions.Configuration;
using System.IO;

namespace Formula.SimpleRepo.Tests;

public class SettingsHelper
{
    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

    public static string GetConnectionString(string name = "SQLiteMemoryConnection")
    {
        return Configuration?.GetConnectionString(name) ?? "Data Source=:memory:;";
    }    
}
