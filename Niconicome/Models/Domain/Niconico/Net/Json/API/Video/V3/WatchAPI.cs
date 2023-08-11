using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Video.V3
{
    public class Comment
    {
        [JsonPropertyName("nvComment")]
        public NvComment NvComment { get; set; } = new();
    }

    public class Data2
    {
        [JsonPropertyName("comment")]
        public Comment Comment { get; set; } = new();
    }

    public class NvComment
    {
        [JsonPropertyName("threadKey")]
        public string ThreadKey { get; set; } = string.Empty;
    }

    public class WatchAPI
    {
        [JsonPropertyName("data")]
        public Data2 Data { get; set; } = new();
    }
}
