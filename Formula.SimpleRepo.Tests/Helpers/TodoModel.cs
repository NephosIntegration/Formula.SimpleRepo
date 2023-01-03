using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

[ConnectionDetails("SQLiteMemoryConnection", typeof(SqliteConnection), SimpleCRUD.Dialect.SQLite)]
[Table("Todos")]
public class TodoModel
{
    [Key]
    public int Id { get; set; }

    [Column("DetailsColumn")]
    public String? Details { get; set; }

    public Boolean Completed { get; set; }

    public int? CategoryId { get; set; }

    [IgnoreInsert]
    [IgnoreUpdate]
    public Boolean Deleted { get; set; }

    [IgnoreSelect]
    public string? Style { get; set; }

    [NotMapped]
    public int? HiddenId { get; set; }
}
