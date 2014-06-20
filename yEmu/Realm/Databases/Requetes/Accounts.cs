using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm.Classes;
using Dapper;
using yEmu.Util;


namespace yEmu.Realm.Databases.Requetes
{
    class Accounts
    {
        public static List<Account> accounts = new List<Account>();

        public static void LoadAccounts()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {

                var account = connection.Query<Account>("SELECT * FROM accounts");

                accounts.AddRange(account);

            }
            Info.Write("database", string.Format("{0} Comptes chargés", accounts.Count()), ConsoleColor.Green);
        }
    }
}
