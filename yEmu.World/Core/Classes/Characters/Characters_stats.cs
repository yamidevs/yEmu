using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.World.Core.Classes.Items;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Characters
{
   public class Characters_stats
    {
       private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
       public int id { get; set; }
       public BaseStats Initiative = new BaseStats();
       public BaseStats Vitality = new BaseStats();
       public BaseStats Wisdom = new BaseStats();
       public BaseStats Strenght = new BaseStats();
       public BaseStats Intelligence = new BaseStats();
       public BaseStats Chance = new BaseStats();
       public BaseStats Agility = new BaseStats();
       public BaseStats PdvNow = new BaseStats();
       public BaseStats Pa = new BaseStats();
       public BaseStats Pm = new BaseStats();
       public BaseStats Po = new BaseStats();
       public BaseStats Prospection = new BaseStats();
       public BaseStats Weight = new BaseStats();
       public BaseStats MaxInvoc = new BaseStats();
       public BaseStats ReducDamageAir = new BaseStats();
       public BaseStats ReduceDamageChance = new BaseStats();
       public BaseStats ReduceDamageIntelligence = new BaseStats();
       public BaseStats ReduceDamageStrength = new BaseStats();
       public BaseStats ReduceDamageNeutral = new BaseStats();
       public BaseStats ReducDamagePvpAir = new BaseStats();
       public BaseStats ReduceDamagePvpChance = new BaseStats();
       public BaseStats ReduceDamagePvpIntelligence = new BaseStats();
       public BaseStats ReduceDamagePvpStrength = new BaseStats();
       public BaseStats ReduceDamagePvpNeutral = new BaseStats();
       public BaseStats ReduceDamagePercentAir = new BaseStats();
       public BaseStats ReduceDamagePercentChance = new BaseStats();
       public BaseStats ReduceDamagePercentIntelligence = new BaseStats();
       public BaseStats ReduceDamagePercentStrenght = new BaseStats();
       public BaseStats ReduceDamagePercentNeutral = new BaseStats();
       public BaseStats ReduceDamagePercentPvPAir = new BaseStats();
       public BaseStats ReduceDamagePercentPvPChance = new BaseStats();
       public BaseStats ReduceDamagePercentPvPIntelligence = new BaseStats();
       public BaseStats ReduceDamagePercentPvPStrenght = new BaseStats();
       public BaseStats ReduceDamagePercentPvPNeutral = new BaseStats();
       public BaseStats ReducPhysicalDamage = new BaseStats();
       public BaseStats ReducMagicDamage = new BaseStats();
       public BaseStats DodgePa = new BaseStats();
       public BaseStats DodgePm = new BaseStats();
       public BaseStats ReturnDamage = new BaseStats();
       public BaseStats CriticalDamage = new BaseStats();
       public BaseStats FailDamage = new BaseStats();
       public BaseStats Damage = new BaseStats();
       public BaseStats PercentDamage = new BaseStats();
       public BaseStats PhysicalDamage = new BaseStats();
       public BaseStats MagicDamage = new BaseStats();
       public BaseStats TrapDamage = new BaseStats();
       public BaseStats TrapPercentDamage = new BaseStats();
       public BaseStats Heal = new BaseStats();

       public void Calculate(yEmu.World.Core.Classes.Characters.Characters character)
       {
           cacheLock.EnterReadLock();
           try
           {
               var items = InventoryItem.Inventory.FindAll(x => x.IDCharacter == character && x.IsEquiped());

               foreach (var stats in from inventoryItem in items
                                     from stats in inventoryItem.IDStats
                                     select stats)
               {
                   ParseStats(stats);
               }

               foreach (var itemSetBonus in from set in character.GetSets()
                                            let numberOfItemsEquipedInSet = character.GetAllItemsEquipedInSet(set).Count
                                            where set.BonusesDictionary.ContainsKey(numberOfItemsEquipedInSet)
                                            from itemSetBonus in set.BonusesDictionary[numberOfItemsEquipedInSet]
                                            select itemSetBonus)
               {
                   ParseStats(itemSetBonus);
               }
           }
           finally
           {
               cacheLock.ExitReadLock();
           }
          
       }
       public void AddItemStats(IEnumerable<Jet> stats)
       {
           foreach (var itemStatse in stats)
           {
               ParseStats(itemStatse);
           }
       }

       public void RemoveItemStats(IEnumerable<Jet> stats)
       {
           cacheLock.EnterReadLock();
           try
           {
               var invertedStats = stats.Select(stat => new Jet
               {
                   Header = stat.Header,
                   MinValue = -stat.MinValue,
               }).ToList();

               foreach (var invertedStat in invertedStats)
               {
                   ParseStats(invertedStat);
               }
           }
           finally
           {
               cacheLock.ExitReadLock();
           }
         
       }

       public void DecreaseItemSetEffect(InventoryItems item)
       {

           var itemSet = item.IDItems.GetSet();
           var character = item.IDCharacter ;

           var numberOfItemsEquipedInSet = InventoryItem.Inventory.Count(
               x => x.IDCharacter == character && x.IDItems.GetSet() == itemSet && x.IsEquiped());

           if (itemSet.BonusesDictionary.ContainsKey(numberOfItemsEquipedInSet + 1))
           {
               var oldBonusSet = itemSet.BonusesDictionary[numberOfItemsEquipedInSet + 1];

               RemoveItemStats(oldBonusSet);
           }

           if (!itemSet.BonusesDictionary.ContainsKey(numberOfItemsEquipedInSet)) return;

           var newBonusSet = itemSet.BonusesDictionary[numberOfItemsEquipedInSet];

           AddItemStats(newBonusSet);
       }

       public void IncreaseItemSetEffect(InventoryItems item)
       {
           var itemSet = item.IDItems.GetSet();
           var character = item.IDCharacter;
           try
           {
               var numberOfItemsEquipedInSet = InventoryItem.Inventory.Count(
              x => x.IDCharacter == character && x.IDItems.GetSet() == itemSet && x.IsEquiped());

               if (itemSet.BonusesDictionary.ContainsKey(numberOfItemsEquipedInSet - 1))
               {
                   var oldBonusSet = itemSet.BonusesDictionary[numberOfItemsEquipedInSet - 1];

                   RemoveItemStats(oldBonusSet);
               }

               if (!itemSet.BonusesDictionary.ContainsKey(numberOfItemsEquipedInSet)) return;

               var newBonusSet = itemSet.BonusesDictionary[numberOfItemsEquipedInSet];

               AddItemStats(newBonusSet);
           }
           catch (Exception e)
           {
               Console.WriteLine(e);
           }
       }


       private void ParseStats(Jet stats)
       {
           if (WeaponEffect.Contains(stats.Header))
               return;

           // Checking stats Header (Element)
           switch (stats.Header)
           {
               // Positive basic effects

               case Effect.AddAgilite:
                   Agility.Items += stats.MinValue;
                   break;
               case Effect.AddChance:
                   Chance.Items += stats.MinValue;
                   break;
               case Effect.AddForce:
                   Strenght.Items += stats.MinValue;
                   break;
               case Effect.AddIntelligence:
                   Intelligence.Items += stats.MinValue;
                   break;
               case Effect.AddVitalite:
                   Vitality.Items += stats.MinValue;
                   break;
               case Effect.AddSagesse:
                   Wisdom.Items += stats.MinValue;
                   break;
               case Effect.AddLife:
                   PdvNow.Items += stats.MinValue;
                   break;
               case Effect.AddPa:
                   Pa.Items += stats.MinValue;
                   break;
               case Effect.AddPm:
                   Pm.Items += stats.MinValue;
                   break;
               case Effect.AddPo:
                   Po.Items += stats.MinValue;
                   break;
               case Effect.AddInvocationMax:
                   MaxInvoc.Items += stats.MinValue;
                   break;
               case Effect.AddInitiative:
                   Initiative.Items += stats.MinValue;
                   break;
               case Effect.AddProspection:
                   Prospection.Items += stats.MinValue;
                   break;
               case Effect.AddPods:
                   Weight.Items += stats.MinValue;
                   break;

               // Negative basic effects

               case Effect.SubAgilite:
                   Agility.Items -= stats.MinValue;
                   break;
               case Effect.SubChance:
                   Chance.Items -= stats.MinValue;
                   break;
               case Effect.SubForce:
                   Strenght.Items -= stats.MinValue;
                   break;
               case Effect.SubIntelligence:
                   Intelligence.Items -= stats.MinValue;
                   break;
               case Effect.SubVitalite:
                   Vitality.Items -= stats.MinValue;
                   break;
               case Effect.SubSagesse:
                   Wisdom.Items -= stats.MinValue;
                   break;
               case Effect.SubPa:
                   Pa.Items -= stats.MinValue;
                   break;
               case Effect.SubPm:
                   Pm.Items -= stats.MinValue;
                   break;
               case Effect.SubPo:
                   Po.Items -= stats.MinValue;
                   break;
               case Effect.SubInitiative:
                   Initiative.Items -= stats.MinValue;
                   break;
               case Effect.SubProspection:
                   Prospection.Items -= stats.MinValue;
                   break;
               case Effect.SubPods:
                   Weight.Items -= stats.MinValue;
                   break;

               // Positive Reduc damage effects

               case Effect.AddReduceDamageAir:
                   ReducDamageAir.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamageEau:
                   ReduceDamageChance.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamageFeu:
                   ReduceDamageIntelligence.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamageTerre:
                   ReduceDamageStrength.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamageNeutre:
                   ReduceDamageNeutral.Items += stats.MinValue;
                   break;

               // Positive Reduc damage Pvp effects

               case Effect.AddReduceDamagePvPAir:
                   ReducDamagePvpAir.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePvPEau:
                   ReduceDamagePvpChance.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePvPFeu:
                   ReduceDamagePvpIntelligence.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePvPTerre:
                   ReduceDamagePvpStrength.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePvPNeutre:
                   ReduceDamagePvpNeutral.Items += stats.MinValue;
                   break;

               // Positive Reduc percent damage effects

               case Effect.AddReduceDamagePourcentAir:
                   ReduceDamagePercentAir.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentEau:
                   ReduceDamagePercentChance.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentFeu:
                   ReduceDamagePercentIntelligence.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentTerre:
                   ReduceDamagePercentStrenght.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentNeutre:
                   ReduceDamagePercentNeutral.Items += stats.MinValue;
                   break;

               // Positive Reduc percent damage Pvp effects

               case Effect.AddReduceDamagePourcentPvPAir:
                   ReduceDamagePercentPvPAir.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentPvPEau:
                   ReduceDamagePercentPvPChance.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentPvPFeu:
                   ReduceDamagePercentPvPIntelligence.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentPvPTerre:
                   ReduceDamagePercentPvPStrenght.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamagePourcentPvpNeutre:
                   ReduceDamagePercentPvPNeutral.Items += stats.MinValue;
                   break;

               // Positive Reduc damages Type

               case Effect.AddReduceDamagePhysic:
                   ReducPhysicalDamage.Items += stats.MinValue;
                   break;
               case Effect.AddReduceDamageMagic:
                   ReducMagicDamage.Items += stats.MinValue;
                   break;

               // Positive Dodge Pa Pm

               case Effect.AddEsquivePa:
                   DodgePa.Items += stats.MinValue;
                   break;
               case Effect.AddEsquivePm:
                   DodgePm.Items += stats.MinValue;
                   break;

               // Negative Dodge Pa Pm

               case Effect.SubEsquivePa:
                   DodgePa.Items -= stats.MinValue;
                   break;
               case Effect.SubEsquivePm:
                   DodgePm.Items -= stats.MinValue;
                   break;

               // Negative Reduc damage effects

               case Effect.SubReduceDamageAir:
                   ReducDamageAir.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamageEau:
                   ReduceDamageChance.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamageFeu:
                   ReduceDamageIntelligence.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamageTerre:
                   ReduceDamageStrength.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamageNeutre:
                   ReduceDamageNeutral.Items -= stats.MinValue;
                   break;

               // Negative Reduc percent damage effects

               case Effect.SubReduceDamagePourcentAir:
                   ReduceDamagePercentAir.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentEau:
                   ReduceDamagePercentChance.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentFeu:
                   ReduceDamagePercentIntelligence.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentTerre:
                   ReduceDamagePercentStrenght.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentNeutre:
                   ReduceDamagePercentNeutral.Items -= stats.MinValue;
                   break;

               // Negative Reduc percent damage Pvp effects

               case Effect.SubReduceDamagePourcentPvPAir:
                   ReduceDamagePercentPvPAir.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentPvPEau:
                   ReduceDamagePercentPvPChance.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentPvPFeu:
                   ReduceDamagePercentPvPIntelligence.Items += stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentPvPTerre:
                   ReduceDamagePercentPvPStrenght.Items -= stats.MinValue;
                   break;
               case Effect.SubReduceDamagePourcentPvpNeutre:
                   ReduceDamagePercentPvPNeutral.Items -= stats.MinValue;
                   break;

               // Positive different types of damage

               case Effect.AddRenvoiDamage:
                   ReturnDamage.Items += stats.MinValue;
                   break;
               case Effect.AddDamageCritic:
                   CriticalDamage.Items += stats.MinValue;
                   break;
               case Effect.AddEchecCritic:
                   FailDamage.Items += stats.MinValue;
                   break;
               case Effect.AddDamage:
                   Damage.Items += stats.MinValue;
                   break;
               case Effect.AddDamagePercent:
                   PercentDamage.Items += stats.MinValue;
                   break;
               case Effect.AddDamagePhysic:
                   PhysicalDamage.Items += stats.MinValue;
                   break;
               case Effect.AddDamageMagic:
                   MagicDamage.Items += stats.MinValue;
                   break;
               case Effect.AddDamagePiege:
                   TrapDamage.Items += stats.MinValue;
                   break;
               case Effect.AddDamagePiegePercent:
                   TrapPercentDamage.Items += stats.MinValue;
                   break;
               case Effect.AddSoins:
                   Heal.Items += stats.MinValue;
                   break;

               // Negative different types of damage

               case Effect.SubDamageCritic:
                   CriticalDamage.Items -= stats.MinValue;
                   break;
               case Effect.SubDamage:
                   Damage.Items -= stats.MinValue;
                   break;
               case Effect.SubDamagePhysic:
                   PhysicalDamage.Items -= stats.MinValue;
                   break;
               case Effect.SubDamageMagic:
                   MagicDamage.Items -= stats.MinValue;
                   break;
               case Effect.SubSoins:
                   Heal.Items -= stats.MinValue;
                   break;
               case Effect.LivingGfxId:
                   Console.WriteLine("saaaaaaaaaaaaaaaad ::: Obji");
                   break;
           }
       }


       public static Effect[] WeaponEffect =
        {
            Effect.VolEau,
            Effect.VolTerre,
            Effect.VolAir,
            Effect.VolFeu,
            Effect.VolNeutre,
            Effect.DamageEau,
            Effect.DamageTerre,
            Effect.DamageAir,
            Effect.DamageFeu,
            Effect.DamageNeutre
        };
    }
}
