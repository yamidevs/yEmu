using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.Network.Realm;
using yEmu.Realm.Classes;
using yEmu.Realm.Databases.Requetes;
using yEmu.Realm.enums;
using yEmu.Realm.Packets;
using yEmu.Realm.Packets.Auth;
using yEmu.Util;

namespace yEmu.Realm
{
   public class Processor
    {
       public AuthClient Client
        {
            get;
            set;
        }

        public RealmStats VerifRealm 
        { 
            get;
            set; 
        }

        public Account GetAccounts 
        {
            get;
            set;
        }

        public Processor(AuthClient client)
        {
            Client = client;
        }

        public void Parser(string data)
        {
            switch (VerifRealm)
            {

                case RealmStats.Version:
                    if (Constant.Version != data)
                    {
                        Client.Send(new VersionClient().ToString());
                        Client._server.Disconnect(Client);
                    }
                    else
                    {
                        VerifRealm = RealmStats.Account;
                    }
                    
                    break;
                case RealmStats.Account:
                    CheckAccounts(data);
                    break;
                case RealmStats.Queue:
                    Client.Send(new QueueList(this).ToString());
                    return;
                case RealmStats.Server:
                    VerificateList(data);
                    break;

            }
        }
        public void VerificateList(string data)
        {
            switch (data.Substring(0, 2))
            {
                case "Ax":
                    Client.Send(new ServerList(this).ToString());
                    break;
        
                case "AF":
                    Client.Send("AF");
                    return;
                case"AYK":
                    break;
                case "AX":
                    InterCommunication(data);
                    break;
            }
        }
  
        public void CheckAccounts(string data)
        {
        var accounts = data.Split('#');
        
        if ( data.Contains('#') || accounts.Length == 2)
        {
            GetAccounts = Accounts.getAccounts(accounts[0]);
            if (GetAccounts != null)
            {
                if (Hash.Encrypt(GetAccounts.pass, Client.Key) == accounts[1])
                {
                    if (Queue.Clients.Count >= Configuration.getInt("Queue_client"))
                    {
                        Client.Send(new QueueFlood().ToString());
                        Client._server.Disconnect(Client);
                    }
                    else if ((Environment.TickCount - Queue.Action) < Configuration.getInt("Time_connexion"))
                    {
                        VerifRealm = RealmStats.Queue;
                        Queue.add(this);

                        Client.Send(new QueueList(this).ToString());
                    }
                    else
                    {
                        SendInformations();
                        VerifRealm = RealmStats.Server;
                    }
                    Queue.Action = Environment.TickCount;
                }
             }
             else
               {
                   Client.Send(new ErrorAuth().ToString()); 
                }
            }
            else
            {
                Client.Send(new ErrorAuth().ToString());
            }
         }

        public void SendInformations()
        {
            Client.Send(string.Format("{0}{1}", "Ad", GetAccounts.pseudo));
            Client.Send(string.Format("{0}{1}", "Ac", 0)); // 0 : communauté fr

            RefreshServerList();

            Client.Send(string.Format("{0}{1}", "AlK", GetAccounts.gmLevel > 0 ? 1 : 0));
            Client.Send(string.Concat("AQ", GetAccounts.question));
    
        }

        public void RefreshServerList()
        {
            var packet = string.Concat("AH",
               string.Join("|", GameServers.servers));
            Client.Send(packet);
        }

        private void InterCommunication(string packet)
        {
            var serverId = Int16.Parse(packet.Substring(2));

            var gameServer = GameServers.servers.Single(gS => gS.id == serverId);

            if (gameServer == null)
            {
                return;
            }
            var key = Hash.RandomString(16);

            packet = string.Format("AYK{0}:{1};{2}", gameServer.ip, gameServer.port, GenerateTicketKey(Client.Sock, GetAccounts));
            Client.Send(packet);

        }

        public  string GenerateTicketKey(Socket socket, Account account)
        {
                IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;

                var ticketKey = string.Format("{0}|{1}|{2}",
                    (DateTime.Now).ToUniversalTime().ToString(CultureInfo.InvariantCulture),
                        remoteIpEndPoint, account);

                return ticketKey; ;
        }           
     }
   }
