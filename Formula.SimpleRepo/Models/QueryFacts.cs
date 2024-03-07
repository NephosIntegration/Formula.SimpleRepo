using System.Collections;
using System.Collections.Generic;

namespace Formula.SimpleRepo;

public class QueryFacts
{
    public Hashtable IdFields { get; set; }
    public List<Constraint> IdConstraints { get; set; }
    public List<Constraint> ScopedConstraints { get; set; }
    public List<Constraint> AllConstraints { get; set; }
    public Bindable ScopedBindings { get; set; }
    public Dictionary<string, object> SanitizedValues { get; set; }
}
