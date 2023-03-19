using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Import.Niconicome.Type
{
    public class Playlist
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("folderPath")]
        public string FolderPath { get; set; } = string.Empty;

        [JsonPropertyName("remoteParameter")]
        public string RemoteParameter { get; set; } = string.Empty;

        [JsonPropertyName("playlistType")]
        public int PlaylistType { get; set; }

        [JsonPropertyName("sortType")]
        public int SortType { get; set; }

        [JsonPropertyName("isAscendant")]
        public bool IsAscendant { get; set; }

        [JsonPropertyName("isExpanded")]
        public bool IsExpanded { get; set; }

        [JsonPropertyName("videos")]
        public List<int> Videos { get; set; } = new();

        [JsonPropertyName("children")]
        public List<int> Children { get; set; } = new();
    }
}
