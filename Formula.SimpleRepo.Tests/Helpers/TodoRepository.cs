using System.Collections;
using Microsoft.Extensions.Configuration;

namespace Formula.SimpleRepo.Tests;

[Repo]
public class TodoRepository : RepositoryBase<TodoModel, TodoConstraints>
{
    public TodoRepository(IConfiguration config) : base (config)
    {
    }

    public override List<Formula.SimpleRepo.Constraint> ScopedConstraints(List<Formula.SimpleRepo.Constraint> currentConstraints)
    {
        var constraints = new Hashtable();
        constraints.Add("Deleted", false);
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
