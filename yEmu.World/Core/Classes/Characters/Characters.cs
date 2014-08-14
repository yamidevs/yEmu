using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
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
        public int accounts { get; set; }
        public int PdvMax { get; set; }
        public int pdvNow { get; set; }
        public int statsId { get; set; }
        public int experience { get; set; }
        public int spellsPoints { get; set; }
        public int statsPoints { get; set; }
        public Maps_data Maps { get; set; }
        public int CellId { get; set; }
        public int Direction { get; set; }
        public int MapId { get; set; }
        public int CellDestination { get; set; }
        public Characters_stats Stats { get; set; }
        public Alignments Alignment { get; set; }
        public Channels Channels { get; set; }
        public int alignmentId { get; set; }
        public int Energy { get; set; }

        public Client Clients { get; set; }

        private long _time_Recrutement;
        private long _time_Trade;

        public const int BaseHp = 50;
        public const int GainHpPerLvl = 5;

        public static List<GameAction> Actions = new List<GameAction>();

        #region proprieter
        public int Initiative
        {
            get
            {
                var initiative = 0;

                initiative += level + Stats.Initiative.Totals();

                initiative += Stats.Intelligence.Totals() > 0 ? (int)Math.Floor(1.5 * Stats.Intelligence.Totals()) : 0;
                initiative += Stats.Intelligence.Totals() > 0 ? (int)Math.Floor(1.5 * Stats.Agility.Totals()) : 0;
                initiative += Stats.Intelligence.Totals() > 0 ? Stats.Wisdom.Totals() : 0;
                initiative += Stats.Intelligence.Totals() > 0 ? Stats.Strenght.Totals() : 0;
                initiative += Stats.Intelligence.Totals() > 0 ? Stats.Chance.Totals() : 0;

                return initiative;
            }
        }

        public int GetProspection
        {
            get
            {
                return Stats.Chance.Totals() / 10 + Classes == Class.Enutrof
                ? 120
                : 100 + Stats.Prospection.Totals();
            }
        }

        public string GetPa
        {
            get
            {
                return String.Format("{0},{1},0,0,{2}",
               level < 100 ? 6 : 7, Stats.Pa.Items,
               level < 100
                   ? 6 + Stats.Pa.Items
                   : 7 + Stats.Pa.Items);
            }

        }

        public string GetPm
        {
            get
            {
                return String.Format("{0},{1},0,0,{2}",
                    3, Stats.Pm.Items, 3 + Stats.Pm.Items);
            }
        }

        public int MaxLife
        {
            get
            {

            return  Stats.Vitality.Totals() + ((level - 1) * 5)+ 50;

            }
        }
        #endregion

        #region ctor
        public Characters()
        {
            Channels = new Channels();
            _time_Recrutement = 0;
            _time_Trade = 0;
        }
        #endregion

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

             Alignment = new Alignments { Id = id };
             yEmu.World.Core.Databases.Requetes.Alignment.Create(Alignment);

             Stats = new Characters_stats { id = id };
             yEmu.World.Core.Databases.Requetes.Stats.Create(Stats);

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
            var Packet = new StringBuilder();
            {
                Packet.Append(CellId).Append(";");
                Packet.Append(Direction).Append(";0;");
                Packet.Append(id).Append(";");
                Packet.Append(nom).Append(";");
                Packet.Append(Classes).Append(";");
                Packet.Append(skin).Append("^").Append("100").Append(";");

                // TODO : debug align info
                Packet.Append(sexe).Append(";").Append(Alignment.PatternInfos).Append(",").Append(level + id).Append(";");

                Packet.Append(Algorithme.DeciToHex(color1)).Append(";");
                Packet.Append(Algorithme.DeciToHex(color2)).Append(";");
                Packet.Append(Algorithme.DeciToHex(color3)).Append(";");
             //   builder.Append(GetItemsWheneChooseCharacter()).Append(";"); // Items
                Packet.Append((level >= 200 ? '2' : (level >= 100 ? '1' : '0'))).Append(';'); //Aura
                Packet.Append("").Append(';'); // DisplayEmotes
                Packet.Append("").Append(';'); // EmotesTimer
                Packet.Append(";"); // Guild
                Packet.Append(";0;");
                Packet.Append(";"); // Mount
            }

            return Packet.ToString();
        }

        public string GetStats()
        {
            var Packet = new StringBuilder("As");

            Packet.Append(GetExperiance()).Append('|');                       
            Packet.Append(kamas).Append('|');
            Packet.Append(statsPoints).Append('|');
            Packet.Append(spellsPoints).Append('|');
            Packet.Append(Alignment.ToString()).Append('|');
            Packet.Append(pdvNow).Append(',').Append(MaxLife).Append('|');
            Packet.Append(Energy).Append(',') 
                  .Append(10000).Append('|');// energy max
            Packet.Append(Initiative).Append('|');
            Packet.Append(GetProspection).Append('|');
            Packet.Append(GetPa).Append('|');
            Packet.Append(GetPm).Append('|');
            Packet.Append(Stats.Strenght).Append('|');
            Packet.Append(Stats.Vitality).Append('|');
            Packet.Append(Stats.Wisdom).Append('|');
            Packet.Append(Stats.Chance).Append('|');
            Packet.Append(Stats.Agility).Append('|');
            Packet.Append(Stats.Intelligence).Append('|');
            Packet.Append(Stats.Po).Append('|');
            Packet.Append(Stats.MaxInvoc).Append('|');
            Packet.Append(Stats.Damage).Append('|');
            Packet.Append(Stats.PhysicalDamage).Append('|');
            Packet.Append(Stats.MagicDamage).Append('|');
            Packet.Append(Stats.PercentDamage).Append('|');
            Packet.Append(Stats.Heal).Append('|');
            Packet.Append(Stats.TrapDamage).Append('|');
            Packet.Append(Stats.TrapPercentDamage).Append('|');
            Packet.Append(Stats.ReturnDamage).Append('|');
            Packet.Append(Stats.CriticalDamage).Append('|');
            Packet.Append(Stats.FailDamage).Append('|');
            Packet.Append(Stats.DodgePa).Append('|');
            Packet.Append(Stats.DodgePm).Append('|');
            Packet.Append(Stats.ReduceDamageNeutral).Append('|');
            Packet.Append(Stats.ReduceDamagePercentNeutral).Append('|');
            Packet.Append(Stats.ReduceDamagePvpNeutral).Append('|');
            Packet.Append(Stats.ReduceDamagePercentPvPNeutral).Append('|');
            Packet.Append(Stats.ReduceDamageStrength).Append('|');
            Packet.Append(Stats.ReduceDamagePercentStrenght).Append('|');
            Packet.Append(Stats.ReduceDamagePvpStrength).Append('|');
            Packet.Append(Stats.ReduceDamagePercentPvPStrenght).Append('|');
            Packet.Append(Stats.ReduceDamageChance).Append('|');
            Packet.Append(Stats.ReduceDamagePercentChance).Append('|');
            Packet.Append(Stats.ReduceDamagePvpChance).Append('|');
            Packet.Append(Stats.ReduceDamagePercentPvPChance).Append('|');
            Packet.Append(Stats.ReducDamageAir).Append('|');
            Packet.Append(Stats.ReduceDamagePercentAir).Append('|');
            Packet.Append(Stats.ReducDamagePvpAir).Append('|');
            Packet.Append(Stats.ReduceDamagePercentPvPAir).Append('|');
            Packet.Append(Stats.ReduceDamageIntelligence).Append('|');
            Packet.Append(Stats.ReduceDamagePercentIntelligence).Append('|');
            Packet.Append(Stats.ReduceDamagePvpIntelligence).Append('|');
            Packet.Append(Stats.ReduceDamagePercentPvPIntelligence).Append('|');
            Packet.Append("1");


            return Packet.ToString();
        }

        private string GetExperiance()
        {
            return String.Format("{0},{1},{2}",
                experience,
                Experience.Experiences.Find(x => x.lvl == level).perso,
                Experience.Experiences.Find(x => x.lvl == level + 1).perso);
        }

        public Maps_data GetMap()
        {
            return Map.Maps.First(x => x.ID == this.Maps.ID);
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

        #region Chat

        #region Recrutement
        public long TimeRecrutement()
        {
            return (long)Math.Ceiling((double)((_time_Recrutement - Environment.TickCount) / 1000));
        }

        public bool CantRecrutement()
        {
            return (TimeRecrutement() <= 0 ? true : false);
        }

        public void RefreshRecrutement()
        {
            _time_Recrutement = Environment.TickCount + 60000;
        }
        #endregion

        #region Trade
        public long TimeTrade()
        {
            return (long)Math.Ceiling((double)((_time_Trade - Environment.TickCount) / 1000));
        }

        public bool CantTrade()
        {
            return (TimeTrade() <= 0 ? true : false);
        }

        public void RefreshTrade()
        {
            _time_Trade = Environment.TickCount + 60000;
        }
        #endregion

        #endregion

        
        public void Disconnected()
        {
            if (this.Maps != null)
                this.Maps.Remove(this);

           // Processor.CharacterStates = CharacterState.Free;
        }
        public void Send(string data)
        {
            Processor.Clients.Send(data);
        }

    }
}
