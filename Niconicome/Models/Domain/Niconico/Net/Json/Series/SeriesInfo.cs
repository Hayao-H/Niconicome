using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Json.Series
{


    public partial class Temperatures
    {
        [JsonPropertyName("itemListElement")]
        public List<ItemListElement> ItemListElement { get; set; } = new();
    }

    public partial class ItemListElement
    {

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("uploadDate")]
        public DateTimeOffset UploadDate { get; set; } = DateTime.Now;

        [JsonPropertyName("thumbnailUrl")]
        public List<Uri> ThumbnailUrl { get; set; } = new();
    }

}
