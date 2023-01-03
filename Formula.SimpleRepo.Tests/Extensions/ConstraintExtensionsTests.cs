namespace Formula.SimpleRepo.Tests;

public class ConstraintExtensionsTests
{
    [Fact]
    public void ConstraintExtensions_GetByColumn()
    {
        var column = ConstrainableTypeClass.ConstraintsList().GetByColumn("StringProperty");
        Assert.Equal(ConstrainableTypeClass.Instance().StringProperty, column.Value);

        var invalidColumn = ConstrainableTypeClass.ConstraintsList().GetByColumn("InvalidColumn");
        Assert.Null(invalidColumn);
    }

    [Fact]
    public void ConstraintExtensions_EnsureConstraintExists()
    {
        var column = ConstrainableTypeClass.ConstraintsList().EnsureConstraintExists("StringProperty");
        Assert.Equal(ConstrainableTypeClass.Instance().StringProperty, column);

        Assert.Throws<ArgumentException>(() => ConstrainableTypeClass.ConstraintsList().EnsureConstraintExists("InvalidColumn"));
    }

    [Fact]
    public void ConstraintExtensions_TransformConstraint()
    {
        // Transform the int by multiplying it by 2
        var column = ConstrainableTypeClass.ConstraintsList().TransformConstraint("IntProperty", (constraint) =>
            {
                constraint.DataType = System.TypeCode.Int32;
                constraint.Value = (int)constraint.Value * 2;
                return constraint;
            });

        var results = column.Where(i => i.DatabaseColumnName == "IntColumn").FirstOrDefault();
        Assert.Equal((ConstrainableTypeClass.Instance().IntProperty * 2).ToString(), results?.Value?.ToString());

        // Transform a non existent column
        column = ConstrainableTypeClass.ConstraintsList().TransformConstraint("InvalidProperty", (constraint) =>
            {
                constraint.DataType = System.TypeCode.Int32;
                constraint.Value = (int)constraint.Value * 2;
                return constraint;
            });

        // Nothing should have changed on our original list
        results = column.Where(i => i.DatabaseColumnName == "IntColumn").FirstOrDefault();
        Assert.Equal(ConstrainableTypeClass.Instance().IntProperty.ToString(), results?.Value?.ToString());
    }

    [Fact]
    public void ConstraintExtensions_IsConstrainable()
    {
        var instance = ConstrainableTypeClass.Instance();
        
        // Test if the id property is constrainable
        var idProperty = instance.GetType().GetProperty("Id");
        Assert.True(idProperty.IsConstrainable());

        // Test if the string property is constrainable
        var stringProperty = instance.GetType().GetProperty("StringProperty");
        Assert.True(stringProperty.IsConstrainable());

        // Test if int property is constrainable
        var intProperty = instance.GetType().GetProperty("IntProperty");
        Assert.True(intProperty.IsConstrainable());

        // Test if the datetime property is constrainable
        var dateTimeProperty = instance.GetType().GetProperty("DateTimeProperty");
        Assert.True(dateTimeProperty.IsConstrainable());

        // Test if the not mapped property is constrainable
        var notMappedProperty = instance.GetType().GetProperty("NotMappedStringProperty");
        Assert.False(notMappedProperty.IsConstrainable());

        // Test if the ignore select property is constrainable
        var ignoreSelectProperty = instance.GetType().GetProperty("IgnoreSelectStringProperty");
        Assert.False(ignoreSelectProperty.IsConstrainable());
    }
}