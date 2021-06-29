using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Watch
{

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

    public class Result : IResult
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }


}
