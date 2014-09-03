using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Classes.Characters;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Items.Stats
{
    public class StatsItems : Jet
    {
        public int MaxValue { get; set; }
        public string JetDecimal = "0d0+0";

       
        public override string ToString()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}",
                Info.DeciToHex((int)Header),
                Info.DeciToHex(MinValue),
                Info.DeciToHex(MaxValue),
                "0",
                JetDecimal);
        }

        public static IEnumerable<StatsItems> GenerateRandomStats(IEnumerable<StatsItems> stats)
        {
            var rand = new Random();

            foreach (var itemStats in stats)
            {
                if (Characters_stats.WeaponEffect.Contains(itemStats.Header))
                {
                    yield return new StatsItems
                    {
                        Header = itemStats.Header,
                        MinValue = itemStats.MinValue,
                        MaxValue = itemStats.MaxValue,
                        JetDecimal = itemStats.JetDecimal
                    };
                }
                else
                {
                    yield return new StatsItems
                    {
                        Header = itemStats.Header,
                        MinValue =
                            itemStats.MaxValue == 0
                                ? itemStats.MinValue
                                : rand.Next(itemStats.MinValue, itemStats.MaxValue),
                        MaxValue = 0,
                        JetDecimal = itemStats.JetDecimal,
                    };
                }
            }
        }

        public static List<StatsItems> ToStats(string stats)
        {
            return (from stat in stats.Split(',')
                    where stat.Split('#').Length == 5
                    select new StatsItems
                    {
                        Header = (Effect)Info.HexToDeci(stat.Split('#')[0]),
                        MinValue = Info.HexToDeci(stat.Split('#')[1]),
                        MaxValue = Info.HexToDeci(stat.Split('#')[2]),
                        JetDecimal = stat.Split('#')[4]
                    }).ToList();
        }
    }
}
