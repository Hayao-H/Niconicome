using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Niconicome.Models.Domain.Niconico.Remote.V2
{
    public class RemotePlaylistInfo
    {
        public List<VideoInfo> Videos { get; init; } = new();

        public IImmutableList<string> FailedVideos { get; init; } = (new List<string>()).ToImmutableList();

        public string PlaylistName { get; set; } = string.Empty;
    }

    public class VideoInfo
    {
        public string NiconicoID { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public string ThumbUrl { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

        public string ChannelID { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime UploadedDT { get; set; } = DateTime.Now;

        public DateTime AddedAt { get; set; } = DateTime.Now;

        public string OwnerID { get; set; } = string.Empty;

        public int Duration { get; set; }

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int MylistCount { get; set; }

        public int LikeCount { get; init; }

        public IReadOnlyList<Tag> Tags { get; init; } = new List<Tag>();

    }

    public class Tag
    {
        public string Name { get; set; } = string.Empty;

        public bool IsNicodicExist { get; set; }
    }
}
