using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using yEmu.Util;
using yEmu.World.Core.Classes.Mount;
using Dapper;

namespace yEmu.World.Core.Databases.Requetes.Mount
{
    public class MountPark : Singleton<MountPark>
    {
        public static List<MountParks> MountParks = new List<MountParks>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<MountParks>("SELECT * FROM mountpark_data");

                foreach (var result in results)
                {
                    MountParks.Add(result);
                }

                Info.Write("database", string.Format("{0} MountPark chargés", MountParks.Count()), ConsoleColor.Green);
            }
        }
    }
}
