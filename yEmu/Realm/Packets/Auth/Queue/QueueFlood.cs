﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Realm.Packets.Auth
{
    sealed class QueueFlood : IPacket
    {
        public override string ToString()
        {
            return "M00\0";
        }
    }
}
