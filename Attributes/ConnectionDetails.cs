using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Formula.SimpleRepo
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionDetails : System.Attribute
    {
        public ConnectionDetails(string connectionName, Type connectionType = null, SimpleCRUD.Dialect dialect = SimpleCRUD.Dialect.SQLServer)
        {
            ConnectionName = connectionName;
            ConnectionType = connectionType;
            Dialect = dialect;

            if (ConnectionType == null)
            {
                ConnectionType = typeof(SqlConnection);
            }
        }

        public string ConnectionName { get; set; }
        public Type ConnectionType { get; set; }
        public SimpleCRUD.Dialect Dialect { get; set; }

        public static string GetConnectionName<T>()
        {
            var details = typeof(T).GetCustomAttributes(typeof(ConnectionDetails), true).FirstOrDefault() as ConnectionDetails;

            return (details == null || string.IsNullOrEmpty(details.ConnectionName)) ? "DefaultConnection" : details.ConnectionName;
        }

        public static IDbConnection GetConnection<T>(string connectionString)
        {
            var details = typeof(T).GetCustomAttributes(typeof(ConnectionDetails), true).FirstOrDefault() as ConnectionDetails;
            var connection = (IDbConnection)Activator.CreateInstance(details.ConnectionType);
            connection.ConnectionString = connectionString;
            Dapper.SimpleCRUD.SetDialect(details.Dialect);
            return connection;
        }
    }
}
