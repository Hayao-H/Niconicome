using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Nicovideo.V1.Users
{
    public partial class Response
    {

        [JsonPropertyName("data")]
        public Data Data { get; set; } = new Data();
    }

    public partial class Data
    {
        [JsonPropertyName("user")]
        public User User { get; set; } = new User();

        [JsonPropertyName("relationships")]
        public Relationships Relationships { get; set; } = new Relationships();
    }

    public partial class Relationships
    {
        [JsonPropertyName("isMe")]
        public bool IsMe { get; set; }
    }


    public partial class User
    {

        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nickname")] 
        public string Nickname { get; set; } = string.Empty;
    }

}
