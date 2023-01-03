using System.Collections;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Formula.SimpleRepo.Tests;

public class ConstrainableTypeClassWithoutAttribute
{
    public int Id { get; set; }
    public string? StringProperty { get; set; }
    public int IntProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }
    public string? NotMappedStringProperty { get; set; }
    public string? IgnoreSelectStringProperty { get; set; }
}


[ConnectionDetails("TestConnection", typeof(SqliteConnection), SimpleCRUD.Dialect.SQLite)]
[Table("ConstrainableType")]
public class ConstrainableTypeClass
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }
    
    [Column("StringColumn")]
    public string? StringProperty { get; set; }

    [Column("IntColumn")]
    public int IntProperty { get; set; }

    [Column("DateTimeColumn")]
    public DateTime DateTimeProperty { get; set; }

    [NotMapped]
    public string? NotMappedStringProperty { get; set; }

    [IgnoreSelect]
    [Column("IgnoreSelectStringColumn")]
    public string? IgnoreSelectStringProperty { get; set; }

    public static List<Constraint> ConstraintsList()
    {
        return new List<Constraint>
        {
            new Constraint("Id", "Id", TypeCode.Int32, false, 1, Comparison.Equals),
            new Constraint("StringProperty", "StringColumn", TypeCode.String, false, "Value", Comparison.Equals),
            new Constraint("IntProperty", "IntColumn", TypeCode.Int32, false, 4, Comparison.Equals),
            new Constraint("DateTimeProperty", "DateTimeColumn", TypeCode.DateTime, false, DateTime.Now, Comparison.Equals),
            new Constraint("NotMappedStringProperty", "NotMappedStringColumn", TypeCode.String, false, "Value", Comparison.Equals),
            new Constraint("IgnoreSelectStringProperty", "IgnoreSelectStringColumn", TypeCode.String, false, "Value", Comparison.Equals),
        };
    }

    public static Hashtable ConstraintsHashtable()
    {
        return new Hashtable() { 
            { "Id", 1 },
            { "StringProperty", "Value" },
            { "IntProperty", 4 },
            { "DateTimeProperty", DateTime.Now },
            { "NotMappedStringProperty", "Value" },
            { "IgnoreSelectStringProperty", "Value" },
        };
    }

    public static ConstrainableTypeClass Instance()
    {
        return new ConstrainableTypeClass
        {
            Id = 1,
            StringProperty = "Value",
            IntProperty = 4,
            DateTimeProperty = DateTime.Now,
            NotMappedStringProperty = "Value",
            IgnoreSelectStringProperty = "Value",
        };
    }
}
