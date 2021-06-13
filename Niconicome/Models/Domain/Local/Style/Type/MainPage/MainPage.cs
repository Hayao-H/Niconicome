using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Local.Style.Type.MainPage
{
    public class MainPage
    {
        [JsonPropertyName("videoList")]
        public VideoList VideoList { get; set; } = new();

        [JsonPropertyName("tree")]
        public Tree Tree { get; set; } = new();
    }
}
