using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.Realm.enums;
using yEmu.Realm.Packets;
using yEmu.Util;

namespace yEmu.Realm
{
    class Processor
    {
        Client _client;
        private RealmStats VerifRealm;


        public Processor(Client client)
        {
            _client = client;
        }
        public void Parser(string data)
        {
            switch (VerifRealm)
            {
                case RealmStats.Version:
                    if (Constant.Version != data)
                    {
                        _client.Send(new VersionClient().ToString());
                        _client._server.OnClose();
                    }
                    else
                        VerifRealm = RealmStats.Account;
                    
                    break;
                case RealmStats.Account:
                   // VerificateAccount(data);
                    Console.WriteLine("TEST VERFICTION ACCOUNT");
                    break;
                case RealmStats.Server:
                    //VerificateList(data);
                    break;

            }
        }
    }
}
