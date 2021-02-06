using System;
using System.Collections.Generic;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Video
{


    public class UserVideo
    {
        public Data Data { get; set; } = new();
    }

    public class Data
    {
        public int TotalCount { get; set; }
        public List<Video> Items { get; set; } = new();
    }

    public class Video
    {
        public string Type { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public Thumbnail Thumbnail { get; set; } = new();
    }


    public class Thumbnail
    {
        public string MiddleUrl { get; set; } = string.Empty;
        public string LargeUrl { get; set; } = string.Empty;
    }

}
