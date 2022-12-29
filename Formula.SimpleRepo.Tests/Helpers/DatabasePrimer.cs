using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

public class DatabasePrimer
{
    public static SqliteConnection CreateTodoDatabase(SqliteConnection? connection)
    {
        connection = new SqliteConnection(SettingsHelper.GetConnectionString());
        connection.Open();

        // Create a test table
        connection.Execute(@"
            CREATE TABLE Todos (
                id INTEGER PRIMARY KEY,
                details NVARCHAR(2048) NULL,
                completed BIT NOT NULL,
                categoryId INTEGER NULL
            )");

        // Insert a test row
        connection.Execute("INSERT INTO Todos (id, details, completed, categoryId) VALUES (1, 'Finish Coding', 0, 0)");

        return connection;
    }
}
