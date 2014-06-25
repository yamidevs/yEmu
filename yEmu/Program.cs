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
            Stopwatch time = new Stopwatch();
                time.Start();
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
                                       
                IPAddress ip = IPAddress.Parse(Configuration.getString("Realm_Ip"));
                IPEndPoint localEndPointRealm = new IPEndPoint(ip, Configuration.getInt("Realm_Port"));

                Server Realm = new Server();
                Realm.StartServer(localEndPointRealm, Configuration.getInt("Max_co"));

                time.Stop();

                GC.Collect();
                GC.WaitForFullGCComplete();

                Console.WriteLine("Time : {0} secondes", time.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture).Substring(0, 4));

                while (Info.Commandes(Console.ReadLine()));

        }
  
    }
}
