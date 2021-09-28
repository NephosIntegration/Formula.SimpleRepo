using System;
using System.Collections.Generic;

namespace Formula.SimpleRepo
{
    public enum Comparison
    {
        Equals = 0,
        Null = 1,
    }

    /// <summary>
    /// A constraint is the basic building block of mapping out all of the properties we can query for data by.
    /// </summary>
    public class Constraint
    {
        public string Column { get; set; }
        public string DatabaseColumnName { get; set; }
        public object Value { get; set; }
        public TypeCode DataType { get; set; }
        public bool Nullable { get; set; }
        public Comparison Comparison { get; set; }

        public Constraint()
        {
            Comparison = Comparison.Equals;
        }

        public Constraint(string column, string databaseColumnName, TypeCode dataType, bool nullable = false, object value = null, Comparison comparison = Comparison.Equals)
        {
            Column = column;
            DatabaseColumnName = databaseColumnName;
            DataType = dataType;
            Nullable = nullable;
            Value = value;
            Comparison = comparison;
        }

        /// <summary>
        /// Determines if the data type allows for empty values, such as strings which allow for ""
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns>True if the data type allows empty values</returns>
        private bool DataTypeAllowsEmpty(TypeCode dataType)
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
        private bool EmptyValue(object value)
        {
            var empty = (value == null);

            if (!empty)
            {
                empty = string.IsNullOrEmpty(value.ToString());
            }

            return empty;
        }

        /// <summary>
        /// Determines if the intent of this query is to search for null values
        /// </summary>
        /// <returns></returns>
        private bool IsNullComparison()
        {
            var isNull = (Value == null);
            string stringValue = null;

            // If it wasn't explicitely null, we need to check for a verbose null
            if (!isNull)
            {
                stringValue = Value.ToString();
                isNull = "NULL".Equals(stringValue.ToUpper());
            }

            // If it wasn't an explicit null or a verbose null, check for an implied null
            if (!isNull)
            {
                // For an implied null, we must have a non null datatype, (a number for example)
                // But the value being supplied be empty.
                // The reason for this, is some data types support concepts of "empty"
                // which is not the same as null.. Consider strings.. "" != null
                isNull = (EmptyValue(stringValue) && DataTypeAllowsEmpty(DataType) == false);
            }

            return isNull;
        }

        /// <summary>
        /// Default binding behavior for a constraint, this can be overridden in a "custom constraint"
        /// to produce a more complicated parameterized SQL query
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual Dictionary<string, object> Bind(Dapper.SqlBuilder builder)
        {
            var parameters = new Dictionary<string, object>();

            // Are we creating an "IS NULL" query?
            if (IsNullComparison())
            {
                Comparison = Comparison.Null;
            }
            // We are doing a parameterized query
            // So construct the parameter
            else
            {
                try
                {
                    var convertedValue = Convert.ChangeType(Value, DataType);
                    parameters.Add(DatabaseColumnName, convertedValue);
                }
                catch (FormatException ex)
                {
                    var value = (Value == null ? "NULL" : Value.ToString());
                    var msg = $"{DatabaseColumnName} failed to convert '{value}' into an {DataType.ToString()} - {ex.Message}";
                    throw new FormatException(msg);
                }
            }

            SetWhereClause(builder);

            return parameters;
        }

        /// <summary>
        /// Set's the "WHERE clause" for this binding
        /// </summary>
        /// <param name="builder">The SQL Builder instance to use</param>
        private void SetWhereClause(Dapper.SqlBuilder builder)
        {
            if (Comparison == Comparison.Equals)
            {
                builder.Where($"{DatabaseColumnName} = @{DatabaseColumnName}");
            }
            else if (Comparison == Comparison.Null)
            {
                builder.Where($"{DatabaseColumnName} IS NULL");
            }
            else
            {
                throw new NotImplementedException("This constraint comparison type is not implemented yet");
            }
        }

    }
}