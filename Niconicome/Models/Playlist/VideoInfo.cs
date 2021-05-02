using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Net = Niconicome.Models.Domain.Network;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Playlist
{
    public interface IListVideoInfo
    {
        ReactiveProperty<int> Id { get; }
        ReactiveProperty<int> ViewCount { get; }
        ReactiveProperty<int> CommentCount { get; }
        ReactiveProperty<int> MylistCount { get; }
        ReactiveProperty<int> LikeCount { get; }
        ReactiveProperty<int> OwnerID { get; }
        ReactiveProperty<int> Duration { get; }
        ReactiveProperty<string> NiconicoId { get; }
        ReactiveProperty<string> Title { get; }
        ReactiveProperty<bool> IsDeleted { get; }
        ReactiveProperty<bool> IsSelected { get; }
        ReactiveProperty<bool> IsDownloaded { get; }
        ReactiveProperty<string> OwnerName { get; }
        ReactiveProperty<string> LargeThumbUrl { get; }
        ReactiveProperty<string> ThumbUrl { get; }
        ReactiveProperty<string> Message { get; }
        ReactiveProperty<string> ThumbPath { get; }
        ReactiveProperty<string> FileName { get; }
        ReactiveProperty<string> ChannelID { get; }
        ReactiveProperty<string> ChannelName { get; }
        ReactiveProperty<DateTime> UploadedOn { get; }
        IEnumerable<string> Tags { get; set; }
        string BindableThumbPath { get;  }
        string MessageGuid { get; set; }
        Uri GetNiconicoPageUri();
        bool CheckDownloaded(string folderPath);
        string GetFilePath(string folderPath);
        static NonBindableListVideoInfo ConvertDbDataToVideoListInfo(STypes::Video video)
        {
            video.Void();
            return new NonBindableListVideoInfo();
        }

    }


    /// <summary>
    /// バインド不可能な動画情報
    /// </summary>
    public class NonBindableListVideoInfo : BindableBase, IListVideoInfo
    {

        public ReactiveProperty<int> Id { get; init; } = new ReactiveProperty<int>();
       public ReactiveProperty<int> ViewCount { get; init; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> CommentCount { get; init; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> MylistCount { get; init; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> LikeCount { get; init; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> OwnerID { get; init; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Duration { get; init; } = new ReactiveProperty<int>();
        public ReactiveProperty<string> NiconicoId { get; init; } = new ReactiveProperty<string>();
       public ReactiveProperty<string> Title { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> OwnerName { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> LargeThumbUrl { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ThumbUrl { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Message { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ThumbPath { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> FileName { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ChannelID { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ChannelName { get; init; } = new ReactiveProperty<string>();

        public ReactiveProperty<bool> IsDeleted { get; init; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsSelected { get; init; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsDownloaded { get; init; } = new ReactiveProperty<bool>();
        public ReactiveProperty<DateTime> UploadedOn { get; init; } = new ReactiveProperty<DateTime>();
        /// <summary>
        /// バインド可能なサムネイルフィルパス
        /// </summary>
        public string BindableThumbPath
        {
            get
            {
                var dir = AppContext.BaseDirectory;
                if (this.ThumbPath is null || this.ThumbPath.Value == string.Empty)
                {
                    var cacheHandler = Utils::DIFactory.Provider.GetRequiredService<Net::ICacheHandler>();
                    string cachePath = cacheHandler.GetCachePath("0", Net.CacheType.Thumbnail);
                    return Path.Combine(dir, cachePath);
                }
                else
                {
                    return Path.Combine(dir, this.ThumbPath.Value);

                }
            }
        }

        /// <summary>
        /// タグ
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// メッセージID
        /// </summary>
        public string MessageGuid { get; set; } = Guid.NewGuid().ToString("D");

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
            if (this.FileName.Value.IsNullOrEmpty())
            {
                return false;
            }
            else
            {
                var p = Path.Combine(folderPath, this.FileName.Value);
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
            if (this.FileName.Value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            else
            {
                return Path.Combine(folderName, this.FileName.Value);
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
            videoInfo.Id.Value = dbVideo.Id;
            videoInfo.NiconicoId.Value = dbVideo.NiconicoId;
            videoInfo.Title.Value = dbVideo.Title;
            videoInfo.IsDeleted.Value = dbVideo.IsDeleted;
            videoInfo.OwnerName.Value = dbVideo.OwnerName;
            videoInfo.UploadedOn.Value = dbVideo.UploadedOn;
            videoInfo.LargeThumbUrl.Value = dbVideo.LargeThumbUrl;
            videoInfo.ThumbUrl.Value = dbVideo.ThumbUrl;
            videoInfo.ThumbPath.Value = dbVideo.ThumbPath;
            videoInfo.FileName.Value = dbVideo.FileName;
            videoInfo.IsSelected.Value = dbVideo.IsSelected;
            videoInfo.Tags = dbVideo.Tags ?? new List<string>();
            videoInfo.ViewCount.Value = dbVideo.ViewCount;
            videoInfo.CommentCount.Value = dbVideo.CommentCount;
            videoInfo.MylistCount.Value = dbVideo.MylistCount;
            videoInfo.LikeCount.Value = dbVideo.LikeCount;
            videoInfo.OwnerID.Value = dbVideo.OwnerID;
            videoInfo.Duration.Value = dbVideo.Duration;
        }

        /// <summary>
        /// 値をセットする
        /// </summary>
        /// <param name="newVideo"></param>
        /// <param name="oldVideo"></param>
        public static void SetData(IListVideoInfo newVideo, IListVideoInfo oldVideo)
        {
            newVideo.ViewCount.Value= oldVideo.ViewCount.Value;
            newVideo.NiconicoId.Value= oldVideo.NiconicoId.Value;
            newVideo.Title.Value= oldVideo.Title.Value;
            newVideo.IsDeleted.Value= oldVideo.IsDeleted.Value;
            newVideo.IsSelected.Value= oldVideo.IsSelected.Value;
            newVideo.OwnerName.Value= oldVideo.OwnerName.Value;
            newVideo.LargeThumbUrl.Value= oldVideo.LargeThumbUrl.Value;
            newVideo.ThumbUrl.Value= oldVideo.ThumbUrl.Value;
            newVideo.Message.Value= oldVideo.Message.Value;
            if (!oldVideo.ThumbPath.Value.IsNullOrEmpty()) newVideo.ThumbPath.Value= oldVideo.ThumbPath.Value;
            newVideo.FileName.Value= oldVideo.FileName.Value;
            newVideo.MessageGuid= oldVideo.MessageGuid;
            newVideo.Tags= oldVideo.Tags;
            newVideo.UploadedOn.Value= oldVideo.UploadedOn.Value;
            newVideo.CommentCount.Value= oldVideo.CommentCount.Value;
            newVideo.MylistCount.Value= oldVideo.MylistCount.Value;
            newVideo.LikeCount.Value= oldVideo.LikeCount.Value;
            newVideo.Duration.Value= oldVideo.Duration.Value;
            newVideo.OwnerID.Value= oldVideo.OwnerID.Value;
            newVideo.IsDownloaded.Value= oldVideo.IsDownloaded.Value;
        }
    }

}
