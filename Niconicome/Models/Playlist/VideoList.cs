using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.ViewModels;
using Net = Niconicome.Models.Domain.Network;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Playlist
{
    public interface IListVideoInfo
    {
        int Id { get; set; }
        int ViewCount { get; set; }
        int CommentCount { get; set; }
        int MylistCount { get; set; }
        int LikeCount { get; set; }
        int OwnerID { get; set; }
        int Duration { get; set; }
        string NiconicoId { get; set; }
        string Title { get; set; }
        bool IsDeleted { get; set; }
        bool IsSelected { get; set; }
        bool IsDownloaded { get; set; }
        string OwnerName { get; set; }
        string LargeThumbUrl { get; set; }
        string ThumbUrl { get; set; }
        string Message { get; set; }
        string ThumbPath { get; set; }
        string FileName { get; set; }
        string BindableThumbPath { get; }
        string MessageGuid { get; set; }
        string ChannelId { get; set; }
        string ChannelName { get; set; }
        IEnumerable<string> Tags { get; set; }
        DateTime UploadedOn { get; set; }
        Uri GetNiconicoPageUri();
        bool CheckDownloaded(string folderPath);
        string GetFilePath(string folderPath);
        static BindableListVIdeoInfo ConvertDbDataToVideoListInfo(STypes::Video video)
        {
            video.Void();
            return new BindableListVIdeoInfo();
        }

    }


    /// <summary>
    /// バインド不可能な動画情報
    /// </summary>
    public class NonBindableListVideoInfo : BindableBase, IListVideoInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 再生回数
        /// </summary>
        public virtual int ViewCount { get; set; }

        /// <summary>
        /// コメント数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// マイリス数
        /// </summary>
        public int MylistCount { get; set; }

        /// <summary>
        /// いいね数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 投稿者ID
        /// </summary>
        public int OwnerID { get; set; }

        /// <summary>
        /// 再生時間
        /// </summary>
        public int Duration { get; set; }


        /// <summary>
        /// ニコニコ動画におけるID
        /// </summary>
        public string NiconicoId { get; set; } = string.Empty;

        /// <summary>
        /// 動画タイトル
        /// </summary>
        public virtual string Title { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 選択フラグ
        /// </summary>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// DL済みフラグ
        /// </summary>
        public virtual bool IsDownloaded { get; set; }

        /// <summary>
        /// 投稿者名
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// 大きなサムネイルのURL
        /// </summary>
        public string LargeThumbUrl { get; set; } = string.Empty;

        /// <summary>
        /// サムネイルのURL
        /// </summary>
        public string ThumbUrl { get; set; } = string.Empty;

        /// <summary>
        /// サムネイルのパス
        /// </summary>
        public string ThumbPath { get; set; } = string.Empty;

        /// <summary>
        /// バインド可能なサムネイルフィルパス
        /// </summary>
        public string BindableThumbPath
        {
            get
            {
                var dir = AppContext.BaseDirectory;
                if (this.ThumbPath is null || this.ThumbPath == string.Empty)
                {
                    var cacheHandler = Utils::DIFactory.Provider.GetRequiredService<Net::ICacheHandler>();
                    string cachePath = cacheHandler.GetCachePath("0", Net.CacheType.Thumbnail);
                    return Path.Combine(dir, cachePath);
                }
                else
                {
                    return Path.Combine(dir, this.ThumbPath);

                }
            }
        }

        /// <summary>
        /// タグ
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// メッセージ
        /// </summary>
        public virtual string Message { get; set; } = string.Empty;

        /// <summary>
        /// メッセージID
        /// </summary>
        public string MessageGuid { get; set; } = Guid.NewGuid().ToString("D");

        /// <summary>
        /// チャンネルID
        /// </summary>
        public string ChannelId { get; set; } = string.Empty;

        /// <summary>
        /// チャンネル名
        /// </summary>
        public string ChannelName { get; set; } = string.Empty;


        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime UploadedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 視聴ページのURIを取得する
        /// </summary>
        /// <returns></returns>
        public Uri GetNiconicoPageUri()
        {
            return new Uri($"https://nico.ms/{this.NiconicoId}");
        }

        public bool CheckDownloaded(string folderPath)
        {
            if (this.FileName.IsNullOrEmpty())
            {
                return false;
            }
            else
            {
                var p = Path.Combine(folderPath, this.FileName);
                if (!p.StartsWith(@"\\?\"))
                {
                    p = @"\\?\" + p;
                }
                return File.Exists(p);
            }
        }

        /// <summary>
        /// ファイルパスを取得する
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public string GetFilePath(string folderName)
        {
            if (this.FileName.IsNullOrEmpty())
            {
                return string.Empty;
            }
            else
            {
                return Path.Combine(folderName, this.FileName);
            }
        }

        /// <summary>
        /// DBのデータをTreeVideoInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="dbVideo"></param>
        /// <returns></returns>
        public static NonBindableListVideoInfo ConvertDbDataToVideoListInfo(STypes::Video dbVideo)
        {
            var converted = new NonBindableListVideoInfo();
            NonBindableListVideoInfo.SetDbData(converted, dbVideo);
            return converted;
        }

        /// <summary>
        /// オブジェクトをクローンする
        /// </summary>
        /// <returns></returns>
        public IListVideoInfo Clone()
        {
            return (IListVideoInfo)this.MemberwiseClone();
        }

        /// <summary>
        /// データを設定する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <param name="dbVideo"></param>
        protected static void SetDbData(IListVideoInfo videoInfo, STypes::Video dbVideo)
        {
            videoInfo.Id = dbVideo.Id;
            videoInfo.NiconicoId = dbVideo.NiconicoId;
            videoInfo.Title = dbVideo.Title;
            videoInfo.IsDeleted = dbVideo.IsDeleted;
            videoInfo.OwnerName = dbVideo.OwnerName;
            videoInfo.UploadedOn = dbVideo.UploadedOn;
            videoInfo.LargeThumbUrl = dbVideo.LargeThumbUrl;
            videoInfo.ThumbUrl = dbVideo.ThumbUrl;
            videoInfo.ThumbPath = dbVideo.ThumbPath;
            videoInfo.FileName = dbVideo.FileName;
            videoInfo.IsSelected = dbVideo.IsSelected;
            videoInfo.Tags = dbVideo.Tags ?? new List<string>();
            videoInfo.ViewCount = dbVideo.ViewCount;
            videoInfo.CommentCount = dbVideo.CommentCount;
            videoInfo.MylistCount = dbVideo.MylistCount;
            videoInfo.LikeCount = dbVideo.LikeCount;
            videoInfo.OwnerID = dbVideo.OwnerID;
            videoInfo.Duration = dbVideo.Duration;
        }

        /// <summary>
        /// 値をセットする
        /// </summary>
        /// <param name="newVideo"></param>
        /// <param name="oldVideo"></param>
        public static void SetData(IListVideoInfo newVideo, IListVideoInfo oldVideo)
        {
            newVideo.Id = oldVideo.Id;
            newVideo.ViewCount = oldVideo.ViewCount;
            newVideo.NiconicoId = oldVideo.NiconicoId;
            newVideo.Title = oldVideo.Title;
            newVideo.IsDeleted = oldVideo.IsDeleted;
            newVideo.IsSelected = oldVideo.IsSelected;
            newVideo.OwnerName = oldVideo.OwnerName;
            newVideo.LargeThumbUrl = oldVideo.LargeThumbUrl;
            newVideo.ThumbUrl = oldVideo.ThumbUrl;
            newVideo.Message = oldVideo.Message;
            newVideo.ThumbPath = oldVideo.ThumbPath;
            newVideo.FileName = oldVideo.FileName;
            newVideo.MessageGuid = oldVideo.MessageGuid;
            newVideo.Tags = oldVideo.Tags;
            newVideo.UploadedOn = oldVideo.UploadedOn;
            newVideo.CommentCount = oldVideo.CommentCount;
            newVideo.MylistCount = oldVideo.MylistCount;
            newVideo.LikeCount = oldVideo.LikeCount;
            newVideo.Duration = oldVideo.Duration;
            newVideo.OwnerID = oldVideo.OwnerID;
            newVideo.IsDownloaded = oldVideo.IsDownloaded;
        }
    }

    public class BindableListVIdeoInfo : NonBindableListVideoInfo
    {
        public BindableListVIdeoInfo()
        {
            VideoMessanger.VideoMessageChange += this.ListenMessageChange;
        }

        ~BindableListVIdeoInfo()
        {
            VideoMessanger.VideoMessageChange -= this.ListenMessageChange;
        }

        private bool isSelectedField;

        private int viewCountField;

        private string titleField = string.Empty;

        private bool isDownloadedField;

        public override string Title { get => this.titleField; set => this.SetProperty(ref this.titleField, value); }

        public override int ViewCount { get => this.viewCountField; set => this.SetProperty(ref this.viewCountField, value); }

        public override string Message
        {
            get => VideoMessanger.GetMessage(this.MessageGuid);
            set
            {
                VideoMessanger.Write(this.MessageGuid, value);
                this.OnPropertyChanged();
            }
        }

        public override bool IsSelected { get => this.isSelectedField; set => this.SetProperty(ref this.isSelectedField, value); }

        public override bool IsDownloaded { get => this.isDownloadedField; set => this.SetProperty(ref this.isDownloadedField, value); }

        /// <summary>
        /// DBのデータをTreeVideoInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="dbVideo"></param>
        /// <returns></returns>
        public new static BindableListVIdeoInfo ConvertDbDataToVideoListInfo(STypes::Video dbVideo)
        {
            var converted = new BindableListVIdeoInfo();
            NonBindableListVideoInfo.SetDbData(converted, dbVideo);
            return converted;
        }

        protected void ListenMessageChange(object? sender, VideoMessageChangeEventArgs e)
        {
            if (e.ID == this.MessageGuid)
            {
                this.OnPropertyChanged(nameof(this.Message));
            }
        }
    }


}
