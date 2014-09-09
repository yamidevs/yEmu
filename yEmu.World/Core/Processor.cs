using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.World.Core.Attributes;
using yEmu.World.Core.Classes.Accounts;
using yEmu.World.Core.Enums;
using yEmu.World.Core.Databases.Requetes;
using yEmu.Util;
using System.Text.RegularExpressions;
using yEmu.World.Core.Classes.Characters;
using yEmu.World.Core.Classes.Maps;
using System.Runtime.CompilerServices;
using yEmu.World.Core.Classes.Items;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Handler;
using System.Threading;

namespace yEmu.World.Core
{
   public class Processor
    {
       public static object Lock = new object();
       private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

       public  Dictionary<string, Action<string>> PacketHandler
       {
           get;
          private set;
       }
 
       public  AuthClient  Clients
        {
            get;
            set;
        }

       public  CharacterState CharacterStates
       {
           get;
           set;
       }

       public  WorldStats _stats
       {
           get;
           set;
       }

       public HandlerPacket HandlerPackets
       {
           get;
           set;
       }


       public Processor(AuthClient client)
        {
           
            Clients = client;
            HandlerPackets = new HandlerPacket(this);
            PacketHandler = new Dictionary<string, Action<string>>()
            {
              { "AV", Version },
              { "AL", ListCharacters },
              { "AP", GeneratePseudo },
              { "AA", CreateCharacters },
              { "AS", SendInfos },
              { "GI", MapsInfos },
              { "GA", StartRequest },
              { "GK", FinishRequest },
              { "eD", ChangeDirection },
              { "BS", Smiley },
              { "BM", Messages },
              { "AB", BoostStats },
              { "OM", MoveItems },
              { "BD", Dates },
              { "ER", BuysItems },
              { "EB", ExchangeBuy },
              { "EV", Cancel },
              { "WV", ZaapCancel },
              { "WU", ZaapUse },
              { "Wu", ZaapiUse },
              { "Wv", ZaapiCancel },
              { "EA", Exchange },
              { "EM", ExchangeMove },
              { "EK", ExchangeAccept },
              { "PI", PartyInvite },
              { "PR", PartyRefuse },
              { "PA", PartyAccepted },
              { "Er", EnclosReceive },
              { "DC", DialogueCreate },
              { "DV", DialogueExit },
              { "DR", DialogueReaplye },

            };
   
        }


       public void Parser(string packet)
        {
            var header = packet.Substring(0, 2);

            if (header == "AT")
            {
                HandlerPackets.ReponseParse(packet.Substring(2));
            }

            switch (_stats)
            {                  
                case WorldStats.Personnages:

                    if (this.PacketHandler.ContainsKey(header))
                    {
                        this.PacketHandler[header](packet.Substring(2));
                    }
                    break;

                case WorldStats.CreateGame:
                    if (header == "GC")
                        HandlerPackets.GameInfos();

                    break;

                case WorldStats.InGame:

                    if (this.PacketHandler.ContainsKey(header))
                    {
                        this.PacketHandler[header](packet.Substring(2));
                    }
                    break;
            }
        }

       [Packet("AV")]
       public  void Version(string data)
        {
            Clients.Send(string.Format("{0}{1}", "AV", "0"));
        }

       [Packet("AL")]
       public  void ListCharacters(string data)
        {
            cacheLock.EnterReadLock();

            try
            {
                var count = Character.characters.Count(x => x.accounts == Clients.Accounts.Id);
                var personnages = Character.characters.Where(x => x.accounts == Clients.Accounts.Id).
                    Aggregate(string.Empty,
                            (current, x) =>
                                current + x.InfosCharacter());
                Clients.Send(string.Format("{0}{1}|{2}{3}", "ALK", "31536000000", count, personnages));

            }
            finally
            {
                cacheLock.ExitReadLock();
            }
               
        }

