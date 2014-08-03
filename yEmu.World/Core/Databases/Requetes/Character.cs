using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using yEmu.Core.Reflection;
using yEmu.Collections;
using System.Data;
using yEmu.Util;
using yEmu.World.Core.Classes.Characters;

namespace yEmu.World.Core.Databases.Requetes
{
    class Character : Singleton<Character>
    {
        public static ConcurrentList<Characters> characters = new ConcurrentList<Characters>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Characters>("SELECT * FROM personnages");

                foreach (var result in results)
                {
                    characters.Add(result);
                }
            }
            Info.Write("database", string.Format("{0} Characters chargés", characters.Count()), ConsoleColor.Green);
        }
    }
}
