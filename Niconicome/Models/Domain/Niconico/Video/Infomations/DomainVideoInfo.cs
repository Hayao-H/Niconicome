using System.Collections.Generic;

namespace Niconicome.Models.Domain.Niconico.Video.Infomations
{
    public interface IDomainVideoInfo
    {
        string Title { get; set; }
        string Id { get; set; }
        string ChannelID { get; set; }
        string ChannelName { get; set; }
        string Owner { get; set; }
        int OwnerID { get; set; }
        int ViewCount { get; }
        int MylistCount { get; }
        int CommentCount { get; }
        int LikeCount { get; set; }
        int Duration { get; set; }
        IEnumerable<string> Tags { get; set; }
        IDmcInfo DmcInfo { get; set; }
    }

    public interface IThumbInfo
    {
        string? Large { get; set; }
        string? Normal { get; set; }
    }


    /// <summary>
    /// 動画情報(ルート)
    /// </summary>
    public class DomainVideoInfo : IDomainVideoInfo
    {
        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 動画ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// チャンネルID
        /// </summary>
        public string ChannelID { get; set; } = string.Empty;

        /// <summary>
        /// チャンネル名
        /// </summary>
        public string ChannelName { get; set; } = string.Empty;

        /// <summary>
        /// 投稿者名
        /// </summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// 投稿者ユーザーID
        /// </summary>
        public int OwnerID { get; set; }

        /// <summary>
        /// 再生回数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// マイリス数
        /// </summary>
        public int MylistCount { get; set; }

        /// <summary>
        /// コメント数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// いいね数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 再生時間
        /// </summary>
        public int Duration { get; set; }


        /// <summary>
        /// タグ
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// DMC情報
        /// </summary>
        public IDmcInfo DmcInfo { get; set; } = new DmcInfo();
    }

    /// <summary>
    /// サムネイル情報
    /// </summary>
    public class ThumbInfo : IThumbInfo
    {
        /// <summary>
        /// 大サムネイル
        /// </summary>
        public string? Large { get; set; }

        /// <summary>
        /// 通常
        /// </summary>
        public string? Normal { get; set; }
    }
}
