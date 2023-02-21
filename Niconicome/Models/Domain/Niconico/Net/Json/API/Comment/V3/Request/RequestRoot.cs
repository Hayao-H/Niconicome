using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V3.Request
{
    public class RequestRoot
    {
        [JsonPropertyName("params")]
        public Params Params { get; set; } = new();

        [JsonPropertyName("threadKey")]
        public string ThreadKey { get; set; } = string.Empty;

        [JsonPropertyName("additionals")]
        public Additionals Additionals { get; set; } = new();

    }
}
