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

        public static void LoadAccounts()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {

                var account = connection.Query<Account>("SELECT * FROM accounts");
                accounts.AddRange(account);

     
            }
            Info.Write("database", string.Format("{0} Comptes chargés", accounts.Count()), ConsoleColor.Green);
        }
            public static Account getAccounts(string usernames)
            {
                if (accounts.Any(x => x.username == usernames))
                  {
                      return accounts.AsParallel().Where(x => x.username == usernames).First();
                  }
                  using (IDbConnection connection = Databases.GetConnection())
                  {
                      var results = connection.Query<Account>(
                        "SELECT * FROM accounts WHERE username = @username",
                        new { username = usernames });
                      if (results.AsParallel().Count() > 0)
                      {
                          accounts.AddRange(results);
                         var resultats =  results.AsParallel().First();
                         return resultats;
                      }
                      else
                      {
                          return null;
                      }
                   }                      
             }
            public static string getAccounts(int AccountsID)
            {
                var charactersByGameServer = string.Empty;

                using (IDbConnection connection = Databases.GetConnection())
                {

                    var results = connection.Query<Account>(
                                 "Select ServerID, count(personnagesID) AS numberCharacters FROM personnages WHERE AccountsID=@AccountsID GROUP by ServerID",
                                 new { AccountsID = AccountsID }
                                 );
                    foreach(var result in results )
                    {

                             charactersByGameServer = string.Concat(charactersByGameServer,
                                    string.Format("|{0},{1}", result.ServerID, result.numberCharacters));  
                    }
               
                    return charactersByGameServer;

                }
            }        
        }
    }
