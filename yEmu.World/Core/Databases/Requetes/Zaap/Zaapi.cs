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
    public class Zaapi : Singleton<Zaapi>
    {
        public static List<Zaapis> Zaapis = new List<Zaapis>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Zaapis>("SELECT * FROM zaapi");

                foreach (var result in results)
                {

                    Zaapis.Add(result);
                }

                Info.Write("database", string.Format("{0} Zaapis chargés", Zaapis.Count()), ConsoleColor.Green);
            }
        }
    }
}
