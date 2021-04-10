using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;
using System.Linq;

namespace Formula.SimpleRepo
{
    public abstract class BuilderBase<TConstraintsModel> 
        : ConstrainableBase<TConstraintsModel>, IBuilder 
        where TConstraintsModel : new()
    {
        protected SqlBuilder _builder = new SqlBuilder();

        protected Dictionary<String, Object> _parameters { get; set; } = new Dictionary<String, Object>();

        protected Boolean _applyScopedConstraints = true; // By default, if we have any scoped constraints they will be applied
        public ConstrainableBase<TConstraintsModel> ApplyScopedConstraints()
        {
            this._applyScopedConstraints = true;
            return this;
        }
        public ConstrainableBase<TConstraintsModel> RemoveScopedConstraints()
        {
            this._applyScopedConstraints = false;
            return this;
        }

        public virtual List<Constraint> ScopedConstraints(List<Constraint> currentConstraints)
        {
            return null;
        }

        public virtual List<Constraint> MergeConstraints(List<Constraint> original, List<Constraint> additional)
        {
            var output = original;

            if (this._applyScopedConstraints)
            {
                if (original == null || original.Count() <= 0)
                {
                    output = additional;
                }
                else if (additional != null && additional.Count() > 0)
                {
                    foreach(var constraint in additional)
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
        public void AddParameter(String name, Object value)
        {
            this._parameters.Add(name, value);
        }

        /// <summary>
        /// Give an opportunity for global builder parameters to be applied
        /// </summary>
        /// <param name="bindable"></param>
        /// <returns></returns>
        protected Bindable CombineParameters(Bindable bindable)
        {
            // If we have any global parameters to apply
            if (this._parameters != null && this._parameters.Count() > 0)
            {
                if (bindable.Parameters == null || bindable.Parameters.Count() == 0)
                {
                    bindable.Parameters = this._parameters;
                }
                else
                {
                    foreach(var entry in this._parameters)
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

            var scoped = this.ScopedConstraints(finalConstraints);
            var constraints = this.MergeConstraints(finalConstraints, scoped);

            if (constraints != null && constraints.Count() > 0)
            {
                foreach(var constraint in constraints) 
                {
                    constraint.Bind(this._builder).AsList().ForEach(x => output.Parameters[x.Key] = x.Value);
                }

                output.Sql = this._builder.AddTemplate("/**where**/").RawSql;
            }

            return this.CombineParameters(output);
        }

        public Bindable Where(Hashtable constraints)
        {
            return this.Where(this.GetConstraints(constraints));
        }

        public Bindable Where(JObject json)
        {
            return this.Where(this.GetConstraints(json));
        }

        public Bindable WhereFromJson(String json)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            return this.Where(obj);
        }
    }
}