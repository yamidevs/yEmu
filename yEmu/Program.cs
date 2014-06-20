using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.Util;

namespace yEmu
{
    class Program
    {

        public const Int32 port = 4444;

        static void Main(string[] args)
        {
            
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Info.Start();
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
                Server s = new Server();
                s.StartServer(localEndPoint, 20);
                sw.Stop();
            
                Console.WriteLine("Time : {0}", sw.Elapsed);

                while (Parse(
                    Console.ReadLine())
                    );

        }
        static bool Parse(string command)
        {
            switch (command)
            {
                case "stats":
            Info.Write("",  string.Format("Nombre Thread : {0}",Performance.cThread()), ConsoleColor.Blue);
            Info.Write("", string.Format("CPU USAGE : {0}", Performance.CurrentCPUusage()), ConsoleColor.Blue);
                    break;

          
            }

            return true;
        }
    }
}
