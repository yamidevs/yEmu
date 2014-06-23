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

                foreach (var acc in account)
                {
                    acc.personnages = getAccounts(acc.id);
                }
                accounts.AddRange(account);

        
            }
            Info.Write("database", string.Format("{0} Comptes chargés", accounts.Count()), ConsoleColor.Green);
        }
            public static Account getAccounts(string usernames)
            {
                if (accounts.Any(x => x.username == usernames))
                  {
                      return accounts.Where(x => x.username == usernames).First();
                  }
                  using (IDbConnection connection = Databases.GetConnection())
                  {
                      var results = connection.Query<Account>(
                        "SELECT * FROM accounts WHERE username = @username",
                        new { username = usernames });
                      if (results.Count() > 0)
                      {
                          accounts.AddRange(results);
                         var resultats =  results.Where(x => x.username == usernames).First();
                         getAccounts(resultats.id);
                         return resultats;
                      }
                      else
                      {
                          return null;
                      }
                   }                      
             }
            public static Dictionary<int, List<string>> getAccounts(int id)
            {
                var personnages = new Dictionary<int, List<string>>();
                using (IDbConnection connection = Databases.GetConnection())
                {

                    var results = connection.Query<Account>(
                                 "SELECT Nom,ServerID FROM personnages WHERE AccountsID=@id",
                                 new { id = id , }
                                 );                  
                    foreach(var result in results){
                        if (!personnages.ContainsKey(result.ServerID))
                            personnages.Add(result.ServerID, new List<string>());

                        if (!personnages[result.ServerID].Contains(result.Nom))
                            personnages[result.ServerID].Add(result.Nom);
                    }
                }
                return personnages;
            }        
        }
    }
