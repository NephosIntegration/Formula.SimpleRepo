using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

[ConnectionDetails("SQLiteMemoryConnection", typeof(SqliteConnection), SimpleCRUD.Dialect.SQLite)]
[Table("NoScopeTests")]
public class NoScopeModel
{
    [Key]
    [Column("uniqueId")]
    public int Id { get; set; }

    [Column("testData")]
    public string? TestData { get; set; }
}
