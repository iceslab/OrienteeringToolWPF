using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Utils
{
    public static class ConnectionStringUtils
    {
        public static string GetMySqlConnectionString(DatabaseConnectionData databaseConnectionData)
        {
            return string.Format(Properties.Resources.ConnectionStringMysql,
                    databaseConnectionData.Server,
                    databaseConnectionData.Port,
                    databaseConnectionData.Schema,
                    databaseConnectionData.User,
                    databaseConnectionData.Password);
        }

        public static string GetSqliteConnectionString(string databasePath)
        {
            return string.Format(
                    Properties.Resources.ConnectionStringSqlite,
                    databasePath);
        }
    }
}
