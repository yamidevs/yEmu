using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;

namespace yEmu.World.Core.Databases
{
    class AuthDatabases
    {
        private static readonly string ConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};convert zero datetime=True",
               Configuration.getString("ServerAuth"),
               Configuration.getString("UsernameAuth"),
               Configuration.getString("PasswordAuth"),
               Configuration.getString("DatabaseAuth")
   );
        public static MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(ConnectionString);

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                Info.Write("", "Connexion mysql : " + e.HResult, ConsoleColor.Red);
            }

            return connection;
        }
    }
}
