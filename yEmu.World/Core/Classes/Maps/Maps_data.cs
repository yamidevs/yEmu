using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Collections;
using yEmu.Network;
using yEmu.World.Core.Databases.Requetes;

namespace yEmu.World.Core.Classes.Maps
{
    public class Maps_data
    {
        public int ID { get; set; }
        public SByte Width { get; set; } 
        public string DecryptKey { get; set; }
        public string CreateTime { get; set; }
        public string MapData { get; set; }
        private static  List<yEmu.World.Core.Classes.Characters.Characters> Characters = new List<yEmu.World.Core.Classes.Characters.Characters>();
        public List<int> Cells = new List<int>();

        public void Add(yEmu.World.Core.Classes.Characters.Characters character)
        {
            lock (Characters)
                Characters.Add(character);
        }

        public  void Send(string data)
        {
            
              var maps = Characters.Where(x => x.Maps.ID == Processor.Clients.Character.Maps.ID).ToList();
              maps.Find(x => x.id == Processor.Clients.Character.id).Send(data);
                       
        }

        public  string DisplayChars()
        {
            var characters = Characters.Where(x => x.Maps.ID == Processor.Clients.Character.Maps.ID).ToList();
            return characters.Aggregate(string.Empty, (current, character) => current + string.Format("|+{0}", character.DisplayChar()));
        }

        public void Remove(yEmu.World.Core.Classes.Characters.Characters character)
        {
            Send(string.Format("{0}|-{1}", "GM", character.id));
            lock (Characters)
                Characters.Remove(character);


        }

        #region PathFinding
        public List<int> UncompressDatas()
        {
            var newList = new List<int>();

            const string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            var data = DecypherData(MapData, "");

            for (var i = 0; i < data.Length; i += 10)
            {
                var currentCell = data.Substring(i, 10);
                byte[] cellInfo = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (var i2 = currentCell.Length - 1; i2 >= 0; i2--)
                    cellInfo[i2] = (byte)hash.IndexOf(currentCell[i2]);

                var type = (cellInfo[2] & 56) >> 3;

                if (type != 0)
                    newList.Add(i / 10);
            }

            return newList;
        }

        private static string DecypherData(string data, string decryptKey)
        {
            try
            {
                var result = string.Empty;

                if (decryptKey == "") return data;

                decryptKey = PrepareKey(decryptKey);
                var checkSum = CheckSum(decryptKey) * 2;

                for (int i = 0, k = 0; i < data.Length; i += 2)
                    result += (char)(int.Parse(data.Substring(i, 2), System.Globalization.NumberStyles.HexNumber) ^ decryptKey[(k++ + checkSum) % decryptKey.Length]);

                return Uri.UnescapeDataString(result);
            }
            catch { return ""; }
        }

        private static string PrepareKey(string key)
        {
            var keyResult = "";

            for (var i = 0; i < key.Length; i += 2)
                keyResult += Convert.ToChar(int.Parse(key.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));

            return Uri.UnescapeDataString(keyResult);
        }

        private static int CheckSum(string data)
        {
            var result = data.Sum(t => t % 16);

            return result % 16;
        }
        #endregion

    }
}
