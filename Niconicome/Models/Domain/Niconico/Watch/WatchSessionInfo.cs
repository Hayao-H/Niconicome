using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Watch
{
    public interface IWatchSessionInfo
    {
        string DmcResponseJsonData { get; }
        string ContentUrl { get; }
        string SessionId { get; }
        string KeyURI { get; }

    }

    /// <summary>
    /// セッション情報
    /// </summary>
    public class WatchSessionInfo : IWatchSessionInfo
    {
        public string DmcResponseJsonData { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string KeyURI { get; set; } = string.Empty;
    }
}
