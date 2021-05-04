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
    }

    public interface IResult
    {
        bool IsSucceeded { get; }
        string Message { get; }
    }

    public class Watch : IWatch
    {
        public Watch(IWatchInfohandler handler, ILogger logger, IDomainModelConverter converter)
        {
            this.handler = handler;
            this.logger = logger;
            this.converter = converter;
        }

        #region DIされるクラス
        private readonly IWatchInfohandler handler;

        private readonly ILogger logger;

        private readonly IDomainModelConverter converter;
        #endregion

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

            this.converter.ConvertDomainVideoInfoToListVideoInfo(info, retrieved);

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

        public Uri LargeThumbUri { get; set; } = new Uri(DeletedVideoThumb);

        public Uri ThumbUri { get; set; } = new Uri(DeletedVideoThumb);

        /// <summary>
        /// Viewから参照可能な形式に変換する
        /// </summary>
        /// <returns></returns>
        public IListVideoInfo ConvertToTreeVideoInfo()
        {
            var video = new NonBindableListVideoInfo();
            video.Title.Value = this.Title;
            video.NiconicoId.Value = this.Id;
            video.UploadedOn.Value = this.UploadedOn;
            video.LargeThumbUrl.Value = this.LargeThumbUri.AbsoluteUri;
            video.ThumbUrl.Value = this.ThumbUri.AbsoluteUri;
            video.Tags = this.Tags;
            video.ViewCount.Value = this.ViewCount;
            video.FileName.Value = this.FileName;
            video.ChannelID.Value = this.ChannelID;
            video.ChannelName.Value = this.ChannelName;
            video.MylistCount.Value = this.MylistCount;
            video.CommentCount.Value = this.CommentCount;
            video.LikeCount.Value = this.LikeCount;
            video.OwnerID.Value = this.OwnerID;
            video.Duration.Value = this.Duration;
            video.OwnerName.Value = this.OwnerName;
            return video;
        }

    }

    public class Result : IResult
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }


}
