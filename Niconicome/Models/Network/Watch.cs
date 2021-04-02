using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchInfo = Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Playlist;
using Niconicome.Models.Local;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Utils;
using Niconicome.Extensions.System;

namespace Niconicome.Models.Network
{
    public interface IVideoInfo
    {
        string Title { get; set; }
        string Id { get; set; }
        string FileName { get; set; }
        string ChannelID { get; set; }
        string ChannelName { get; set; }
        string OwnerName { get; set; }
        int ViewCount { get; set; }
        int CommentCount { get; set; }
        int MylistCount { get; set; }
        int LikeCount { get; set; }
        int OwnerID { get; set; }
        int Duration { get; set; }
        IEnumerable<string> Tags { get; set; }
        DateTime UploadedOn { get; set; }
        Uri LargeThumbUri { get; set; }
        Uri ThumbUri { get; set; }
        IListVideoInfo ConvertToTreeVideoInfo();
    }

    public interface IWatch
    {
        Task<IResult> TryGetVideoInfoAsync(string nicoId, IVideoInfo outInfo, WatchInfo::WatchInfoOptions options = WatchInfo::WatchInfoOptions.Default);
    }

    public interface IResult
    {
        bool IsSucceeded { get; }
        string Message { get; }
    }

    public class Watch : IWatch
    {
        public Watch(WatchInfo::IWatchInfohandler handler, ILocalSettingHandler settingHandler, INiconicoUtils utils, ILogger logger)
        {
            this.handler = handler;
            this.settingHandler = settingHandler;
            this.utils = utils;
            this.logger = logger;
        }

        private readonly WatchInfo::IWatchInfohandler handler;

        private readonly ILocalSettingHandler settingHandler;

        private readonly INiconicoUtils utils;

        private readonly ILogger logger;

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="nicoId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IResult> TryGetVideoInfoAsync(string nicoId, IVideoInfo info, WatchInfo::WatchInfoOptions options = WatchInfo::WatchInfoOptions.Default)
        {
            IResult result;

            try
            {
                result = await this.GetVideoInfoAsync(nicoId, info, options);
            }
            catch (Exception e)
            {
                var failedResult = new Result();
                this.logger.Error($"動画情報の取得中にエラーが発生しました。(id:{nicoId})", e);
                failedResult.IsSucceeded = false;
                failedResult.Message = $"動画情報の取得中にエラーが発生しました。(詳細: id:{nicoId}, message: {e.Message})";
                return failedResult;
            }

            return result;

        }

        /// <summary>
        /// 実装
        /// </summary>
        /// <param name="nicoId"></param>
        /// <param name="info"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task<IResult> GetVideoInfoAsync(string nicoId, IVideoInfo info, WatchInfo::WatchInfoOptions options = WatchInfo::WatchInfoOptions.Default)
        {
            WatchInfo::IDomainVideoInfo retrieved;
            var result = new Result();
            string filenameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";

            try
            {
                retrieved = await this.handler.GetVideoInfoAsync(nicoId, options);
            }
            catch
            {
                result.IsSucceeded = false;
                result.Message = this.handler.State switch
                {
                    WatchInfo::WatchInfoHandlerState.HttpRequestFailure => "httpリクエストに失敗しました。(サーバーエラー・IDの指定間違い)",
                    WatchInfo::WatchInfoHandlerState.JsonParsingFailure => "視聴ページの解析に失敗しました。(サーバーエラー)",
                    WatchInfo::WatchInfoHandlerState.NoJsDataElement => "視聴ページの解析に失敗しました。(サーバーエラー・権利のない有料動画など)",
                    WatchInfo::WatchInfoHandlerState.OK => "取得完了",
                    _ => "不明なエラー"
                };

                return result;
            }

            var replaceStricted = this.settingHandler.GetBoolSetting(Settings.ReplaceSBToMB);

            //タイトル
            info.Title = retrieved.Title;

            //ID
            info.Id = retrieved.Id;

            //タグ
            info.Tags = retrieved.Tags;

            //ファイル名
            info.FileName = this.utils.GetFileName(filenameFormat, retrieved.DmcInfo, ".mp4", replaceStricted);

            //再生回数
            info.ViewCount = retrieved.ViewCount;
            info.CommentCount = retrieved.CommentCount;
            info.MylistCount = retrieved.MylistCount;
            info.LikeCount = retrieved.LikeCount;

            //チャンネル情報
            info.ChannelID = retrieved.ChannelID;
            info.ChannelName = retrieved.ChannelName;

            //投稿者情報
            info.OwnerID = retrieved.OwnerID;
            info.OwnerName = retrieved.Owner;

            //再生時間
            info.Duration = retrieved.Duration;

            //投稿日時
            if (retrieved.DmcInfo is not null)
            {
                info.UploadedOn = retrieved.DmcInfo.UploadedOn;
            }

            //サムネイル
            if (retrieved.DmcInfo is not null && retrieved.DmcInfo.ThumbInfo is not null)
            {
                if (!retrieved.DmcInfo.ThumbInfo.Large.IsNullOrEmpty())
                {
                    info.LargeThumbUri = new Uri(retrieved.DmcInfo.ThumbInfo.Large);
                }

                if (!retrieved.DmcInfo.ThumbInfo.Normal.IsNullOrEmpty())
                {
                    info.ThumbUri = new Uri(retrieved.DmcInfo.ThumbInfo.Normal);
                }
            }

            result.IsSucceeded = true;
            result.Message = "取得成功";


            return result;
        }
    }

    /// <summary>
    /// 動画情報
    /// </summary>
    public class VIdeoInfo : IVideoInfo
    {
        public static string DeletedVideoThumb { get; private set; } = "https://nicovideo.cdn.nimg.jp/web/img/common/video_deleted.jpg";

        public string Title { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ChannelID { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int MylistCount { get; set; }

        public int LikeCount { get; set; }

        public int OwnerID { get; set; }

        public int Duration { get; set; }

        public IEnumerable<string> Tags { get; set; } = new List<string>();

        public DateTime UploadedOn { get; set; }

        public Uri LargeThumbUri { get; set; } = new Uri(VIdeoInfo.DeletedVideoThumb);

        public Uri ThumbUri { get; set; } = new Uri(VIdeoInfo.DeletedVideoThumb);

        /// <summary>
        /// Viewから参照可能な形式に変換する
        /// </summary>
        /// <returns></returns>
        public IListVideoInfo ConvertToTreeVideoInfo()
        {
            return new BindableListVIdeoInfo()
            {
                Title = this.Title,
                NiconicoId = this.Id,
                UploadedOn = this.UploadedOn,
                LargeThumbUrl = this.LargeThumbUri.AbsoluteUri,
                ThumbUrl = this.ThumbUri.AbsoluteUri,
                Tags = this.Tags,
                ViewCount = this.ViewCount,
                FileName = this.FileName,
                ChannelId = this.ChannelID,
                ChannelName = this.ChannelName,
                MylistCount = this.MylistCount,
                CommentCount = this.CommentCount,
                LikeCount = this.LikeCount,
                OwnerID = this.OwnerID,
                Duration = this.Duration,
                OwnerName = this.OwnerName,
            };
        }

    }

    public class Result : IResult
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }


}
