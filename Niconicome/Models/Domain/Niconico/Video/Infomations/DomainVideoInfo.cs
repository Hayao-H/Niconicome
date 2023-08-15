using System;
using System.Collections.Generic;
using Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using CS = Microsoft.ClearScript;

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
        IReadOnlyList<ITag> Tags { get; }
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
        public IReadOnlyList<ITag> Tags => this.DmcInfo.Tags;

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


                    List<dynamic> tags = JsUtils.ToClrArray<dynamic>(this.RawDmcInfo.Tags);
                    var clrTags = new List<ITag>();

                    foreach (var jsTag in tags)
                    {
                        var tag = new Tag()
                        {
                            IsNicodicExist = jsTag.IsNicodicExist,
                            Name = jsTag.Name,
                        };
                        clrTags.Add(tag);
                    }


                    List<dynamic> targets = JsUtils.ToClrArray<dynamic>(this.RawDmcInfo.CommentTargets);
                    var clrTargets = new List<ITarget>();

                    foreach (var rawThread in targets)
                    {
                        var target = new Target()
                        {
                            Id = rawThread.Id,
                            Fork = rawThread.Fork,
                        };
                        clrTargets.Add(target);
                    }


                    this.cachedDmcInfo = new DmcInfo()
                    {
                        Title = this.RawDmcInfo.Title,
                        Id = this.RawDmcInfo.Id,
                        Owner = this.RawDmcInfo.Owner,
                        OwnerID = this.RawDmcInfo.OwnerID,
                        UserId = this.RawDmcInfo.UserId,
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
                        Tags = clrTags,
                        CommentServer = this.RawDmcInfo.CommentServer is CS::Undefined ? string.Empty : this.RawDmcInfo.CommentServer,
                        Threadkey = this.RawDmcInfo.Threadkey,
                        CommentLanguage = this.RawDmcInfo.CommentLanguage,
                        CommentTargets = clrTargets.AsReadOnly(),
                    };



                }

                return this.cachedDmcInfo;
            }
        }

        public dynamic RawDmcInfo { get; init; } = new object();

    }
}
