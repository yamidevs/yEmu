﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using yEmu.Collections;
using yEmu.Util;
using yEmu.Core.Reflection;

namespace yEmu.World.Core.Databases.Requetes
{
    class Accounts : Singleton<Accounts>
    {

        public static ConcurrentList<Accounts> accounts = new ConcurrentList<Accounts>();

        public void Load()
        {
            using (IDbConnection connection = AuthDatabases.GetConnection())
            {
                var results = connection.Query<Accounts>("SELECT * FROM accounts");

                foreach (var result in results)
                {
                    accounts.Add(result);
                }
            }
            Info.Write("database", string.Format("{0} Comptes chargés", accounts.Count()), ConsoleColor.Green);
        }

    }
}
