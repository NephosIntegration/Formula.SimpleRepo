using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Formula.SimpleRepo
{
    public interface IConstrainable
    {
        List<Constraint> GetConstrainables();
        List<Constraint> GetConstraints(Hashtable constraints);
        List<Constraint> GetConstraints(JObject json);
        List<Constraint> GetConstraintsFromJson(String json);
    }
}