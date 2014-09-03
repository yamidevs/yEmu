using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.World.Core.Classes;
using yEmu.World.Core.Classes.Characters;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Handler
{
    public class ExchangeHandler
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private object Lock = new object();

       static  List<Exchanges> Exchanges = new List<Exchanges>();

        public void ExchangeStart(Processor Processor, int receiverID, int request)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (Character.characters.Any(x => x.id == receiverID))
                {
                    var character = Character.characters.FirstOrDefault(x => x.id == receiverID);

                    if (character == null)
                    {
                        Processor.Clients.Send("EREE");
                    }
                    var client = Server.AuthClient.Find(x => x.Character == character);

                    if (client == null)
                    {
                        Processor.Clients.Send("EREE");
                    }
                    if (client.Character.Maps.ID != Processor.Clients.Character.Maps.ID)
                    {
                        Processor.Clients.Send("EREE");
                    }

                    Processor.Clients.Character.Exchange = true;
                    Processor.Clients.Character.CharacterExchange = character;

                    client.Character.Exchange = true;
                    client.Character.CharacterExchange = Processor.Clients.Character;

                    client.Send(string.Format("{0}{1}|{2}|{3}", "ERK", Processor.Clients.Character.id, receiverID, request));
                    Processor.Clients.Send(string.Format("{0}{1}|{2}|{3}", "ERK", Processor.Clients.Character.id, receiverID, request));

                }
            }
            finally
            {
                cacheLock.ExitReadLock();
                cacheLock.Dispose();
            }
          
        }

        public void Exchange(Processor Processor, string data)
        {
            if (Processor.Clients.Character.Exchange != true)
            {
                return;
            }

            var receive = Processor.Clients.Character.CharacterExchange;
            var receiverClient = Server.AuthClient.Find(x => x.Character == receive);
            if (receive == null)
            {
                return;
            }

            lock (Lock)
            {
                Exchanges.Add(new Exchanges(Processor.Clients.Character, receiverClient.Character));
            }

            receiverClient.Send(string.Concat("ECK", "1"));

            Processor.Clients.Send(string.Concat("ECK", "1"));
        }

        public void ExchangeMove(Processor Processor, string data)
        {
            var id = data[0];

            var receiverCharacter = Processor.Clients.Character.CharacterExchange;

            var receiverClient = Server.AuthClient.Find(x => x.Character == receiverCharacter);

            if (receiverClient == null)
                return;

            var exchangeSession =
                Exchanges.Find(
                    x => x.FirstTrader == Processor.Clients.Character && x.SecondTrader == receiverClient.Character
                         || x.FirstTrader == receiverClient.Character && x.SecondTrader == Processor.Clients.Character);

            if (exchangeSession == null)
                return;

            switch (id)
            {
                case 'O':
                    ExchangeMoveObject(Processor,data.Substring(1), receiverClient, exchangeSession);
                    break;
                case 'G':
                    ExchangeMoveKamas(Processor,data.Substring(1), receiverClient, exchangeSession);
                    break;
            }
        }

        private void ExchangeMoveKamas(Processor Processor,string data, AuthClient receiverClient, Exchanges exchangeSession)
        {
            if (receiverClient.Character.Exchange != true)
            {
                return;
            }
            var kamas = int.Parse(data);
            if (Processor.Clients.Character.kamas >= kamas)
                exchangeSession.AddKamas(receiverClient.Character, kamas, Processor.Clients, receiverClient);
        }


        private void ExchangeMoveObject(Processor Processor, string data, AuthClient receiverClient, Exchanges exchangeSession)
        {
            if (Processor.Clients.Character.Exchange != true)
            {
                return;
            }

            var state = data[0];
            var itemId = int.Parse(data.Substring(1).Split('|')[0]);
            var quantity = int.Parse(data.Substring(1).Split('|')[1]);

            var item =
                InventoryItem.Inventory.Find(
                    x => x.IDCharacter == Processor.Clients.Character && x.id == itemId && x.quantity >= quantity);

            if (item == null)
                return;

            if (item.position != Position.None)
            {
                return;
            }

            switch (state)
            {
                case '+':
                    exchangeSession.AddItem(Processor.Clients.Character, item, quantity, Processor.Clients, receiverClient);
                    break;
                case '-':
                    exchangeSession.RemoveItem(Processor.Clients.Character, item, quantity, Processor.Clients, receiverClient);
                    break;
            }
        }

        public void ExchangeAccept(Processor Processor , string data)
        {
            if (Processor.Clients.Character.Exchange != true)
            {
                return;
            }

            var receiverCharacter = Processor.Clients.Character.CharacterExchange;

            var receiverClient = Server.AuthClient.Find(x => x.Character == receiverCharacter);

            if (receiverClient == null) 
                return;

            var exchangeSession =
                Exchanges.Find(
                    x => x.FirstTrader == Processor.Clients.Character && x.SecondTrader == receiverClient.Character
                         || x.FirstTrader == receiverClient.Character && x.SecondTrader == Processor.Clients.Character);

            if (exchangeSession == null) return;

            var finishedExchange = exchangeSession.Accepted(Processor.Clients.Character, Processor.Clients, receiverClient);

            if (!finishedExchange) return;

            exchangeSession.Swap();

            lock (Exchanges)
                Exchanges.Remove(exchangeSession);

            receiverClient.Character.CharacterExchange = null;

            Processor.Clients.Character.CharacterExchange = null;

            receiverCharacter.Exchange = false;
            Processor.Clients.Character.Exchange = false;
        }

        public void ExchangeLeave(Processor Processor, string data)
        {
            if (Processor.Clients.Character.Exchange != true)
            {
                return;
            }

            var receiverCharacter = Processor.Clients.Character.CharacterExchange ;

            if (receiverCharacter != null)
            {
                receiverCharacter.CharacterExchange = null;
                Processor.Clients.Character.CharacterExchange = null;
                receiverCharacter.Exchange = false;
                Processor.Clients.Character.Exchange = false;
                
            }
                

            var receiverClient = Server.AuthClient.Find(x => x.Character == receiverCharacter);

            var exchangeSession =
                Exchanges.Find(
                    x => x.FirstTrader == Processor.Clients.Character && x.SecondTrader == receiverClient.Character
                         || x.FirstTrader == receiverClient.Character && x.SecondTrader == Processor.Clients.Character);

            if (exchangeSession != null)
            {
                lock (Exchanges)
                Exchanges.Remove(exchangeSession);
            }

            receiverClient.Send("EV");
            Processor.Clients.Send("EV");
        }
    }
}
