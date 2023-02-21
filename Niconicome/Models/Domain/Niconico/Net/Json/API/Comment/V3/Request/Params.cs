using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V3.Request
{
    public class Params
    {
        [JsonPropertyName("targets")]
        public List<Target> Targets { get; set; } = new();

        [JsonPropertyName("language")]
        public string Language { get; set; } = "ja-jp";
    }
}
