using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Local.Style.Type.MainPage
{
    public class VideoList
    {
        [JsonPropertyName("videoListItemHeight")]
        public int ItemHeight { get; set; } = 80;
    }
}
