using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;
using LiteDB;

namespace Niconicome.Models.Domain.Local.Store.Types
{
    public record Video : IStorable
    {
        public static string TableName { get; } = "videos";

        public int Id { get; private set; }
        public int ViewCount { get; set; }

        public string NiconicoId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string LargeThumbUrl { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ThumbUrl { get; set; } = string.Empty;

        public string ThumbPath { get; set; } = string.Empty;

        public List<string>? Tags { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsDownloaded { get; set; }

        public bool IsSelected { get; set; }

        public List<int>? PlaylistIds { get; set; }

        public User? Owner { get; set; }

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

    public record Playlist : IStorable
    {

        public Playlist() { }

        public Playlist(Playlist parent)
        {
            this.ParentPlaylist = parent;
        }

        public static string TableName { get; } = "playlists";

        public int Id { get; set; }

        public int Sequence { get; set; }

        public string? PlaylistName { get; set; }
        public string? FolderPath { get; set; }

        public List<Video> Videos { get; set; } = new();

        public Playlist? ParentPlaylist { get; set; }

        public bool IsRemotePlaylist { get; set; }

        public bool IsRoot { get; set; }

        public bool IsMylist { get; set; }

        public bool IsUserVideos { get; set; }

        public bool IsWatchLater { get; set; }

        public bool IsChannel { get; set; }

        [BsonIgnore]
        public bool IsExpanded { get; set; }

        [BsonIgnore]
        public bool IsConcretePlaylist
        {
            get
            {
                return this.Videos.Count > 0;
            }
        }

        [BsonIgnore]
        public Visibility BeforeSeparateorVisibility { get; set; } = Visibility.Hidden;

        [BsonIgnore]
        public Visibility AfterSeparateorVisibility { get; set; } = Visibility.Hidden;

        public string? RemoteId { get; set; }

        /// <summary>
        /// レイヤー
        /// </summary>
        [BsonIgnore]
        public int Layer
        {
            get
            {
                int layer = 1;
                Playlist self = this;
                while (self.ParentPlaylist != null)
                {
                    ++layer;
                    self = self.ParentPlaylist;
                }
                return layer;
            }
        }
    }

    public record RemoteSetting : IStorable
    {
        public static string TableName { get; } = "remotesettings";

        public int Id { get; private set; }

        public string? UrlString { get; private set; }

        public RemoteType Type { get; private set; }
    }

    public enum RemoteType
    {
        Mylist,
        Series,
        UserVideo,
        Ranking,
        Search
    }
}
