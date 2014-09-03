using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes.Npc
{
    public class NPCTemplates
    {
        public int id { get; set; }
        public int gfxID { get; set; }
        public int scaleX { get; set; }
        public int scaleY { get; set; }
        public int sex { get; set; }
        public int color1 { get; set; }
        public int color2 { get; set; }
        public int color3 { get; set; }
        public string accessories { get; set; }
        public int extraClip { get; set; }
        public int customArtWork { get; set; }
        public int iniQuestion { get; set; }
        public string ventes { get; set; }
        public string exchange { get; set; }

        public NPCTemplates()
        {

        }

    }
}
