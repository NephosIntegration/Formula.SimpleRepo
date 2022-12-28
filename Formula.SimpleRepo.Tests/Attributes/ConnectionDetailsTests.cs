using System.Data.SqlClient;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

public class ConnectionDetailsTests
{
    private const string SQL_CONNECTION_STRING = "Server=.;Database=TestDatabase;Trusted_Connection=True;";
    private const string SQLITE_CONNECTION_STRING = "Data Source=TestDatabase.db;";

    [Fact]
    public void GetConnectionName_Returns_FakeConnection_When_No_Attribute()
    {
        var connectionName = ConnectionDetails.GetConnectionName<ConstrainableTypeClassWithoutAttribute>();

        Assert.Equal("DefaultConnection", connectionName);
    }

    [Fact]
    public void GetConnectionName_Returns_Attribute_ConnectionName()
    {
        var connectionName = ConnectionDetails.GetConnectionName<ConstrainableTypeClass>();

        Assert.Equal("TestConnection", connectionName);
    }

    [Fact]
    public void GetConnection_Returns_Null_When_No_Attribute()
    {
        var connection = ConnectionDetails.GetConnection<ConstrainableTypeClassWithoutAttribute>(SQL_CONNECTION_STRING);

        Assert.Null(connection);
    }

    [Fact]
    public void GetConnection_Returns_Attribute_ConnectionType()
    {
        var connection = ConnectionDetails.GetConnection<ConstrainableTypeClass>(SQLITE_CONNECTION_STRING);

        Assert.IsType<SqliteConnection>(connection);
        Assert.NotNull(connection);
    }

    [Fact]
    public void GetConnection_Returns_Attribute_ConnectionString()
    {
        var connection = ConnectionDetails.GetConnection<ConstrainableTypeClass>(SQLITE_CONNECTION_STRING);

        Assert.Equal(SQLITE_CONNECTION_STRING, connection.ConnectionString);
    }

    [Fact]
    public void GetDialect_Returns_SQLServer_When_No_Attribute()
    {
        var dialect = ConnectionDetails.GetDialect<ConstrainableTypeClassWithoutAttribute>();

        Assert.Equal(SimpleCRUD.Dialect.SQLServer, dialect);
    }

    [Fact]
    public void GetDialect_Returns_Attribute_Dialect()
    {
        var dialect = ConnectionDetails.GetDialect<ConstrainableTypeClass>();

        Assert.Equal(SimpleCRUD.Dialect.SQLite, dialect);
    }
}
