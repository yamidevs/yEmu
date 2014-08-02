using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Util
{
    public class Configuration
    {
    /******************
     * Classe repris de SunDofus merci a Ghost
     ******************/
        public static Dictionary<string, string> Values;
        


        public static void LoadConfiguration()
        {
            if (!File.Exists("configuration.txt"))
                throw new Exception("Unable to find the file 'config.txt' !");

            load();
        }
        public static int getInt(string value)
        {
            return int.Parse(Values[value.ToUpper()]);
        }
        public static string getString(string value)
        {
            return Values[value.ToUpper()];
        }
        public static bool getBool(string value)
        {
            return bool.Parse(Values[value.ToUpper()]);
        }
        public static long getLong(string value)
        {
            return long.Parse(Values[value.ToUpper()]);
        }
 
     
     
        public static void load()
        {
            Values = new Dictionary<string, string>();

            using (var reader = new StreamReader("configuration.txt", Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line.StartsWith("~"))
                    {
                        var lineInfos = line.Substring(1, (line.Length - 2)).Split(' ');

                        if (!Values.ContainsKey(lineInfos[0].ToUpper()))
                            Values.Add(lineInfos[0].ToUpper(), lineInfos[1]);
                    }
                }

            }

         
        }
    }
}
