using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Response
{
    public class Thread
    {
        [JsonPropertyName("resultcode")]
        public int ResultCode { get; set; }

        [JsonPropertyName("thread")]
        public string ThreadID { get; set; } = string.Empty;

        [JsonPropertyName("server_time")]
        public long ServerTime { get; set; }

        [JsonPropertyName("last_res")]
        public long LastRes { get; set; }

        [JsonPropertyName("ticket")]
        public string Ticket { get; set; } = string.Empty;

        [JsonPropertyName("revision")]
        public int Revision { get; set; }
    }
}
