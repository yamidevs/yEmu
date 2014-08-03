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

namespace yEmu.World.Core
{
   public class Processor
    {

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

        private WorldStats _stats;

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

            switch (_stats)
            {
                case WorldStats.Parse :

                    if (header == "AT")
                    {
                        ReponseParse(packet.Substring(2));
                    }

                    break;

                case WorldStats.Personnages:

                    if (this.PacketHandler.ContainsKey(header))
                    {
                        this.PacketHandler[header](packet.Substring(2));
                    }

                    break;
            }
        }

        private void ReponseParse(string data)
        {
            var date = data.Split('|')[0];

            var ip = data.Split('|')[1];
            IPEndPoint Ip = Processor.Clients.Sock.RemoteEndPoint as IPEndPoint;

           /* if (DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss", null).AddSeconds(10) <
                DateTime.ParseExact(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), "MM/dd/yyyy HH:mm:ss", null))
            {
                this.Client.Send("M130");
                this.Client.Disconnect(Client);
            }
            else */if (ip.Equals(Ip))
            {
                Processor.Clients.Send("M031");
                Processor.Clients.Disconnect(Clients);
            }
            else
            {
                var account = data.Split('|')[2].Split(',');
                try
                {
                    Processor.Clients.Accounts =
                                        new Accounts
                                        {
                                            Id = int.Parse(account[0]),
                                            Username = account[1],
                                            Password = account[2],
                                            Pseudo = account[3],
                                            Question = account[4],
                                            Reponse = account[5],
                                            Level = int.Parse(account[6]),
                                            BannedUntil =
                                                account[7] == ""
                                                    ? (DateTime?)null
                                                    : DateTime.Parse(account[7].ToString(CultureInfo.InvariantCulture)),
                                            Subscription =
                                                account[8] == ""
                                                    ? (DateTime?)null
                                                    : DateTime.Parse(account[8].ToString(CultureInfo.InvariantCulture))

                                        };
                }
                catch
                {
                    Console.WriteLine("Packet d'account incorrect");
                }

                _stats = WorldStats.Personnages;
                Processor.Clients.Send("ATK0");
            }
        }

        [Packet("AV")]
        public static void Version(string data)
        {
            Processor.Clients.Send(string.Format("{0}{1}", "AV", "0"));
        }

        [Packet("AL")]
        public static void ListPersonnages(string data)
        {
            Processor.Clients.Send(string.Format("{0}{1}", "ALK", "0"));

        }
    }
}
