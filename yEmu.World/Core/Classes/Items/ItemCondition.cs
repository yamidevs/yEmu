using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.World.Core.Databases.Requetes;

namespace yEmu.World.Core.Classes.Items
{
    static class ItemCondition
    {

        public static bool VerifyIfCharacterMeetItemCondition(yEmu.World.Core.Classes.Characters.Characters character, string conditionString)
        {
            if (conditionString.Equals(string.Empty))
                return true;

            if (!conditionString.Contains("&") && !conditionString.Contains("|"))
            {
                conditionString = conditionString.Replace("(", "").Replace(")", "");

                return ParseItemConditionWithoutSpliter(character, conditionString);
            }

            if (!conditionString.Contains("&") && conditionString.Contains("|"))
            {
                return ParseItemConditionWithOrSpliter(character, conditionString);
            }

            return ParseItemConditionWithAndSpliter(character, conditionString);
        }

        private static bool ParseItemConditionWithoutSpliter(yEmu.World.Core.Classes.Characters.Characters character, string conditionString)
        {
            return Parse(conditionString, character);
        }

        private static bool ParseItemConditionWithAndSpliter(yEmu.World.Core.Classes.Characters.Characters character, string conditionString)
        {
            var conditions = conditionString.Split('&');

            return conditions.All(condition => Parse(condition, character) == true);
        }

        private static bool ParseItemConditionWithOrSpliter(yEmu.World.Core.Classes.Characters.Characters character, string conditionString)
        {
            conditionString = conditionString.Replace("(", "").Replace(")", "");

            var conditions = conditionString.Split('|');

            return conditions.Any(condition => Parse(condition, character) == true);
        }

        private static bool Parse(string condition, yEmu.World.Core.Classes.Characters.Characters character)
        {
            
                if (condition.Contains("("))
                    return ParseItemConditionWithOrSpliter(character, condition);

                var header = condition.Substring(0, 2);

                var balance = condition.Substring(2, 1);

                var value = int.Parse(condition.Substring(3));

                int characterStatsValue;

                bool avaliable;

                switch (header.Substring(0, 1))
                {
                    case "C":

                        switch (header.Substring(1, 1))
                        {
                            case "a":
                                characterStatsValue = character.Stats.Agility.Bases;
                                break;

                            case "i":
                                characterStatsValue = character.Stats.Intelligence.Bases;
                                break;

                            case "c":
                                characterStatsValue = character.Stats.Chance.Bases;
                                break;

                            case "s":
                                characterStatsValue = character.Stats.Strenght.Bases;
                                break;

                            case "v":
                                characterStatsValue = character.Stats.Vitality.Bases;
                                break;

                            case "w":
                                characterStatsValue = character.Stats.Wisdom.Bases;
                                break;

                            case "A":
                                characterStatsValue = character.Stats.Agility.Totals();
                                break;

                            case "I":
                                characterStatsValue = character.Stats.Intelligence.Totals();
                                break;

                            case "C":
                                characterStatsValue = character.Stats.Chance.Totals();
                                break;

                            case "S":
                                characterStatsValue = character.Stats.Strenght.Totals();
                                break;

                            case "V":
                                characterStatsValue = character.Stats.Vitality.Totals();
                                break;

                            case "W":
                                characterStatsValue = character.Stats.Wisdom.Totals();
                                break;

                            default:
                                return true;
                        }

                        break;

                    case "P":

                        switch (header.Substring(1, 1))
                        {
                            case "G":
                                characterStatsValue = (int)character.Classes;
                                break;
                            case "L":
                                characterStatsValue = character.level;
                                break;
                            case "K":
                                characterStatsValue = character.kamas;
                                break;
                            case "S":
                                characterStatsValue = character.sexe;
                                break;
                        /*    case "X":
                                 characterStatsValue = Character.characters.Find(x => x.accounts == character.accounts).accounts;
                                break;
                            case "W":
                                  characterStatsValue = character.GetMaxWeight();
                                break;*/
                            case "s":
                                characterStatsValue = character.Alignment.Type;
                                break;
                            case "a":
                                characterStatsValue = character.Alignment.Level;
                                break;
                            case "P":
                                characterStatsValue = character.Alignment.Grade;
                                break;

                            default:
                                return true;
                        }

                        break;

                    default:
                        return true;
                }

                if (balance == "") return false;

                switch (balance)
                {

                    case "<":
                        avaliable = (characterStatsValue < value ? true : false);
                        break;

                    case ">":
                        avaliable = (characterStatsValue > value ? true : false);
                        break;

                    case "=":
                    case "~":
                        avaliable = (characterStatsValue == value ? true : false);
                        break;

                    case "!":
                        avaliable = (characterStatsValue != value ? true : false);
                        break;

                    default:
                        return true;
                }

                return avaliable;
            }
            
        }
    }

