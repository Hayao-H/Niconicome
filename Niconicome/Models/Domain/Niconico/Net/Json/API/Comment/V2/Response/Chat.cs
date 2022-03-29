using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Response
{
    public class Chat
    {
        [JsonPropertyName("thread")]
        public string Thread { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("mail")]
        public string? Mail { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserID { get; set; }

        [JsonPropertyName("no")]
        public int No { get; set; }

        [JsonPropertyName("vpos")]
        public int Vpos { get; set; }

        [JsonPropertyName("date")]
        public long Date { get; set; }

        [JsonPropertyName("date_usec")]
        public long DateUsec { get; set; }

        [JsonPropertyName("nicoru")]
        public int Nicoru { get; set; }

        [JsonPropertyName("premium")]
        public int? Premium { get; set; }

        [JsonPropertyName("anonimity")]
        public int Anonymity { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("deleted")]
        public int? Deleted { get; set; }

        [JsonPropertyName("fork")]
        public int? Fork { get; set; }

    }

}
