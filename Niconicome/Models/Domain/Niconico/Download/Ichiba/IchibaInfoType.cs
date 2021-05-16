using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Download.Ichiba
{
    [XmlRoot("MarketInformation")]
    public class MarketInformation
    {
        [JsonPropertyName("items")]
        [XmlElement("items")]
        public List<MarketItem> Items { get; set; } = new();
    }

    public class MarketItem
    {
        [JsonPropertyName("name")]
        [XmlElement("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        [XmlElement("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        [XmlElement("price")]
        public string Price { get; set; } = string.Empty;

        [JsonPropertyName("linkUrl")]
        [XmlElement("linkUrl")]
        public string LinkUrl { get; set; } = string.Empty;

        [JsonPropertyName("thumbUrl")]
        [XmlElement("thumbUrl")]
        public string ThumbUrl { get; set; } = string.Empty;
    }
}
