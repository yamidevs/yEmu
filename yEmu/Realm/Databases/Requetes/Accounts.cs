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
using yEmu.Reflection;
using System.Collections.Concurrent;
using yEmu.Collections;


namespace yEmu.Realm.Databases.Requetes
{
    class Accounts : Singleton<Accounts>
    {
        public static ConcurrentList<Account> accounts = new ConcurrentList<Account>();

        public  void LoadAccounts()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var account = connection.Query<Account>("SELECT * FROM accounts");
                foreach (var results in account)
                {
                    accounts.Add(results);
                }                    
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
                      if (results.Count() > 0)
                      {
                          foreach (var data in results)
                          {
                              accounts.Add(data);
                          }  
                         var resultats =  results.First();
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
