using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using yEmu.Util;
using yEmu.Core.Reflection;

namespace yEmu.World.Core.Databases.Requetes
{
    class Servers : Singleton<Servers>
    {
        public static int ServerId;

        public void Load()
        {
            using (IDbConnection connection = AuthDatabases.GetConnection())
            {
                var results = connection.Query<int>("SELECT id FROM gameservers WHERE ServerKey=@key LIMIT 1",
                new { key = Configuration.getString("Game_key")});

                foreach (var result in results)
                {
                    ServerId = result;                    
                }
            }
            Info.Write("database","{0} Game Servers chargés", ConsoleColor.Green);
        }
    }
}
