using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using yEmu.Util;
using yEmu.World.Core.Classes.Zaap;
using Dapper;

namespace yEmu.World.Core.Databases.Requetes.Zaap
{
    public class Zaap : Singleton<Zaap>
    {
        public static List<Zaaps> Zaaps = new List<Zaaps>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Zaaps>("SELECT * FROM zaaps");

                foreach (var result in results)
                {

                    Zaaps.Add(result);
                }

                Info.Write("database", string.Format("{0} Zaaps chargés", Zaaps.Count()), ConsoleColor.Green);
            }
        }
    }
}
