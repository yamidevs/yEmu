using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.Util;
using yEmu.World.Core.Classes.Items;
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
        public object BStats = new object();
        public int Npc { get; set; }
        public Client Clients { get; set; }
        public string savezaap { get; set; }

        public bool Exchange { get; set; }
        public static List<int> PacketZaap = new List<int>();

        private long _time_Recrutement;
        private long _time_Trade;

        public const int BaseHp = 50;
        public const int GainHpPerLvl = 5;

        public Characters CharacterExchange { get; set; }

        public AuthClient AuthClient { get; set; }
        public static List<GameAction> Actions = new List<GameAction>();

        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private object _lock = new object();

        public Party Party { get; set; }
        public bool Connected { get; set; }

        public bool party { get; set; }

        public int SendParty { get; set; }
        public int ReceiveParty { get; set; }
        public bool WaitParty { get; set; }

        public int Dialogue { get; set; }
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
        public Characters(AuthClient authClient)
        {
            AuthClient = authClient;
        }
        public Characters()
        {
            Channels = new Channels();
            Maps =  new Maps_data();
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
            var itemInventory = InventoryItem.Inventory.FindAll(x => x.IDCharacter.id == id);

            var items = itemInventory.Aggregate(String.Empty,
                (current, inventoryItem) => string.Format("{0}{1};", current, inventoryItem.ItemInfo()));

            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|",
                id,nom, level, Classes, sexe, skin, Algorithme.DeciToHex(color1), Algorithme.DeciToHex(color2),
                Algorithme.DeciToHex(color3),items);
        }

        public void CalculateItemStats()
        {
            Stats.Calculate(this);
        }

        public string GetItems()
        {
            var items = new StringBuilder();
            {

                var inventoryItems = InventoryItem.Inventory.FindAll(x => x.IDCharacter.id == id);

                if (inventoryItems.Count == 0)
                    return ",,,,";

                if (inventoryItems.Any(x => x.position == Position.Arme))
                    items.Append(Info.DeciToHex(inventoryItems.Find(x => x.position == Position.Arme).IDItems.ID));

                items.Append(",");

                if (inventoryItems.Any(x => x.position == Position.Coiffe))
                {
                    if(inventoryItems.Find(x => x.position == Position.Coiffe).obvi == 1)
                    {
                      items.Append(2412 + "~" + "16~1");
                    }
                    else
                    {
                        items.Append(Info.DeciToHex(inventoryItems.Find(x => x.position == Position.Coiffe).IDItems.ID));
                    }
                    
                   
                }

                items.Append(",");

                if (inventoryItems.Any(x => x.position == Position.Cape))
                    items.Append(Info.DeciToHex(inventoryItems.Find(x => x.position == Position.Cape).IDItems.ID));

                items.Append(",");

                if (inventoryItems.Any(x => x.position == Position.Familier))
                    items.Append(Info.DeciToHex( inventoryItems.Find(x => x.position == Position.Familier).IDItems.ID));

                items.Append(",");

                if (inventoryItems.Any(x => x.position == Position.Bouclier))
                    items.Append(Info.DeciToHex(inventoryItems.Find(x => x.position == Position.Bouclier).IDItems.ID));
            }
            return items.ToString();
        }

        public List<InventoryItems> GetAllItemsEquipedInSet(ItemSets itemSet)
        {
            return
                InventoryItem.Inventory.FindAll(
                    x => x.IDCharacter == this && x.IsEquiped() && x.IDItems.GetSet() == itemSet);
          
        }

        public IEnumerable<ItemSets> GetSets()
        {
            var itemsInSet = InventoryItem.Inventory.FindAll(x => x.IDCharacter == this  && x.IsEquiped() && x.IDItems.HasSet());
            return itemsInSet.Select(inventoryItem => ItemSet.Items.Find(x => x == inventoryItem.IDItems.GetSet())).Distinct();
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
                Packet.Append(GetItems()).Append(";"); // Items
                Packet.Append((level >= 200 ? '2' : (level >= 100 ? '1' : '0'))).Append(';'); //Aura
                Packet.Append("").Append(';'); // DisplayEmotes
                Packet.Append("").Append(';'); // EmotesTimer
                Packet.Append(";"); // Guild
                Packet.Append(";0;");
                Packet.Append(";"); // Mount
            }

            return Packet.ToString();
        }

        public string PatternOnParty()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(id).Append(";");
                builder.Append(nom).Append(";");
                builder.Append(skin).Append(";");
                builder.Append(color1).Append(";");
                builder.Append(color2).Append(";");
                builder.Append(color3).Append(";");
                builder.Append(GetItems()).Append(";");
                builder.Append(pdvNow).Append(",").Append(MaxLife).Append(";");
                builder.Append(level).Append(";");
                builder.Append(Initiative).Append(";");
                builder.Append(GetProspection).Append(";0");
            }

            return builder.ToString();
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

        #region statsBoost

        public void BoostStats(BaseStats stats)
        {
            switch (Classes)
            {
                case Class.Feca:
                case Class.Xelor:
                case Class.Eniripsa:
                case Class.Osamodas:
                    AddMageStats(stats);
                    break;
                case Class.Enutrof:
                    AddEnuStats(stats);
                    break;
                case Class.Sram:
                    AddSramStats(stats);
                    break;
                case Class.Iop:
                    AddIopStats(stats);
                    break;
                case Class.Cra:
                    AddCraStats(stats);
                    break;
                case Class.Ecaflip:
                    AddEcaStats(stats);
                    break;
                case Class.Sadida:
                    AddSadiStats(stats);
                    break;
                case Class.Sacrieur:
                    AddSacriStats(stats);
                    break;
                case Class.Pandawa:
                    AddPandaStats(stats);
                    break;
            }
        }

        private void AddGeneralStats(BaseStats stats, Func<int> intellAction, Func<int> chanceAction,
            Func<int> agilityAction, Func<int> strengthAction)
        {
            lock (_lock)
            {
                switch (stats)
                {
                    case BaseStats.Vitality:
                        const int statsPointVitalityBoost = 1;

                        if (statsPoints >= statsPointVitalityBoost)
                        {
                            Stats.Vitality.Bases += Classes == Class.Sacrieur ? 3 : 1;
                            pdvNow += Classes == Class.Sacrieur ? 3 : 1;
                            PdvMax += Classes == Class.Sacrieur ? 3 : 1;
                        }

                        statsPoints -= statsPointVitalityBoost;
                        break;
                    case BaseStats.Wisdom:
                        const int statsPointWisdomBoost = 3;

                        if (statsPoints >= statsPointWisdomBoost)
                            Stats.Wisdom.Bases += 1;

                        statsPoints -= statsPointWisdomBoost;
                        break;
                    case BaseStats.Strength:
                        var statsPointStrengthBoost = strengthAction();

                        if (statsPoints >= statsPointStrengthBoost)
                            Stats.Strenght.Bases += 1;

                        statsPoints -= statsPointStrengthBoost;
                        break;
                    case BaseStats.Intelligence:
                        var statsPointIntelligenceBoost = intellAction();

                        if (statsPoints >= statsPointIntelligenceBoost)
                            Stats.Intelligence.Bases += 1;

                        statsPoints -= statsPointIntelligenceBoost;
                        break;
                    case BaseStats.Chance:
                        var statsPointChanceBoost = chanceAction();

                        if (statsPoints >= statsPointChanceBoost)
                            Stats.Chance.Bases += 1;

                        statsPoints -= statsPointChanceBoost;
                        break;
                    case BaseStats.Agility:
                        var statsPointAgilityBoost = agilityAction();

                        if (statsPoints >= statsPointAgilityBoost)
                            Stats.Agility.Bases += 1;

                        statsPoints -= statsPointAgilityBoost;
                        break;
                }
            }
        }

        private void AddMageStats(BaseStats stats)
        {
            // Osamodas does not have the same Chance stats as other Mages (feca, xelor, eniripsa) So...

            Func<int> normalMageChanceFunc = delegate
            {
                if (Stats.Chance.Bases <= 20)
                    return 1;
                if (Stats.Chance.Bases <= 40)
                    return 2;
                if (Stats.Chance.Bases <= 60)
                    return 3;
                if (Stats.Chance.Bases <= 80)
                    return 4;
                if (Stats.Chance.Bases >= 81)
                    return 5;
                return 0;
            };

            Func<int> osaChanceFunc = delegate
            {
                if (Stats.Chance.Bases <= 100)
                    return 1;
                if (Stats.Chance.Bases <= 200)
                    return 2;
                if (Stats.Chance.Bases <= 300)
                    return 3;
                if (Stats.Chance.Bases <= 400)
                    return 4;
                if (Stats.Chance.Bases >= 401)
                    return 5;
                return 0;
            };

            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 100)
                        return 1;
                    if (Stats.Intelligence.Bases <= 200)
                        return 2;
                    if (Stats.Intelligence.Bases <= 300)
                        return 3;
                    if (Stats.Intelligence.Bases <= 400)
                        return 4;
                    if (Stats.Intelligence.Bases >= 401)
                        return 5;
                    return 0;
                },
                Classes != Class.Osamodas
                    ? normalMageChanceFunc
                    : osaChanceFunc,
                () =>
                {
                    if (Stats.Agility.Bases <= 20)
                        return 1;
                    if (Stats.Agility.Bases <= 40)
                        return 2;
                    if (Stats.Agility.Bases <= 60)
                        return 3;
                    if (Stats.Agility.Bases <= 80)
                        return 4;
                    if (Stats.Agility.Bases >= 81)
                        return 5;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 50)
                        return 2;
                    if (Stats.Strenght.Bases <= 150)
                        return 3;
                    if (Stats.Strenght.Bases <= 250)
                        return 4;
                    if (Stats.Strenght.Bases >= 251)
                        return 5;
                    return 0;
                });
        }

        private void AddEnuStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 20)
                        return 1;
                    if (Stats.Intelligence.Bases <= 60)
                        return 2;
                    if (Stats.Intelligence.Bases <= 100)
                        return 3;
                    if (Stats.Intelligence.Bases <= 140)
                        return 4;
                    if (Stats.Intelligence.Bases >= 141)
                        return 5;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 100)
                        return 1;
                    if (Stats.Chance.Bases <= 150)
                        return 2;
                    if (Stats.Chance.Bases <= 230)
                        return 3;
                    if (Stats.Chance.Bases <= 330)
                        return 4;
                    if (Stats.Chance.Bases >= 331)
                        return 5;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 20)
                        return 1;
                    if (Stats.Agility.Bases <= 40)
                        return 2;
                    if (Stats.Agility.Bases <= 60)
                        return 3;
                    if (Stats.Agility.Bases <= 80)
                        return 4;
                    if (Stats.Agility.Bases >= 81)
                        return 5;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 50)
                        return 1;
                    if (Stats.Strenght.Bases <= 150)
                        return 2;
                    if (Stats.Strenght.Bases <= 250)
                        return 3;
                    if (Stats.Strenght.Bases <= 350)
                        return 4;
                    if (Stats.Strenght.Bases >= 351)
                        return 5;
                    return 0;
                });
        }

        private void AddSramStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 50)
                        return 2;
                    if (Stats.Intelligence.Bases <= 150)
                        return 3;
                    if (Stats.Intelligence.Bases <= 250)
                        return 4;
                    if (Stats.Intelligence.Bases >= 251)
                        return 5;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 20)
                        return 1;
                    if (Stats.Chance.Bases <= 40)
                        return 2;
                    if (Stats.Chance.Bases <= 60)
                        return 3;
                    if (Stats.Chance.Bases <= 80)
                        return 4;
                    if (Stats.Chance.Bases >= 81)
                        return 5;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 100)
                        return 1;
                    if (Stats.Agility.Bases <= 200)
                        return 2;
                    if (Stats.Agility.Bases <= 300)
                        return 3;
                    if (Stats.Agility.Bases <= 400)
                        return 4;
                    if (Stats.Agility.Bases >= 401)
                        return 5;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 100)
                        return 1;
                    if (Stats.Strenght.Bases <= 200)
                        return 2;
                    if (Stats.Strenght.Bases <= 300)
                        return 3;
                    if (Stats.Strenght.Bases <= 400)
                        return 4;
                    if (Stats.Strenght.Bases >= 401)
                        return 5;
                    return 0;
                });
        }

        private void AddIopStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 20)
                        return 1;
                    if (Stats.Intelligence.Bases <= 40)
                        return 2;
                    if (Stats.Intelligence.Bases <= 60)
                        return 3;
                    if (Stats.Intelligence.Bases <= 80)
                        return 4;
                    if (Stats.Intelligence.Bases >= 81)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 20)
                        return 1;
                    if (Stats.Chance.Bases <= 40)
                        return 2;
                    if (Stats.Chance.Bases <= 60)
                        return 3;
                    if (Stats.Chance.Bases <= 80)
                        return 4;
                    if (Stats.Chance.Bases >= 81)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 20)
                        return 1;
                    if (Stats.Agility.Bases <= 40)
                        return 2;
                    if (Stats.Agility.Bases <= 60)
                        return 3;
                    if (Stats.Agility.Bases <= 80)
                        return 4;
                    if (Stats.Agility.Bases >= 81)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 100)
                        return 1;
                    if (Stats.Strenght.Bases <= 200)
                        return 2;
                    if (Stats.Strenght.Bases <= 300)
                        return 3;
                    if (Stats.Strenght.Bases <= 400)
                        return 4;
                    if (Stats.Strenght.Bases >= 401)
                        return 5;
                    return 0;
                });
        }

        private void AddCraStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 50)
                        return 1;
                    if (Stats.Intelligence.Bases <= 150)
                        return 2;
                    if (Stats.Intelligence.Bases <= 250)
                        return 3;
                    if (Stats.Intelligence.Bases <= 350)
                        return 4;
                    if (Stats.Intelligence.Bases >= 351)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 20)
                        return 1;
                    if (Stats.Chance.Bases <= 40)
                        return 2;
                    if (Stats.Chance.Bases <= 60)
                        return 3;
                    if (Stats.Chance.Bases <= 80)
                        return 4;
                    if (Stats.Chance.Bases >= 81)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 50)
                        return 1;
                    if (Stats.Agility.Bases <= 100)
                        return 2;
                    if (Stats.Agility.Bases <= 150)
                        return 3;
                    if (Stats.Agility.Bases <= 200)
                        return 4;
                    if (Stats.Agility.Bases >= 201)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 50)
                        return 1;
                    if (Stats.Strenght.Bases <= 150)
                        return 2;
                    if (Stats.Strenght.Bases <= 250)
                        return 3;
                    if (Stats.Strenght.Bases <= 350)
                        return 4;
                    if (Stats.Strenght.Bases >= 351)
                        return 4;
                    return 0;
                });
        }

        private void AddEcaStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 20)
                        return 1;
                    if (Stats.Intelligence.Bases <= 40)
                        return 2;
                    if (Stats.Intelligence.Bases <= 60)
                        return 3;
                    if (Stats.Intelligence.Bases <= 80)
                        return 4;
                    if (Stats.Intelligence.Bases >= 81)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 20)
                        return 1;
                    if (Stats.Chance.Bases <= 40)
                        return 2;
                    if (Stats.Chance.Bases <= 60)
                        return 3;
                    if (Stats.Chance.Bases <= 80)
                        return 4;
                    if (Stats.Chance.Bases >= 81)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 50)
                        return 1;
                    if (Stats.Agility.Bases <= 100)
                        return 2;
                    if (Stats.Agility.Bases <= 150)
                        return 3;
                    if (Stats.Agility.Bases <= 200)
                        return 4;
                    if (Stats.Agility.Bases >= 201)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 100)
                        return 1;
                    if (Stats.Strenght.Bases <= 200)
                        return 2;
                    if (Stats.Strenght.Bases <= 300)
                        return 3;
                    if (Stats.Strenght.Bases <= 400)
                        return 4;
                    if (Stats.Strenght.Bases >= 401)
                        return 4;
                    return 0;
                });
        }

        private void AddSadiStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 100)
                        return 1;
                    if (Stats.Intelligence.Bases <= 200)
                        return 2;
                    if (Stats.Intelligence.Bases <= 300)
                        return 3;
                    if (Stats.Intelligence.Bases <= 400)
                        return 4;
                    if (Stats.Intelligence.Bases >= 401)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 100)
                        return 1;
                    if (Stats.Chance.Bases <= 200)
                        return 2;
                    if (Stats.Chance.Bases <= 300)
                        return 3;
                    if (Stats.Chance.Bases <= 400)
                        return 4;
                    if (Stats.Chance.Bases >= 401)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 20)
                        return 1;
                    if (Stats.Agility.Bases <= 40)
                        return 2;
                    if (Stats.Agility.Bases <= 60)
                        return 3;
                    if (Stats.Agility.Bases <= 80)
                        return 4;
                    if (Stats.Agility.Bases >= 81)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 100)
                        return 1;
                    if (Stats.Strenght.Bases <= 200)
                        return 2;
                    if (Stats.Strenght.Bases <= 300)
                        return 3;
                    if (Stats.Strenght.Bases <= 400)
                        return 4;
                    if (Stats.Strenght.Bases >= 401)
                        return 4;
                    return 0;
                });
        }

        private void AddSacriStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 100)
                        return 3;
                    if (Stats.Intelligence.Bases <= 150)
                        return 4;
                    if (Stats.Intelligence.Bases >= 151)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 100)
                        return 3;
                    if (Stats.Chance.Bases <= 150)
                        return 4;
                    if (Stats.Chance.Bases >= 151)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 100)
                        return 3;
                    if (Stats.Agility.Bases <= 150)
                        return 4;
                    if (Stats.Agility.Bases >= 151)
                        return 4;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 100)
                        return 3;
                    if (Stats.Strenght.Bases <= 150)
                        return 4;
                    if (Stats.Strenght.Bases >= 151)
                        return 4;
                    return 0;
                });
        }

        private void AddPandaStats(BaseStats stats)
        {
            AddGeneralStats(stats,
                () =>
                {
                    if (Stats.Intelligence.Bases <= 50)
                        return 1;
                    if (Stats.Intelligence.Bases <= 200)
                        return 2;
                    if (Stats.Intelligence.Bases >= 201)
                        return 3;
                    return 0;
                },
                () =>
                {
                    if (Stats.Chance.Bases <= 50)
                        return 1;
                    if (Stats.Chance.Bases <= 200)
                        return 2;
                    if (Stats.Chance.Bases >= 201)
                        return 3;
                    return 0;
                },
                () =>
                {
                    if (Stats.Agility.Bases <= 50)
                        return 1;
                    if (Stats.Agility.Bases <= 200)
                        return 2;
                    if (Stats.Agility.Bases >= 201)
                        return 3;
                    return 0;
                },
                () =>
                {
                    if (Stats.Strenght.Bases <= 50)
                        return 1;
                    if (Stats.Strenght.Bases <= 200)
                        return 2;
                    if (Stats.Strenght.Bases >= 201)
                        return 3;
                    return 0;
                });
        }

        public int GetMaxWeight()
        {

            return 1000 + Stats.Weight.Items + Stats.Strenght.Totals() * 5;
        }

        public int GetCurrentWeight()
        {

            return
                InventoryItem.Inventory.FindAll(x => x.IDCharacter == this)
                    .Sum(inventoryItem => inventoryItem.IDItems.Weight * inventoryItem.quantity);

        }

        public enum BaseStats
        {
            Strength = 10,
            Vitality = 11,
            Wisdom = 12,
            Chance = 13,
            Agility = 14,
            Intelligence = 15,
        }

        #endregion

        #region Items

        public bool CheckAnneau(ItemSets itemSet)
        {
            cacheLock.EnterReadLock();

            try
            {
                return InventoryItem.Inventory.Any(
                                x =>
                                    x.IDCharacter == this && x.IsEquiped() &&
                                    (x.position == Position.Anneau1 ||
                                     x.position == Position.Anneau2) &&
                                    x.IDItems.GetSet() == itemSet);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
            
        }

        #endregion

        public void Disconnected()
        {
            if (this.Maps != null)
                this.Maps.Remove(AuthClient,this);

           // Processor.CharacterStates = CharacterState.Free;
        }

        public void Send(AuthClient AuthClients, string data)
        {
            AuthClients.Send(data);
        }

    }
}
