using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface
{
    class DBConnection
    {
        public static bool useAuth = false;
        //private readonly string connectionString = @"Data Source=KAYSER\MSSQLSERV;Initial Catalog=TestDB;Integrated Security=True;TrustServerCertificate=true;";
        //private readonly string connectionString = @"Server=192.168.0.106;Database=TestMainDB;Integrated Security=True;TrustServerCertificate=true;";
        //private readonly string connectionString = @"Data Source=KAYSER\MSSQLSERV;Initial Catalog=TestMainDB;Integrated Security=True;TrustServerCertificate=true;";
        //private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True;TrustServerCertificate=true;";
        //private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProtosDB;Integrated Security=True;Trust Server Certificate=true;";
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog = ProtosDBActual; Integrated Security = True; Trust Server Certificate=true;";
        public string ConnectionString
        {
            get { return connectionString; }
        }
    }
}
