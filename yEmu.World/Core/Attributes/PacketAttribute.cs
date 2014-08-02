using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Attributes
{
    class PacketAttribute : Attribute
    {
        string Packet;

        public PacketAttribute(string packet)
        {
            //  this.
            Packet = packet;
        }

        public string PacketData
        {
            get
            {
                return Packet;
            }
        }
    }
}
