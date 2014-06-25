using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Realm.Classes
{
    class Account
    {
        public int id 
        { 
            get;
            set;
        }

        public string username
        { 
            get;
            set;
        }

        public string pass 
        { 
            get;
            set;
        }

        public string pseudo 
        { 
            get;
            set;
        }

        public int gmLevel 
        { 
            get;
            set;
        }

        public string question 
        { 
            get;
            set;
        }

        public string Nom 
        { 
            get;
            set;
        }

        public int ServerID 
        { 
            get;
            set;
        }

        public Dictionary<int, int> personnages 
        { 
            get;
            set; 
        }

        public DateTime SubscriptionDate 
        { 
            get;
            set; 
        }

        public int AccountsID 
        { 
            get;
            set; 
        }

        public long numberCharacters 
        {
            get;
            set;
        }

        public Account()
        {
            personnages = new Dictionary<int, int>();
            SubscriptionDate = new DateTime();
        }

    }
}
