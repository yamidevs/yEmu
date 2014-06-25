using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm.Databases.Requetes;

namespace yEmu.Realm.Packets
{
    sealed class ServerList :IPacket
    {
        Processor _p;
        public ServerList(Processor p)
        {
            _p = p;
        }
        public override string ToString()
        {
            var packets = string.Empty;
                 
              var server = Accounts.getAccounts(_p.GetAccounts.id);
                packets = string.Format("{0}{1}{2}", "AxK", 31536000000, server);                
              return packets;

            }
        }
    }

