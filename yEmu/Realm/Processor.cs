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
        public Client _client;
        public RealmStats VerifRealm;
        public Account getAccounts;

        public Processor(Client client)
        {
            _client = client;
        }
        public void Parser(string data)
        {
            switch (VerifRealm)
            {

                case RealmStats.Version:
                    if (Constant.Version != data)
                    {
                        _client.Send(new VersionClient().ToString());
                        _client._server.OnClose();
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
                    _client.Send(new QueueList(this).ToString());
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
                    _client.Send(new ServerList(this).ToString());
                    break;
            }
        }
  
        public void CheckAccounts(string data)
        {
        var accounts = data.Split('#');
        
        if ( data.Contains('#') || accounts.Length == 2)
        {
             getAccounts = Accounts.getAccounts(accounts[0]);
            if (getAccounts != null)
            {                
                if (Hash.Encrypt(getAccounts.pass, _client._key) == accounts[1])
                {
                    if (Queue.Clients.Count >= Configuration.getInt("Queue_client"))
                    {
                        _client.Send(new QueueFlood().ToString());
                        _client._server.OnClose();
                    }
                    else if ((Environment.TickCount - Queue.action) < Configuration.getInt("Time_connexion"))
                    {
                        VerifRealm = RealmStats.Queue;
                        Queue.add(this);

                        _client.Send(new QueueList(this).ToString());
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
                    _client.Send(new ErrorAuth().ToString()); 
                }
            }
            else
            {
                _client.Send(new ErrorAuth().ToString());
            }
         }

        public void SendInformations()
        {
            _client.Send(string.Format("{0}{1}", "Ad", getAccounts.pseudo));
            _client.Send(string.Format("{0}{1}", "Ac", 0)); // 0 : communauté fr

            RefreshServerList();

            _client.Send(string.Format("{0}{1}", "AlK", getAccounts.gmLevel > 0 ? 1 : 0));
            _client.Send(string.Concat("AQ", getAccounts.question));
    
        }
        public void RefreshServerList()
        {
            _client.Send(string.Format("{0}{1}", "AH", string.Join("|", GameServers.servers)));
        }
              

     }
   }
