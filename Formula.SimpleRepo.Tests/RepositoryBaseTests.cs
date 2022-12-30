using Microsoft.Data.Sqlite;
using System.Collections;

namespace Formula.SimpleRepo.Tests;

public class RepositoryBaseTests
{
    [Fact]
    public async Task RepositoryBase_Basics()
    {
        using (var connection = DatabasePrimer.CreateTodoDatabase())
        {
            var repo = new TodoRepository(SettingsHelper.Configuration);

            var existingTodo = await repo.GetAsync(1);
            Assert.Equal(1, existingTodo.Id);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Id = @Id AND Deleted = @Deleted\n", repo.LastQuery);

            // Test Insert
            var todo = new TodoModel { Details = "Test 1", Completed = false, Style = "Blue" };
            var id = await repo.InsertAsync(todo);
            Assert.True(id > 0);
            Assert.Equal("Insert: insert into \"Todos\" (\"DetailsColumn\", \"Completed\", \"CategoryId\", \"Style\") values (@Details, @Completed, @CategoryId, @Style);SELECT LAST_INSERT_ROWID() AS id", repo.LastQuery);

            // Test Get
            var todo2 = await repo.GetAsync(id);
            Assert.NotNull(todo2);
            Assert.Equal(todo.Details, todo2.Details);
            Assert.Equal(todo.Completed, todo2.Completed);
            Assert.Equal(todo.CategoryId, todo2.CategoryId);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Id = @Id AND Deleted = @Deleted\n", repo.LastQuery);

            // Test Update
            todo2.Details = "Test 2";
            todo2.Completed = true;
            todo2.Deleted = true; // This should't be updated cause it's scoped to false and ignored on the update
            todo2.CategoryId = 5;
            var rows = await repo.UpdateAsync(todo2);
            Assert.Equal(1, rows);
            Assert.Equal("Update: update \"Todos\" set \"DetailsColumn\" = @Details, \"Completed\" = @Completed, \"CategoryId\" = @CategoryId, \"Style\" = @Style WHERE Deleted = @Deleted AND \"Id\" = @Id", repo.LastQuery);

            // Ensure it didn't update the deleted column (cause it's scoped to true)
            todo2 = await repo.GetAsync(id);
            Assert.NotNull(todo2);
            Assert.False(todo2.Deleted);
            Assert.Equal("Test 2", todo2.Details);
            Assert.Equal(5, todo2.CategoryId);

            // Confirm only 3 records
            var recordCount = await repo.Basic.RecordCountAsync();
            Assert.Equal(3, recordCount);
            Assert.Equal("RecordCount<Formula.SimpleRepo.Tests.TodoModel>: Select count(1) from \"Todos\" ", repo.LastQuery);

            // Test GetListPagedAsync
            var byDetails = new Hashtable { 
                { "Details", "Test 2" }
            };
            var list = await repo.GetListPagedAsync(1, 10, byDetails, "Id");
            Assert.NotNull(list);
            var count = list.Count();
            Assert.Equal(1, count); // Should only be one since we are constraining it
            Assert.Equal("GetListPaged<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE DetailsColumn = @DetailsColumn AND Deleted = @Deleted\n Order By Id LIMIT 10 OFFSET ((1-1) * 10)", repo.LastQuery);

            // Test GetListAsync
            var list2 = await repo.GetAsync();
            Assert.NotNull(list2);
            count = list2.Count();
            Assert.Equal(2, count);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Deleted = @Deleted\n", repo.LastQuery);

            // Verify the update
            var todo3 = await repo.GetAsync(id);
            Assert.NotNull(todo3);
            Assert.Equal(todo2.Details, todo3.Details);
            Assert.Equal(todo2.Completed, todo3.Completed);
            Assert.Equal(todo2.CategoryId, todo3.CategoryId);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Id = @Id AND Deleted = @Deleted\n", repo.LastQuery);

            // Test Delete
            rows = await repo.DeleteAsync(todo3.Id);
            Assert.Equal(1, rows);
            Assert.Equal("DeleteList<Formula.SimpleRepo.Tests.TodoModel> Delete from \"Todos\" WHERE Id = @Id AND Deleted = @Deleted\n", repo.LastQuery);

            // Confirm Delete
            var todo4 = await repo.GetAsync(todo3.Id);
            Assert.Null(todo4);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Id = @Id AND Deleted = @Deleted\n", repo.LastQuery);
            
            // Confirm only 2 records
            recordCount = await repo.Basic.RecordCountAsync();
            Assert.Equal(2, recordCount);
            Assert.Equal("RecordCount<Formula.SimpleRepo.Tests.TodoModel>: Select count(1) from \"Todos\" ", repo.LastQuery);

            connection.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    [Fact]
    public async Task RepositoryBase_Custom_Constraints()
    {
        using (var connection = DatabasePrimer.CreateTodoDatabase())
        {
            var repo = new TodoRepository(SettingsHelper.Configuration);

            // Create some data
            await repo.InsertAsync(new TodoModel { Details = "Car 1", Completed = false });
            await repo.InsertAsync(new TodoModel { Details = "Shopping 1", Completed = true });
            await repo.InsertAsync(new TodoModel { Details = "Car 2", Completed = false });
            await repo.InsertAsync(new TodoModel { Details = "Shopping 2", Completed = true });
            await repo.InsertAsync(new TodoModel { Details = "Shopping 3", Completed = false });

            var results = await repo.GetAsync("{DetailsLike:'Car'}");
            Assert.NotNull(results);      
            var count = results.Count();
            Assert.Equal(2, count);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE UPPER(DetailsColumn) like @DetailsLike AND Deleted = @Deleted\n", repo.LastQuery);

            results = await repo.GetAsync("{DetailsLike:'Shopping'}");
            Assert.NotNull(results);      
            count = results.Count();
            Assert.Equal(3, count);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE UPPER(DetailsColumn) like @DetailsLike AND Deleted = @Deleted\n", repo.LastQuery);

            connection.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    /// <summary>
    /// See Issue #17
    /// https://github.com/NephosIntegration/Formula.SimpleRepo/issues/17
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RepositoryBase_Predicates_Reset()
    {
        using (var connection = DatabasePrimer.CreateTodoDatabase())
        {
            var repo = new TodoRepository(SettingsHelper.Configuration);

            // Create some data
            await repo.InsertAsync(new TodoModel { Details = "Test 1", Completed = false });
            await repo.InsertAsync(new TodoModel { Details = "Test 2", Completed = true });
            await repo.InsertAsync(new TodoModel { Details = "Test 3", Completed = false });
            await repo.InsertAsync(new TodoModel { Details = "Test 4", Completed = true });
            await repo.InsertAsync(new TodoModel { Details = "Test 5", Completed = false });

            // Run an initial query to prime the predicate
            var results = await repo.GetAsync("{Completed:true}");
            Assert.NotNull(results);      
            var count = results.Count();
            Assert.Equal(2, count);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Completed = @Completed AND Deleted = @Deleted\n", repo.LastQuery);

            // Ensure that a new query gets created if the predicates change
            results = await repo.GetAsync("{Details:'Test 1'}");
            Assert.NotNull(results);      
            count = results.Count();
            Assert.Equal(1, count);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE DetailsColumn = @DetailsColumn AND Deleted = @Deleted\n", repo.LastQuery);

            // Ensure a previously executed query does not affect the current query
            results = await repo.GetAsync("{Completed:false}");
            Assert.NotNull(results);      
            count = results.Count();
            Assert.Equal(4, count);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Completed = @Completed AND Deleted = @Deleted\n", repo.LastQuery);

            // Executing again shouldn't produce any different results
            results = await repo.GetAsync("{Completed:false}");
            Assert.NotNull(results);      
            count = results.Count();
            Assert.Equal(4, count);
            Assert.Equal("GetList<Formula.SimpleRepo.Tests.TodoModel>: Select \"Id\",\"DetailsColumn\" as \"Details\",\"Completed\",\"CategoryId\",\"Deleted\" from \"Todos\" WHERE Completed = @Completed AND Deleted = @Deleted\n", repo.LastQuery);

            connection.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
