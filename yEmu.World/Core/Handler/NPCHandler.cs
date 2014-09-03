using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.World.Core.Classes.Items;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Handler
{
    class NPCHandler
    {
        private object Lock = new object();
        public void BuyNpc(Processor Processor, int receiverID)
        {
            var npc = yEmu.World.Core.Databases.Requetes.NPC.Npc.Npcs.Find(x => x.npcid == receiverID);
            var items = npc.NPCTemplates.ventes.Split(',');

            Processor.Clients.Character.Npc = npc.npcid;
            Processor.Clients.Send(string.Concat("ECK0|", npc.npcid));

            var newPacket = new StringBuilder();
            newPacket.Append("EL");

            foreach (var result in items)
            {
                var infoitems = ItemsInfo.ItemsInfos.FirstOrDefault(x => x.ID == int.Parse(result));
                newPacket.Append(string.Format("{0};{1}|", result, infoitems.EffectInfos()));
            }

            Processor.Clients.Send(newPacket.ToString().Substring(0, newPacket.Length - 1));

        }

        public void ExchangeBuy(Processor Processor, int itemID, int quantity)
        {
            var item = ItemsInfo.ItemsInfos.First(x => x.ID == itemID);
            var NPC = yEmu.World.Core.Databases.Requetes.NPC.Npc.Npcs.First(x => x.npcid == Processor.Clients.Character.Npc);


            /*  if (quantity <= 0 || !items.Contains(itemID))
              {
                  Clients.Send("EBE");
                  return;
              }*/

            var price = item.Price * quantity;

            if (Processor.Clients.Character.kamas >= price)
            {
                var inventaire = new InventoryItems();
                int Id = InventoryItem.Inventory.Count > 0
                 ? InventoryItem.Inventory.OrderByDescending(x => x.id).First().id + 1
                 : 1;
                inventaire.id = Id;
                inventaire.position = Position.None;
                inventaire.quantity = quantity;
                inventaire.IDStats = StatsItems.GenerateRandomStats(item.StatsItems).ToList();
                inventaire.IDItems = item;

                lock (InventoryItem.Lock)
                    InventoryItem.Inventory.Add(new InventoryItems
                    {
                        id = Id,
                        IDCharacter = Processor.Clients.Character,
                        IDItems = inventaire.IDItems,
                        position = inventaire.position,
                        quantity = inventaire.quantity,
                        IDStats = inventaire.IDStats
                    });

                lock (Lock)
                {
                    Processor.Clients.Character.kamas -= price;
                }

                Processor.Clients.Send(string.Format("{0}{1}", "OAKO", inventaire.ItemInfo()));
                Processor.Clients.Send("EBK");
                Processor.Clients.Send(string.Format("OQ{0}|{1}", Id, inventaire.quantity));

            }
            else
                Processor.Clients.Send("EBE");
        }
    }
}
