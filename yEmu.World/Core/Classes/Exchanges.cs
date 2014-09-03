using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.World.Core.Classes.Items;
using yEmu.World.Core.Databases.Requetes;
namespace yEmu.World.Core.Classes
{
    class Exchanges
    {
         private const int TimeInvtervalToAccept = 2000;

         private object Lock = new object();
        public yEmu.World.Core.Classes.Characters.Characters FirstTrader { get; private set; }
        private  static  List<InventoryItems> _firstTraderItems;
        private int FirstKamas { get; set; }
        private bool _firstValidated;

        public yEmu.World.Core.Classes.Characters.Characters SecondTrader { get; set; }
        private  static List<InventoryItems> _secondTraderItems;
        private int SecondKamas { get; set; }
        private bool _secondValidated;

        private DateTime _timer;

        public Exchanges(yEmu.World.Core.Classes.Characters.Characters firstTrader, yEmu.World.Core.Classes.Characters.Characters secondTrader)
        {

            FirstTrader = firstTrader;
            SecondTrader = secondTrader;

            _firstTraderItems = new List<InventoryItems>();
            _secondTraderItems = new List<InventoryItems>();

            _timer = DateTime.Now;
        }

        public void AddItem(yEmu.World.Core.Classes.Characters.Characters character, InventoryItems item, int quantity, AuthClient client,
     
            AuthClient receiverClient)
        {
            _timer = DateTime.Now;

            UnLockExchangeCases(client, receiverClient); // Update exchange board

            InventoryItems existItem;

            if (FirstTrader == character)
            {
                existItem =
                    _firstTraderItems.Find(
                        x =>
                            x.IDCharacter == item.IDCharacter && x.position == item.position &&
                            x.IDItems == item.IDItems && x.IDStats == item.IDStats);
            }
            else
            {
                existItem =
                    _secondTraderItems.Find(
                        x =>
                            x.IDCharacter == item.IDCharacter && x.position == item.position &&
                            x.IDItems == item.IDItems && x.IDStats == item.IDStats);
            }

            if (existItem != null)
            {
                existItem.quantity += quantity;

                client.Send(string.Format("{0}+{1}|{2}", "EMKO", item.id,
                    existItem.quantity));

                receiverClient.Send(string.Format("{0}+{1}", "EmKO",
                    item.ToExchangeFormat(existItem.quantity)));
            }
            else
            {
                var newItem = item.Copy(quantity: quantity);

                if (character == FirstTrader)
                    _firstTraderItems.Add(newItem);
                else
                    _secondTraderItems.Add(newItem);

                client.Send(string.Format("{0}+{1}|{2}", "EMKO", item.id,
                    newItem.quantity));

                receiverClient.Send(string.Format("{0}+{1}", "EmKO",
                    item.ToExchangeFormat(newItem.quantity)));
            }
        }


        public void RemoveItem(yEmu.World.Core.Classes.Characters.Characters character, InventoryItems item, int quantity, AuthClient client,
            AuthClient receiverClient)
        {
            _timer = DateTime.Now;

            UnLockExchangeCases(client, receiverClient);

            InventoryItems existItem;

            if (character == FirstTrader)
            {
                existItem =
                    _firstTraderItems.Find(
                        x =>
                            x.IDCharacter == item.IDCharacter && x.position == item.position &&
                            x.IDItems == item.IDItems && x.IDStats == item.IDStats);
            }
            else
            {
                existItem =
                    _secondTraderItems.Find(
                        x =>
                            x.IDCharacter == item.IDCharacter && x.position == item.position &&
                            x.IDItems == item.IDItems && x.IDStats == item.IDStats);
            }

            if (existItem == null) return;

            if (existItem.quantity >= quantity)
                existItem.quantity -= quantity;

            if (existItem.quantity == 0)
            {
                client.Send(string.Format("{0}-{1}", "EMKO", item.id));

                receiverClient.Send(string.Format("{0}-{1}", "EmKO", item.id));

                if (FirstTrader == character)
                    _firstTraderItems.Remove(existItem);
                else
                    _secondTraderItems.Remove(existItem);
            }
            else
            {
                client.Send(string.Format("{0}+{1}|{2}", "EMKO", item.id,
                    existItem.quantity));

                receiverClient.Send(string.Format("{0}+{1}", "EmKO",
                    item.ToExchangeFormat(existItem.quantity)));
            }
        }

        public void AddKamas(yEmu.World.Core.Classes.Characters.Characters character, int kamas, AuthClient client, AuthClient receiverClient)
        {
            _timer = DateTime.Now;

            UnLockExchangeCases(client, receiverClient);

            if (FirstTrader == character)
                FirstKamas = kamas;
            else
            {
                SecondKamas = kamas;
            }

            client.Send(string.Concat( "EMKG", kamas));
            receiverClient.Send(string.Concat("EmKG", kamas));
        }

        private void UnLockExchangeCases(AuthClient client, AuthClient receiverClient)
        {
            _firstValidated = false;
            _secondValidated = false;

            client.Send(string.Concat("EK", "0",
                    FirstTrader.id));

            receiverClient.Send(string.Concat("EK", "0",
                FirstTrader.id));

            client.Send(string.Concat("EK", "0",
                    SecondTrader.id));

            receiverClient.Send(string.Concat("EK", "0",
                SecondTrader.id));
        }

