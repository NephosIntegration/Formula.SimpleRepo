using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

public class InMemorySqliteTests
{
    [Fact]
    public void TestInMemorySqlite()
    {
        // Set up in-memory SQLite database
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = ":memory:"
        };
        var connectionString = connectionStringBuilder.ToString();
        var connection = new SqliteConnection(connectionString);
        connection.Open();

        // Create a test table
        connection.Execute(@"
            CREATE TABLE test (
                id INTEGER PRIMARY KEY,
                value NVARCHAR(2048) NOT NULL
            )");

        // Insert a test row
        connection.Execute("INSERT INTO test (id, value) VALUES (1, 'test value 1')");

        // Query the test table using Dapper
        var results = connection.Query<dynamic>("SELECT * FROM test").ToList();

        // Assert that the test row was returned
        Assert.Single(results);
        Assert.Equal(1, results[0].id);
        Assert.Equal("test value 1", results[0].value);
    }
}
