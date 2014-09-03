using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using yEmu.Util;
using yEmu.World.Core.Classes.Npc;
using Dapper;

namespace yEmu.World.Core.Databases.Requetes.NPC
{
    public class NPCQuestion : Singleton<NPCQuestion>
    {
        public static List<NPCQuestions> NPCQuestions = new List<NPCQuestions>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<NPCQuestions>("SELECT * FROM npcs_questions");

                foreach (var result in results)
                {

                    NPCQuestions.Add(result);
                }

                Info.Write("database", string.Format("{0} NPCQuestion chargés", NPCQuestions.Count()), ConsoleColor.Green);
            }
        }
    }
}
