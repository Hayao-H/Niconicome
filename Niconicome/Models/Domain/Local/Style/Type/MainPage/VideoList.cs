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
    }
}
