using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;

namespace yEmu.World.Core.Databases
{
    class Databases
    {
        private static readonly string ConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};convert zero datetime=True",
                   Configuration.getString("Server"),
                   Configuration.getString("Username"),
                   Configuration.getString("Password"),
                   Configuration.getString("Database")
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
