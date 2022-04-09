using System.Reactive.Disposables;
using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Local.Style.Type.MainPage
{
    public class VideoList
    {
        [JsonPropertyName("videoListItemHeight")]
        public int ItemHeight { get; set; } = 100;


        [JsonPropertyName("tabsHeight")]
        public int TabsHeight { get; set; } = 260;

        [JsonPropertyName("titleHeight")]
        public int TitleHeight { get; set; } = 40;

        [JsonPropertyName("buttonsHeight")]
        public int ButtonsHeight { get; set; } = 50;

        [JsonPropertyName("column")]
        public Column Column { get; set; } = new();
    }

    public class Column
    {
        [JsonPropertyName("thumbnail")]
        public bool Thumbnail { get; set; } = true;

        [JsonPropertyName("niconicoID")]
        public bool NiconicoID { get; set; } = true;

        [JsonPropertyName("title")]
        public bool Title { get; set; } = true;

        [JsonPropertyName("uploadedDT")]
        public bool UploadedDT { get; set; } = true;

        [JsonPropertyName("viewCount")]
        public bool ViewCount { get; set; } = true;

        [JsonPropertyName("dlFlag")]
        public bool DLFlag { get; set; } = true;

        [JsonPropertyName("bookMark")]
        public bool BookMark { get; set; } = true;

        [JsonPropertyName("economy")]
        public bool Economy { get; set; } = true;
    }
}
