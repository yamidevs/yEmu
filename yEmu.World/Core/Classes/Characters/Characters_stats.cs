using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes.Characters
{
   public class Characters_stats
    {
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

    }
}
