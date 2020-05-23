using System;
using System.Collections.Generic;

namespace Formula.SimpleRepo
{
    public enum Comparison 
    {
        Equals = 0,
        Null = 1,
    }

    public static class ConstraintExtensions
    {
        public static Constraint GetByColumn(this List<Constraint> constraints, String column)
        {
            Constraint output = null;

            foreach(var item in constraints)
            {
                if (item.Column.Equals(column, StringComparison.InvariantCultureIgnoreCase))
                {
                    output = item;
                    break;
                }
            }

            return output;
        }    
    }

    public class NoQueryConstraint : Constraint
    {
        // Logic scope only, no impact on the database
        public override Dictionary<String, Object> Bind(Dapper.SqlBuilder builder)
        {
            var parameters = new Dictionary<String, Object>();

            parameters.Add("", null);

            return parameters;
        }
    }

    public class Constraint
    {
        public String Column { get; set; }
        public String DatabaseColumnName { get; set; }
        public Object Value { get; set; }
        public TypeCode DataType { get; set; }
        public Boolean Nullable { get; set; }
        public Comparison Comparison { get; set; }


        public Constraint()
        {
            this.Comparison = Comparison.Equals;
        }

        public Constraint(String column, String databaseColumnName, TypeCode dataType, Boolean nullable = false, Object value = null, Comparison comparison = Comparison.Equals)
        {
            this.Column = column;
            this.DatabaseColumnName = databaseColumnName;
            this.DataType = dataType;
            this.Nullable = nullable;
            this.Value = value;
            this.Comparison = comparison;
        }

        /// <summary>
        /// Determines if the data type allows for empty values, such as strings which allow for ""
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns>True if the data type allows empty values</returns>
        private Boolean DataTypeAllowsEmpty(TypeCode dataType)
        {
            var emptyTypes = new List<TypeCode> {
                TypeCode.Empty,
                TypeCode.String,
            };
            return emptyTypes.Contains(dataType);
        }

        /// <summary>
        /// Determines if the value of the object is "empty"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Boolean EmptyValue(Object value)
        {
            var empty = (value == null);

            if (!empty)
            {
                empty = String.IsNullOrEmpty(value.ToString());
            }

            return empty;
        }

        /// <summary>
        /// Determines if the intent of this query is to search for null values
        /// </summary>
        /// <returns></returns>
        private Boolean IsNullComparison()
        {
            var isNull = (this.Value == null);
            String stringValue = null;

            // If it wasn't explicitely null, we need to check for a verbose null
            if (!isNull)
            {
                stringValue = this.Value.ToString();
                isNull = "NULL".Equals(stringValue.ToUpper());
            }

            // If it wasn't an explicit null or a verbose null, check for an implied null
            if (!isNull)
            {
                // For an implied null, we must have a non null datatype, (a number for example)
                // But the value being supplied be empty.
                // The reason for this, is some data types support concepts of "empty"
                // which is not the same as null.. Consider strings.. "" != null
                isNull = (this.EmptyValue(stringValue) && this.DataTypeAllowsEmpty(this.DataType) == false);
            }

            return isNull;
        }

        public virtual Dictionary<String, Object> Bind(Dapper.SqlBuilder builder)
        {
            var parameters = new Dictionary<String, Object>();

            // Are we creating an "IS NULL" query?
            if (this.IsNullComparison())
            {
                this.Comparison = Comparison.Null;
            }
            // We are doing a parameterized query
            // So construct the parameter
            else
            {
                try
                {
                    var convertedValue = Convert.ChangeType(this.Value, this.DataType);
                    parameters.Add(this.DatabaseColumnName, convertedValue);
                }
                catch (FormatException ex)
                {
                    var value = (this.Value == null ? "NULL" : this.Value.ToString());
                    var msg = $"{this.DatabaseColumnName} failed to convert '{value}' into an {this.DataType.ToString()} - {ex.Message}"; 
                    throw new FormatException(msg);
                }
            }

            this.SetWhereClause(builder);

            return parameters;
        }

        /// <summary>
        /// Set's the "WHERE clause" for this binding
        /// </summary>
        /// <param name="builder">The SQL Builder instance to use</param>
        private void SetWhereClause(Dapper.SqlBuilder builder)
        {
            if (this.Comparison == Comparison.Equals) {
                builder.Where($"{this.DatabaseColumnName} = @{this.DatabaseColumnName}");
            }
            else if (this.Comparison == Comparison.Null) {
                builder.Where($"{this.DatabaseColumnName} IS NULL");
            }
            else {
                throw new NotImplementedException("This constraint comparison type is not implemented yet");
            }
        }

    }
}