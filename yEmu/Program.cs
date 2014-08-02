using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using yEmu.InterCommunication;
using yEmu.Network;
using yEmu.Realm.Databases.Requetes;
using yEmu.Util;

namespace yEmu
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "yEmu Auth";

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

                IPAddress Ip = IPAddress.Parse("127.0.0.1");

                IPEndPoint LocalEndPointInter = new IPEndPoint(Ip, 3450);
                InterServer apps = new InterServer(LocalEndPointInter, 100);
                apps.Run();

                IPEndPoint LocalEndPoint = new IPEndPoint(Ip, 4444);
                TCPServer app = new TCPServer(LocalEndPoint, 100);
                app.Run();

                Time.Stop();

                GC.Collect();
                GC.WaitForFullGCComplete();

                Console.WriteLine("Time : {0} secondes", Time.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture).Substring(0, 4));

                while (Info.Commandes(Console.ReadLine()));

        }
  
    }
}
