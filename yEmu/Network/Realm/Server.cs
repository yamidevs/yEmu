using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Network.Realm;
using yEmu.Util;

namespace yEmu.Network
{
    class Server : TCPServer
    {
        public List<Client> Clients { get; private set; }
        public static Object Lock = new Object();

       
        public Server()
        {
            Clients = new List<Client>();

        }

        public void StartServer(IPEndPoint listeningAdress, int maxco)
        {
            LisenAdress = listeningAdress;
            maxConnexion = maxco;
            base.Start();
            base.connected += this.Connexion;
            Queue.Start();
        }
        
        public void Connexion(ServerManager socket )
        {
            var client = new Client(socket);
            lock (Lock)
                Clients.Add(client);
            Info.Write("", string.Format(" Nouvelle Connection < {0} >", socket.ip(socket)), ConsoleColor.Gray);

            socket.OnSocketClose += () =>
            {
                lock (Clients)
                    Clients.Remove(client);
                client.Dispose(true);
            };

        }
   
     
    

    }

}
