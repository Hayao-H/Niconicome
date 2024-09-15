using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Search.V2
{
    public partial class Response
    {

        [JsonPropertyName("data")]
        public Data Data { get; set; } = new Data();
    }

    public partial class Data
    {

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new List<Item>();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

    }


    public partial class Item
    {

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("registeredAt")]
        public DateTime RegisteredAt { get; set; }

        [JsonPropertyName("count")]
        public Count Count { get; set; } = new Count();

        [JsonPropertyName("thumbnail")]
        public Thumbnail Thumbnail { get; set; } = new Thumbnail();

        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }

    public partial class Count
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


    public partial class Thumbnail
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }=string.Empty;
    }


}
