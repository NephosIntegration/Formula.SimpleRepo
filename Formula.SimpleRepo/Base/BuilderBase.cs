using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Formula.SimpleRepo;

public abstract class BuilderBase<TConstraintsModel>
    : ConstrainableBase<TConstraintsModel>, IBuilder
    where TConstraintsModel : new()
{
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

    /// <summary>
    /// You might also only want certain records to be returned based on some certain "scope". 
    /// Scoped constraints, are contraints that get applied automatically with every request. 
    /// These are applied in addition to (and instead of) any contraints applied that might be present. 
    /// These are useful for applying default constraints that need to be applied every time, and also as 
    /// a strategy for limiting the scope of the data returned for security reasons, or other creative 
    /// business rule purposes. 
    /// You can also programatically turn these on and off with .RemoveScopedConstraints()
    /// </summary>
    /// <param name="currentConstraints"></param>
    /// <returns></returns>
    public virtual List<Constraint> ScopedConstraints(List<Constraint> currentConstraints)
    {
        // Default behavior is no automatically applied constraints, this method must be overridden
        return null;
    }

    /// <summary>
    /// Give an opportunity for data transformations and constraint behaviors to be modified
    /// before the query is constructed..
    /// This is useful if you are allowing your constraint model to represent and accept user input
    /// in one fashion, but it needs to be transformed in some way before it is handed over as the value
    /// to be used by your custom constrains, or by the default binding behavior of Dapper.
    /// </summary>
    /// <param name="currentConstraints">The currently mapped out constraints</param>
    /// <returns></returns>
    public virtual List<Constraint> TransformConstraints(List<Constraint> currentConstraints)
    {
        // Default behavior is no transformations need to occur
        return currentConstraints;
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

        QueryLogger.Log(bindable?.Parameters);

        return bindable;
    }

    public Bindable Where(List<Constraint> currentConstraints)
    {
        var output = new Bindable();

        // Scoped constraints are applied via middleware within the repositories
        // Typically used to supply global default constraints
        var scoped = ScopedConstraints(currentConstraints);

        // Combine our list as we might have picked up some more constraints via ScopedConstraints
        var combined = MergeConstraints(currentConstraints, scoped);

        // Give a last chance for the outside world (aka repository) to modify the behavior of the constraints
        var constraints = TransformConstraints(combined);

        if (constraints != null && constraints.Count() > 0)
        {
            var builder = new SqlBuilder();
            foreach (var constraint in constraints)
            {
                constraint.Bind(builder).AsList().ForEach(x => output.Parameters[x.Key] = x.Value);
            }

            output.Sql = builder.AddTemplate("/**where**/").RawSql;
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