using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Formula.SimpleRepo
{
    public abstract class ConstrainableBase<TConstraintsModel>
        : IConstrainable
        where TConstraintsModel : new()
    {
        public string GetDatabaseColumnName(PropertyInfo prop)
        {
            var details = prop.GetCustomAttributes(typeof(Dapper.ColumnAttribute), true).FirstOrDefault() as Dapper.ColumnAttribute;

            return (details == null || string.IsNullOrEmpty(details.Name)) ? prop.Name : details.Name;
        }

        public List<Constraint> GetConstrainables()
        {
            var output = new List<Constraint>();

            foreach (var prop in typeof(TConstraintsModel).GetProperties())
            {
                var nullable = false;
                var typeCode = System.TypeCode.Empty;

                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    nullable = true;
                    typeCode = System.Type.GetTypeCode(prop.PropertyType.GetGenericArguments()[0]);
                }
                else
                {
                    typeCode = System.Type.GetTypeCode(prop.PropertyType);
                }

                output.Add(
                    new Constraint(
                        prop.Name,
                        GetDatabaseColumnName(prop),
                        typeCode,
                        nullable,
                        null,   // We don't know the value yet so initialize with nothing
                        Comparison.Equals
                    )
                );
            }

            return output;
        }

        public List<Constraint> GetConstraints(Hashtable constraints)
        {
            var output = new List<Constraint>();
            var constrainables = GetConstrainables();

            if (constraints.Count > 0)
            {
                var instance = new TConstraintsModel();

                foreach (var key in constraints.Keys)
                {
                    var validConstraint = constrainables.GetByColumn(key.ToString());
                    if (validConstraint != null)
                    {
                        var constraint = (Constraint)null;

                        if (validConstraint.DataType == TypeCode.Object)
                        {
                            var customObjType = typeof(TConstraintsModel).GetProperty(validConstraint.Column).PropertyType;
                            var constraintType = typeof(Constraint);
                            var isConstraint = (customObjType.IsSubclassOf(constraintType) || customObjType == constraintType);
                            if (isConstraint)
                            {
                                constraint = (Constraint)Activator.CreateInstance(customObjType);
                                constraint.DataType = TypeCode.Object;
                                constraint.Value = constraints[key].ToString();
                                constraint.Comparison = validConstraint.Comparison;

                                // If column isn't specified, use the key as the column name
                                if (string.IsNullOrWhiteSpace(constraint.Column))
                                {
                                    constraint.Column = validConstraint.Column;
                                }
                            }
                        }

                        if (constraint == null)
                        {
                            constraint = new Constraint(validConstraint.Column, validConstraint.DatabaseColumnName, validConstraint.DataType, validConstraint.Nullable, constraints[key].ToString(), validConstraint.Comparison);
                        }

                        output.Add(constraint);
                    }
                }
            }

            return output;
        }

        public List<Constraint> GetConstraints(JObject json)
        {
            var hash = new Hashtable();

            foreach (var item in json)
            {
                hash.Add(item.Key, item.Value.ToString());
            }

            return GetConstraints(hash);
        }

        public List<Constraint> GetConstraintsFromJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            return GetConstraints(obj);
        }
    }
}