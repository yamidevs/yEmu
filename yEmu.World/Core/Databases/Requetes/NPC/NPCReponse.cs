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
    public class NPCReponse : Singleton<NPCReponse>
    {
        public static List<NPCReponses> NPCReponses = new List<NPCReponses>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<NPCReponses>("SELECT * FROM npc_reponses_actions");

                foreach (var result in results)
                {

                    if (result.args.Contains(","))
                    {
                        var split = result.args.Split(',');

                        foreach (var args in split)
                        {
                            result.Params.Add(args);
                        }
                    }
                    else
                    {
                        result.Params.Add(result.args);
                    }
                    NPCReponses.Add(result);
                }

                Info.Write("database", string.Format("{0} NPCReponses chargés", NPCReponses.Count()), ConsoleColor.Green);
            }
        }
    }
}
