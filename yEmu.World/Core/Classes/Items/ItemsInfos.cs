using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Items
{
   public class ItemsInfos
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public int Level { get; set; }
        public int Weight { get; set; }
        public string WeaponInfo { get; set; }
        public int TwoHands { get; set; }
        public int IsEthereal { get; set; }
        public int IsBuff { get; set; }
        public int Usable { get; set; }
        public int Targetable { get; set; }
        public int Price { get; set; }
        public string Conditions { get; set; }

        public string Stats { get; set; }
        public List<StatsItems> StatsItems { get; set; }

        public ItemsInfos()
        {

        }
        public bool HasSet()
        {
            return ItemSet.Items.Any(x => x.ItemsStats.Any(y => y.ID == ID));
        }
        public string EffectInfos()
        {
            return string.Join(",", StatsItems);
        }
        public ItemSets GetSet()
        {
            return !HasSet() ? null : ItemSet.Items.Find(x => x.ItemsStats.Any(y => y.ID == ID));
        }
    }
}
