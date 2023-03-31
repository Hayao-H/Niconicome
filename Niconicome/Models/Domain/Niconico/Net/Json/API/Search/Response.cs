using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Search
{
    public class Meta
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class Datum
    {
        [JsonPropertyName("contentId")]
        public string ContentId { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("viewCounter")]
        public int ViewCounter { get; set; }

        [JsonPropertyName("mylistCounter")]
        public int MylistCounter { get; set; }

        [JsonPropertyName("commentCounter")]
        public int CommentCounter { get; set; }

        [JsonPropertyName("likeCounter")]
        public int LikeCounter { get; set; }

        [JsonPropertyName("thumbnailUrl")]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [JsonPropertyName("lengthSeconds")]
        public int LengthSeconds { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; } = new();

        [JsonPropertyName("data")]
        public List<Datum> Data { get; set; } = new();
    }


}
