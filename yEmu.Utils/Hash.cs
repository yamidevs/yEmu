using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Util
{
    public static class Hash
    {
        private static readonly Random Rand = new Random();

        // tableau de caractère pour le hashage du mots de passe

        private static readonly char[] HashTable = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
                't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};

        // genère un string de lenght lengueur qui servira à créer une clé pour identifier chaque compte

        public static string RandomString(int lenght)
        {
            var str = string.Empty;

            for (var i = 1; i <= lenght; i++)
            {
                var randomInt = Rand.Next(0, HashTable.Length);
                str += HashTable[randomInt];
            }

            return str;
        }

        public static string Encrypt(string password, string key)
        {
            var crypted = "1";

            for (var i = 0; i < password.Length; i++)
            {
                var pPass = password[i];
                var pKey = key[i];
                var aPass = pPass / 16;
                var aKey = pPass % 16;
                var anb = (aPass + pKey) % HashTable.Length;
                var anb2 = (aKey + pKey) % HashTable.Length;

                crypted += HashTable[anb];
                crypted += HashTable[anb2];
            }

            return crypted;
        }
      
    }
}
