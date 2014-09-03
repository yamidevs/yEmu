using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Handler
{
    class ChannelHandler
    {
        public void Messages(Processor Processor, string channel, string message)
        {
            switch (channel)
            {
                case "*":
                    Processor.HandlerPackets.GeneralMessages(message);
                    return;

                case "?":
                    Processor.HandlerPackets.RecrutementMessages(message);
                    break;

                case ":":
                    Processor.HandlerPackets.TradeMessages(message);
                    break;

                case"$":
                    Processor.HandlerPackets.PartyMessages(message);
                    break;
                default:

                    if (channel.Length > 1)
                    {
                        Processor.HandlerPackets.PrivateMessages(channel, message);
                    }
                    break;
            }
        }
    }
}
