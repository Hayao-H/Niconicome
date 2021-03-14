using System.Text.Json.Serialization;

namespace NiconicomeTest.Local.Cookies
{

    public partial class LocalStateType
    {

        [JsonPropertyName("os_crypt")]
        public OsCrypt OsCrypt { get; set; } = new();
    }

    public partial class OsCrypt
    {
        [JsonPropertyName("encrypted_key")]
        public string EncryptedKey { get; set; } = string.Empty;
    }
}
