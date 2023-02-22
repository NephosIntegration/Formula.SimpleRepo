using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Formula.SimpleRepo;

public interface IBuilder : IConstrainable
{
    Bindable Where(List<Constraint> constraints);
    Bindable Where(Hashtable constraints);
    Bindable Where(JObject json);
    Bindable WhereFromJson(string json);
}