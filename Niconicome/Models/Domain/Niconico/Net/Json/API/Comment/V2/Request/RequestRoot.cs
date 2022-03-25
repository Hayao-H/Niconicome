using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Request
{
    public class RequestRoot
    {
        [JsonPropertyName("ping")]
        public Ping? Ping { get; set; }

        [JsonPropertyName("thread")]
        public Thread? Thread { get; set; }

        [JsonPropertyName("thread_leaves")]
        public ThreadLeaves? ThreadLeaves { get; set; }
    }
}
