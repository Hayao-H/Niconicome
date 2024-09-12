using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Server.API.VideoInfo.V1.Types
{
    public class JsWatchInfo
    {
        public Meta Meta { get; set; } = new();

        public API API { get; set; } = new();

        public Media Media { get; set; } = new();

        public Comment Comment { get; set; } = new();

        public Thumbnail Thumbnail { get; set; } = new();

        public Video Video { get; set; } = new();

        public List<PlaylistVideo> PlaylistVideos { get; set; } = new();
    }

    public class Meta
    {
        public int Status { get; set; }

        public string Message { get; set; } = string.Empty;
    }

    public class Media
    {
        public bool IsDownloaded { get; set; }

        public bool IsDMS { get; set; }

        public string ContentUrl { get; set; } = string.Empty;

        public string CreateUrl { get; set; } = string.Empty;
    }

    public class Comment
    {
        public string ContentUrl { get; set; } = string.Empty;


        public string CommentNGAPIBaseUrl { get; set; } = string.Empty;
    }

    public class Thumbnail
    {
        public string ContentUrl { get; set; } = string.Empty;
    }

    public class PlaylistVideo
    {
        public string Title { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public string NiconicoID { get; set; } = string.Empty;

        public string ThumbnailURL { get; set; } = string.Empty;

        public int Duration { get; set; }

        public int ViewCount { get; set; }
    }

    public class Video : PlaylistVideo
    {
        public List<Tag> Tags { get; set; } = new();

        public Owner Owner { get; set; } = new();

        public string Description { get; set; } = string.Empty;

        public Count Count { get; set; } = new();
    }

    public class Tag
    {
        public string Name { get; set; } = string.Empty;

        public bool IsNicodicExists { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; } = string.Empty;

        public string ID { get; set; } = string.Empty;
    }

    public class Count
    {
        public int View { get; set; }

        public int Comment { get; set; }

        public int Mylist { get; set; }

        public int Like { get; set; }
    }

    public class API
    {
        public string BaseUrl { get; set; } = string.Empty;
    }
}
