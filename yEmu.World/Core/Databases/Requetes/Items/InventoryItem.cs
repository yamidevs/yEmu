using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using Dapper;
using yEmu.World.Core.Classes.Items.Stats;
using System.Data;
using yEmu.Util;
using yEmu.World.Core.Classes.Items;

namespace yEmu.World.Core.Databases.Requetes
{
    public class InventoryItem : Singleton<InventoryItem>
    {
        public static object _lock = new object();

        public static object Lock 
        {
            get 
            { 
                return _lock; 
            }
        }
        public static readonly List<InventoryItems> Inventory = new List<InventoryItems>();
        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<InventoryItems>("SELECT * FROM inventory_items", buffered: false);
               
                foreach (var result in results)
                {
                   result.IDCharacter = Character.characters.Find(x => x.id == result.characterId);
                   result.IDItems = ItemsInfo.ItemsInfos.Find(x => x.ID == result.itemId);
                   result.IDStats = StatsItems.ToStats(result.stats);

                   lock (_lock)
                   Inventory.Add(result);
                }
             
            }
            foreach (var Personnages in Character.characters)
            {
                Personnages.CalculateItemStats();
            }
            Info.Write("database", string.Format("{0} Inventaire chargés", Inventory.Count()), ConsoleColor.Green);
        }

       
    }
}
