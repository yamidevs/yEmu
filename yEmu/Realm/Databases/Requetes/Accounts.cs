using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm.Classes;
using Dapper;
using yEmu.Util;
using yEmu.Realm.Databases.Interfaces;
using Ninject;


namespace yEmu.Realm.Databases.Requetes
{
    class Accounts 
    {
        public static List<Account> accounts = new List<Account>();

          IDatabases _db;

        public Accounts(IDatabases db)
        {
            this._db = db;
        }
        public  void LoadAccounts()
        {
            using (IDbConnection connection = _db.GetConnection())
            {

                var account = connection.Query<Account>("SELECT * FROM accounts");

                accounts.AddRange(account);

            }
            Info.Write("database", string.Format("{0} Comptes chargés", accounts.Count()), ConsoleColor.Green);
        }
    }
}
