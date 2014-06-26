using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.Realm.Databases.Requetes;
using yEmu.Util;

namespace yEmu
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch Time = new Stopwatch();
            Time.Start();
                Info.Start();
                Configuration.LoadConfiguration();

                try
                {
                Accounts.Instance.LoadAccounts();
                GameServers.Instance.LoadServers();
                }
                catch(Exception e)
                {
                    Info.Write("ERROR","CHargement de database : "+ e.Message, ConsoleColor.Red);
                }
                                       
                IPAddress Ip = IPAddress.Parse(Configuration.getString("Realm_Ip"));
                IPEndPoint LocalEndPointRealm = new IPEndPoint(Ip, Configuration.getInt("Realm_Port"));

                Server Realm = new Server();
                Realm.StartServer(LocalEndPointRealm, Configuration.getInt("Max_co"));

                Time.Stop();

                GC.Collect();
                GC.WaitForFullGCComplete();

                Console.WriteLine("Time : {0} secondes", Time.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture).Substring(0, 4));

                while (Info.Commandes(Console.ReadLine()));

        }
  
    }
}
