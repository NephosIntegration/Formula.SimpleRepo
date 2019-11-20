using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;

namespace Formula.SimpleRepo
{
    public abstract class BuilderBase<TConstraintsModel> 
        : ConstrainableBase<TConstraintsModel>, IBuilder 
        where TConstraintsModel : new()
    {
        protected SqlBuilder _builder = new SqlBuilder();

        public Bindable Where(List<Constraint> constraints)
        {
            var output = new Bindable();

            foreach(var constraint in constraints) 
            {
                constraint.Bind(this._builder).AsList().ForEach(x => output.Parameters[x.Key] = x.Value);
            }

            output.Sql = this._builder.AddTemplate("/**where**/").RawSql;

            return output;
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