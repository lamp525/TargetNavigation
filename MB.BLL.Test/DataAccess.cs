using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.BLL.Test
{
    class DataAccess : SqlHelper
    {
        private static string DbConnection
        {
            get
            {
                return ConfigurationManager.AppSettings["Connection"].ToString();
            }
        }

        public static SqlConnection DbConn()
        {
            SqlConnection conn = new SqlConnection(DbConnection);

            conn.Open();

            return conn;
        }

        public static void DbClose(SqlConnection conn)
        {
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public static void Rollback(SqlTransaction trans)
        {
            if (trans != null)
            {
                trans.Rollback();
                trans = null;
            }
        }

        public static bool isDupulicateKey(SqlException ex)
        {
            return (ex.Number == 2627);
        }
    }
}
