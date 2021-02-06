using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response
{
    public partial class Comment
    {
        [JsonPropertyName("ping")]
        public Ping? Ping { get; set; }

        [JsonPropertyName("thread")]
        public Thread? Thread { get; set; }

        [JsonPropertyName("chat")]
        public Chat? Chat { get; set; }
    }

    public partial class Chat
    {
        [JsonPropertyName("thread")]
        public string Thread { get; set; } = string.Empty;

        [JsonPropertyName("no")]
        public long No { get; set; }

        [JsonPropertyName("vpos")]
        public long Vpos { get; set; }

        [JsonPropertyName("leaf")]
        public long? Leaf { get; set; }

        [JsonPropertyName("date")]
        public long Date { get; set; }

        [JsonPropertyName("score")]
        public long? Score { get; set; }

        [JsonPropertyName("nicoru")]
        public long? Nicoru { get; set; }

        [JsonPropertyName("last_nicoru_date")]
        public string LastNicoruDate { get; set; } = string.Empty;

        [JsonPropertyName("premium")]
        public long? Premium { get; set; }

        [JsonPropertyName("anonymity")]
        public long? Anonymity { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("mail")]
        public string Mail { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("date_usec")]
        public long? DateUsec { get; set; }

        [JsonPropertyName("deleted")]
        public long? Deleted { get; set; }

        [JsonPropertyName("fork")]
        public long? Fork { get; set; }
    }

    public partial class Ping
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public partial class Thread
    {
        [JsonPropertyName("resultcode")]
        public long Resultcode { get; set; }

        [JsonPropertyName("thread")]
        public string ThreadThread { get; set; } = string.Empty;

        [JsonPropertyName("server_time")]
        public long ServerTime { get; set; }

        [JsonPropertyName("last_res")]
        public long LastRes { get; set; }

        [JsonPropertyName("ticket")]
        public string Ticket { get; set; } = string.Empty;

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("click_revision")]
        public long? ClickRevision { get; set; }

        [JsonPropertyName("fork")]
        public long? Fork { get; set; }
    }
}
