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
    public class NPCTemplate : Singleton<NPCTemplate>
    {
        public static List<NPCTemplates> NPCTemplates = new List<NPCTemplates>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<NPCTemplates>("SELECT * FROM npc_template");

                foreach (var result in results)
                {
                    result.NPCQuestions = NPCQuestion.NPCQuestions.FirstOrDefault(x => x.ID == result.initQuestion);

                    NPCTemplates.Add(result);
                }

                Info.Write("database", string.Format("{0} NPCTemplates chargés", NPCTemplates.Count()), ConsoleColor.Green);
            }
        }
    }
}
