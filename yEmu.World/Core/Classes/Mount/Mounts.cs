using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Classes.Items;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Mount
{
    public class Mounts
    {
        public int id { get; set; }
        public int color { get; set; }
        public int sexe { get; set; }
        public string name { get; set; }
        public int xp { get; set; }
        public int level { get; set; }
        public int endurance { get; set; }
        public int amour { get; set; }
        public int maturiter { get; set; }
        public int serenite { get; set; }
        public int reproductions { get; set; }
        public int fatigue { get; set; }
        public int energie { get; set; }
        public string items { get; set; }
        public string ancetres { get; set; }
        public string ability { get; set; }
        public Dictionary<int, int> Stats = new Dictionary<int, int>();

        public int GetXp
        {
            get
            {
                return xp;
            }
        }
        public int GetEnergy
        {
            get
            {
                return 1000 + level * 90;
            }
        }

        public int GetMaturity
        {
            get
            {
                return 1000;
            }
        }

        public string GetExperience
        {
            get
            {
                return string.Concat(xp, ",", Experience.Experiences.FirstOrDefault(x => x.lvl == level).dinde, ",", Experience.Experiences.FirstOrDefault(x => x.lvl == level+1).dinde);
            }
        }

        public Mounts()
        {

        }

        public Mounts(ItemsInfos ItemsInfo, InventoryItems InventoryItem, int IdMounts)
        {
            id = InventoryItem.id;
            color = IdMounts;
            level = 1;
            xp = 0;
            fatigue = 0;
            reproductions = 0;
            serenite = 0;
            ability = "0";
            ancetres = ",,,,,,,,,,,,,"; ;
            name = "SansNom";
            energie = GetEnergy;
            maturiter = GetMaturity;
            GetStatsMounts(color,this.level);
            yEmu.World.Core.Databases.Requetes.Mount.Mount.Add(this);
        }

        public string Pattern()
        {
            var Packet = new StringBuilder();

            Packet.Append(id);
            Packet.Append(":");
            Packet.Append(color);
            Packet.Append(":");
            Packet.Append(ancetres);
            Packet.Append(":");
            Packet.Append(",,");
            Packet.Append(ability);
            Packet.Append(":");
            Packet.Append(name);
            Packet.Append(":");
            Packet.Append(sexe);
            Packet.Append(":");
            // tdod :exp
            Packet.Append(GetExperience);
            Packet.Append(":");
            Packet.Append(level);
            Packet.Append(":");
            Packet.Append("1");
            Packet.Append(":");
            Packet.Append("1000");
            Packet.Append(":");
            Packet.Append("0");
            Packet.Append(":");
            Packet.Append(endurance);
            Packet.Append(",10000:");
            Packet.Append(maturiter);
            Packet.Append(",");
            Packet.Append(GetMaturity);
            Packet.Append(":");
            Packet.Append(energie);
            Packet.Append(",");
            Packet.Append(GetEnergy);
            Packet.Append(":");
            Packet.Append(serenite);
            Packet.Append(",-10000,10000:");
            Packet.Append(amour);
            Packet.Append(",10000:");
            Packet.Append("-1");
            Packet.Append(":");
            Packet.Append("0");
            Packet.Append(":");            
            Packet.Append(GetStats());
            Packet.Append(":");
            Packet.Append(fatigue);
            Packet.Append(",240:");
            Packet.Append(reproductions);
            Packet.Append(",20:");

            return Packet.ToString();
        }

        public StringBuilder GetStats()
        {
            var packet = new StringBuilder();

            foreach (var result in Stats)
            {

                if (result.Value <= 0)
                {
                    continue;
                }

                packet.Append(Info.DeciToHex(result.Key));
                packet.Append("#");
                packet.Append(Info.DeciToHex(result.Value));
                packet.Append("#0#0");
                packet.Append(",");             

            }

            return packet;
        }
        private void GetStatsMounts(int id , int level)
        {
            switch(id)
            {
                case 1 :
                    break;

                case 3:
                    Stats.Add(125,(level/2));
                    Stats.Add(119, (int)(level/1.25));
                    break;
                case 10:
                    Stats.Add(125, (level));
                    break;
                case 20:
                    Stats.Add(174, (level*10));
                    break;
                case 18:
                    Stats.Add(125, (level / 2));
                    Stats.Add(124, (int)(level / 2.50));
                    break;
                case 38:
                    Stats.Add(174, (level * 5));
                    Stats.Add(125, (level / 2));
                    Stats.Add(182, (int)(level / 50));
                    break;
                case 46:
                    Stats.Add(125, (level));
                    Stats.Add(124, (level));
                    break;
                case 33:
                    Stats.Add(174, (level*5));
                    Stats.Add(124, (int)(level /4));
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(182, (int)(level / 100));
                    break;
                case 17:
                    Stats.Add(123, (int)(level / 1.25));
                    Stats.Add(125, (int)(level / 2));
                    break;
                case 62:
                    Stats.Add(125, (int)(level * 1.50));
                    Stats.Add(123, (int)(level / 1.65));
                    break;
                case 12:
                    Stats.Add(125, (int)(level * 1.50));
                    Stats.Add(119, (int)(level / 1.65));
                    break;
                case 36 :
                    Stats.Add(174, (level * 5));
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(123, (int)(level * 1.65));
                    Stats.Add(182, (int)(level / 100));
                    break;
                case 53:
                    Stats.Add(125,(level));
                    Stats.Add(119,(level / 2));
                    Stats.Add(126,(level / 2));
                    break;
                case 19 :
                    Stats.Add(118, (int)(level / 1.25));
                    Stats.Add(125, (int)(level / 2));
                    break;
                case 22:
                    Stats.Add(126, (int)(level / 1.25));
                    Stats.Add(125, (int)(level / 2));
                    break;
                case 48:
                    Stats.Add(125, (level));
                    Stats.Add(124, (int)(level / 4));
                    Stats.Add(126, (int)(level / 1.65));
                    break;
                case 65:
                    Stats.Add(125, (level));
                    Stats.Add(123, (int)(level / 2));
                    Stats.Add(118, (int)(level / 2));
                    break;
                case 67 :
                    Stats.Add(125, (level));
                    Stats.Add(123, (int)(level / 2));
                    Stats.Add(126, (int)(level / 2));
                    break;
                case 54 :
                    Stats.Add(125, (level));
                    Stats.Add(118, (int)(level / 2));
                    Stats.Add(119, (int)(level / 2));
                    break;
                case 76 :
                    Stats.Add(125, (level));
                    Stats.Add(126, (int)(level / 2));
                    Stats.Add(118, (int)(level / 2));
                    break;
                case 37:
                    Stats.Add(174, (level *5));
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(119, (int)(level / 1.65));
                    Stats.Add(182, (int)(level / 100));
                    break;
                case 44:
                    Stats.Add(125, (level));
                    Stats.Add(124, (int)(level / 4));
                    Stats.Add(123, (int)(level / 1.65));
                    break;
                case 42:
                    Stats.Add(125, (level));
                    Stats.Add(124, (int)(level / 4));
                    Stats.Add(119, (int)(level / 1.65));
                    break;
                case 51:
                    Stats.Add(125, (level));
                    Stats.Add(123, (int)(level / 2));
                    Stats.Add(119, (int)(level / 2));
                    break;
                case 71:
                    Stats.Add(125, (int)(level * 1.5));
                    Stats.Add(118, (int)(level / 1.65));
                    break;
                case 70 :
                    Stats.Add(125, (int)(level * 1.5));
                    Stats.Add(126, (int)(level / 1.65));
                    break;
                case 41:
                    Stats.Add(174, (level *5));
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(118, (int)(level / 1.65));
                    Stats.Add(182, (int)(level / 100));
                    break;
                case 40:
                    Stats.Add(174, (level * 5));
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(126, (int)(level / 1.65));
                    Stats.Add(182, (int)(level / 100));
                    break;
                case 49:
                    Stats.Add(125, (level));
                    Stats.Add(124, (int)(level / 4));
                    Stats.Add(118, (int)(level / 1.65));
                    break;
                case 16:
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(138, (int)(level / 2));
                    break;
                case 15:
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(176, (int)(level / 1.25));
                    break;
                case 11:
                    Stats.Add(125, (int)(level * 2));
                    Stats.Add(138, (int)(level / 2.5));
                    break;
                case 69:
                    Stats.Add(125, (int)(level * 2));
                    Stats.Add(176, (int)(level / 2.5));
                    break;
                case 39:
                    Stats.Add(174, (level * 5));
                    Stats.Add(125, (int)(level / 2));
                    Stats.Add(176, (int)(level / 2.50));
                    Stats.Add(182, (int)(level / 100));
                    break;
                case 45:
                    Stats.Add(125, level );
                    Stats.Add(138, (int)(level / 2.5));
                    Stats.Add(124, (int)(level / 4));
                    break;
                case 47:
                    Stats.Add(125, level );
                    Stats.Add(176, (int)(level / 2.5));
                    Stats.Add(124, (int)(level / 4));
                    break;
                case 61:
                    Stats.Add(125, level );
                    Stats.Add(123, (int)(level / 2.5));
                    Stats.Add(138, (int)(level / 2.5));
                    break;
                case 63:
                    Stats.Add(125, level);
                    Stats.Add(123, (int)(level / 1.65));
                    Stats.Add(138, (int)(level / 2.5));
                    break;
                case 9:
                    Stats.Add(125, level);
                    Stats.Add(119, (int)(level / 2.5));
                    Stats.Add(138, (int)(level / 2.5));
                    break;
                case 52:
                    Stats.Add(125, level);
                    Stats.Add(119, (int)(level / 1.65));
                    Stats.Add(176, (int)(level / 2.5));
                    break;
                case 68:
                    Stats.Add(125, level);
                    Stats.Add(118, (int)(level / 1.65));
                    Stats.Add(138, (int)(level / 2.5));
                    break;
                case 73:
                    Stats.Add(125, level);
                    Stats.Add(118, (int)(level / 1.65));
                    Stats.Add(176, (int)(level / 2.5));
                    break;
                case 72:
                    Stats.Add(125, level);
                    Stats.Add(126, (int)(level / 1.65));
                    Stats.Add(138, (int)(level / 2.5));
                    break;
                case 66:
                    Stats.Add(125, level);
                    Stats.Add(126, (int)(level / 2.5));
                    Stats.Add(176, (int)(level / 2.5));
                    break;
                case 21:
                    Stats.Add(125, (level * 2));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 23:
                    Stats.Add(125, (int)(level * 2));
                    Stats.Add(117, (int)(level / 50));
                    break;
                case 57:
                    Stats.Add(125, (level * 3));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 84:
                    Stats.Add(125, (level * 3));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 35:
                    Stats.Add(125,level);
                    Stats.Add(128, (int)(level / 100));
                    Stats.Add(182, (int)(level / 100));
                    Stats.Add(174, (level * 5));
                    break;
                case 77:
                    Stats.Add(125,(level * 2));
                    Stats.Add(174, (level * 5));
                    Stats.Add(117, (int)(level / 100));
                    Stats.Add(182, (int)(level / 100));
                    break;
                case 43:
                    Stats.Add(125,level );
                    Stats.Add(124, (level / 4));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 78:
                    Stats.Add(175, level * 2);
                    Stats.Add(124, (int)(level / 4));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 55:
                    Stats.Add(125, level);
                    Stats.Add(123, (int)(level / 3.33));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 82:
                    Stats.Add(125, level * 2);
                    Stats.Add(123, (int)(level / 1.65));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 50:
                    Stats.Add(125, level);
                    Stats.Add(119, (int)(level / 3.33));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 79:
                    Stats.Add(125, level * 2);
                    Stats.Add(119, (int)(level / 1.65));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 60:
                    Stats.Add(125, level);
                    Stats.Add(118, (int)(level / 3.33));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 87:
                    Stats.Add(125, level * 2);
                    Stats.Add(118, (int)(level / 1.65));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 59:
                    Stats.Add(125, level);
                    Stats.Add(126, (int)(level / 3.33));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 86:
                    Stats.Add(125, level * 2);
                    Stats.Add(126, (int)(level / 1.65));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 56:
                    Stats.Add(125, level);
                    Stats.Add(138, (int)(level / 3.33));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 83:
                    Stats.Add(125, level * 2);
                    Stats.Add(138, (int)(level / 1.65));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 58:
                    Stats.Add(125, level);
                    Stats.Add(176, (int)(level / 3.33));
                    Stats.Add(128, (int)(level / 100));
                    break;
                case 85:
                    Stats.Add(125, level * 2);
                    Stats.Add(176, (int)(level / 1.65));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 80:
                    Stats.Add(125, level * 2);
                    Stats.Add(128, (int)(level / 100));
                    Stats.Add(117, (int)(level / 100));
                    break;
                case 88:
                    Stats.Add(138, (int)(level / 2));
                    Stats.Add(212, (int)(level / 20));
                    Stats.Add(211, (int)(level / 20));
                    Stats.Add(215, (int)(level / 20));
                    Stats.Add(213, (int)(level / 20));
                    Stats.Add(214, (int)(level / 20));
                    break;
                case 89:
                    Stats.Add(111, (int)(level / 2));
                    Stats.Add(128, (int)(level / 2));
                    break;
                case 90:
                    Stats.Add(118, (int)(level / 150));
                    Stats.Add(119, (int)(level / 150));
                    Stats.Add(123, (int)(level / 150));
                    break;
                case 91:
                    Stats.Add(212, (int)(level / 15));
                    Stats.Add(211, (int)(level / 15));
                    Stats.Add(210, (int)(level / 15));
                    Stats.Add(213, (int)(level / 15));
                    Stats.Add(214, (int)(level / 15));
                    break;
            }
        }

    }
}
