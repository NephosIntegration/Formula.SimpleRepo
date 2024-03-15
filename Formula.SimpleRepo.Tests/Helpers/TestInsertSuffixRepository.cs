using System.Collections;
using Microsoft.Extensions.Configuration;

namespace Formula.SimpleRepo.Tests;

[Repo]
public class TestInsertSuffixRepository : RepositoryBase<TestInsertSuffixModel, TestInsertSuffixModel>
{
    public TestInsertSuffixRepository(IConfiguration config) : base (config)
    {
    }

    // Used to allow us to create test around the queries produced
    public string LastQuery { get; private set; } = "";

    public override void LogQuery(string query)
    {
        base.LogQuery(query);
        LastQuery = query;
    }

}
