﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Request
{
    public class ThreadLeaves
    {
        [JsonPropertyName("thread")]
        public string ThreadNo { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserID { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("userkey")]
        public string? UserKey { get; set; }

        [JsonPropertyName("threadkey")]
        public string? ThreadKey { get; set; }

        [JsonPropertyName("waybackkey")]
        public string? WayBackKey { get; set; }

        [JsonPropertyName("force_184")]
        public string? Force184 { get; set; }

        [JsonPropertyName("fork")]
        public int Fork { get; set; }

        [JsonPropertyName("language")]
        public int Language { get; set; }

        [JsonPropertyName("scores")]
        public int Scores { get; set; }

        [JsonPropertyName("nicoru")]
        public int Nicoru { get; set; }

        [JsonPropertyName("when")]
        public long? When { get; set; }
    }
}
