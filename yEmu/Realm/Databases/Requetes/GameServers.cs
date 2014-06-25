using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm.Classes;
using yEmu.Util;
using Dapper;
using yEmu.Reflection;
using yEmu.Collections;


namespace yEmu.Realm.Databases.Requetes
{
    class GameServers : Singleton<GameServers>
    {
        public static ConcurrentList<GameServer> servers = new ConcurrentList<GameServer>();

        public  void LoadServers()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var server = connection.Query<GameServer>("SELECT * FROM gameservers");
                foreach (var results in server)
                {
                    servers.Add(results);
                }                 
            }
            Info.Write("", string.Format("{0} Servers chargés", servers.Count()), ConsoleColor.Green);
        }
    }
}
