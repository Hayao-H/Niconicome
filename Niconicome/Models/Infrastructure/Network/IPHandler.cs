using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.Core;

namespace Niconicome.Models.Infrastructure.Network
{
    public class IPHandler : IIPHandler
    {
        public string GetMyLocalIP()
        {
            string hostname = Dns.GetHostName();
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);

            foreach (var address in adrList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }

            return "localhost";
        }

    }
}
