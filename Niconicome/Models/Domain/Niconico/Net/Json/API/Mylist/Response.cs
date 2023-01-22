using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Mylist
{

    public class MylistResponse
    {
        public Data Data { get; set; } = new();
    }


    public class Data
    {
        public Mylist Mylist { get; set; } = new();
    }

    public class Mylist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Item> Items { get; set; } = new();
        public bool HasNext { get; set; }
    }
    public class Item
    {
        public string WatchId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime AddtedAt { get; set; } = new();
        public Video Video { get; set; } = new();
    }

    public class Video
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public Thumbnail Thumbnail { get; set; } = new();
        public Owner Owner { get; set; } = new();
        public Count Count { get; set; } = new();
    }

    public class Thumbnail
    {
        public string Url { get; set; } = string.Empty;
        public string MiddleUrl { get; set; } = string.Empty;
        public string LargeUrl { get; set; } = string.Empty;
    }

    public class Count
    {
        public int View { get; set; }
        public int Comment { get; set; }
        public int Mylist { get; set; }
        public int Like { get; set; }
    }

    public class Owner
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

}
