using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using Dapper;
using System.Data;
using yEmu.World.Core.Classes.Maps;
using yEmu.Util;
using yEmu.Collections;

namespace yEmu.World.Core.Databases.Requetes
{
    public class Trigger : Singleton<Trigger>
    {
        public static readonly List<Triggers> Triggers = new List<Triggers>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {

                var results = connection.Query<Triggers>("SELECT * FROM  maps_triggers");

                foreach (var result in results)
                {
                    Triggers.Add(result);
                }
            }
            Info.Write("database", string.Format("{0} Triggers chargés", Triggers.Count()), ConsoleColor.Green);
        }

    }
}
