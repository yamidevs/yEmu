﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core;
using yEmu.World.Core.Classes.Accounts;

namespace yEmu.Network
{
    public class AuthClient : Client
    {
        public Accounts Accounts
        {
            get;
            set;
        }

        public Processor Processor
        {
            get;
            set;
        }
        public AuthClient(Socket socket, Server server)
            : base(socket, server)
        {
            Processor  = new Processor(this);
            Send("HG");

        }
     
        public override bool DataArriavls(byte[] data)
        {
            if (data.Length == 0)
            {
                return false;
            }
            foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                Console.WriteLine("PACKET  : " + packet);
                Processor.Parser(packet);
            }
            return true;
        }

        public void Send(string Messages)
        {
            base.send(Messages);
        }

        public void Disconnect(Client client)
        {
            Console.WriteLine("Client deconnecter du game ");
            this._server.Disconnect(client);
        } 
    }
}