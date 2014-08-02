using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Collections;
using yEmu.Network;
using yEmu.Util;

namespace yEmu.InterCommunication
{
    public class InterServer : Server
    {
        public InterServer(IPEndPoint IPEndPoints, int IsListens)
        {
            IPEndPoint = IPEndPoints;
            IsListen = IsListens;

        }

        public void Run()
        {
            Console.WriteLine("Loading connexion Intercommunication");

            base.Start();
        }

        public override Client CreateClient(Socket sock, Server server)
        {
            return new InterClient(sock, server);
        }
    }

  }

