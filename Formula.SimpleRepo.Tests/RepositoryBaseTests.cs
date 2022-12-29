using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

public class RepositoryBaseTests
{
    [Fact]
    public async Task RepositoryBase_General()
    {
        using var connection = (SqliteConnection?)null;
        
        DatabasePrimer.CreateTodoDatabase(connection);

        var repo = new TodoRepository(SettingsHelper.Configuration);

        var existingTodo = await repo.GetAsync(1);
        Assert.Equal(1, existingTodo.Id);
        Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"Details\",\"Completed\",\"CategoryId\" from \"Todos\" WHERE Id = @Id\n", repo.LastQuery);

        // Test Insert
        var todo = new TodoModel { Details = "Test 1", Completed = false };
        var id = await repo.InsertAsync(todo);
        Assert.True(id > 0);
        Assert.Equal("Insert: insert into \"Todos\" (\"Details\", \"Completed\", \"CategoryId\") values (@Details, @Completed, @CategoryId);SELECT LAST_INSERT_ROWID() AS id", repo.LastQuery);

        // Test Get
        var todo2 = await repo.GetAsync(id);
        Assert.NotNull(todo2);
        Assert.Equal(todo.Details, todo2.Details);
        Assert.Equal(todo.Completed, todo2.Completed);
        Assert.Equal(todo.CategoryId, todo2.CategoryId);
        //Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"Details\",\"Completed\",\"CategoryId\" from \"Todos\" WHERE Id = @Id\n", repo.LastQuery);

        // Test Update
        todo2.Details = "Test 2";
        todo2.Completed = true;
        todo2.CategoryId = 1;
        var rows = await repo.UpdateAsync(todo2);
        Assert.True(rows > 0);

        // Verify the update
        var todo3 = await repo.GetAsync(id);
        Assert.NotNull(todo3);
        Assert.Equal(todo2.Details, todo3.Details);
        Assert.Equal(todo2.Completed, todo3.Completed);
        Assert.Equal(todo2.CategoryId, todo3.CategoryId);

        // Test Delete
        rows = await repo.DeleteAsync(todo3.Id);
        Assert.True(rows > 0);

        // Confirm Delete
        var todo4 = await repo.GetAsync(todo3.Id);
        Assert.Null(todo4);
    }
}
