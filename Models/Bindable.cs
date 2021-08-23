using System.Collections.Generic;

namespace Formula.SimpleRepo
{
    public class Bindable
    {
        public string Sql { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public Bindable()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}