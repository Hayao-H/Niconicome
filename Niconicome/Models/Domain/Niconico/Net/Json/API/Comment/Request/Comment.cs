using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Request
{
    public class Comment
    {
        [JsonPropertyName("ping")]
        public Ping? Ping { get; set; }

        [JsonPropertyName("thread")]
        public Thread? Thread { get; set; }

        [JsonPropertyName("thread_leaves")]
        public ThreadLeaves? ThreadLeaves { get; set; }
    }

    public class Ping
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class Thread
    {
        [JsonPropertyName("thread")]
        public string Thread_ { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("fork")]
        public long Fork { get; set; }

        [JsonPropertyName("language")]
        public int Language { get; set; }

        [JsonPropertyName("when")]
        public long? When { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("force_184")]
        public string? Force184 { get; set; }

        [JsonPropertyName("with_global")]
        public int WithGlobal { get; set; }

        [JsonPropertyName("scores")]
        public int Scores { get; set; }

        [JsonPropertyName("nicoru")]
        public int Nicoru { get; set; }

        [JsonPropertyName("res_from")]
        public int? ResFrom { get; set; }

        [JsonPropertyName("userkey")]
        public string? UserKey { get; set; }

        [JsonPropertyName("threadkey")]
        public string? Threadkey { get; set; }

        [JsonPropertyName("waybackkey")]
        public string? Waybackkey { get; set; }
    }

    public class ThreadLeaves
    {
        [JsonPropertyName("thread")]
        public string Thread { get; set; } = string.Empty;

        [JsonPropertyName("fork")]
        public long Fork { get; set; }

        [JsonPropertyName("language")]
        public int Language { get; set; }

        [JsonPropertyName("when")]
        public long? When { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("scores")]
        public int Scores { get; set; }

        [JsonPropertyName("nicoru")]
        public int Nicoru { get; set; }

        [JsonPropertyName("force_184")]
        public string? Force184 { get; set; }

        [JsonPropertyName("threadkey")]
        public string? Threadkey { get; set; }

        [JsonPropertyName("userkey")]
        public string? UserKey { get; set; }

        [JsonPropertyName("waybackkey")]
        public string? Waybackkey { get; set; }

    }



}
