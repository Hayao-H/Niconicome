using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Json.CrossOrigin
{
    namespace V3
    {
        public class IchibaEmbedApiResponse
        {
            [JsonPropertyName("main")]
            public string Main { get; set; } = string.Empty;
        }
    }
}
