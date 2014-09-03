using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Items.Stats
{
    public class Jet
    {
        public Effect Header { get; set; }
        public int MinValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0}#{1}#0#0", Info.DeciToHex((int)Header), Info.DeciToHex(MinValue));
        }
    }
}
