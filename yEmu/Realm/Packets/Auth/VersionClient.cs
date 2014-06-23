using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Realm.Packets
{
    sealed class VersionClient : IPacket
    {
           private string _key;

        public VersionClient()
        {
            _key = "AlEv";
            data = "1.29.1";
        }
       
        public override string ToString()
        {
            return _key + data;
        }
    }

}
