using Ngot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm;
using yEmu.Util;

namespace yEmu.Network
{
    public class AuthClient : Client
    {
        public string Key
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
            this.HelloClient();
            Processor  = new Processor(this);
            this.Processors = Processor;
        }

        public void HelloClient()
        {
            Key = Hash.RandomString(32);
            Send(string.Format("{0}{1}", "HC", Key));
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
                Processor.Parser(packet);
            }
            return true;
        }

        public void Send(string Messages)
        {
            base.send(Messages);
        }

       
    }
}
