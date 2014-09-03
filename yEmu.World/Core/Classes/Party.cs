using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.World.Core.Databases.Requetes;

namespace yEmu.World.Core.Classes
{
   public class Party
    {
        public Dictionary<yEmu.World.Core.Classes.Characters.Characters, int> Members { get; set; }

        private string ownerName;
        private int ownerID;

        public Party(yEmu.World.Core.Classes.Characters.Characters leader)
        {
            Members = new Dictionary<yEmu.World.Core.Classes.Characters.Characters, int>();

            lock (Members)
                Members.Add(leader, 1);

            ownerID = leader.id;
            ownerName = leader.nom;
        }

        public void AddMember(AuthClient AuthClient)
        {
            lock(Members)
                Members.Add(AuthClient.Character, 0);

            AuthClient.Character.Party = this;

            if (Members.Count == 2)
            {
                Send(AuthClient,string.Concat("PCK", ownerName));
                Send(AuthClient,string.Concat("PL", ownerID));
                Send(AuthClient,string.Concat("PM", PartyPattern()));
            }
            else
            {
                AuthClient.Send(string.Concat("PCK", ownerName));
                AuthClient.Send(string.Concat("PL", ownerID));
                AuthClient.Send(string.Concat("PM", PartyPattern()));

                foreach (var character in Members.Keys.ToList().Where(x => x != AuthClient.Character))
                {
                    var client = Character.characters.FirstOrDefault(x => x.id == character.id);
                    var characters = Server.AuthClient.FirstOrDefault(x => x.Character == client);
                    characters.Send(string.Concat("PM", character.PatternOnParty()));

                }
            }

            UpdateMembers(AuthClient);
        }

        public void UpdateMembers(AuthClient AuthClient)
        {
            Send(AuthClient,string.Concat("PM~", string.Join("|", from x in Members.Keys.ToList().OrderByDescending(x => x.Initiative) select x.PatternOnParty())));
        }

        public void LeaveParty(AuthClient AuthClient, string name, string kicker = "")
        {
            if (!Members.Keys.ToList().Any(x => x.nom == name) || (kicker != "" && ownerID != int.Parse(kicker)))
                return;

            var character = Members.Keys.ToList().First(x => x.nom == name);
            character.Party = null;

            lock (Members)
                Members.Remove(character);

            Send(AuthClient,string.Concat("PM-", character.id));

           /* if (character.State.IsFollow)
            {
                character.State.Followers.Clear();
                character.State.IsFollow = false;
            }*/

            if (character.Connected)
                character.Send(AuthClient,string.Concat("PV", kicker));

            if (Members.Count == 1)
            {
                var last = Members.Keys.ToList()[0];
                last.Party = null;

                Members.Remove(last);

                if (last.Connected)
                    last.Send(AuthClient,string.Concat("PV", kicker));
            }
            else if (ownerID == character.id)
                GetNewLeader(AuthClient);

            if(Members.Count >= 2)
                UpdateMembers(AuthClient);
        }

        private void Send(AuthClient AuthClient,string text)
        {
            
            foreach (var character in Members.Keys)
            {
                
                var client = Character.characters.FirstOrDefault(x => x.id == character.id);
                var characters = Server.AuthClient.FirstOrDefault(x => x.Character == client);
                characters.Send(text);
            }
        }

        private void GetNewLeader(AuthClient AuthClient)
        {
            var character = Members.Keys.ToList()[0];
            Members[character] = 1;

            ownerID = character.id;
            ownerName = character.nom;

            Send(AuthClient,string.Concat("PL", ownerID));
        }

        private string PartyPattern()
        {
            return string.Concat("+", string.Join("|", from x in Members.Keys.ToList().OrderByDescending(x => x.Stats.Initiative.Totals()) select x.PatternOnParty()));
        }
    }
}
