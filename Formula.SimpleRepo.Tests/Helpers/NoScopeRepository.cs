using Microsoft.Extensions.Configuration;
using System.Data;

namespace Formula.SimpleRepo.Tests;

[Repo]
public class NoScopeRepository : RepositoryBase<NoScopeModel, NoScopeModel>
{
    public NoScopeRepository(IConfiguration config) : base(config) {}

    public string LastQuery { get; private set; } = "";
    public override void LogQuery(string query)
    {
        base.LogQuery(query);
        LastQuery = query;
    }
    // Optionally override a method to provide the connection if your base supports it
}
