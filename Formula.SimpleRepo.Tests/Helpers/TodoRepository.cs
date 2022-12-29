using Microsoft.Extensions.Configuration;

namespace Formula.SimpleRepo.Tests;

[Repo]
public class TodoRepository : RepositoryBase<TodoModel, TodoModel>
{
    public TodoRepository(IConfiguration config) : base (config)
    {
    }
}
