using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Enums
{
    public enum GameAction
    {
        MAP_MOVEMENT = 0,
        MAP_TELEPORT = 4,
        MAP_PUSHBACK = 5,
        CHANGE_MAP = 2,
        CHALLENGE_REQUEST = 900,
        CHALLENGE_ACCEPT = 901,
        CHALLENGE_DENY = 902,
        FIGHT_JOIN = 903,
        FIGHT_AGGRESSION = 906,
        EXCHANGE,
        FIGHT,
        FIGHT_HEAL = 100,
        FIGHT_KILLFIGHTER = 103,
        FIGHT_TACLE = 104,
        FIGHT_ARMOR = 105,
        FIGHT_LOSTPA = 102,
        FIGHT_LOSTPM = 129,
        FIGHT_DAMAGE = 100,
        FIGHT_LAUNCHSPELL = 300,
        FIGHT_LAUNCHSPELL_CRITIC = 301,
        FIGHT_LAUNCHSPELL_ECHEC = 302,
        FIGHT_USEWEAPON = 303,
        FIGHT_USEWEAPON_ECHEC = 305,
        FIGHT_DODGE_SUBPA = 308,
        FIGHT_DODGE_SUBPM = 309,
        MountPark = 175,
        BASIC_REQUEST,
        GUILD_CREATE,
    }

}
