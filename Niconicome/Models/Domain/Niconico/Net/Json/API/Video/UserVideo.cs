namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Video
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using LiteDB;
    using System.Text.Json.Serialization;

    public partial class UserVideo
    {

        [JsonPropertyName("data")]
        public Data Data { get; set; } = new();
    }

    public partial class Data
    {
        [JsonPropertyName("totalCount")]
        public long TotalCount { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    public partial class Item
    {

        [JsonPropertyName("essential")]
        public Essential Essential { get; set; } = new();
    }

    public partial class Essential
    {

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("registeredAt")]
        public DateTimeOffset RegisteredAt { get; set; }

        [JsonPropertyName("count")]
        public Count Count { get; set; } = new();

        [JsonPropertyName("thumbnail")]
        public Thumbnail Thumbnail { get; set; } = new();

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; } = new();
    }

    public partial class Count
    {
        [JsonPropertyName("view")]
        public long View { get; set; }

        [JsonPropertyName("comment")]
        public long Comment { get; set; }

        [JsonPropertyName("mylist")]
        public long Mylist { get; set; }

        [JsonPropertyName("like")]
        public long Like { get; set; }
    }

    public partial class Owner
    {

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public partial class Thumbnail
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("middleUrl")]
        public string MiddleUrl { get; set; } = string.Empty;

        [JsonPropertyName("largeUrl")]
        public string LargeUrl { get; set; } = string.Empty;

        [JsonPropertyName("listingUrl")]
        public string ListingUrl { get; set; } = string.Empty;

        [JsonPropertyName("nHdUrl")]
        public string NHdUrl { get; set; } = string.Empty;
    }



}
