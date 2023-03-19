using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Type
{
    public class Video
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("playlistID")]
        public int PlaylistID { get; set; }

        [JsonPropertyName("niconicoId")]
        public string NiconicoId { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("uploadedOn")]
        public DateTime UploadedOn { get; set; } = DateTime.Now;

        [JsonPropertyName("addedAt")]
        public DateTime AddedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("viewCount")]
        public int ViewCount { get; set; }

        [JsonPropertyName("commentCount")]
        public int CommentCount { get; set; }

        [JsonPropertyName("mylistCount")]
        public int MylistCount { get; set; }

        [JsonPropertyName("likeCount")]
        public int LikeCount { get; set; }

        [JsonPropertyName("ownerID ")]
        public string OwnerID { get; set; } = string.Empty;

        [JsonPropertyName("ownerName")]
        public string OwnerName { get; set; } = string.Empty;

        [JsonPropertyName("thumbUrl")]
        public string ThumbUrl { get; set; } = string.Empty;

        [JsonPropertyName("largeThumbUrl")]
        public string LargeThumbUrl { get; set; } = string.Empty;

        [JsonPropertyName("channelName")]
        public string ChannelName { get; set; } = string.Empty;

        [JsonPropertyName("channelID")]
        public string ChannelID { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("tags")]
        public List<int> Tags { get; set; } = new();

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("isSelected")]
        public bool IsSelected { get; set; }
    }

}
