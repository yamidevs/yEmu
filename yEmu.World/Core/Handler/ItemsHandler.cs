using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                        Processor.Clients.Send("BN");
                        return;
                    }

                    if (item.quantity > 1)
                    {
                        var newItem = item.Copy(quantity: item.quantity - 1);

                        //  InventoryItemRepository.Create(newItem, true);

                        item.quantity = 1;

                        item.position = itemPosition;

                        // InventoryItemRepository.Update(item);

                        Processor.Clients.Send(string.Format("{0}{1}|{2}", "ObjectMove", item.id, (int)itemPosition));

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
