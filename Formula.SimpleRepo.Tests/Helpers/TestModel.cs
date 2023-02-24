using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

[ConnectionDetails("SQLiteMemoryConnection", typeof(SqliteConnection), SimpleCRUD.Dialect.SQLite)]
[Table("Tests")]
public class TestModel
{
    [Key]
    [Column("uniqueId")]
    public int Id { get; set; }

    [Column("testData")]
    public String? TestData { get; set; }

    [Column("ownedBy")]
    public String? Owner { get; set; }
}
