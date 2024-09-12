using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public interface IIPHandler
    {
        /// <summary>
        /// ローカルIPアドレスを取得する
        /// </summary>
        /// <returns></returns>
        string GetMyLocalIP();
    }

    public class IPHandler : IIPHandler
    {
        private string? _localIP;

        public string GetMyLocalIP()
        {
            if (this._localIP is not null) return _localIP;

            string hostname = Dns.GetHostName();
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);

            foreach (var address in adrList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    this._localIP = address.ToString();
                    return this._localIP;
                }
            }

            return "localhost";
        }

    }
}
