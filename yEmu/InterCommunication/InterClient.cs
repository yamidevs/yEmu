using Ngot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Enums;
using yEmu.Network;
using yEmu.Realm.Databases.Requetes;
using yEmu.Util;

namespace yEmu.InterCommunication
{
    public class InterClient : Client
    {
        public InterCommunications GameVerification
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }


        public InterClient(Socket socket, Server server)
            : base(socket, server)
        {
            GameVerification = InterCommunications.VerificationGame;
        }


        public override bool DataArriavls(BufferSegment data)
        {
            if (data.Length == 0)
            {
                return false;
            }
            foreach (var packet in Encoding.UTF8.GetString(data.SegmentData).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                Console.WriteLine("PACKET  : " + packet);
                this.Parse(packet);

            }
            return true;
        }

        public void Parse(string packet)
        {
            switch (GameVerification)
            {
                case InterCommunications.VerificationGame:
                    Key = packet.Substring(2);
                    GameServers.servers.Single(gameServer => gameServer.ServerKey.Equals(Key)).State = 1;

                    foreach (var client in this._server.Clients )
                    {
                        
                        //  client[1].Processor.RefreshServerList();
                        client.Processors.RefreshServerList();
                        
                    }

                    break;
            }
        }

        public void Send(string Messages)
        {
            base.send(Messages);
        }
    }

}