       [Packet("AP")]
       public  void GeneratePseudo(string data)
        {
                Clients.Send(string.Format("{0}{1}", "APK",
                                Algorithme.GenerateRandomName()));                        
       
        }

       [Packet("AA")]
       public  void CreateCharacters(string data)
       {
           new CharacterHandler().Character(this, data);
       }

       [Packet("AS")]
       public  void SendInfos(string data)
       {
           cacheLock.EnterReadLock();

           try
           {
               var character = Character.characters.Find(x => x.id == int.Parse(data));

               if (character == null)
               {
                   return;
               }
               character.Connected = true;
               Clients.Character = character;

               Clients.Send(string.Format("{0}|{1}", "ASK", character.SelectedCharacters()));
               _stats = WorldStats.CreateGame;
           }
           finally
           {
               cacheLock.ExitReadLock();
           }
       }

       [Packet("GI")]
       public  void MapsInfos(string data)
       {
           Clients.Character.GetMap().Add(Clients, Clients.Character);
           Clients.Character.GetMap().Send(Clients,string.Format("{0}{1}", "GM", Clients.Character.GetMap().DisplayChars(Clients)));
           Clients.Send("GDK");

       }

       [Packet("GA")]
       public void StartRequest(string data)
       {
           Console.WriteLine((GameAction)int.Parse(data.Substring(0, 1)));

           switch ((GameAction)int.Parse(data.Substring(0, 1)))
           {
               case GameAction.MAP_MOVEMENT:
                   HandlerPackets.CharacterMove(data);
                   break;
               case GameAction.MAP_PUSHBACK:
                   var result = data.Split(';')[1];

                   if (result.Equals("114"))
                   {
                       new ZaapHandler().ZaapAction(this, data);
                   }
                   else if (result.Equals("157"))
                   {
                       new ZaapHandler().ZaapiAction(this, data);
                   }
                   else if (result.Equals("175"))
                   {
                       Clients.Send("ECK16|~");
                   }
                   break;
           }
       }

       [Packet("GK")]
       public void FinishRequest(string data)
       {
           switch (data.Substring(0, 1))
           {
               case "E":
                   HandlerPackets.ChangeDestination(data);
                   break;

               case "K":
                   HandlerPackets.CharacterEndMove();
                   break;
           }
       }

       [Packet("eD")]
       public  void ChangeDirection(string data)
       {
           int direction;

           if (!int.TryParse(data, out direction))
           {
               return;
           }

           if (direction < 0 | direction > 7)
           {
               Console.WriteLine("direction");
               return;
           }
           Clients.Character.Maps.Send(Clients,string.Format("{0}{1}|{2}", "eD", Clients.Character.id, direction));
       }

       [Packet("BS")]
       public  void Smiley(string data)
       {
           int smiley;

           if (!int.TryParse(data, out smiley))
           {
               return;
           }

           if (smiley < 1 | smiley > 15)
           {
               return;
           }
           Clients.Character.Maps.Send(Clients,string.Format("{0}{1}|{2}","cS", Clients.Character.id, data));
       }

       [Packet("BM")]
       public  void Messages(string data)
       {
           var infos = data.Split('|');

           var channel = infos[0];
           var message = infos[1];

           new ChannelHandler().Messages(this, channel, message);
       }

       [Packet("AB")]
       public void BoostStats(string data)
       {
           var baseStats = int.Parse(data);
           if (baseStats < 10 || baseStats > 15)
           {
               return;

           }

           Clients.Character.BoostStats((Characters.BaseStats)baseStats);
           Clients.Send(string.Format("{0}{1}|{2}", "Ow", Clients.Character.GetCurrentWeight(), Clients.Character.GetMaxWeight()));
           Clients.Send( Clients.Character.GetStats());


       }

       [Packet("OM")]
       public void MoveItems(string data)
       {

           var itemId = int.Parse(data.Split('|')[0]);

           var itemPosition = (Position)int.Parse(data.Split('|')[1]);

           new ItemsHandler().MoveItems(this, itemId, itemPosition,data);

       }

