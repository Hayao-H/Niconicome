using System.Collections.Generic;

namespace Niconicome.Models.Domain.Niconico.Video.Infomations
{
    public interface IDomainVideoInfo
    {
        string Title { get; }
        string Id { get; }
        string ChannelID { get; }
        string ChannelName { get; }
        string Owner { get; }
        int OwnerID { get; }
        int ViewCount { get; }
        int MylistCount { get; }
        int CommentCount { get; }
        int LikeCount { get; }
        int Duration { get; }
        IEnumerable<string> Tags { get; }
        IDmcInfo DmcInfo { get; }
    }


    /// <summary>
    /// 動画情報(ルート)
    /// </summary>
    public class DomainVideoInfo : IDomainVideoInfo
    {
        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title => this.DmcInfo.Title;

        /// <summary>
        /// 動画ID
        /// </summary>
        public string Id => this.DmcInfo.Id;

        /// <summary>
        /// チャンネルID
        /// </summary>
        public string ChannelID => this.DmcInfo.ChannelID;

        /// <summary>
        /// チャンネル名
        /// </summary>
        public string ChannelName => this.DmcInfo.ChannelName;

        /// <summary>
        /// 投稿者名
        /// </summary>
        public string Owner => this.DmcInfo.Owner;

        /// <summary>
        /// 投稿者ユーザーID
        /// </summary>
        public int OwnerID => this.DmcInfo.OwnerID;

        /// <summary>
        /// 再生回数
        /// </summary>
        public int ViewCount => this.DmcInfo.ViewCount;

        /// <summary>
        /// マイリス数
        /// </summary>
        public int MylistCount => this.DmcInfo.MylistCount;

        /// <summary>
        /// コメント数
        /// </summary>
        public int CommentCount => this.DmcInfo.CommentCount;

        /// <summary>
        /// いいね数
        /// </summary>
        public int LikeCount => this.DmcInfo.LikeCount;

        /// <summary>
        /// 再生時間
        /// </summary>
        public int Duration => this.DmcInfo.Duration;


        /// <summary>
        /// タグ
        /// </summary>
        public IEnumerable<string> Tags => this.DmcInfo.Tags;

        /// <summary>
        /// DMC情報
        /// </summary>
        public IDmcInfo DmcInfo { get; set; } = new DmcInfo();
    }
}
