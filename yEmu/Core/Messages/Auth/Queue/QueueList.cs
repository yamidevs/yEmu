using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network.Realm;

namespace yEmu.Realm.Packets.Auth
{
    public sealed class QueueList : IPacket
    {
        Processor _p;

        public QueueList(Processor p)
        {
            _p = p;
        }
        public override string ToString()
        {
          return  string.Format("Af{0}|{1}|0|2", (Queue.Clients.IndexOf(_p) + 2),
         (Queue.Clients.Count > 2 ? Queue.Clients.Count : 3));
        }
    }
}
