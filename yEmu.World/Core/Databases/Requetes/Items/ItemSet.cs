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

namespace yEmu.World.Core.Databases.Requetes
{
    public class ItemSet : Singleton<ItemSet>
    {
        public Object Lock = new Object();
        public static readonly List<ItemSets> Items = new List<ItemSets>();
        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<ItemSets>("SELECT * FROM items_set");

                Parallel.ForEach(results, result =>
                {
                    result.ItemsStats = result.Items.Split(',').Select(x => ItemsInfo.ItemsInfos.Find(y => y.ID == int.Parse(x))).ToList();

                    result.BonusesDictionary = ItemSets.ToBonusDictionary
                        (
                        result.Effects2, result.Effects3, result.Effects4, result.Effects5
                        , result.Effects6, result.Effects7, result.Effects8
                        ).ToDictionary(x => x.Key, x => x.Value);
                    lock (Lock)
                        Items.Add(result);
                });

            }
            Info.Write("database", string.Format("{0} items set chargés", Items.Count()), ConsoleColor.Green);
        }
    }
}
