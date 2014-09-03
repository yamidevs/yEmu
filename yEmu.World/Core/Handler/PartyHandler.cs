using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.World.Core.Classes;
using yEmu.World.Core.Classes.Characters;
using yEmu.World.Core.Databases.Requetes;

namespace yEmu.World.Core.Handler
{
    class PartyHandler
    {

        public void PartyInvite(Processor Processor, string data)
        {
            if (Character.characters.Any(x => x.nom == data && x.Connected))
            {
                var client = Character.characters.FirstOrDefault(x => x.nom == data);
                var character = Server.AuthClient.FirstOrDefault(x => x.Character == client);

                if (character.Character.party == true || character.Character.Party != null)
                {
                    Processor.Clients.Send(string.Concat("PIEa", data));
                    return;
                }

                if (character.Character.party == true || character.Character.Party != null)
                {
                    if (character.Character.Party.Members.Count < 8)
                    {
                        character.Character.SendParty = Processor.Clients.Character.id;
                        character.Character.WaitParty = true;

                        Processor.Clients.Character.ReceiveParty = character.Character.id;
                        Processor.Clients.Character.WaitParty = true;

                        Processor.Clients.Send(string.Format("PIK{0}|{1}", Processor.Clients.Character.nom, character.Character.nom));
                        character.Send(string.Format("PIK{0}|{1}", Processor.Clients.Character.nom, character.Character.nom));
                    }
                    else
                    {
                        Processor.Clients.Send(string.Concat("PIEf", data));
                        return;
                    }
                }
                else
                {

                    character.Character.SendParty = Processor.Clients.Character.id;
                    character.Character.WaitParty = true;

                    Processor.Clients.Character.ReceiveParty = character.Character.id;
                    Processor.Clients.Character.WaitParty = true;

                    Processor.Clients.Send(string.Format("PIK{0}|{1}", Processor.Clients.Character.nom, character.Character.nom));
                    character.Send(string.Format("PIK{0}|{1}", Processor.Clients.Character.nom, character.Character.nom));
                }
            }
            else
            {
                Processor.Clients.Send(string.Concat("PIEn", data));
            }
        }

        public void PartyRefuse(Processor Processor, string data)
        {

            if (Processor.Clients.Character.SendParty == -1)
            {
                Processor.Clients.Send("BN");
                return;
            }

            var client = Character.characters.FirstOrDefault(x => x.id == Processor.Clients.Character.SendParty);
            var character = Server.AuthClient.FirstOrDefault(x => x.Character == client);
            if (character.Character.Connected == false || character.Character.ReceiveParty != Processor.Clients.Character.id)
            {

                Processor.Clients.Send("BN");
                return;
            }

            character.Character.ReceiveParty = -1;
            character.Character.WaitParty = false;

            Processor.Clients.Character.SendParty = -1;
            Processor.Clients.Character.WaitParty = false;    

            character.Send("PR");

        }

        public void PartyAccepted(Processor Processor, string data)
        {
            if (Processor.Clients.Character.SendParty != -1 && Processor.Clients.Character.WaitParty)
            {
                var client = Character.characters.FirstOrDefault(x => x.id == Processor.Clients.Character.SendParty);
                var character = Server.AuthClient.FirstOrDefault(x => x.Character == client);

                if (character.Character.Connected == false || character.Character.ReceiveParty != Processor.Clients.Character.id)
                {

                    Processor.Clients.Character.SendParty = -1;
                    Processor.Clients.Character.WaitParty = false;
                    Processor.Clients.Send("BN");
                    return;
                }

                Processor.Clients.Character.SendParty = -1;
                Processor.Clients.Character.WaitParty = false;

                character.Character.SendParty = -1;
                character.Character.WaitParty = false;

                if (character.Character.Party == null)
                {
                    character.Character.Party = new Party(character.Character);
                    character.Character.Party.AddMember(Processor.Clients);
                }
                else
                {
                    if (character.Character.Party.Members.Count > 7)
                    {

                        Processor.Clients.Send("BN");
                        character.Send("PR");
                        return;
                    }
                    character.Character.Party.AddMember(Processor.Clients);
                }

                character.Send("PR");
            }
            else
            {

                Processor.Clients.Character.SendParty = -1;
                Processor.Clients.Character.WaitParty = false;
                Processor.Clients.Send("BN");
            }
        }
    }
}
