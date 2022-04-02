using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Request
{
    public class Ping
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
