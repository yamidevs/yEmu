using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Classes.Items;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Handler
{
    class ItemsHandler
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public void MoveItems(Processor Processor, int itemId, Position itemPosition, string data)
        {
            cacheLock.EnterReadLock();
            try
            {
                var item =
                             InventoryItem.Inventory.Find(
                                 x =>
                                     x.id == itemId &&
                                     x.IDCharacter.id == Processor.Clients.Character.id);

                if (item == null)
                    return;

                if (itemPosition == Position.None)
                {
                    var existItem = InventoryItems.ExistItem(item, item.IDCharacter, itemPosition);

                    if (existItem != null)
                    {
                        var id = item.id;
                        existItem.quantity += item.quantity;

                        //   InventoryItemRepository.Remove(item, true);

                        Processor.Clients.Send(string.Format("{0}{1}", "OR", id));

                        //  InventoryItemRepository.Update(existItem);

                        Processor.Clients.Send(string.Format("{0}{1}|{2}", "OQ", existItem.id,
                            existItem.quantity));
                    }
                    else
                    {
                        item.position = Position.None;
                        //  InventoryItemRepository.Update(item);

                        Processor.Clients.Send(string.Format("{0}{1}|{2}", "OM", item.id, (int)itemPosition));
                    }

                    Processor.Clients.Character.Stats.RemoveItemStats(item.IDStats);

                    if (item.IDItems.HasSet())
                    {
                        Processor.Clients.Character.Stats.DecreaseItemSetEffect(item);
                    }
                }
                else
                {
                    if (item.IDItems.Level > Processor.Clients.Character.level)
                    {
                        Processor.Clients.Send("OAEL");
                        return;
                    }

                    if (itemPosition == Position.Anneau1 || itemPosition == Position.Anneau2)
                    {
                        if (item.IDItems.HasSet())
                        {
                            var itemSet = item.IDItems.GetSet();

                            if (Processor.Clients.Character.CheckAnneau(itemSet) == true)
                            {
                                Processor.Clients.Send("OAEA");
                                return;
                            }
                        }
                    }

                    if (ItemCondition.VerifyIfCharacterMeetItemCondition(Processor.Clients.Character, item.IDItems.Conditions) ==
                        false)
                    {
                        Processor.Clients.Send(string.Format("{0}{1}", "Im", "119"));
                        return;
                    }

                    var existItem = InventoryItem.Inventory.Find(
                        x =>
                            x.IDCharacter == Processor.Clients.Character &&
                            x.position == itemPosition);

                    // TODO : debug Pets food & Obvis

                    if (existItem != null)
                    {
                        var obvi = InventoryItem.Inventory.Find(x => x.id == itemId);
                        if ((obvi.IDItems.ID == 9234))
                        {
                            existItem.obvi = 1;
                            // 3cb : etat de l'obji , 3cc niveau de l'obji

                            var Effect = new StringBuilder();
                            Effect.Append(existItem.stats).Append(",");
                            Effect.Append("3ca#0#0#");
                            Effect.Append(Info.DeciToHex(obvi.IDItems.ID)).Append(",");
                            Effect.Append("3cb#0#0#");
                            //todo : etat de l'obji
                            Effect.Append("18").Append(",");
                            Effect.Append("3cc#0#0#");
                            //NIVEAU OBJI
                            Effect.Append("1").Append(",");
                            Effect.Append("3cd#0#0#10,3ce#0#0#3e7");
                            existItem.stats = Effect.ToString();

                            Effect.Clear();

                            if (obvi.quantity > 1)
                            {
                                obvi.quantity -= 1;
                                Processor.Clients.Send(string.Format("{0}{1}|{2}", "OQ", obvi.id,
                                obvi.quantity));
                            }
                            obvi.quantity = 0;
                            Processor.Clients.Send(string.Concat("OR",obvi.id));
                            Processor.Clients.Send(string.Concat("OC;", existItem.ItemInfo()));
                            Processor.Clients.Character.Maps.Send(Processor.Clients, string.Format("{0}{1}|{2}", "Oa", Processor.Clients.Character.id,
                            Processor.Clients.Character.GetItems()));
                        }

                        return;
                    }

                    if (item.quantity > 1)
                    {
                        var newItem = item.Copy(quantity: item.quantity - 1);

                        //  InventoryItemRepository.Create(newItem, true);

                        item.quantity = 1;

                        item.position = itemPosition;

                        // InventoryItemRepository.Update(item);

                        Processor.Clients.Send(string.Format("{0}{1}|{2}", "OM", item.id, (int)itemPosition));

                        Processor.Clients.Send(string.Format("{0}{1}", "OAKO", newItem.ItemInfo()));
                    }
                    else
                    {
                        item.position = itemPosition;

                        //   InventoryItemRepository.Update(item);

                        Processor.Clients.Send(string.Format("{0}{1}", "OM", data));
                    }

                    Processor.Clients.Character.Stats.AddItemStats(item.IDStats);

                    if (item.IDItems.HasSet())
                    {
                        Processor.Clients.Character.Stats.IncreaseItemSetEffect(item);
                    }

                }

                if (item.IDItems.HasSet())
                {
                    var set = item.IDItems.GetSet();

                    Processor.HandlerPackets.SendItemSetBonnus(set);
                }

                Processor.Clients.Send(Processor.Clients.Character.GetStats());

                Processor.Clients.Character.Maps.Send(Processor.Clients, string.Format("{0}{1}|{2}", "Oa", Processor.Clients.Character.id,
                    Processor.Clients.Character.GetItems()));

            }
            finally
            {
                cacheLock.ExitReadLock();
                cacheLock.Dispose();
            }

        }
    }
}
