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
        //private readonly string connectionString = @"Data Source=KAYSER\MSSQLSERV;Initial Catalog=TestDB;Integrated Security=True;TrustServerCertificate=true;";
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True;TrustServerCertificate=true;";
        public string ConnectionString
        {
            get { return connectionString; }
        }
    }
}
