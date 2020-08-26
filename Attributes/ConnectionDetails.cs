using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Formula.SimpleRepo
{
    [AttributeUsage (AttributeTargets.Class)]
    public class ConnectionDetails : System.Attribute
    {
        public ConnectionDetails(String connectionName, Type connectionType = null, SimpleCRUD.Dialect dialect = SimpleCRUD.Dialect.SQLServer) {
            this.ConnectionName = connectionName;
            this.ConnectionType = connectionType;
            this.Dialect = dialect;
            
            if (this.ConnectionType == null) 
            {
                this.ConnectionType = typeof(SqlConnection);
            }
        }

        public String ConnectionName { get; set; }
        public Type ConnectionType { get; set; }
        public SimpleCRUD.Dialect Dialect { get; set; }

        public static String GetConnectionName<T>()
        {
            var details = typeof(T).GetCustomAttributes(typeof(ConnectionDetails), true).FirstOrDefault() as ConnectionDetails;

            return (details == null || String.IsNullOrEmpty(details.ConnectionName)) ? "DefaultConnection" : details.ConnectionName;
        }

        public static IDbConnection GetConnection<T>(String connectionString)
        {
            var details = typeof(T).GetCustomAttributes(typeof(ConnectionDetails), true).FirstOrDefault() as ConnectionDetails;
            var connection = (IDbConnection)Activator.CreateInstance(details.ConnectionType);
            connection.ConnectionString = connectionString;
            Dapper.SimpleCRUD.SetDialect(details.Dialect);
            return connection;
        }
    }
}
