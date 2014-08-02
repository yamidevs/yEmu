using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using yEmu.Realm;
using yEmu.Realm.enums;
using yEmu.Util;

namespace yEmu.Network.Realm
{

    class Queue
    {
        public static List<Processor> Clients 
        {
            get;
            set;
        }
       
        private static System.Timers.Timer timer;
       
        private static bool _run;

        public static long Action 
        { 
            get;
            set;
        }
       
        public static void Start()
        {
            Clients = new List<Processor>();
            timer = new System.Timers.Timer();
            // intervale 1 seconde == 1000ms
            timer.Interval = 4000;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _run = true;

        }

        public static void add(Processor client)
        {
            Info.Write("",string.Format("Nouveau client dans la Queue"),ConsoleColor.Gray);

            lock (Clients)
                Clients.Add(client);

            if (!_run)
            {
                _run = true;
                timer.Start();
            }
        }

        public static void OnTimedEvent(object sender, EventArgs e)
        {
            if (Clients.Count <= 0)
                return;
            Clients[0].SendInformations();
            Clients[0].VerifRealm = RealmStats.Server;

            lock (Clients)
                Clients.Remove(Clients[0]);

            if (Clients.Count <= 0)
            {
                _run = false;
                timer.Stop();
            }
        }
    }
}
