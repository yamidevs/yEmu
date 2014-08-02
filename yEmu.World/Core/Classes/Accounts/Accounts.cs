using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes.Accounts
{
   public class Accounts
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Question { get; set; }
        public string Reponse { get; set; }
        public string Pseudo { get; set; }
        public int Level { get; set; }
        public DateTime? BannedUntil { get; set; }
        public DateTime? Subscription { get; set; }

        public bool IsNotSubscribed()
        {
            return Subscription == null || Subscription.Value < DateTime.Now;
        }
    }
}
