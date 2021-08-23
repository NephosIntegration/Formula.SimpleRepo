using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Formula.SimpleRepo
{
    public abstract class BuilderBase<TConstraintsModel>
        : ConstrainableBase<TConstraintsModel>, IBuilder
        where TConstraintsModel : new()
    {
        protected SqlBuilder _builder = new SqlBuilder();

        protected Dictionary<string, object> _parameters { get; set; } = new Dictionary<string, object>();

        protected bool _applyScopedConstraints = true; // By default, if we have any scoped constraints they will be applied
        public ConstrainableBase<TConstraintsModel> ApplyScopedConstraints()
        {
            _applyScopedConstraints = true;
            return this;
        }
        public ConstrainableBase<TConstraintsModel> RemoveScopedConstraints()
        {
            _applyScopedConstraints = false;
            return this;
        }

        public virtual List<Constraint> ScopedConstraints(List<Constraint> currentConstraints)
        {
            return null;
        }

        public virtual List<Constraint> MergeConstraints(List<Constraint> original, List<Constraint> additional)
        {
            var output = original;

            if (_applyScopedConstraints)
            {
                if (original == null || original.Count() <= 0)
                {
                    output = additional;
                }
                else if (additional != null && additional.Count() > 0)
                {
                    foreach (var constraint in additional)
                    {
                        int existingIndex = original.FindIndex(i => i.Column.Equals(constraint.Column));
                        if (existingIndex > -1)
                        {
                            output[existingIndex] = constraint;
                        }
                        else
                        {
                            output.Add(constraint);
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Add a global parameter to be applied at the end
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddParameter(string name, object value)
        {
            _parameters.Add(name, value);
        }

        /// <summary>
        /// Give an opportunity for global builder parameters to be applied
        /// </summary>
        /// <param name="bindable"></param>
        /// <returns></returns>
        protected Bindable CombineParameters(Bindable bindable)
        {
            // If we have any global parameters to apply
            if (_parameters != null && _parameters.Count() > 0)
            {
                if (bindable.Parameters == null || bindable.Parameters.Count() == 0)
                {
                    bindable.Parameters = _parameters;
                }
                else
                {
                    foreach (var entry in _parameters)
                    {
                        // If this key already exists, replace it, else add it
                        if (bindable.Parameters.ContainsKey(entry.Key))
                        {
                            bindable.Parameters[entry.Key] = entry.Value;
                        }
                        else
                        {
                            bindable.Parameters.Add(entry.Key, entry.Value);
                        }
                    }
                }
            }

            return bindable;
        }

        public Bindable Where(List<Constraint> finalConstraints)
        {
            var output = new Bindable();

            var scoped = ScopedConstraints(finalConstraints);
            var constraints = MergeConstraints(finalConstraints, scoped);

            if (constraints != null && constraints.Count() > 0)
            {
                foreach (var constraint in constraints)
                {
                    constraint.Bind(_builder).AsList().ForEach(x => output.Parameters[x.Key] = x.Value);
                }

                output.Sql = _builder.AddTemplate("/**where**/").RawSql;
            }

            return CombineParameters(output);
        }

        public Bindable Where(Hashtable constraints)
        {
            return Where(GetConstraints(constraints));
        }

        public Bindable Where(JObject json)
        {
            return Where(GetConstraints(json));
        }

        public Bindable WhereFromJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            return Where(obj);
        }
    }
}