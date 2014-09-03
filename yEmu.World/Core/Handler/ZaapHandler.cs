using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.World.Core.Classes.Characters;
using yEmu.World.Core.Classes.Maps;
using yEmu.World.Core.Databases.Requetes;
using yEmu.World.Core.Databases.Requetes.Zaap;

namespace yEmu.World.Core.Handler
{
    class ZaapHandler
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private object Lock = new object();
        public void ZaapAction(Processor Processor , string data)
        {
            cacheLock.EnterReadLock();
            try
            {
                var zaap = Zaap.Zaaps.FirstOrDefault(x => x.mapID == Processor.Clients.Character.Maps.ID);
                var packet = new StringBuilder();
                if (Characters.PacketZaap.Any(x => x == zaap.mapID))
                {
                    packet.Append("WC");
                    packet.Append(zaap.mapID);


                    if (Characters.PacketZaap.Count >= 1)
                    {
                        foreach (var result in Characters.PacketZaap)
                        {
                            var maps = Map.Maps.FirstOrDefault(x => x.ID == result);

                            packet.Append("|");
                            packet.Append(result);
                            packet.Append(";");
                            packet.Append(CalcPrice(Processor.Clients.Character.Maps, maps));
                        }
                    }

                    Processor.Clients.Send(packet.ToString());
                    packet.Clear();

                }
                else
                {
                    packet.Append("WC");
                    packet.Append(zaap.mapID);

                    if (Characters.PacketZaap.Count >= 0)
                    {
                        foreach (var result in Characters.PacketZaap)
                        {
                            packet.Append("|");
                            var maps = Map.Maps.FirstOrDefault(x => x.ID == result);
                            packet.Append(result);
                            packet.Append(";");
                            packet.Append(CalcPrice(Processor.Clients.Character.Maps, maps));

                        }
                    }

                    packet.Append("|");
                    packet.Append(zaap.mapID);
                    packet.Append(";");
                    packet.Append(CalcPrice(Processor.Clients.Character.Maps, Processor.Clients.Character.Maps));

                    Processor.Clients.Send("Im024");

                    Processor.Clients.Send(packet.ToString());

                    packet.Clear();

                    lock (Lock)
                    Characters.PacketZaap.Add(zaap.mapID);

                }
            }
            finally
            {
                cacheLock.ExitReadLock();
                cacheLock.Dispose();
            }
             
        }

        public void ZaapUse(Processor Processor, string data)
        {
            if (Zaap.Zaaps.Any(x => x.mapID == int.Parse(data)))
            {
                var zaap = Zaap.Zaaps.First(x => x.mapID == int.Parse(data));
                var maps = Map.Maps.FirstOrDefault(x => x.ID == zaap.mapID);
                var price = CalcPrice(Processor.Clients.Character.Maps, maps);

                lock (Lock)
                {
                    Processor.Clients.Character.kamas -= price;
                }
                Processor.Clients.Send(string.Concat("Im046;", price));
                Processor.HandlerPackets.Teleport(zaap.mapID,zaap.cellID);
                Processor.Clients.Send(string.Format("{0}{1}|{2}", "Ow", Processor.Clients.Character.GetCurrentWeight(), Processor.Clients.Character.GetMaxWeight()));
                Processor.Clients.Send(Processor.Clients.Character.GetStats());
                Processor.Clients.Send("WV");

            }
        }

        public void ZaapiAction(Processor Processor, string data)
        {
            var zaapiId = Zaapi.Zaapis.FirstOrDefault(x => x.mapid == Processor.Clients.Character.Maps.ID);
            var zaapiList = Zaapi.Zaapis.Where(x => x.zone == zaapiId.zone);

            var packet = new StringBuilder("Wc");

            packet.Append(Processor.Clients.Character.Maps.ID);
            packet.Append("|");

            foreach (var result in zaapiList)
            {
                packet.Append(result.mapid);
                packet.Append(";20|");
            }

            Processor.Clients.Send(packet.ToString());
            packet.Clear();

        }

        public void ZaapiUse(Processor Processor, string data)
        {
            Processor.Clients.Send("Im046;500");

            var ZaapId = Zaapi.Zaapis.FirstOrDefault(x => x.mapid == int.Parse(data));
            Processor.HandlerPackets.Teleport(ZaapId.mapid, ZaapId.cellid);
            Processor.Clients.Send("Wv");
        }

        private static int CalcPrice(Maps_data startMap, Maps_data nextMap)
        {
            return (int)(10 * (Math.Abs(nextMap.PosX - startMap.PosX) + Math.Abs(nextMap.PosY - startMap.PosY)));
        }

    }
}
