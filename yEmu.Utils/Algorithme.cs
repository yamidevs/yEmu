using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Util
{
    class Algorithme
    {
        public static string GenerateRandomName()
        {
            const string consonant = "aeiouy";
            const string vowel = "bcdfghjklmnpqrstvwxz";

            var name = string.Empty;

            var rand = new Random();

            var nameLength = rand.Next(3, 10);

            for (var i = 0; i < nameLength; i++)
            {
                name += i % 2 == 0 ? vowel[rand.Next(vowel.Length - 1)] : consonant[rand.Next(consonant.Length - 1)];
            }

            return name;
        }

        public static string DeciToHex(int decimalNumber)
        {
            return decimalNumber == -1 ? "-1" : decimalNumber.ToString("x");
        }

        public static int HexToDeci(string hexaDecimalNumber)
        {
            return hexaDecimalNumber == "" || hexaDecimalNumber == "-1" ? -1 : Convert.ToInt32(hexaDecimalNumber, 16);
        }
    }
}
