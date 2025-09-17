using Dapper;
using Microsoft.Data.Sqlite;
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Formula.SimpleRepo.Tests;

public class DatabasePrimer
{
    private static readonly object lockObject = new object();

    public static SqliteConnection CreateTodoDatabase()
    {
        var connection = new SqliteConnection(SettingsHelper.GetConnectionString());
        connection.Open();

        // Delete existing tables if they exist
        connection.Execute("DROP TABLE IF EXISTS Todos");
        connection.Execute("DROP TABLE IF EXISTS Tests");

        // Create a todo table
        connection.Execute(@"
            CREATE TABLE Todos (
                id INTEGER PRIMARY KEY,
                detailsColumn NVARCHAR(2048) NULL,
                completed BIT NOT NULL,
                categoryId INTEGER NULL,
                style NVARCHAR(2048) NULL,
                deleted BIT NOT NULL DEFAULT 0
            )");

        // Insert some todo row's
        connection.Execute("INSERT INTO Todos (id, detailsColumn, completed, categoryId, style, deleted) VALUES (1, 'Finish Coding', 0, 1, 'red', 0)");
        connection.Execute("INSERT INTO Todos (id, detailsColumn, completed, categoryId, style, deleted) VALUES (2, 'Build Unit Tests', 1, 2, 'blue', 1)");

        // Create a test table
        connection.Execute(@"
            CREATE TABLE Tests (
                uniqueId INTEGER PRIMARY KEY,
                testData NVARCHAR(100) NULL,
                ownedBy NVARCHAR(100) NOT NULL DEFAULT 'system'
            )");

        connection.Execute("INSERT INTO Tests (uniqueId, testData, ownedBy) VALUES (1, 'Test 1', 'system')");
        connection.Execute("INSERT INTO Tests (uniqueId, testData, ownedBy) VALUES (2, 'Test 2', 'user1')");

        return connection;
    }

    public static SqliteConnection CreateNoScopeDatabase()
    {
        var connection = new SqliteConnection(SettingsHelper.GetConnectionString());
        connection.Open();

        // Delete existing tables if they exist
        connection.Execute("DROP TABLE IF EXISTS NoScopeTests");

        // Create a NoScopeTests table
        connection.Execute(@"
            CREATE TABLE NoScopeTests (
                uniqueId INTEGER PRIMARY KEY,
                testData NVARCHAR(100) NULL
            )
        ");

        return connection;
    }


    public static SqliteConnection CreateTestDatabase(int recordCount)
    {
            var connection = new SqliteConnection(SettingsHelper.GetConnectionString());
            connection.Open();

            // Delete existing tables if they exist
            connection.Execute("DROP TABLE IF EXISTS Tests");

            // Create a test table
            connection.Execute(@"
            CREATE TABLE Tests (
                uniqueId INTEGER PRIMARY KEY,
                testData NVARCHAR(100) NULL,
                ownedBy NVARCHAR(100) NOT NULL DEFAULT 'system'
            );");

            // Insert recordCount row's
            for (int i = 1; i <= recordCount; i++)
            {
                connection.Execute("INSERT INTO Tests (uniqueId, testData, ownedBy) VALUES (@Id, @TestData, @Owner)", new { Id = i, TestData = $"Test {i}", Owner = i % 2 == 0 ? "user1" : "system" });
            }

            return connection;
        
    }
}
