using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using Dapper;
using yEmu.World.Core.Classes.Npc;
using System.Data;
using yEmu.Util;

namespace yEmu.World.Core.Databases.Requetes.NPC
{
    public class Npc : Singleton<Npc>
    {
        public static List<Npcs> Npcs = new List<Npcs>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Npcs>("SELECT * FROM npcs");

                foreach (var result in results)
                {
                    result.NPCTemplates = NPCTemplate.NPCTemplates.FirstOrDefault(x => x.id == result.npcid);
                    Npcs.Add(result);
                }

                Info.Write("database", string.Format("{0} Npcs chargés", Npcs.Count()), ConsoleColor.Green);
            }
        }
    }
}
