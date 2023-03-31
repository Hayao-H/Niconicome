using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Update
{
    public class UpdateJSON
    {
        [JsonPropertyName("version")]
        public string Version { get; init; } = string.Empty;

        [JsonPropertyName("application-file")]
        public string ApplicationFile { get; init; } = string.Empty;

        [JsonPropertyName("changelog")]
        public string Changelog { get; init; } = string.Empty;
    }
}
