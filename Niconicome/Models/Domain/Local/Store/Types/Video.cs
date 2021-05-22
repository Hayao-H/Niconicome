using System;
using System.Collections.Generic;

namespace Niconicome.Models.Domain.Local.Store.Types
{
    public record Video : IStorable
    {
        public static string TableName { get; } = "videos";

        public int Id { get; private set; }

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

        public List<int>? PlaylistIds { get; set; }

        public DateTime UploadedOn { get; set; }

        /// <summary>
        /// ニコニコ動画のページUriを取得
        /// </summary>
        /// <returns></returns>
        public Uri GetNiconicoPageUri()
        {
            if (this.NiconicoId == null)
            {
                throw new InvalidOperationException("ニコニコ動画におけるIDが設定されていません。");
            }
            else
            {
                return new Uri(this.NiconicoId);
            }
        }

        public override string ToString()
        {
            return $"[{this.NiconicoId}]{this.Title}";
        }
    }
}
