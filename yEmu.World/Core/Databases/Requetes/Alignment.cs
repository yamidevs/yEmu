using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using Dapper;
using yEmu.World.Core.Classes.Characters;
using yEmu.Util;
using System.Data;

namespace yEmu.World.Core.Databases.Requetes
{
    public class Alignment : Singleton<Alignment>
    {
        public static List<Alignments> Alignments = new List<Alignments>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Alignments>("SELECT * FROM alignments");

                foreach (var result in results)
                {

                    Alignments.Add(result);
                }

                Info.Write("database", string.Format("{0} Aligenements chargés", Alignments.Count()), ConsoleColor.Green);
            }
        }

        public static void Create(Alignments alignment)
        {
            
            using (IDbConnection connection = Databases.GetConnection())
            {
                connection.Execute("INSERT INTO alignments SET Id=@Id, Type=@Type ,Honor = @Honor,Deshonor=@Deshonor,"+
                "Level=@Level,Grade=@Grade,Enabled=@Enabled",
                    new
                    {
                        Id = alignment.Id,
                        Type = alignment.Type,
                        Honor = alignment.Honor,
                        Deshonor = alignment.Deshonor,
                        Level = alignment.Level,
                        Grade = alignment.Grade,
                        Enabled = alignment.Enabled
                    });
            }

            lock (Alignments)
                Alignments.Add(alignment);
            
        }
    }
}
