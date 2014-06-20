using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Util
{
    class Info
    {
        public static void Start()
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("yEMU Lancement : ");
            Console.ForegroundColor = old;

        }
        public static void Write(string type, string message, ConsoleColor color )
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            switch(type)
            {
                case "realmS":
                    Console.WriteLine(string.Format("[ REALM.S ] {0}" , message));
                    break;
                case "gameS":
                    Console.WriteLine(string.Format("[ GAME.S ] {0}", message));
                    break;
                case "realmR":
                    Console.WriteLine(string.Format("[ REALM.R ] {0}", message));
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
    }
}
