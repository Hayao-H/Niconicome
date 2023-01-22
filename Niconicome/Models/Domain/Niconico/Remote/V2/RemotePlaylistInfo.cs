using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Niconicome.Models.Domain.Niconico.Remote.V2
{
    public class RemotePlaylistInfo
    {
        public List<VideoInfo> Videos { get; init; } = new();

        public string PlaylistName { get; set; } = string.Empty;
    }

    public class VideoInfo
    {
        public string NiconicoID { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public string ThumbUrl { get; set; } = string.Empty;

        public DateTime UploadedDT { get; set; } = DateTime.Now;

        public string OwnerID { get; set; } = string.Empty;

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int MylistCount { get; set; }

        public int LikeCount { get; init; }

    }
}
