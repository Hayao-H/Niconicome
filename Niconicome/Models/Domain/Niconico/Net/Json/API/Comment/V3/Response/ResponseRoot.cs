using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V3.Response
{
    public class Comment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("no")]
        public int No { get; set; }

        [JsonPropertyName("vposMs")]
        public int VposMs { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        [JsonPropertyName("commands")]
        public List<string> Commands { get; set; } = new();

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("postedAt")]
        public DateTime PostedAt { get; set; }

        [JsonPropertyName("nicoruCount")]
        public int NicoruCount { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("isMyPost")]
        public bool IsMyPost { get; set; }
    }

    public class Data
    {

        [JsonPropertyName("threads")]
        public List<Thread> Threads { get; set; } = new();
    }


    public class ResponseRoot
    {

        [JsonPropertyName("data")]
        public Data Data { get; set; } = new();
    }

    public class Thread
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("fork")]
        public string Fork { get; set; } = string.Empty;

        [JsonPropertyName("commentCount")]
        public int CommentCount { get; set; }

        [JsonPropertyName("comments")]
        public List<Comment> Comments { get; set; } = new();
    }


}
