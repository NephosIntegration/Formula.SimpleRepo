using System;
using System.Collections.Generic;

namespace Formula.SimpleRepo
{
    public class Bindable
    {
        public String Sql { get; set; }
        public Dictionary<String, Object> Parameters { get; set; }

        public Bindable()
        {
            this.Parameters = new Dictionary<String, Object>();
        }
    }
}