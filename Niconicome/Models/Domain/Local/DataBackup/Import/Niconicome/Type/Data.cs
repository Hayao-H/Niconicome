﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Type
{
    public class Data
    {
        [JsonPropertyName("videos")]
        public List<Video> Videos { get; set; } = new();

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; } = new();

        [JsonPropertyName("playlists")]
        public List<Playlist> Playlists { get; set; } = new();
    }
}
