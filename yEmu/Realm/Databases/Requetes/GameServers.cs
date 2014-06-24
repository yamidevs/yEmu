using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm.Classes;
using yEmu.Util;
using Dapper;


namespace yEmu.Realm.Databases.Requetes
{
    class GameServers
    {
        public static List<GameServer> servers = new List<GameServer>();

        public static void LoadServers()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {

                var server = connection.Query<GameServer>("SELECT * FROM gameservers");
                servers.AddRange(server);

                
            }
            Info.Write("", string.Format("{0} Servers chargés", servers.Count()), ConsoleColor.Green);
        }
    }
}
