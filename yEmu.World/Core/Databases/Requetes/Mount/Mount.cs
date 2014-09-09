using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Classes.Mount;
using Dapper;
using yEmu.Core.Reflection;

namespace yEmu.World.Core.Databases.Requetes.Mount
{
    public class Mount : Singleton<Mount>
    {
        public static Object Lock = new Object();
        public static List<Mounts> Mounts = new List<Mounts>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Mounts>("SELECT * FROM mounts_data");

                foreach (var result in results)
                {
                    Mounts.Add(result);
                }

                Info.Write("database", string.Format("{0} MountPark chargés", Mounts.Count()), ConsoleColor.Green);
            }
        }

        public static void Add(Mounts MountsData)
        {
            lock (Lock)
            {
                Mounts.Add(MountsData);
            }
        }
    }
}
