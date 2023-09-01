using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DmcRequest = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request;

namespace Niconicome.Models.Domain.Niconico.Watch
{

    public interface ISessionInfo
    {
        string? RecipeId { get; set; }
        string? ContentId { get; set; }
        int HeartbeatLifetime { get; set; }
        string? Token { get; set; }
        string? Signature { get; set; }
        string? AuthType { get; set; }
        int ContentKeyTimeout { get; set; }
        string? ServiceUserId { get; set; }
        string? PlayerId { get; set; }
        string? TransferPriset { get; set; }
        string KeyURL { get; set; }
        double Priority { get; set; }
        List<string> Videos { get; }
        List<string> Audios { get; }
    }

    /// <summary>
    /// セッション情報
    /// </summary>
    public class SessionInfo : ISessionInfo
    {
        public string? RecipeId { get; set; }

        public string? ContentId { get; set; }

        public int HeartbeatLifetime { get; set; }

        public string? Token { get; set; }

        public string? Signature { get; set; }

        public string? AuthType { get; set; }

        public string? TransferPriset { get; set; }

        public string KeyURL { get; set; } = string.Empty;

        public int ContentKeyTimeout { get; set; }

        public string? ServiceUserId { get; set; }

        public string? PlayerId { get; set; }

        public double Priority { get; set; }

        public List<string> Videos { get; init; } = new();

        public List<string> Audios { get; init; } = new();
    }

}
