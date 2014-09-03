using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Collections;
using yEmu.Network;
using yEmu.World.Core.Classes.Npc;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Databases.Requetes.Mount;

namespace yEmu.World.Core.Classes.Maps
{
    public class Maps_data
    {
        public int ID { get; set; }
        public SByte Width { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string DecryptKey { get; set; }
        public string CreateTime { get; set; }
        public string MapData { get; set; }
        private static List<yEmu.World.Core.Classes.Characters.Characters> Characters = new List<yEmu.World.Core.Classes.Characters.Characters>();
        public List<int> Cells = new List<int>();

        public Maps_data()
        {
        }

        public void Add(AuthClient AuthClient, yEmu.World.Core.Classes.Characters.Characters character)
        {
            lock (Characters)
                Characters.Add(character);

            if (yEmu.World.Core.Databases.Requetes.NPC.Npc.Npcs.FindAll(x => x.mapid == ID).Count >= 1)
            {
                Send(AuthClient, string.Concat("GM", NPCsPattern()));
            }
            if (MountPark.MountParks.Any(x => x.mapid == ID))
            {
                Send(AuthClient, MountParkPattern());

            }
            
        }

        public void Send(AuthClient AuthClient ,string data)
        {
            var maps = Characters.Where(x => x.Maps.ID == AuthClient.Character.Maps.ID).ToList();
            foreach (var result in maps)
            {
                var client = Server.AuthClient.Find(x => x.Character == result);
                client.Send(data);
            }
                                   
        }

        public string DisplayChars(AuthClient AuthClient)
        {
            var characters = Characters.Where(x => x.Maps.ID == AuthClient.Character.Maps.ID).ToList();
            return characters.Aggregate(string.Empty, (current, character) => current + string.Format("|+{0}", character.DisplayChar()));
        }

        public void Remove(AuthClient AuthClient, yEmu.World.Core.Classes.Characters.Characters character)
        {
            Send(AuthClient,string.Format("{0}|-{1}", "GM", character.id));

            lock (Characters)
                Characters.Remove(character);
        }

        private string NPCsPattern()
        {
            return string.Concat("|+", string.Join("|+", from n in yEmu.World.Core.Databases.Requetes.NPC.Npc.Npcs.FindAll(x => x.mapid == ID) select n.Pattern()));
        }

        private string MountParkPattern()
        {
            var MountParks = MountPark.MountParks.Find(x => x.mapid == ID);
            var packet = new StringBuilder("Rp");
            packet.Append(MountParks.owner);
            packet.Append(";");
            packet.Append(MountParks.price);
            packet.Append(";");
            packet.Append(MountParks.size);
            packet.Append(";");
            packet.Append(0);
            packet.Append(";;");

            return packet.ToString();
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
