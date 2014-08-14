using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes.Characters
{
   public class BaseStats
    {
        public int Bases { get; set; }
        public int Items { get; set; }
        public int Donation { get; set; }
        public int Boosts { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}",
                Bases, Items, Donation, Boosts);
        }

        public int Totals()
        {
            return Bases + Items + Donation + Boosts;
        }
    }
}
