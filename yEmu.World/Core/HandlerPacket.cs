using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core;
using yEmu.World.Core.Classes.Accounts;
using yEmu.World.Core.Classes.Characters;
using yEmu.World.Core.Classes.Maps;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Enums;

namespace yEmu.World
{
    public class HandlerPacket
    {
        public static object  Lock = new Object();
        public static void ReponseParse(string data)
        {
       
            var date = data.Split('|')[0];

            var ip = data.Split('|')[1];
            IPEndPoint Ip = Processor.Clients.Sock.RemoteEndPoint as IPEndPoint;


                var account = data.Split('|')[2].Split(',');
                try
                {
                    Processor.Clients.Accounts =
                                        new Accounts
                                        {
                                            Id = int.Parse(account[0]),
                                            Username = account[1],
                                            Password = account[2],
                                            Pseudo = account[3],
                                            Question = account[4],
                                            Reponse = account[5],
                                            Level = int.Parse(account[6]),
                                            BannedUntil =
                                                account[7] == ""
                                                    ? (DateTime?)null
                                                    : DateTime.Parse(account[7].ToString(CultureInfo.InvariantCulture)),
                                            Subscription =
                                                account[8] == ""
                                                    ? (DateTime?)null
                                                    : DateTime.Parse(account[8].ToString(CultureInfo.InvariantCulture))

                                        };
                }
                catch
                {
                    Console.WriteLine("Packet d'account incorrect");
                }

                Processor._stats = WorldStats.Personnages;
                Processor.Clients.Send("ATK0");
                       
        }

        public static void GameInfos()
        {
            if (Processor.Clients.Character == null)
            {
                return;
            }

            Processor.Clients.Send(string.Format("{0}|1|{1}", "GCK", Processor.Clients.Character.nom));
            Processor.Clients.Send(Processor.Clients.Character.GetStats());
            Processor.Clients.Send("AR6bk");
            Processor.Clients.Character.Channels.Send();
            Processor.Clients.Send("SLo+");
            Processor.Clients.Send(string.Concat("BT", Info.GetActualTime()));

            Processor.Clients.Send(string.Format("{0}{1}", "Im", "189"));

            Processor.Clients.Send(string.Format("{0}|{1}|{2}|{3}", "GDM", Processor.Clients.Character.Maps.ID,
            Processor.Clients.Character.Maps.CreateTime, Processor.Clients.Character.Maps.DecryptKey));
            Processor._stats = WorldStats.InGame;
        }

        public static void CharacterMove(string data)
        {
            if (Processor.CharacterStates != CharacterState.Free && Processor.CharacterStates != CharacterState.OnMove)
            {
                return;
            }

            if (data.Length < 3)
            {
                return;
            }
            data = data.Substring(3);
            
            var path = new PathFinding(
                data,
                Processor.Clients.Character.Maps,
                Processor.Clients.Character.CellId,
                Processor.Clients.Character.Direction);

            var newPath = path.RemakePath();

            newPath = path.GetStartPath + newPath;

            if (!Processor.Clients.Character.Maps.Cells.Contains(path.Destination))
            {
                Processor.Clients.Send(string.Format("{0};0", "GA"));
                return;
            }

            Processor.Clients.Character.Direction = path.Direction;
            Processor.Clients.Character.CellDestination = path.Destination;

            Processor.CharacterStates = CharacterState.OnMove;

            Processor.Clients.Character.Maps.Send(string.Format("{0}0;1;{1};{2}", "GA", Processor.Clients.Character.id, newPath));
        }

        public static void CharacterEndMove()
        {
            if (Processor.CharacterStates != CharacterState.Free && Processor.CharacterStates != CharacterState.OnMove)
            {
                return;
            }

            Processor.Clients.Character.CellId = Processor.Clients.Character.CellDestination;

            var trigger =
               Trigger.Triggers.Find(
                   x => x.CellID == Processor.Clients.Character.CellId
                        && x.MapID == Processor.Clients.Character.GetMap().ID);

            if (trigger != null)
            {
                HandlerPacket.Teleport(trigger.NewMap, trigger.NewCell);

            }

            Processor.Clients.Send("BN");


        }

        public static void Teleport(int maps , int cell)
        {

            Processor.Clients.Character.GetMap().Remove(Processor.Clients.Character);

            Processor.Clients.Send(string.Format("{0};2;{1};", "GA", Processor.Clients.Character.id));

            Processor.Clients.Character.CellId = cell;

            Processor.Clients.Character.Maps = Map.Maps.Find(x => x.ID == maps);

            Processor.Clients.Send(string.Format("{0}|{1}|{2}|{3}", "GDM", Processor.Clients.Character.Maps.ID,
            Processor.Clients.Character.Maps.CreateTime, Processor.Clients.Character.Maps.DecryptKey));

        }

        public static void ChangeDestination(string data)
        {
            Processor.Clients.Send("BN");

            if (Processor.CharacterStates != CharacterState.OnMove)
            {
                return;
            }

            if (!data.Contains('|'))
            {
                return;
            }

            var cell = int.Parse(data.Split('|')[1]);

            if (Processor.Clients.Character.Maps.Cells.Contains(cell))
            {
                Processor.Clients.Character.CellId = cell;
            }
        }

        #region Partie Chat

        public static void GeneralMessages(string data)
        {
            if (Processor.Clients.Character.Maps == null)
            {
                return;
            }
            if (data.Substring(0, 1) == ".")
            {
                Console.WriteLine("une commande");
            }
            Processor.Clients.Character.Maps.Send(string.Format("cMK|{0}|{1}|{2}", Processor.Clients.Character.id, Processor.Clients.Character.nom, data));
        }

        public static void RecrutementMessages(string data)
        {
            if(Processor.Clients.Character.CantRecrutement() == true)
            {
                Processor.Clients.Character.Maps.Send(string.Format("cMK?|{0}|{1}|{2}", Processor.Clients.Character.id, Processor.Clients.Character.nom, data));
                Processor.Clients.Character.RefreshRecrutement();
            }
            else
            {
                Processor.Clients.Send(string.Concat("Im0115;", Processor.Clients.Character.TimeRecrutement()));
            }
        }

        public static void TradeMessages(string data)
        {
            if (Processor.Clients.Character.CantTrade() == true)
            {
                Processor.Clients.Character.Maps.Send(string.Format("cMK:|{0}|{1}|{2}", Processor.Clients.Character.id, Processor.Clients.Character.nom, data));
                Processor.Clients.Character.RefreshTrade();
            }
            else
            {
                Processor.Clients.Send(string.Concat("Im0115;", Processor.Clients.Character.TimeTrade()));
            }

        }

        public static void PrivateMessages(string receive , string messages)
        {
            if (Character.characters.Any(x => x.nom == receive) && receive != null)
            {
                var character = Character.characters.First(x => x.nom == receive);
                character.Send(string.Format("cMKF|{0}|{1}|{2}", Processor.Clients.Character.id, Processor.Clients.Character.nom, messages));

                Processor.Clients.Send(string.Format("cMKT|{0}|{1}|{2}", Processor.Clients.Character.id,character.nom, messages));
            }
            else
            {
                Processor.Clients.Send(string.Concat("cMEf", receive));
            }
        }

        #endregion
    }
}
