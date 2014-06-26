using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Collections;
using yEmu.Network.Realm;
using yEmu.Util;

namespace yEmu.Network
{
    class Server : TCPServer
    {
        public ConcurrentList<Client> Clients 
        { 
            get;
            private set;
        }

        private static Object Lock = new Object();
      
        public Server()
        {
            Clients = new ConcurrentList<Client>();

        }

        public void StartServer(IPEndPoint listeningAdress, int maxco)
        {
            LisenAdress = listeningAdress;
            MaxConnexion = maxco;
            base.Start();
            base.Connected += this.Connexion;
            Queue.Start();
            Info.Write("", "Connexion Realm Servers", ConsoleColor.DarkRed);
        }
        
        public void Connexion(ServerManager socket )
        {
            var client = new Client(socket);
                Clients.Add(client);
            Info.Write("", string.Format(" Nouvelle Connection < {0} >", socket.Ip(socket)), ConsoleColor.Gray);

            socket.OnSocketClose += () =>
            {
                    Clients.Remove(client);
                client.Dispose(true);
            };
        }            
    }
}
