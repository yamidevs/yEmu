using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network.Realm;

namespace yEmu.Network
{
    public class TCPServer : Server
    {
        public TCPServer(IPEndPoint IPEndPoints, int IsListens)
        {
            IPEndPoint = IPEndPoints;
            IsListen = IsListens;

        }

        public void Run()
        {
            Console.WriteLine("Loading connexion auth ");

            base.Start();
            Queue.Start();
        }

        public override Client CreateClient(Socket sock, Server server)
        {
            return new AuthClient(sock, server);
        }
    }
}
