using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;

namespace yEmu.World.Core.Classes.Npc
{
    public class Npcs
    {
        public int mapid { get; set; }
        public int npcid { get; set; }
        public int cellid { get; set; }
        public int orientation { get; set; }

        public NPCTemplates NPCTemplates { get; set; }

        public Npcs()
        {

        }

        public string Pattern()
        {
            var builder = new StringBuilder();
            {
                builder.Append(cellid).Append(";").Append(orientation).Append(";0;");
                builder.Append(npcid).Append(";");
                builder.Append(NPCTemplates.id).Append(";-4;");
                builder.Append(NPCTemplates.gfxID).Append("^").Append(NPCTemplates.scaleY).Append(";");
                builder.Append(NPCTemplates.sex).Append(";").Append(Info.DeciToHex(NPCTemplates.color1)).Append(";");
                builder.Append(Info.DeciToHex(NPCTemplates.color2)).Append(";").Append(Info.DeciToHex(NPCTemplates.color3)).Append(";");
                builder.Append(NPCTemplates.accessories).Append(";;");
            }

            return builder.ToString();
        }
    }

}
