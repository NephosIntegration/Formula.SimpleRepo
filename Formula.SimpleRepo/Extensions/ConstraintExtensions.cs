using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Formula.SimpleRepo;

public static class ConstraintExtensions
{
    public delegate Constraint TransformConstraintDelegate(Constraint toTransform);

    /// <summary>
    /// Get a constraint by it's property / column name (not database column name)
    /// </summary>
    /// <param name="constraints">List of constraints to search</param>
    /// <param name="column">The property / column name (not database column name)</param>
    /// <returns>The constraint if found to exist</returns>
    public static Constraint GetByColumn(this List<Constraint> constraints, string propertyName)
    {
        var output = constraints?.Where(i => propertyName.Equals(i.Column, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        return output;
    }

    /// <summary>
    /// A common pattern used in some of the repositories is to throw an exception if a constraint doesn't exists
    /// as a means of enforcing required / scoped constraints that can be determined automatically from middleware.
    /// </summary>
    /// <param name="constraints">The list of constraints to search</param>
    /// <param name="propertyName">The property name that is a required constraint</param>
    /// <returns>An exception is thrown if it doesn't exist, otherwise the value will be returned as a string</returns>
    public static string EnsureConstraintExists(this List<Constraint> constraints, string propertyName)
    {
        var value = constraints?.Where(i => propertyName.Equals(i.Column, StringComparison.InvariantCultureIgnoreCase)).Select(i => i.Value.ToString()).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{propertyName} is a required.");
        }

        return value;
    }

    /// <summary>
    /// An extension to give a simpler syntax in assisting with the conversion of specific fields within the list of constraints before they are 
    /// handed over the the binding.
    /// </summary>
    /// <param name="constraints"></param>
    /// <param name="propertyName"></param>
    /// <param name="transformConstraintDelegate"></param>
    /// <returns></returns>
    public static List<Constraint> TransformConstraint(this List<Constraint> constraints, string propertyName, TransformConstraintDelegate transformConstraintDelegate)
    {
        var output = constraints;

        var constraint = constraints.GetByColumn(propertyName);
        if (constraint != null)
        {
            // Transform it and update it
            constraint = transformConstraintDelegate(constraint);
        }

        return output;
    }

    /// <summary>
    /// Used to determine whether or not a property is valid for use as a constrainable.
    /// Properties can be be skipped if decorated with certain attributes.
    /// </summary>
    /// <param name="propertyInfo">The property to examine</param>
    /// <returns>true / false representing if it was found to be a constrainable property</returns>
    public static bool IsConstrainable(this PropertyInfo propertyInfo)
    {
        // If it has NotMapped it's not constrainable
        var isConstrainable = !Attribute.IsDefined(propertyInfo, typeof(Dapper.NotMappedAttribute));

        // If it's not eliminated yet (using short circuit evaluation), check the next attribute (IgnoreSelect)
        isConstrainable = (isConstrainable && !Attribute.IsDefined(propertyInfo, typeof(Dapper.IgnoreSelectAttribute)));

        return isConstrainable;
    }
}
