﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using yEmu.World.Core.Classes.Maps;
using yEmu.Util;
using yEmu.Core.Reflection;

namespace yEmu.World.Core.Databases.Requetes
{
   public class Map : Singleton<Map>
    {

        public static readonly List<Maps_data> Maps = new List<Maps_data>();


        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {

                var results = connection.Query<Maps_data>("SELECT * FROM  maps_data" , buffered: false);

                foreach (var result in results)
                {
                    result.Cells = result.UncompressDatas(); 
                    Maps.Add(result);
                }
            }
            Info.Write("database", string.Format("{0} Maps chargés", Maps.Count()), ConsoleColor.Green);
        }
    }
}
