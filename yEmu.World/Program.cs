using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Network;
using yEmu.Util;
using yEmu.World.Core.Classes.Accounts;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Databases.Requetes.Mount;
using yEmu.World.Core.Databases.Requetes.NPC;
using yEmu.World.Core.Databases.Requetes.Zaap;
using yEmu.World.InterCommunication;

namespace yEmu.World
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "yEmu World";

            Stopwatch Time = new Stopwatch();
            Time.Start();
            Info.Start();

            Configuration.LoadConfiguration();

                Servers.Instance.Load();
                Account.Instance.Load();
                Map.Instance.Load();
                Trigger.Instance.Load();
                Experience.Instance.Load();
                Character_Stats.Instance.Load();
                Alignment.Instance.Load();
                Character.Instance.Load();
                ItemsInfo.Instance.Load();
                ItemSet.Instance.Load();
                InventoryItem.Instance.Load();
                NPCReponse.Instance.Load();
                NPCQuestion.Instance.Load();
                NPCTemplate.Instance.Load();
                Npc.Instance.Load();
                Zaap.Instance.Load();
                Zaapi.Instance.Load();
                MountPark.Instance.Load();
                Mount.Instance.Load();

            IPAddress Ip = IPAddress.Parse(Configuration.getString("Game_Ip"));

            


            IPEndPoint LocalEndPoint = new IPEndPoint(Ip, 5556);
            TCPServer app = new TCPServer(LocalEndPoint, 100);

            IPEndPoint InterCommunicationLocalEndPointRealm = new IPEndPoint(Ip, 3450);
            InterClient InterCommunication = new InterClient();
            InterCommunication.Initialize("127.0.0.1", 3450);
            app.Run();
 

            Time.Stop();
    
 

            Console.WriteLine("Time : {0} secondes", Time.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture).Substring(0, 4));

            Console.ReadLine();
        

        }
    }
}
