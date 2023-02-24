using System.Collections;
using Microsoft.Extensions.Configuration;

namespace Formula.SimpleRepo.Tests;

[Repo]
public class TestRepository : RepositoryBase<TestModel, TestModel>
{
    public TestRepository(IConfiguration config) : base (config)
    {
    }

    public override List<Formula.SimpleRepo.Constraint> ScopedConstraints(List<Formula.SimpleRepo.Constraint> currentConstraints)
    {
        var constraints = new Hashtable();
        constraints.Add("Owner", "user1");
        return this.GetConstraints(constraints);
    }
    
    // Used to allow us to create test around the queries produced
    public string LastQuery { get; private set; } = "";

    public override void LogQuery(string query)
    {
        base.LogQuery(query);
        LastQuery = query;
    }

}
