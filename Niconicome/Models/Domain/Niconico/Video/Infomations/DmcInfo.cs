using System;
using System.Collections.Generic;
using Niconicome.Models.Domain.Niconico.Watch;
using Const = Niconicome.Models.Const;

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
        List<string> Tags { get; set; }
        bool IsDownloadable { get; set; }
        bool IsEncrypted { get; set; }
        bool IsOfficial { get; set; }
        bool IsPremium { get; set; }
        bool IsPeakTime { get; set; }
        bool IsEnonomy { get; }
        DateTime UploadedOn { get; set; }
        DateTime DownloadStartedOn { get; set; }
        IThumbInfo ThumbInfo { get; set; }
        ISessionInfo SessionInfo { get; }
        List<IThread> CommentThreads { get; }
    }

    public interface IThread
    {
        long ID { get; }
        long Fork { get; }
        bool IsActive { get; }
        bool IsDefaultPostTarget { get; }
        bool IsEasyCommentPostTarget { get; }
        bool IsLeafRequired { get; }
        bool IsOwnerThread { get; }
        bool IsThreadkeyRequired { get; }
        string? Threadkey { get; }
        bool Is184Forced { get; }
        bool HasNicoscript { get; }
        string Label { get; }
        int PostkeyStatus { get; }
        string Server { get; }
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

        public List<string> Tags { get; set; } = new List<string>();

        public bool IsDownloadable { get; set; } = true;

        public bool IsEncrypted { get; set; }

        public bool IsOfficial { get; set; }

        public bool IsPremium { get; set; }

        public bool IsPeakTime { get; set; }

        public bool IsEnonomy => !this.IsPremium && this.IsPeakTime && this.ViewCount >= Const::Net.EconomyAvoidableViewCount;

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
        public ISessionInfo SessionInfo { get; init; } = new SessionInfo();

        /// <summary>
        /// コメントスレッド
        /// </summary>
        public List<IThread> CommentThreads { get; set; } = new();
    }

    public class Thread : IThread
    {
        public long ID { get; init; }

        public long Fork { get; init; }

        public bool IsActive { get; init; }

        public bool IsDefaultPostTarget { get; init; }

        public bool IsEasyCommentPostTarget { get; init; }

        public bool IsLeafRequired { get; init; }

        public bool IsOwnerThread { get; init; }

        public bool IsThreadkeyRequired { get; init; }

        public string? Threadkey { get; init; }

        public bool Is184Forced { get; init; }

        public bool HasNicoscript { get; init; }

        public string Label { get; init; } = string.Empty;

        public int PostkeyStatus { get; init; }

        public string Server { get; init; } = string.Empty;
    }
}
