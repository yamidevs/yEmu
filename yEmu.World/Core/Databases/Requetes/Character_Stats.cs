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
using MySql.Data.MySqlClient;


namespace yEmu.World.Core.Databases.Requetes
{
    public class Character_Stats : Singleton<Character_Stats>
    {
        public static List<Characters_stats> Characters_stats = new List<Characters_stats>();

        public void Load()
        {
            using (var connection = Databases.GetConnection())
            {
                using (var command = new MySqlCommand("SELECT * FROM stats", connection))
                {
                    var reader = command.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                        {

                            Characters_stats.Add(new Characters_stats
                            {
                                id = reader.GetInt16("id"),
                                Vitality = new BaseStats { Bases = reader.GetInt16("Vitality") },
                                Chance = new BaseStats { Bases = reader.GetInt16("Chance") },
                                Agility = new BaseStats { Bases = reader.GetInt16("Agility") },
                                Wisdom = new BaseStats { Bases = reader.GetInt16("Wisdom") },
                                Intelligence = new BaseStats { Bases = reader.GetInt16("Intelligence") },
                                Strenght = new BaseStats { Bases = reader.GetInt16("Strenght") }
                            });
                        }
                        reader.Close();
                    }
                    catch
                    {
                        Info.Write("database", string.Format("Probléme Table stats", Characters_stats.Count()), ConsoleColor.Green);
                    }
                }
            }
            Info.Write("database", string.Format("{0} Table Stats chargés", Characters_stats.Count()), ConsoleColor.Green);

        }
    }
}
