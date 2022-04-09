using System;
using System.Collections.Generic;

namespace Niconicome.Models.Domain.Local.Store.Types
{
    public record Video : IStorable
    {
        public static string TableName { get; } = "videos";

        public int Id { get; set; }

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int MylistCount { get; set; }

        public int LikeCount { get; set; }

        public int Duration { get; set; }

        public int OwnerID { get; set; }

        public string NiconicoId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string LargeThumbUrl { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ThumbUrl { get; set; } = string.Empty;

        public string ThumbPath { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public List<string>? Tags { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSelected { get; set; }

        public List<int> PlaylistIds { get; set; } = new();

        public DateTime UploadedOn { get; set; }

        public override string ToString()
        {
            return $"[{this.NiconicoId}]{this.Title}";
        }
    }
}
