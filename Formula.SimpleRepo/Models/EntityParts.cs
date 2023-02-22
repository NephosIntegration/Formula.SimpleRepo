using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Formula.SimpleRepo;

public class EntityParts
{
    public JObject JSONObject { get; set; }
    public Dictionary<string, object> Params { get; set; }
    public Hashtable IdFields { get; set; }
    public List<Constraint> IdConstraints { get; set; }
    public List<Constraint> ScopedConstraints { get; set; }
    public List<Constraint> AllConstraints { get; set; }
}