using System;
using System.Collections.Generic;
using Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;

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
        dynamic RawDmcInfo { get; }
    }


    /// <summary>
    /// 動画情報(ルート)
    /// </summary>
    public class DomainVideoInfo : IDomainVideoInfo
    {
        #region field

        private IDmcInfo? cachedDmcInfo;

        #endregion

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
        public IDmcInfo DmcInfo
        {
            get
            {
                if (this.cachedDmcInfo is null)
                {
                    dynamic rawhumb = this.RawDmcInfo.ThumbInfo;
                    dynamic rawSesison = this.RawDmcInfo.SessionInfo;

                    ISessionInfo sessionInfo;

                    if (this.RawDmcInfo.IsDownloadable)
                    {

                        sessionInfo = new SessionInfo()
                        {
                            RecipeId = rawSesison.RecipeId,
                            ContentId = rawSesison.ContentId,
                            HeartbeatLifetime = rawSesison.HeartbeatLifetime,
                            Token = rawSesison.Token,
                            Signature = rawSesison.Signature,
                            AuthType = rawSesison.AuthType,
                            ContentKeyTimeout = rawSesison.ContentKeyTimeout,
                            ServiceUserId = rawSesison.ServiceUserId,
                            PlayerId = rawSesison.PlayerId,
                            TransferPriset = rawSesison.TransferPriset,
                            Priority = rawSesison.Priority,
                        };
                        sessionInfo.Videos.AddRange(JsUtils.ToClrArray<string>(rawSesison.Videos));
                        sessionInfo.Audios.AddRange(JsUtils.ToClrArray<string>(rawSesison.Audios));
                    }
                    else
                    {
                        sessionInfo = new SessionInfo();
                    }


                    this.cachedDmcInfo = new DmcInfo()
                    {
                        Title = this.RawDmcInfo.Title,
                        Id = this.RawDmcInfo.Id,
                        Owner = this.RawDmcInfo.Owner,
                        OwnerID = this.RawDmcInfo.OwnerID,
                        UserId = this.RawDmcInfo.UserId,
                        Userkey = this.RawDmcInfo.Userkey,
                        ChannelID = this.RawDmcInfo.ChannelID,
                        ChannelName = this.RawDmcInfo.ChannelName,
                        Description = this.RawDmcInfo.Description,
                        ViewCount = this.RawDmcInfo.ViewCount,
                        CommentCount = this.RawDmcInfo.CommentCount,
                        MylistCount = this.RawDmcInfo.MylistCount,
                        LikeCount = this.RawDmcInfo.LikeCount,
                        Duration = this.RawDmcInfo.Duration,
                        IsDownloadable = this.RawDmcInfo.IsDownloadable,
                        IsEncrypted = this.RawDmcInfo.IsEncrypted,
                        IsOfficial = this.RawDmcInfo.IsOfficial,
                        UploadedOn = JsUtils.ToLocalDateTime(this.RawDmcInfo.UploadedOn),
                        DownloadStartedOn = JsUtils.ToLocalDateTime(this.RawDmcInfo.DownloadStartedOn),
                        ThumbInfo = new ThumbInfo(rawhumb.large, rawhumb.middle, rawhumb.normal, rawhumb.player),
                        SessionInfo = sessionInfo,
                        IsPremium = this.RawDmcInfo.IsPremium,
                        IsPeakTime = this.RawDmcInfo.IsPeakTime,
                    };
                    this.cachedDmcInfo.Tags.AddRange(JsUtils.ToClrArray<string>(this.RawDmcInfo.Tags));


                    List<dynamic> threads = JsUtils.ToClrArray<dynamic>(this.RawDmcInfo.CommentThreads);

                    foreach (var rawThread in threads)
                    {
                        var thread = new Thread()
                        {
                            ID = rawThread.ID,
                            Fork = rawThread.Fork,
                            IsActive = rawThread.IsActive,
                            IsDefaultPostTarget = rawThread.IsDefaultPostTarget,
                            IsEasyCommentPostTarget = rawThread.IsEasyCommentPostTarget,
                            IsLeafRequired = rawThread.IsLeafRequired,
                            IsOwnerThread = rawThread.IsOwnerThread,
                            IsThreadkeyRequired = rawThread.IsThreadkeyRequired,
                            Threadkey = rawThread.Threadkey,
                            Is184Forced = rawThread.Is184Forced,
                            HasNicoscript = rawThread.HasNicoscript,
                            Label = rawThread.Label,
                            PostkeyStatus = rawThread.PostkeyStatus,
                            Server = rawThread.Server,
                            VideoID = rawThread.VideoID,
                            ForkLabel = rawThread.ForkLabel,
                        };
                        this.cachedDmcInfo.CommentThreads.Add(thread);
                    }

                }

                return this.cachedDmcInfo;
            }
        }

        public dynamic RawDmcInfo { get; set; } = new object();

    }
}
