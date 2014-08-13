using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Classes.Maps;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Characters
{
    public class Characters
    {
        public int id { get; set; }
        public string nom { get; set; }
        public int level { get; set; }
        public Class Classes { get; set; }
        public int skin { get; set; }
        public int sexe { get; set; }
        public int color1 { get; set; }
        public int color2 { get; set; }
        public int color3 { get; set; }
        public int kamas { get; set; }
        public int statsPoints { get; set; }
        public int spellPoints { get; set; }
        public int accounts { get; set; }
        public int PdvMax { get; set; }
        public int PdvNow { get; set; }
        public Maps_data Maps { get; set; }
        public int CellId { get; set; }
        public int Direction { get; set; }
        public int MapId { get; set; }
        public int CellDestination { get; set; }

        public const int BaseHp = 50;
        public const int GainHpPerLvl = 5;

        public static List<GameAction> Actions = new List<GameAction>();
        
 
        #region Information sur le personnages

        public string InfosCharacter()
        {
            return String.Format("|{0};{1};{2};{3};{4};{5};{6};{7};0;{8};;;",
                         id, nom, level, skin, Algorithme.DeciToHex(color1), Algorithme.DeciToHex(color2),
                         Algorithme.DeciToHex(color3),
                        "", Servers.ServerId);
        }

        public void GenerateInfos(int gmLevel)
        {
             switch (Classes)
            {
                case Class.Feca:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Feca")));
                    CellId = int.Parse(Configuration.getString("StartCell_Feca"));
                    Direction = int.Parse(Configuration.getString("StartDir_Feca"));
                    break;
                case Class.Osamodas:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Osa")));
                    CellId = int.Parse(Configuration.getString("StartCell_Osa"));
                    Direction = int.Parse(Configuration.getString("StartDir_Osa"));
                    break;
                case Class.Enutrof:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Enu")));
                    CellId = int.Parse(Configuration.getString("StartCell_Enu"));
                    Direction = int.Parse(Configuration.getString("StartDir_Enu"));
                    break;
                case Class.Sram:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Sram")));
                    CellId = int.Parse(Configuration.getString("StartCell_Sram"));
                    Direction = int.Parse(Configuration.getString("StartDir_Sram"));
                    break;
                case Class.Xelor:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Xel")));
                    CellId = int.Parse(Configuration.getString("StartCell_Xel"));
                    Direction = int.Parse(Configuration.getString("StartDir_Xel"));
                    break;
                case Class.Ecaflip:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Eca")));
                    CellId = int.Parse(Configuration.getString("StartCell_Eca"));
                    Direction = int.Parse(Configuration.getString("StartDir_Eca"));
                    break;
                case Class.Eniripsa:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Eni")));
                    CellId = int.Parse(Configuration.getString("StartCell_Eni"));
                    Direction = int.Parse(Configuration.getString("StartDir_Eni"));
                    break;
                case Class.Iop:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Iop")));
                    CellId = int.Parse(Configuration.getString("StartCell_Iop"));
                    Direction = int.Parse(Configuration.getString("StartDir_Iop"));
                    break;
                case Class.Cra:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Cra")));
                    CellId = int.Parse(Configuration.getString("StartCell_Cra"));
                    Direction = int.Parse(Configuration.getString("StartDir_Cra"));
                    break;
                case Class.Sadida:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Sadi")));
                    CellId = int.Parse(Configuration.getString("StartCell_Sadi"));
                    Direction = int.Parse(Configuration.getString("StartDir_Sadi"));
                    break;
                case Class.Sacrieur:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Sacri")));
                    CellId = int.Parse(Configuration.getString("StartCell_Sacri"));
                    Direction = int.Parse(Configuration.getString("StartDir_Sacri"));
                    break;
                case Class.Pandawa:
                    Maps = Map.Maps.Find(x => x.ID == int.Parse(Configuration.getString("StartMap_Panda")));
                    CellId = int.Parse(Configuration.getString("StartCell_Panda"));
                    Direction = int.Parse(Configuration.getString("StartDir_Panda"));
                    break;
            }

            /*
            var alignmentId = DatabaseProvider.StatsManager.Count > 0
                ? DatabaseProvider.StatsManager.OrderByDescending(x => x.Id).First().Id + 1
                : 1;

            Alignment = new Alignment.Alignment { Id = alignmentId };
            AlignmentRepository.Create(Alignment);

            // Create stats row in database & list
            var statsId = DatabaseProvider.StatsManager.Count > 0
                ? DatabaseProvider.StatsManager.OrderByDescending(x => x.Id).First().Id + 1
                : 1;

            Stats = new StatsManager { Id = statsId };
            StatsRepository.Create(Stats);

            Channels = (gmLevel > 0)
                ? Channel.Headers.Select(channel => new Channel
                {
                    Header = (Channel.ChannelHeader)channel
                }).ToList()
                : Channel.Headers.Where(x => x != (char)Channel.ChannelHeader.AdminChannel)
                    .Select(channel => new Channel
                    {
                        Header = (Channel.ChannelHeader)channel
                    }).ToList();*/
        }

        public string SelectedCharacters()
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8};",
                id,nom, level, Classes, sexe, skin, Algorithme.DeciToHex(color1), Algorithme.DeciToHex(color2),
                Algorithme.DeciToHex(color3));
        }
        #endregion

        #region Information Personnages sur la maps
        public string DisplayChar()
        {
            var builder = new StringBuilder();
            {
                builder.Append(CellId).Append(";");
                builder.Append(Direction).Append(";0;");
                builder.Append(id).Append(";");
                builder.Append(nom).Append(";");
                builder.Append(Classes).Append(";");
                builder.Append(skin).Append("^").Append("100").Append(";");

                // TODO : debug align info
                builder.Append(sexe).Append(";").Append("0,0,0").Append(",").Append(level + id).Append(";");

                builder.Append(Algorithme.DeciToHex(color1)).Append(";");
                builder.Append(Algorithme.DeciToHex(color2)).Append(";");
                builder.Append(Algorithme.DeciToHex(color3)).Append(";");
             //   builder.Append(GetItemsWheneChooseCharacter()).Append(";"); // Items
                builder.Append("0;"); //Aura
                builder.Append(";;");
                builder.Append(";"); // Guild
                builder.Append(";0;");
                builder.Append(";"); // Mount
            }

            return builder.ToString();
        }
        #endregion

        #region Methodes Actions personnages

        public static void AddActions(GameAction actions)
        {
           lock(Actions)
            Actions.Add(actions);
        }

        public static void RemoveActions(GameAction actions)
        {
            lock (Actions)
                Actions.Remove(actions);
        }

        public static bool ContainsActions(GameAction actions)
        {
            if (Actions.Contains(actions))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ClearActions(GameAction actions)
        {
            Actions.Clear();
        }
        #endregion
    }
}
