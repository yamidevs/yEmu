using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Util
{
    public class Info
    {
        public static void Start()
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("yEMU Lancement : ");
            Console.ForegroundColor = old;

        }
        public static void Write(string type, string message, ConsoleColor color)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            switch (type)
            {
                case "realmS":
                    Console.WriteLine(string.Format("[ Auth.S ] {0}", message));
                    break;
                case "gameS":
                    Console.WriteLine(string.Format("[ GAME.S ] {0}", message));
                    break;
                case "realmR":
                    Console.WriteLine(string.Format("[ Auth.R ] {0}", message));
                    break;
                case "gameR":
                    Console.WriteLine(string.Format("[ GAME.R ] {0}", message));
                    break;
                case "error":
                    Console.WriteLine(string.Format("[ ERROR ] {0}", message));
                    break;
                case "database":
                    Console.WriteLine(string.Format("[ DATABASE ] {0}", message));
                    break;
                default:
                    Console.WriteLine(string.Format("[ INFO ] {0}", message));
                    break;

            }
        }
        public static bool Commandes(string command)
        {
            switch (command)
            {
                case "stats":
                    Info.Write("", string.Format("Nombre Thread : {0}", Performance.cThread()), ConsoleColor.Blue);
                    Info.Write("", string.Format("CPU USAGE : {0}", Performance.CurrentCPUusage()), ConsoleColor.Blue);
                    break;
            }
            return true;
        }

        public static string GetActualTime()
        {
            return string.Format("{0}{1}{2}{3}", (DateTime.Now.Hour * 3600000), (DateTime.Now.Minute * 60000),
                (DateTime.Now.Second * 1000), DateTime.Now.Millisecond.ToString());
        }
    }
}