        private void LockExchangeCases(AuthClient client, AuthClient receiverClient, yEmu.World.Core.Classes.Characters.Characters character, bool validated)
        {
            client.Send(string.Format("{0}{1}{2}", "EK", validated ? "1" : "0",
                    character.id));

            receiverClient.Send(string.Format("{0}{1}{2}", "EK", validated ? "1" : "0",
                character.id));
        }

        private void ExchangeKamas()
        {
            lock(Lock)
            {
                FirstTrader.kamas -= FirstKamas;
                SecondTrader.kamas -= SecondKamas;

                FirstTrader.kamas += SecondKamas;
                SecondTrader.kamas += FirstKamas;
            }
          
        }

        public bool Accepted(yEmu.World.Core.Classes.Characters.Characters character, AuthClient client, AuthClient receiverClient)
        {
            if (DateTime.Now < _timer.AddMilliseconds(TimeInvtervalToAccept))
                return false;

            if (character == FirstTrader)
            {
                _firstValidated = _firstValidated == false;

                LockExchangeCases(client, receiverClient, FirstTrader, _firstValidated);
            }
            else
            {
                _secondValidated = _secondValidated == false;

                LockExchangeCases(client, receiverClient, SecondTrader, _secondValidated);
            }

            return _firstValidated && _secondValidated;
        }

        public void Swap()
        {
            var firstTraderClient = Server.AuthClient.Find(x => x.Character == FirstTrader);
            var secondTraderClient = Server.AuthClient.Find(x => x.Character == SecondTrader);

            if (firstTraderClient == null || secondTraderClient == null)
                return;

            try
            {



                foreach (var firstTraderItem in _firstTraderItems)
                    EchangeItem(firstTraderClient, secondTraderClient, firstTraderItem, FirstTrader);

                foreach (var secondTraderItem in _secondTraderItems)
                    EchangeItem(firstTraderClient, secondTraderClient, secondTraderItem, SecondTrader);

                ExchangeKamas();



                    firstTraderClient.Send(string.Format("{0}{1}|{2}", "Ow",
                    firstTraderClient.Character.GetCurrentWeight(), firstTraderClient.Character.GetMaxWeight()));

                    secondTraderClient.Send(string.Format("{0}{1}|{2}", "Ow",
                    secondTraderClient.Character.GetCurrentWeight(), secondTraderClient.Character.GetMaxWeight()));

                firstTraderClient.Send(firstTraderClient.Character.GetStats());
                secondTraderClient.Send(secondTraderClient.Character.GetStats());

                firstTraderClient.Send("EVa");
                secondTraderClient.Send("EVa");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.HResult + e.Source + e.StackTrace);

                firstTraderClient.Send("EV");
                secondTraderClient.Send("EV");
            }
        }

        private void EchangeItem(AuthClient firstTraderClient, AuthClient secondTraderClient, InventoryItems item,
            yEmu.World.Core.Classes.Characters.Characters trader)
        {
            var existItemSecondTrader = InventoryItems.ExistItem(item, SecondTrader);
            var existItemFirstTrader = InventoryItems.ExistItem(item, FirstTrader);

            if (FirstTrader == trader)
            {
                RemoveTraderItem(existItemFirstTrader, firstTraderClient, item);
                CreateTraderItem(SecondTrader, existItemSecondTrader, secondTraderClient, item);
            }
            else
            {
                RemoveTraderItem(existItemSecondTrader, secondTraderClient, item);
                CreateTraderItem(FirstTrader, existItemFirstTrader, firstTraderClient, item);
            }
        }

        private void RemoveTraderItem(InventoryItems existItemTrader, AuthClient traderClient, InventoryItems item)
        {
            Console.WriteLine("a");
            Console.WriteLine("aaad :"+existItemTrader.quantity);
            Console.WriteLine("saaad : "+item.quantity);

            existItemTrader.quantity -= item.quantity;
            Console.WriteLine("b");
            if (existItemTrader.quantity <= 0)
            {
                Console.WriteLine("c");

                traderClient.Send(string.Format("{0}{1}", "OR", existItemTrader.id));
                Console.WriteLine("d");


                InventoryItem.Inventory.Remove(existItemTrader);
                Console.WriteLine("e");

              //  InventoryItemRepository.Remove(existItemTrader, true);
            }
            else
            {
                Console.WriteLine("f");

                traderClient.Send(string.Format("{0}{1}|{2}", "OQ", existItemTrader.id,
                    existItemTrader.quantity));
                Console.WriteLine("g");

               // InventoryItemRepository.Update(existItemTrader);
            }
        }

        private void CreateTraderItem(yEmu.World.Core.Classes.Characters.Characters trader, InventoryItems existItemTrader, AuthClient traderClient,
            InventoryItems item)
        {
            if (existItemTrader != null)
            {
                existItemTrader.quantity += item.quantity;

                traderClient.Send(string.Format("{0}{1}|{2}", "OQ", existItemTrader.id,
                    existItemTrader.quantity));

              //  InventoryItemRepository.Update(existItemTrader);
            }
            else
            {
                var newItem = item.Copy(quantity: item.quantity);

                newItem.IDCharacter = trader;

                traderClient.Send(string.Format("{0}{1}", "OAKO", newItem.ItemInfo()));

                InventoryItem.Inventory.Add(newItem);
            }
        }
    }
}
