using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using yEmu.World.Core.Classes.Characters;

namespace yEmu.World.Core.Databases.Requetes
{
    public class Stats
    {
        public static void Create(Characters_stats stats)
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                connection.Execute("INSERT INTO stats SET id=@id, Vitality=@Vitality,Wisdom = @Wisdom,Strenght=@Strenght,"+
                    "Intelligence=@Intelligence,Chance = @Chance,Agility = @Agility",
                    new
                    {
                        id = stats.id,
                        Vitality = stats.Vitality.Bases,
                        Wisdom = stats.Wisdom.Bases,
                        Strenght = stats.Strenght.Bases,
                        Intelligence = stats.Intelligence.Bases,
                        Chance = stats.Chance.Bases,
                        Agility = stats.Agility.Bases
              
                    });
            }

            lock (Character_Stats.Characters_stats)
                Character_Stats.Characters_stats.Add(stats);

        }
    }
}
