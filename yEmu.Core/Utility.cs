using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ngot.Core
{
    public static class Utility
    {

        public static IPAddress ParseOrResolve(string input)
        {
            IPAddress addr;
            if (IPAddress.TryParse(input, out addr))
            {
                return addr;
            }

            // try resolve synchronously
            var addresses = Dns.GetHostAddresses(input);

            // for now only do Ipv4 address (apparently the wow client doesnt support Ipv6 yet)
            addr = addresses.Where(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault();

            return addr ?? IPAddress.Loopback;
        }
    }
}
