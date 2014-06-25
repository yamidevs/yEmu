using System;
using System.Collections.Generic;
using System.Linq;
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
    class Processor
    {
        public Client Client
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

        public Processor(Client client)
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
                        Client.ServerManager.OnClose();
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
            switch (data)
            {
                case "Ax":
                    Client.Send(new ServerList(this).ToString());
                    break;
        
                case "AF":
                    Client.Send("AF");
                    return;
                case"AYK":
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
                        Client.ServerManager.OnClose();
                    }
                    else if ((Environment.TickCount - Queue.action) < Configuration.getInt("Time_connexion"))
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
                    Queue.action = Environment.TickCount;
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
              

     }
   }
