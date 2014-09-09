using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.World.Core.Classes.Items.Stats;
using yEmu.World.Core.Classes.Mount;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Databases.Requetes.Mount;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Handler
{
    class MountHandler
    {
        public static Dictionary<int, int> IdMounts = new Dictionary<int, int>()
        {
            {7807 , 2},
            {7808 , 3},
            {7809 , 4},
            {7810 , 9},
            {7811 , 10},
            {7812 , 11},
            {7813 , 12},
            {7814 , 15},
            {7815 , 16},
            {7816 , 17},
            {7817 , 18},
            {7818 , 19},
            {7819 , 20},
            {7820 , 21},
            {7821 , 22},
            {7822 , 23},
            {7823 , 33},
            {7824 , 34},
            {7825 , 35},
            {7826 , 36},
            {7827 , 37},
            {7828 , 38},
            {7829 , 39},
            {7830 , 40},
            {7831 , 41},
            {7832 , 42},
            {7833 , 43},
            {7834 , 44},
            {7835 , 45},
            {7836 , 46},
            {7837 , 47},
            {7838 , 48},
            {7839 , 49},
            {7840 , 50},
            {7841 , 51},
            {7842 , 52},
            {7843 , 53},
            {7844 , 54},
            {7845 , 55},
            {7846 , 56},
            {7847 , 57},
            {7848 , 58},
            {7849 , 59},
            {7850 , 60},
            {7851 , 61},
            {7852 , 62},
            {7853 , 63},
            {7854 , 64},
            {7855 , 65},
            {7856 , 66},
            {7857 , 67},
            {7858 , 68},
            {7859 , 69},
            {7860 , 70},
            {7861 , 71},
            {7862 , 72},
            {7863 , 73},
            {7864 , 74},
            {7865 , 75},
            {7866 , 76},
            {7867 , 77},
            {7868 , 78},
            {7869 , 79},
            {7870 , 80},
            {7871 , 82},
            {7872 , 83},
            {7873 , 84},
            {7874 , 85},
            {7875 , 86},
            {7876 , 87},
            {9582 , 88},
            {50000 , 50000},
            {50001 , 50001},
            {50002 , 50002}  
        };

        public void EnclosReceive(Processor Processor , string data)
        {

            string Type = data.Substring(0,1);

            int packet;

            if (!int.TryParse(data.Substring(1), out packet))
            {

                return;
            }

            switch (Type)
            {
                case "C" :
                    this.Stock(Processor, packet);
                    break;
                case "g":
                    this.Equip(Processor, packet);
                    break;
                case "c":
                    this.Exchange(Processor, packet);
                    break;
                case "p":
                    this.Desiable(Processor, packet);
                    break;
            }
        }

        public void Stock(Processor Processor , int packet)
        {
            var Inventory = InventoryItem.Inventory.FirstOrDefault(x => x.id == packet);

            var Object = ItemsInfo.ItemsInfos.FirstOrDefault(x => x.ID == Inventory.itemId);

            var Mounts = Mount.Mounts.FirstOrDefault(x => x.id == Inventory.id);

            if (Mounts == null)
            {
                var id = IdMounts[Object.ID];
                Mounts = new Mounts(Object, Inventory, id);
            }

            Processor.Clients.Send(string.Concat("OR" , packet));
            Processor.Clients.Send(string.Concat("Ee+", Mounts.Pattern()));
        }

        public void Equip(Processor Processor, int packet)
        {
            var Inventory = InventoryItem.Inventory.FirstOrDefault(x => x.id == packet);

            var Mounts = Mount.Mounts.FirstOrDefault(x => x.id == Inventory.id);

            if (Mounts == null)
            {
                return;
            }

            Processor.Clients.Send(string.Concat("Re+", Mounts.Pattern()));
            Processor.Clients.Send(string.Concat("Ee-", packet));
            Processor.Clients.Send(string.Concat("Rx", Mounts.GetXp));
        }

        public void Exchange(Processor Processor, int packet)
        {
            var Inventory = InventoryItem.Inventory.FirstOrDefault(x => x.id == packet);

            var Mounts = Mount.Mounts.FirstOrDefault(x => x.id == Inventory.id);

            if (Mounts == null)
            {
                return;
            }
            Inventory.IDStats.Clear();

            //TODO :   Partis a refaire dégeulasse  

            StatsItems Id = new StatsItems();
            Id.Header = (Effect)995;
            Id.MinValue = packet;
            Id.JetDecimal = string.Concat("0d0+", packet);
            Inventory.IDStats.Add(Id);

            StatsItems Name = new StatsItems();
            Name.Header = (Effect)996;
            Name.JetDecimal = Processor.Clients.Character.nom;
            Inventory.IDStats.Add(Name);

            StatsItems MountsName = new StatsItems();
            MountsName.Header = (Effect)997;
            MountsName.JetDecimal = Mounts.name;
            Inventory.IDStats.Add(MountsName);

            Processor.Clients.Send(string.Concat("OAKO", InventoryItem.Inventory.FirstOrDefault(x => x.id == Inventory.id).ItemInfo()));
            Processor.Clients.Send(string.Concat("Ee-", packet));

        }

        public void Desiable(Processor Processor, int packet)
        {
            var Inventory = InventoryItem.Inventory.FirstOrDefault(x => x.id == packet);

            var Mounts = Mount.Mounts.FirstOrDefault(x => x.id == Inventory.id);

            if (Mounts == null)
            {
                return;
            }

            Processor.Clients.Send(string.Concat("Ee+", Mounts.Pattern()));
            Processor.Clients.Send("Re-");
            Processor.Clients.Send(string.Concat("Rx", Mounts.GetXp));
        }
    }
}
