using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Formula.SimpleRepo
{
    public static class ConstraintExtensions
    {
        public delegate Constraint TransformConstraintDelegate(Constraint toTransform);

        /// <summary>
        /// Get a contraint by it's prperty / column name (not database column name)
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
                throw new MissingPrimaryKeyException($"{propertyName} is a required.");
            }

            return value;
        }

        /// <summary>
        /// An extension to give a simpler syntax in assisting with the conversion of specific fields within the list of contraints before they are 
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
    }
}
