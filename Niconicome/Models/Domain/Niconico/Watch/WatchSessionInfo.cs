using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Watch
{
    public interface IWatchSessionInfo
    {
        /// <summary>
        /// 旧サーバーハートビート
        /// </summary>
        string DmcResponseJsonData { get; }

        /// <summary>
        /// URL
        /// </summary>
        string ContentUrl { get; }

        /// <summary>
        /// SessionID
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// Domandサーバーフラグ
        /// </summary>
        bool IsDMS { get; }

    }

    public class WatchSessionInfo : IWatchSessionInfo
    {
        public string DmcResponseJsonData { get; set; } = string.Empty;

        public string ContentUrl { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;

        public bool IsDMS { get; set; }
    }

}
