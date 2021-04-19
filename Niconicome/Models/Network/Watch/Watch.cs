using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Watch
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
        Task<IResult> TryGetVideoInfoAsync(string nicoId, IListVideoInfo outInfo, WatchInfoOptions options = WatchInfoOptions.Default);
        void ConvertDomainVideoInfoToListVideoInfo(IListVideoInfo videoInfo, IDomainVideoInfo domwinVideoInfo);
    }

    public interface IResult
    {
        bool IsSucceeded { get; }
        string Message { get; }
    }

    public class Watch : IWatch
    {
        public Watch(IWatchInfohandler handler, ILogger logger)
        {
            this.handler = handler;
            this.logger = logger;
        }

        private readonly IWatchInfohandler handler;


        private readonly ILogger logger;

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="nicoId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IResult> TryGetVideoInfoAsync(string nicoId, IListVideoInfo info, WatchInfoOptions options = WatchInfoOptions.Default)
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
        private async Task<IResult> GetVideoInfoAsync(string nicoId, IListVideoInfo info, WatchInfoOptions options = WatchInfoOptions.Default)
        {
            IDomainVideoInfo retrieved;
            var result = new Result();

            try
            {
                retrieved = await this.handler.GetVideoInfoAsync(nicoId, options);
            }
            catch
            {
                result.IsSucceeded = false;
                result.Message = this.handler.State switch
                {
                    WatchInfoHandlerState.HttpRequestFailure => "httpリクエストに失敗しました。(サーバーエラー・IDの指定間違い)",
                    WatchInfoHandlerState.JsonParsingFailure => "視聴ページの解析に失敗しました。(サーバーエラー)",
                    WatchInfoHandlerState.NoJsDataElement => "視聴ページの解析に失敗しました。(サーバーエラー・権利のない有料動画など)",
                    WatchInfoHandlerState.OK => "取得完了",
                    _ => "不明なエラー"
                };

                return result;
            }

            this.ConvertDomainVideoInfoToListVideoInfo(info, retrieved);

            result.IsSucceeded = true;
            result.Message = "取得成功";


            return result;
        }

        /// <summary>
        /// Domainの動画情報を変換する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        public void ConvertDomainVideoInfoToListVideoInfo(IListVideoInfo info, IDomainVideoInfo domainVideoInfo)
        {

            //タイトル
            info.Title = domainVideoInfo.Title;

            //ID
            info.NiconicoId = domainVideoInfo.Id;

            //タグ
            info.Tags = domainVideoInfo.Tags;

            //再生回数
            info.ViewCount = domainVideoInfo.ViewCount;
            info.CommentCount = domainVideoInfo.CommentCount;
            info.MylistCount = domainVideoInfo.MylistCount;
            info.LikeCount = domainVideoInfo.LikeCount;

            //チャンネル情報
            info.ChannelID = domainVideoInfo.ChannelID;
            info.ChannelName = domainVideoInfo.ChannelName;

            //投稿者情報
            info.OwnerID = domainVideoInfo.OwnerID;
            info.OwnerName = domainVideoInfo.Owner;

            //再生時間
            info.Duration = domainVideoInfo.Duration;

            //投稿日時
            if (domainVideoInfo.DmcInfo is not null)
            {
                info.UploadedOn = domainVideoInfo.DmcInfo.UploadedOn;
            }

            //サムネイル
            if (domainVideoInfo.DmcInfo is not null && domainVideoInfo.DmcInfo.ThumbInfo is not null)
            {
                if (!domainVideoInfo.DmcInfo.ThumbInfo.Large.IsNullOrEmpty())
                {
                    info.ThumbUrl = domainVideoInfo.DmcInfo.ThumbInfo.Large;
                }

                if (!domainVideoInfo.DmcInfo.ThumbInfo.Normal.IsNullOrEmpty())
                {
                    info.ThumbUrl = domainVideoInfo.DmcInfo.ThumbInfo.Normal;
                }
            }

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

        public Uri LargeThumbUri { get; set; } = new Uri(DeletedVideoThumb);

        public Uri ThumbUri { get; set; } = new Uri(DeletedVideoThumb);

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
                ChannelID = this.ChannelID,
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
