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

namespace yEmu.World.Core
{
   public class Processor
    {
       public static object Lock = new object();
       public  Dictionary<string, Action<string>> PacketHandler
       {
           get;
          private set;
       }
 
       public static AuthClient  Clients
        {
            get;
            set;
        }

       public static CharacterState CharacterStates
       {
           get;
           set;
       }

       public static WorldStats _stats
       {
           get;
           set;
       }

       public Processor(AuthClient client)
        {
            Clients = client;
            PacketHandler = new Dictionary<string, Action<string>>();
            this.Inits();
   
        }

       public void Inits()
       {
           var methods = from type in typeof(Processor).Assembly.GetTypes()
                         from method in type.GetMethods()
                         where method.GetCustomAttribute(typeof(PacketAttribute), false) != null
                         select method;

           foreach (var item in methods)
           {
               var attribute = item.GetCustomAttribute<PacketAttribute>();

               Action<string> action = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), item);
               PacketHandler.Add(attribute.PacketData, action);

           }
       }

       public void Parser(string packet)
        {
            var header = packet.Substring(0, 2);

            if (header == "AT")
            {
                HandlerPacket.ReponseParse(packet.Substring(2));
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
                        HandlerPacket.GameInfos();

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
       public static void Version(string data)
        {
            Processor.Clients.Send(string.Format("{0}{1}", "AV", "0"));
        }

       [Packet("AL")]
       public static void ListCharacters(string data)
        {
            lock (Processor.Lock)
            {
                var count = Character.characters.Count(x => x.accounts == Processor.Clients.Accounts.Id);
                var personnages = Character.characters.Where(x => x.accounts == Processor.Clients.Accounts.Id).
                    Aggregate(string.Empty,
                            (current, x) =>
                                current + x.InfosCharacter());
                Processor.Clients.Send(string.Format("{0}{1}|{2}{3}", "ALK", "31536000000", count, personnages));
            }

        }

       [Packet("AP")]
       public static void GeneratePseudo(string data)
        {
            lock (Processor.Lock)
            {
                Processor.Clients.Send(string.Format("{0}{1}", "APK",
                                Algorithme.GenerateRandomName()));
            }
            
       
        }

       [Packet("AA")]
       public static void CreateCharacters(string data)
       {
           if (Character.characters.FindAll(x => x.accounts == Processor.Clients.Accounts.Id).Count >= int.Parse(Configuration.getString("PersonnagesComptes")))
           {
               Processor.Clients.Send("AAEf");
               return;
           }

           var datas = data.Split('|');

           var name = datas[0];

           var classe = int.Parse(datas[1]);

           var sex = int.Parse(datas[2]);

           var color1 = int.Parse(datas[3]);

           var color2 = int.Parse(datas[4]);

           var color3 = int.Parse(datas[5]);

           if (Character.characters.All(x => x.nom != name) && name.Length >= 3 && name.Length <= 20)
           {
               var reg = new Regex("^[a-zA-Z-]+$");

               if (reg.IsMatch(name) && name.Count(c => c == '-') < 3)
               {
                   if (classe >= 1 && classe <= 12 && (sex == 1 || sex == 0))
                   {
                       var newCharacter = new Characters
                       {
                           id =
                               Character.characters.Count > 0
                                   ? Character.characters.OrderByDescending(x => x.id).First().id + 1
                                   : 1,
                           nom = name,
                           Classes = (Class)classe,
                           sexe = sex,
                           color1 = color1,
                           color2 = color2,
                           color3 = color3,
                           level = int.Parse(Configuration.getString("Start_level")),
                           skin = int.Parse(classe + "" + sex),
                           accounts = Processor.Clients.Accounts.Id,
                           pdvNow =
                               (int.Parse(Configuration.getString("Start_level")) - 1) * Characters.GainHpPerLvl + Characters.BaseHp,
                           PdvMax =
                               (int.Parse(Configuration.getString("Start_level")) - 1) * Characters.GainHpPerLvl + Characters.BaseHp,
                       };

                       newCharacter.GenerateInfos(Processor.Clients.Accounts.Level);

                       Character.Create(newCharacter);

                       Processor.Clients.Send("AAK");

                       Processor.Clients.Send("TB");

                       ListCharacters("");
                   }
                   else
                   {
                       Processor.Clients.Send("AAEf");
                   }
               }
               else
               {
                   Processor.Clients.Send("AAEn");
               }
           }
           else
           {
               Processor.Clients.Send("AAEa");
           }

       }

       [Packet("AS")]
       public static void SendInfos(string data)
       {
           var character = Character.characters.Find(x => x.id == int.Parse(data));

           if (character == null)
           {
               return;
           }

           Processor.Clients.Character = character;

           Processor.Clients.Send(string.Format("{0}|{1}", "ASK" , character.SelectedCharacters()));
           _stats = WorldStats.CreateGame;
       }

       [Packet("GI")]
       public static void MapsInfos(string data)
       {
           Processor.Clients.Character.GetMap().Add(Processor.Clients.Character);
           Processor.Clients.Character.GetMap().Send(string.Format("{0}{1}", "GM", Processor.Clients.Character.GetMap().DisplayChars()));
           Processor.Clients.Send("GDK");

       }

       [Packet("GA")]
       public static void StartRequest(string data)
       {
           switch ((GameAction)int.Parse(data.Substring(0, 1)))
           {
               case GameAction.MAP_MOVEMENT:
                   HandlerPacket.CharacterMove(data);
                   break;
           }
       }

       [Packet("GK")]
       public static void FinishRequest(string data)
       {
           switch (data.Substring(0, 1))
           {
               case "E":
                   HandlerPacket.ChangeDestination(data);
                   break;

               case "K":
                   HandlerPacket.CharacterEndMove();
                   break;
           }
       }

       [Packet("eD")]
       public static void ChangeDirection(string data)
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
           Processor.Clients.Character.Maps.Send(string.Format("{0}{1}|{2}", "eD", Processor.Clients.Character.id, direction));
       }

       [Packet("BS")]
       public static void Smiley(string data)
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
           Processor.Clients.Character.Maps.Send(string.Format("{0}{1}|{2}","cS", Processor.Clients.Character.id, data));
       }

       [Packet("BM")]
       public static void Messages(string data)
       {
           var infos = data.Split('|');

           var channel = infos[0];
           var message = infos[1];

           switch (channel)
           {
               case "*":
                   HandlerPacket.GeneralMessages(message);
                   return;

               case "?":
                   HandlerPacket.RecrutementMessages(message);
                   break;

               case ":":
                   HandlerPacket.TradeMessages(message);
                   break;

               default :
                   if (channel.Length > 1)
                   {
                       HandlerPacket.PrivateMessages(channel, message);
                   }
                   break;
           }
       }
    }
}
