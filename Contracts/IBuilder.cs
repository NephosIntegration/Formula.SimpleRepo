using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Formula.SimpleRepo
{
    public interface IBuilder : IConstrainable 
    {
        Bindable Where(List<Constraint> constraints);
        Bindable Where(Hashtable constraints);
        Bindable Where(JObject json);
        Bindable WhereFromJson(String json);
    }
}