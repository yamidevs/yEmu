using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Realm.Classes
{
    class GameServer
    {
        public int id { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string ServerKey { get; set; }
        public int State { get; set; }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};1", id, State, (id * 75));
        }
    }
}
