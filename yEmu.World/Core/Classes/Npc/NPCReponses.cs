using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes.Npc
{
    public class NPCReponses
    {
        public int ID { get; set; }
        public int type { get; set; }
        public string args { get; set; }
        public List<string> Params = new List<string>();

        public NPCReponses()
        {

        }
    }
}
