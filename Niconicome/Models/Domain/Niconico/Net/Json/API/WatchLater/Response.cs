using System.Collections.Generic;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.WatchLater
{

    public partial class WatchLaterResponse
    {
        public Data Data { get; set; } = new();
    }

    public partial class Data
    {
        public WatchLater WatchLater { get; set; } = new();
    }

    public partial class WatchLater
    {
        public List<Mylist.Item> Items { get; set; } = new();
        public bool HasNext { get; set; }
    }

}
