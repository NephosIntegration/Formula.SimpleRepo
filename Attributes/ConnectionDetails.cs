using System;
using System.Linq;

namespace Formula.SimpleRepo
{
    [AttributeUsage (AttributeTargets.Class)]
    public class ConnectionDetails : System.Attribute
    {
        public ConnectionDetails(String connectionName) {
            this.ConnectionName = connectionName;
        }

        public String ConnectionName { get; set; }

        public static String GetConnectionName<T>()
        {
            var details = typeof(T).GetCustomAttributes(typeof(ConnectionDetails), true).FirstOrDefault() as ConnectionDetails;

            return (details == null || String.IsNullOrEmpty(details.ConnectionName)) ? "DefaultConnection" : details.ConnectionName;
        }
    }
}