       [Packet("BD")]
       public void Dates(string data)
       {
           Clients.Send(string.Format("BD{0}|{1}|{2}", (DateTime.Now.Year - 1370).ToString(), (DateTime.Now.Month - 1), (DateTime.Now.Day)));
       }

       [Packet("ER")]
       public void BuysItems(string data)
       {
           if (!data.Contains('|'))
           {
               Clients.Send("BN");
               return;
           }

           var packet = data.Split('|');

           if (packet.Length != 2)
           {
               Clients.Send("BN");
               return;
           }

           var ID = 0;
           var receiverID = 0;

           if (!int.TryParse(packet[0], out ID) || !int.TryParse(packet[1], out receiverID))
               return;

           switch (ID)
           {
               case 0: // npc
                   new NPCHandler().BuyNpc(this, receiverID);
                   break;
               case 1:
                   new ExchangeHandler().ExchangeStart(this, receiverID, ID);

                   break;
           }
       }

       [Packet("EB")]
       public void ExchangeBuy(string data)
       {
           var datas = data.Split('|');
           var itemID = 0;
           var quantity = 1;

           if (!int.TryParse(datas[0], out itemID) || !int.TryParse(datas[1], out quantity))
               return;

           new NPCHandler().ExchangeBuy(this, itemID, quantity);
        
       }

       [Packet("EV")]
       public void Cancel(string data)
       {
           if (Clients.Character.Exchange == true)
           {
               ExchangeLeave(data);
               return;
           }
           Clients.Send("EV");
       }

       [Packet("WV")]
       public void ZaapCancel(string data)
       {
           Clients.Send("WV");
       }

       [Packet("WU")]
       public void ZaapUse(string data)
       {
           new ZaapHandler().ZaapUse(this,data);
       }

       [Packet("Wu")]
       public void ZaapiUse(string data)
       {
           new ZaapHandler().ZaapiUse(this, data);
       }

       [Packet("Wv")]
       public void ZaapiCancel(string data)
       {
           Clients.Send("Wv");
       }

       [Packet("EA")]
       public void Exchange(string data)
       {
           new ExchangeHandler().Exchange(this, data);
       }

       [Packet("EM")]
       public void ExchangeMove(string data)
       {
           new ExchangeHandler().ExchangeMove(this, data);
       }

       [Packet("EK")]
       public void ExchangeAccept(string data)
       {
           new ExchangeHandler().ExchangeAccept(this, data);
       }

       [Packet("EV")]
       public void ExchangeLeave(string data)
       {
           new ExchangeHandler().ExchangeLeave(this, data);
       }

       [Packet("PI")]
       public void PartyInvite(string data)
       {
           PartyHandler PartyHandler = new PartyHandler();
           PartyHandler.PartyInvite(this, data);
           
       }

       [Packet("PR")]
       public void PartyRefuse(string data)
       {
           var PartyHandler = new PartyHandler();
           PartyHandler.PartyRefuse(this, data);
       }

       [Packet("PA")]
       public void PartyAccepted(string data)
       {
           var PartyHandler = new PartyHandler();
           PartyHandler.PartyAccepted(this, data);
       }

       [Packet("Er")]
       public void EnclosReceive(string data)
       {
           MountHandler MountHandler = new MountHandler();
           MountHandler.EnclosReceive(this, data);
       }

       [Packet("DC")]
       public void DialogueCreate(string data)
       {
           var NPCHandler = new NPCHandler();
           NPCHandler.DialogueCreate(this, data);
       }

       [Packet("DE")]
       public void DialogueExit(string data)
       {
           Clients.Send("DV");
           Clients.Character.Dialogue = -1;
       }

       public void DialogueReaplye(string data)
       {
           var NPCHandler = new NPCHandler();
           NPCHandler.DialogueReaplye(this, data);
       }
    }
}
