using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Realm.Classes
{
    class GameServer
    {
        public int id 
        { 
            get;
            set; 
        }

        public string ip 
        { 
            get;
            set;
        }

        public int port 
        {
            get;
            set;
        }

        public string ServerKey
        { 
            get; 
            set; 
        }

        public int State 
        {
            get;
            set;
        }

        public List<string> Clients
        { 
            get;
            set;
        }

        public GameServer()
        {
            Clients = new List<string>();
        }
        
        public override string ToString()
        {
            return string.Format("{0};{1};0;1", id, State);

        }
    }
}
