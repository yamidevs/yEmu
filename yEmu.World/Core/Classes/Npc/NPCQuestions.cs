using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes.Npc
{
    public  class NPCQuestions
    {
        public int ID { get; set; }
        public string responses { get; set; }
        public List<NPCReponses> Reponse = new List<NPCReponses>();
        public string cond { get; set; }
        public int ifFalse { get; set; }

        public NPCQuestions()
        {

        }

    }
}
