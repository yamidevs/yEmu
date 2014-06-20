using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Realm.Packets
{
    abstract class IPacket
    {
        protected string data;
        public abstract string ToString();
    }
}
