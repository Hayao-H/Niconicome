using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Niconicome.Extensions.System;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Series
{
    public class SeriesResponse
    {

        [JsonPropertyName("data")]
        public Data Data { get; set; } = new();
    }

    public class Data
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("detail")]
        public Detail Detail { get; set; } = new();
        

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    public class Detail
    {
        public string Title { get; set; } = string.Empty;
    }

    public class Item
    {

        [JsonPropertyName("video")]
        public Video Video { get; set; } = new();
    }

    public class Video
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

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; } = new();
    }

    public class Owner
    {

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }


    public class Thumbnail
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("middleUrl")]
        public string MiddleUrl { get; set; } = string.Empty;

        [JsonPropertyName("largeUrl")]
        public string LargeUrl { get; set; } = string.Empty;

        public string GetURL()
        {
            if (!this.LargeUrl.IsNullOrEmpty())
            {
                return this.LargeUrl;
            }
            else if (!this.MiddleUrl.IsNullOrEmpty())
            {
                return this.MiddleUrl;
            }
            else
            {
                return this.Url;
            }
        }
    }

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

}
