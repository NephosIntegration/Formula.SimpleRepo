using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

public class RepositoryBaseNoScopeTests
{
    [Fact]
    public async Task NoScopeRepository_CRUD_Works_By_Id_Only()
    {
        using (var connection = DatabasePrimer.CreateNoScopeDatabase())
        {
            var repo = new NoScopeRepository(SettingsHelper.Configuration);

            // Insert
            var model = new NoScopeModel { TestData = "Test 1" };
            var id = await repo.InsertAsync(model);
            Assert.True(id > 0);
            Assert.Contains("Insert", repo.LastQuery);

            // Insert another one so we leave the first one for testing
            var model2 = new NoScopeModel { TestData = "Test 2" };
            await repo.InsertAsync(model2);
            Assert.Contains("Insert", repo.LastQuery);

            // Ensure we have 2 records
            var allModels = await repo.Basic.GetListAsync();
            Assert.NotNull(allModels);
            Assert.Equal(2, allModels.Count());

            // Get
            var fetched = await repo.Basic.GetAsync(id);
            Assert.NotNull(fetched);
            Assert.Equal("Test 1", fetched.TestData);

            // Update
            fetched.TestData = "Updated!";
            var rows = await repo.UpdateAsync(fetched);
            Assert.Equal(1, rows);
            Assert.Contains("Update", repo.LastQuery);
            // Should only contain WHERE on uniqueId (Id)
            Assert.EndsWith("where \"uniqueId\" = @Id", repo.LastQuery);
            // Should not contain any additional ANDs (no extra constraints)
            var whereIndex = repo.LastQuery.IndexOf("WHERE");
            if (whereIndex >= 0)
            {
                var whereClause = repo.LastQuery.Substring(whereIndex);
                Assert.DoesNotContain("AND", whereClause);
            }

            // Delete
            var deleted = await repo.DeleteAsync(fetched.Id);
            Assert.Equal(1, deleted);
            Assert.Contains("Delete", repo.LastQuery);

            // Verify deletion
            var deletedModel = await repo.Basic.GetAsync(fetched.Id);
            Assert.Null(deletedModel);

            // Ensure we still have 1 record left
            var remainingModels = await repo.Basic.GetListAsync();
            Assert.NotNull(remainingModels);
            Assert.Single(remainingModels);
            Assert.Equal("Test 2", remainingModels.First().TestData);
        }
    }
}
