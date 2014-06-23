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
            foreach (var server in GameServers.servers)
            {
                if (!_p.getAccounts.personnages.ContainsKey(server.id))
                    _p.getAccounts.personnages.Add(server.id, new List<string>());
                packets = string.Format("{0}{1}|{2}{3}", "AxK", 31536000000, server.id, _p.getAccounts.personnages[server.id].Count);
            }
             return packets;
        }
    }
}
