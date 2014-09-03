using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Items
{
    public class ItemSets
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string Items;

        public string Effects2;
        public string Effects3;
        public string Effects4;
        public string Effects5;
        public string Effects6;
        public string Effects7;
        public string Effects8;

        public List<ItemsInfos> ItemsStats;
        public Dictionary<int, List<Jet>> BonusesDictionary { get; set; }

        public static IEnumerable<KeyValuePair<int, List<Jet>>> ToBonusDictionary(params string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(string.Empty))
                    continue;

                var listItemSetBonus = args[i].Split(';').Select(bonus => new Jet
                {
                    Header = (Effect)int.Parse(bonus.Split(',')[0]),
                    MinValue = int.Parse(bonus.Split(',')[1])
                }).ToList();

                yield return new KeyValuePair<int, List<Jet>>(i + 2, listItemSetBonus);
            }
        }

    }
}
