using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using Dapper;
using yEmu.World.Core.Classes.Characters;
using System.Data;
using yEmu.Util;

namespace yEmu.World.Core.Databases.Requetes
{
    public class Experience : Singleton<Experience>
    {
        public static List<Experiences> Experiences = new List<Experiences>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Experiences>("SELECT * FROM experiences");

                foreach (var result in results)
                {
                    Experiences.Add(result);
                }
            }
            Info.Write("database", string.Format("{0} Table Experiences chargés", Experiences.Count()), ConsoleColor.Green);
        }
    }
}
