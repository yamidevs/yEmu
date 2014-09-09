using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.World.Core.Classes.Items;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Databases.Requetes.NPC;
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
        public void DialogueCreate(Processor Processor, string data)
        {
            var id = 0;

            if (!int.TryParse(data, out id))
                return;
            var NPC = yEmu.World.Core.Databases.Requetes.NPC.Npc.Npcs.FirstOrDefault(x => x.npcid == id);

            if (NPC != null || yEmu.World.Core.Databases.Requetes.NPC.Npc.Npcs.Any(x => x.npcid == id))
            {
                if (NPC.NPCTemplates.NPCQuestions == null)
                {
                    Processor.Clients.Send("BN");
                    return;
                }
                Processor.Clients.Send(string.Concat("DCK", NPC.npcid));
                Processor.Clients.Character.Dialogue = NPC.npcid;
                var packet = new StringBuilder("DQ");

                packet.Append(NPC.NPCTemplates.NPCQuestions.ID);
               // TODO : debug de params a faire

                packet.Append("|");

                    foreach (var result in NPC.NPCTemplates.NPCQuestions.Reponse)
                    {
                        if (result != null)
                        {
                            packet.Append(string.Concat(result.ID, ";"));
                        }
                       
                    }
                 
                Processor.Clients.Send(packet.ToString().Substring(0, packet.Length - 1));
                packet.Clear();
            }
        }

        public void DialogueReaplye(Processor Processor, string data)
        {
            var id = 0;

            if (!int.TryParse(data.Split('|')[1], out id))
                return;

            var NPC = yEmu.World.Core.Databases.Requetes
                .NPC.Npc.Npcs.FirstOrDefault(x => x.npcid == Processor.Clients.Character.Dialogue);

            if (!NPC.NPCTemplates.NPCQuestions.Reponse.Any(x => x.ID == id))
            {
                Processor.DialogueExit("");
                Processor.Clients.Send("BN");
                return;
            }

            var question = NPC.NPCTemplates.NPCQuestions.Reponse.FirstOrDefault(x => x.ID == id).Params;
            var packet = new StringBuilder("DQ");
            foreach (var result in question)
            {

                if (result == "DV")
                {

                    Processor.DialogueExit("");
                    return;
                }

                if (!NPCQuestion.NPCQuestions.Any(x => x.ID == int.Parse(result)))
                {
                    Processor.DialogueExit("");
                    return;
                }
                    var newNPC = NPCQuestion.NPCQuestions.Find(x => x.ID == int.Parse(result));
                               

                packet.Append(result);
                packet.Append("|");

                foreach (var results in newNPC.Reponse)
                {

                      if (result != null)
                      {
                          packet.Append(string.Concat(results.ID, ";"));
                      }
                     
                }                
            }

            Processor.Clients.Send(packet.ToString().Substring(0, packet.Length - 1));
            packet.Clear();   
           // var reponse = NPC.NPCTemplates.NPCQuestions.Reponse.FirstOrDefault(x => x.ID == id);
        }
    }
}
