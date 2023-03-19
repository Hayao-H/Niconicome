using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Import.Niconicome.Type
{
    public class Tag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("isNicodicExist")]
        public bool IsNicodicExist { get; set; }
    }
}
