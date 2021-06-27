using System;
using System.Collections.Generic;
using Niconicome.Models.Domain.Niconico.Watch;
using WatchJson = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;

namespace Niconicome.Models.Domain.Niconico.Video.Infomations
{
    public interface IDmcInfo
    {
        string Title { get; set; }
        string Id { get; set; }
        string Owner { get; set; }
        int OwnerID { get; set; }
        string Userkey { get; }
        string UserId { get; }
        string ChannelID { get; }
        string ChannelName { get; }
        string Description { get; }
        int ViewCount { get; }
        int CommentCount { get; }
        int MylistCount { get; }
        int LikeCount { get; }
        int Duration { get; }
        IEnumerable<string> Tags { get; set; }
        bool IsDownloadsble { get; set; }
        bool IsEncrypted { get; set; }
        bool IsOfficial { get; set; }
        DateTime UploadedOn { get; set; }
        DateTime DownloadStartedOn { get; set; }
        IThumbInfo ThumbInfo { get; set; }
        ISessionInfo SessionInfo { get; }
        List<WatchJson::Thread> CommentThreads { get; }
    }


    /// <summary>
    /// APi等へのアクセスに必要な情報を格納する
    /// </summary>
    public class DmcInfo : IDmcInfo
    {
        public string Title { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public string Userkey { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string ChannelID { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int LikeCount { get; set; }

        public int MylistCount { get; set; }

        public int Duration { get; set; }

        public int OwnerID { get; set; }

        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// ダウンロード可能フラグ
        /// </summary>
        public bool IsDownloadsble { get; set; } = true;

        /// <summary>
        /// 暗号化フラグ
        /// </summary>
       　public bool IsEncrypted { get; set; }

        /// <summary>
        /// 公式動画フラグ
        /// </summary>
        public bool IsOfficial { get; set; }


        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime UploadedOn { get; set; }

        /// <summary>
        /// DL開始日時
        /// </summary>
        public DateTime DownloadStartedOn { get; set; }


        /// <summary>
        /// サムネイル
        /// </summary>
        public IThumbInfo ThumbInfo { get; set; } = new ThumbInfo();

        /// <summary>
        /// セッション情報
        /// </summary>
        public ISessionInfo SessionInfo { get; private set; } = new SessionInfo();

        /// <summary>
        /// コメントスレッド
        /// </summary>
        public List<WatchJson::Thread> CommentThreads { get; set; } = new();
    }

}
