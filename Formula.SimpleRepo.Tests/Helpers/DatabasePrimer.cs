using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

public class DatabasePrimer
{
    public static SqliteConnection CreateTodoDatabase()
    {
        var connection = new SqliteConnection(SettingsHelper.GetConnectionString());
        connection.Open();

        // Create a test table
        connection.Execute(@"
            CREATE TABLE Todos (
                id INTEGER PRIMARY KEY,
                detailsColumn NVARCHAR(2048) NULL,
                completed BIT NOT NULL,
                categoryId INTEGER NULL,
                style NVARCHAR(2048) NULL,
                deleted BIT NOT NULL DEFAULT 0
            )");

        // Insert a test row
        connection.Execute("INSERT INTO Todos (id, detailsColumn, completed, categoryId, style, deleted) VALUES (1, 'Finish Coding', 0, 1, 'red', 0)");
        connection.Execute("INSERT INTO Todos (id, detailsColumn, completed, categoryId, style, deleted) VALUES (2, 'Build Unit Tests', 1, 2, 'blue', 1)");

        return connection;
    }
}
