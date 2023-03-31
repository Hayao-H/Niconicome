using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Video.V3
{
    public class Count
    {
        [JsonPropertyName("view")]
        public int View { get; set; }

        [JsonPropertyName("comment")]
        public int Comment { get; set; }

        [JsonPropertyName("mylist")]
        public int Mylist { get; set; }

        [JsonPropertyName("like")]
        public int Like { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    public class Essential
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("registeredAt")]
        public DateTime RegisteredAt { get; set; }

        [JsonPropertyName("count")]
        public Count Count { get; set; } = new();

        [JsonPropertyName("thumbnail")]
        public Thumbnail Thumbnail { get; set; } = new();

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; } = new();
    }

    public class Item
    {
        [JsonPropertyName("essential")]
        public Essential Essential { get; set; } = new();
    }

    public class Owner
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;


        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class UserVideo
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; } = new();
    }


    public class Thumbnail
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;


        [JsonPropertyName("largeUrl")]
        public string LargeUrl { get; set; } = string.Empty;
    }


}
