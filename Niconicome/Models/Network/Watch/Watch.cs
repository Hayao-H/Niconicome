using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Watch
{

    public interface IWatch
    {
        Task<IAttemptResult<IListVideoInfo>> TryGetVideoInfoAsync(string nicoId, WatchInfoOptions options = WatchInfoOptions.Default);
    }

    public interface IResult
    {
        bool IsSucceeded { get; }
        string Message { get; }
    }

    public class Watch : IWatch
    {
        public Watch(IWatchInfohandler handler, ILogger logger, IDomainModelConverter converter,IVideoInfoContainer container)
        {
            this.handler = handler;
            this.logger = logger;
            this.converter = converter;
            this.container = container;
        }

        #region DIされるクラス
        private readonly IWatchInfohandler handler;

        private readonly ILogger logger;

        private readonly IDomainModelConverter converter;

        private readonly IVideoInfoContainer container;
        #endregion

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="nicoId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<IListVideoInfo>> TryGetVideoInfoAsync(string nicoId, WatchInfoOptions options = WatchInfoOptions.Default)
        {

            IAttemptResult<IListVideoInfo> result;

            try
            {
                result = await this.GetVideoInfoAsync(nicoId, options);
            }
            catch (Exception e)
            {
                this.logger.Error($"動画情報の取得中に不明なエラーが発生しました。(id:{nicoId})", e);
                return new AttemptResult<IListVideoInfo>() { Message = "動画情報の取得中にエラーが発生しました。", Exception = e };
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
        private async Task<IAttemptResult<IListVideoInfo>> GetVideoInfoAsync(string nicoId, WatchInfoOptions options = WatchInfoOptions.Default)
        {
            IAttemptResult<IDomainVideoInfo> result;

            try
            {
                result = await this.handler.GetVideoInfoAsync(nicoId, options);
            }
            catch (Exception e)
            {
                this.logger.Error($"動画情報を取得中に不明なエラーが発生しました。(id:{nicoId})", e);
                return new AttemptResult<IListVideoInfo>() { Message = $"不明なエラーが発生しました。(詳細:{e.Message})" };
            }

            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<IListVideoInfo>() { Message = result.Message, Exception = result.Exception };
            }

            IListVideoInfo info = this.container.GetVideo(nicoId);
            this.converter.ConvertDomainVideoInfoToListVideoInfo(info, result.Data);


            return new AttemptResult<IListVideoInfo>() { IsSucceeded = true, Data = info };
        }

    }

    public class Result : IResult
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }


}
