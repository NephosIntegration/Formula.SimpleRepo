using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Formula.SimpleRepo;

public interface IConstrainable
{
    List<Constraint> GetConstrainables();
    List<Constraint> GetConstraints(Hashtable constraints);
    List<Constraint> GetConstraints(JObject json);
    List<Constraint> GetConstraintsFromJson(string json);
}