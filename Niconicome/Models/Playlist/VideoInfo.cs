using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Remote;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
        ReactiveProperty<bool> IsSelected { get; set; }
        ReactiveProperty<bool> IsDownloaded { get; }
        ReactiveProperty<bool> IsThumbDownloading { get; }
        ReactiveProperty<string> OwnerName { get; }
        ReactiveProperty<string> LargeThumbUrl { get; }
        ReactiveProperty<string> ThumbUrl { get; }
        ReactiveProperty<string> Message { get; set; }
        ReactiveProperty<string> ThumbPath { get; }
        ReactiveProperty<string> FileName { get; }
        ReactiveProperty<string> FolderPath { get; }
        ReactiveProperty<string> ChannelID { get; }
        ReactiveProperty<string> ChannelName { get; }
        ReactiveProperty<DateTime> UploadedOn { get; }
        IEnumerable<string> Tags { get; set; }
        string MessageGuid { get; set; }
        Uri GetNiconicoPageUri();
        bool CheckDownloaded(string folderPath);
        string GetFilePath(string folderPath);
        void SetDataBaseData(STypes::Video dbVideo);
        void SetNewData(IListVideoInfo video);

    }


    /// <summary>
    /// バインド不可能な動画情報
    /// </summary>
    public class NonBindableListVideoInfo : BindableBase, IListVideoInfo
    {
        public NonBindableListVideoInfo()
        {
            this.Message = new ReactiveProperty<string>().AddTo(this.disposables);
        }

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
        public ReactiveProperty<string> Message { get; set; }
        public ReactiveProperty<string> ThumbPath { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> FileName { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> FolderPath { get; init; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ChannelID { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ChannelName { get; init; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> IsDeleted { get; init; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsSelected { get; set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsDownloaded { get; init; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsThumbDownloading { get; init; } = new ReactiveProperty<bool>();
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
        /// 文字列化
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{this.NiconicoId.Value}]{this.Title.Value}";
        }

        /// <summary>
        /// DBの値をセットする
        /// </summary>
        /// <param name="dbVideo"></param>
        public void SetDataBaseData(STypes::Video dbVideo)
        {
            this.Id.Value = dbVideo.Id;
            this.NiconicoId.Value = dbVideo.NiconicoId;
            this.Title.Value = dbVideo.Title;
            this.IsDeleted.Value = dbVideo.IsDeleted;
            this.OwnerName.Value = dbVideo.OwnerName;
            this.UploadedOn.Value = dbVideo.UploadedOn;
            this.LargeThumbUrl.Value = dbVideo.LargeThumbUrl;
            this.ThumbUrl.Value = dbVideo.ThumbUrl;
            this.ThumbPath.Value = dbVideo.ThumbPath;
            this.FileName.Value = dbVideo.FileName;
            this.Tags = dbVideo.Tags ?? new List<string>();
            this.ViewCount.Value = dbVideo.ViewCount;
            this.CommentCount.Value = dbVideo.CommentCount;
            this.MylistCount.Value = dbVideo.MylistCount;
            this.LikeCount.Value = dbVideo.LikeCount;
            this.OwnerID.Value = dbVideo.OwnerID;
            this.Duration.Value = dbVideo.Duration;
        }

        /// <summary>
        /// 別のインスタンスから値をコピーする
        /// </summary>
        /// <param name="newVideo"></param>
        public void SetNewData(IListVideoInfo newVideo)
        {

            this.ViewCount.Value = newVideo.ViewCount.Value;
            this.NiconicoId.Value = newVideo.NiconicoId.Value;
            this.Title.Value = newVideo.Title.Value;
            this.IsDeleted.Value = newVideo.IsDeleted.Value;
            this.IsSelected.Value = newVideo.IsSelected.Value;
            this.OwnerName.Value = newVideo.OwnerName.Value;
            this.LargeThumbUrl.Value = newVideo.LargeThumbUrl.Value;
            this.ThumbUrl.Value = newVideo.ThumbUrl.Value;
            this.Message.Value = newVideo.Message.Value;
            if (!newVideo.ThumbPath.Value.IsNullOrEmpty()) this.ThumbPath.Value = newVideo.ThumbPath.Value;
            this.FileName.Value = newVideo.FileName.Value;
            this.MessageGuid = newVideo.MessageGuid;
            this.Tags = newVideo.Tags;
            this.UploadedOn.Value = newVideo.UploadedOn.Value;
            this.CommentCount.Value = newVideo.CommentCount.Value;
            this.MylistCount.Value = newVideo.MylistCount.Value;
            this.LikeCount.Value = newVideo.LikeCount.Value;
            this.Duration.Value = newVideo.Duration.Value;
            this.OwnerID.Value = newVideo.OwnerID.Value;
            this.IsDownloaded.Value = newVideo.IsDownloaded.Value;
        }
    }
}
