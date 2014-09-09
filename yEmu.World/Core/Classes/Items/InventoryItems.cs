using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Items
{
   public class InventoryItems
    {
        public int id { get; set; }
        public int quantity { get; set; }
        public Position position { get; set; }

        public int characterId { get; set; }
        public int itemId { get; set; }

        public string stats { get; set; }
        public List<StatsItems> IDStats { get; set; }

        public yEmu.World.Core.Classes.Characters.Characters IDCharacter { get; set; }
        public ItemsInfos IDItems { get; set; }

        public int obvi { get; set; }
        private  static object cacheLock = new object();

        public InventoryItems()
        {
                
        }
        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}",
                Info.DeciToHex(id),
                Info.DeciToHex(quantity),
                Info.DeciToHex((int)position),
                string.Join(",", IDStats));
        }

        public string ItemInfo()
        {

            if (Info.DeciToHex(IDItems.ID).Equals("2412") || obvi == 1)
            {
                var items =  InventoryItem.Inventory.Find(x => x.IDItems.ID == IDItems.ID);

                return string.Format("{0}~{1}~{2}~{3}~{4}",
                 Info.DeciToHex(id),
                 Info.DeciToHex(IDItems.ID),
                 Info.DeciToHex(quantity),
                 Info.DeciToHex((int)position),
                 string.Join(",", items.stats));
            }
            else
            {
                return string.Format("{0}~{1}~{2}~{3}~{4}",
                                Info.DeciToHex(id),
                                Info.DeciToHex(IDItems.ID),
                                Info.DeciToHex(quantity),
                                Info.DeciToHex((int)position),
                                string.Join(",", IDStats));
            }
            
        }

        public bool IsEquiped()
        {
            return (int)position > (int)Position.None &&
                   (int)position < (int)Position.Bar1;
        }

        public static InventoryItems ExistItem(InventoryItems item, yEmu.World.Core.Classes.Characters.Characters character, Position position = Position.None, int quantity = 1)
        {
         
                return InventoryItem.Inventory.Find(
                       x =>
                           x.IDItems == item.IDItems &&
                           string.Join(",", x.IDStats).Equals(string.Join(",", item.IDStats)) &&
                           x.IDCharacter == character && x.position == position && x.quantity >= quantity);
            
   
        }

        public InventoryItems Copy(Position position = Position.None, int quantity = 1)
        {
            return new InventoryItems
            {
                id =
                    InventoryItem.Inventory.Count > 0
                        ? InventoryItem.Inventory.OrderByDescending(x => x.id).First().id + 1
                        : 1,
                IDCharacter = this.IDCharacter,
                IDItems = this.IDItems,
                quantity = quantity,
                IDStats = this.IDStats,
                position = position,
            };
        }

        public string ToExchangeFormat(int quantity)
        {
            return string.Format("{0}|{1}|{2}|{3}",
                id,
                quantity,
                IDItems.ID,
                string.Join(",", IDStats));
        }

    }
}
