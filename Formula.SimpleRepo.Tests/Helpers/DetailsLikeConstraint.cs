namespace Formula.SimpleRepo.Tests;

public class DetailsLike : Constraint
{
    public override Dictionary<String, Object> Bind(Dapper.SqlBuilder builder)
    {
        var parameters = new Dictionary<String, Object>();

        builder.Where("UPPER(DetailsColumn) like @DetailsLike");
        parameters.Add("DetailsLike", "%" + this.Value?.ToString()?.ToUpper() + "%");

        return parameters;
    }
}