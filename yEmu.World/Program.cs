using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.Util;
using yEmu.World.InterCommunication;

namespace yEmu.World
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "yEmu World";

            Stopwatch Time = new Stopwatch();
            Time.Start();
            Info.Start();
            Configuration.LoadConfiguration();

            IPAddress Ip = IPAddress.Parse(Configuration.getString("Game_Ip"));

            IPEndPoint LocalEndPoint = new IPEndPoint(Ip, 5556);
            TCPServer app = new TCPServer(LocalEndPoint, 100);
            app.Run();

            IPEndPoint InterCommunicationLocalEndPointRealm = new IPEndPoint(Ip, 3450);
            InterClient InterCommunication = new InterClient();
            InterCommunication.Initialize("127.0.0.1", 3450);

            Time.Stop();

            GC.Collect();
            GC.WaitForFullGCComplete();

            Console.WriteLine("Time : {0} secondes", Time.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture).Substring(0, 4));

            Console.ReadLine();
        }
    }
}
