using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Realm.Packets
{
    public sealed class HelloConnection : IPacket
    {
        private string _key;

        public HelloConnection(string key)
        {
            _key = key;
            data = "HC";
        }
        public override string ToString()
        {
            return data + _key;
        }
    }
}
