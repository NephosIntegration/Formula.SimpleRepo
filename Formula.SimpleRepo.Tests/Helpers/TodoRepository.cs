using Microsoft.Extensions.Configuration;

namespace Formula.SimpleRepo.Tests;

[Repo]
public class TodoRepository : RepositoryBase<TodoModel, TodoModel>
{
    public string LastQuery { get; private set; } = "";

    public TodoRepository(IConfiguration config) : base (config)
    {
    }

    public override void LogQuery(string query)
    {
        base.LogQuery(query);
        LastQuery = query;
    }

}
