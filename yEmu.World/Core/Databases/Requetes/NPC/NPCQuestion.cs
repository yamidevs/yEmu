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
                var results = connection.Query<NPCQuestions>("SELECT * FROM npc_questions");

                foreach (var result in results)
                {
                    if (result.responses.Contains(';'))
                    {
                        var split = result.responses.Split(';');

                        foreach (var reponse in split)
                        {
             
                                result.Reponse.Add(NPCReponse.NPCReponses.FirstOrDefault(x => x.ID == int.Parse(reponse)));                            
                        }
                    }
                    else
                    {
                        try
                        {
                            result.Reponse.Add(NPCReponse.NPCReponses.FirstOrDefault(x => x.ID == int.Parse(result.responses)));
                        }
                        catch
                        {
                        }

                    }

                    NPCQuestions.Add(result);
                }

                Info.Write("database", string.Format("{0} NPCQuestion chargés", NPCQuestions.Count()), ConsoleColor.Green);
            }
        }
    }
}
