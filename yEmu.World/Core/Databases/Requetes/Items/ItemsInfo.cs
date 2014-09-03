using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.Reflection;
using Dapper;
using yEmu.World.Core.Classes.Items;
using System.Data;
using yEmu.Util;
using yEmu.World.Core.Enums;
using yEmu.World.Core.Classes.Items.Stats;

namespace yEmu.World.Core.Databases.Requetes
{
    class ItemsInfo : Singleton<ItemsInfo>
    {
        public Object Lock = new Object();
        public static readonly List<ItemsInfos> ItemsInfos = new List<ItemsInfos>();
        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {

                var results = connection.Query<ItemsInfos>("SELECT * FROM items_infos");

                Parallel.ForEach(results, result =>
                {
                    result.StatsItems = StatsItems.ToStats(result.Stats);
                    lock (Lock)
                        ItemsInfos.Add(result);
                });
            }
            Info.Write("database", string.Format("{0} Information sur les items chargés", ItemsInfos.Count()), ConsoleColor.Green);
        }
    }
}
