using Dapper;

namespace Formula.SimpleRepo.Tests;

public class ConstraintTests
{
    private const string QUERY_NULL = "WHERE DatabaseColumnName IS NULL\n";
    private const string QUERY_DEFAULT = "WHERE DatabaseColumnName = @Column\n";

    [Fact]
    public void Constraint_String_Bind()
    {
        // String
        var constraint = new Constraint("Column", "DatabaseColumnName", TypeCode.String, false, "Value", Comparison.Equals);
        var builder = new SqlBuilder();
        var bindable = constraint.Bind(builder);

        Assert.Equal("Column", bindable.Keys.FirstOrDefault());
        Assert.Equal("Value", bindable.Values.FirstOrDefault());
        var sql = builder.AddTemplate("/**where**/").RawSql;
        Assert.Equal(QUERY_DEFAULT, sql);
    }

    [Fact]
    public void Constraint_Number_Bind()
    {
        // Number
        var constraint = new Constraint("Column", "DatabaseColumnName", TypeCode.Int32, false, 4, Comparison.Equals);
        var builder = new SqlBuilder();
        var bindable = constraint.Bind(builder);

        Assert.Equal("Column", bindable.Keys.FirstOrDefault());
        Assert.Equal(4, bindable.Values.FirstOrDefault());
        var sql = builder.AddTemplate("/**where**/").RawSql;
        Assert.Equal(QUERY_DEFAULT, sql);
    }

    [Fact]
    public void Constraint_Date_Bind()
    {
        // Date
        var now = DateTime.Now;
        var constraint = new Constraint("Column", "DatabaseColumnName", TypeCode.DateTime, false, now, Comparison.Equals);
        var builder = new SqlBuilder();
        var bindable = constraint.Bind(builder);

        Assert.Equal("Column", bindable.Keys.FirstOrDefault());
        Assert.Equal(now, bindable.Values.FirstOrDefault());
        var sql = builder.AddTemplate("/**where**/").RawSql;
        Assert.Equal(QUERY_DEFAULT, sql);
    }

    [Fact]
    public void Constraint_Nullable_Explicit_Bind()
    {
        // Explicit null
        var constraint = new Constraint("Column", "DatabaseColumnName", TypeCode.String, true, null, Comparison.Null);
        var builder = new SqlBuilder();
        var bindable = constraint.Bind(builder);

        Assert.Null(bindable.Keys.FirstOrDefault());
        Assert.Null(bindable.Values.FirstOrDefault());
        var sql = builder.AddTemplate("/**where**/").RawSql;
        Assert.Equal(QUERY_NULL, sql);
    }
    
    [Fact]
    public void Constraint_Nullable_Implicit_Bind()
    {
        // Implied null
        var constraint = new Constraint("Column", "DatabaseColumnName", TypeCode.Int16, true, "", Comparison.Equals);
        var builder = new SqlBuilder();
        var bindable = constraint.Bind(builder);

        Assert.Null(bindable.Keys.FirstOrDefault());
        Assert.Null(bindable.Values.FirstOrDefault());
        var sql = builder.AddTemplate("/**where**/").RawSql;
        Assert.Equal(QUERY_NULL, sql);
    }

    [Fact]
    public void Constraint_Nullable_Verbose_Bind()
    {
        // Verbose null
        var constraint = new Constraint("Column", "DatabaseColumnName", TypeCode.String, true, "NULL", Comparison.Equals);
        var builder = new SqlBuilder();
        var bindable = constraint.Bind(builder);

        Assert.Null(bindable.Keys.FirstOrDefault());
        Assert.Null(bindable.Values.FirstOrDefault());
        var sql = builder.AddTemplate("/**where**/").RawSql;
        Assert.Equal(QUERY_NULL, sql);
    }

    [Fact]
    public void Constraint_Nullable_Empty_Bind()
    {
        // Empty Isn't Null
        var constraint = new Constraint("Column", "DatabaseColumnName", TypeCode.String, false, "", Comparison.Equals);
        var builder = new SqlBuilder();
        var bindable = constraint.Bind(builder);

        Assert.NotNull(bindable.Keys.FirstOrDefault());
        Assert.NotNull(bindable.Values.FirstOrDefault());
        var sql = builder.AddTemplate("/**where**/").RawSql;
        Assert.Equal(QUERY_DEFAULT, sql);
    }
}
