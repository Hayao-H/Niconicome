using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V3.Error.SegmentDownloaderError;
using SC = Niconicome.Models.Domain.Niconico.Download.Video.V3.Fetch.Segment.StringContent.SegmentDownloaderSC;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Fetch.Segment
{

    public interface ISegmentDownloader
    {
        /// <summary>
        /// セグメントをダウンロードする
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> DownloadAsync();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="infomation"></param>
        /// <param name="container"></param>
        void Initialize(ISegmentInfomation infomation, ISegmentDLResultContainer container);
    }

    public class SegmentDownloader : ISegmentDownloader
    {
        public SegmentDownloader(INicoHttp http, ISegmentWriter writer, IErrorHandler errorHandler, IStringHandler stringHandler)
        {
            this._http = http;
            this._writer = writer;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ISegmentWriter _writer;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        private bool _isInitialized;

        private ISegmentDLResultContainer? _resultContainer;

        private ISegmentInfomation? _segmentInfomation;

        #endregion

        #region Method

        public async Task<IAttemptResult> DownloadAsync()
        {
            if (!this._isInitialized)
            {
                this._errorHandler.HandleError(Err.NotInitialized);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.NotInitialized));
            }

            //中止処理
            if (this._segmentInfomation!.CT.IsCancellationRequested)
            {
                this._resultContainer!.SetResult(false, this._segmentInfomation.Index);
                this._errorHandler.HandleError(Err.Canceled, this._segmentInfomation.NiconicoID);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.Canceled, this._segmentInfomation.NiconicoID));
            }

            //他のセグメントのDLに失敗した場合は中止
            if (this._resultContainer!.IsFailedInAny)
            {
                this._resultContainer.SetResult(false, this._segmentInfomation.Index);
                this._errorHandler.HandleError(Err.FailedInAny, this._segmentInfomation.NiconicoID);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.FailedInAny, this._segmentInfomation.NiconicoID));
            }

            //DL
            IAttemptResult<byte[]> result = await this.DownloadInternalAsync(this._segmentInfomation);

            if (!result.IsSucceeded || result.Data is null)
            {
                this._resultContainer.SetResult(false, this._segmentInfomation.Index);
                return AttemptResult.Fail(result.Message);
            }

            this._errorHandler.HandleError(Err.SucceededToFetch, this._segmentInfomation.Index, this._segmentInfomation.NiconicoID);

            if (this._segmentInfomation.CT.IsCancellationRequested)
            {
                this._resultContainer!.SetResult(false, this._segmentInfomation.Index);
                this._errorHandler.HandleError(Err.Canceled, this._segmentInfomation.NiconicoID);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.Canceled, this._segmentInfomation.NiconicoID));
            }


            //書き込み
            IAttemptResult writeResult = this._writer.Write(result.Data, this._segmentInfomation.FilePath);

            if (!writeResult.IsSucceeded)
            {
                this._resultContainer.SetResult(false, this._segmentInfomation.Index);
                return writeResult;
            }

            this._resultContainer.SetResult(true, this._segmentInfomation.Index);
            this._segmentInfomation.OnMessage(this._stringHandler.GetContent(SC.CompletedMessage, this._resultContainer.CompletedCount, this._resultContainer.Length, this._segmentInfomation.VerticalResolution));

            return AttemptResult.Succeeded();

        }

        public void Initialize(ISegmentInfomation infomation, ISegmentDLResultContainer container)
        {
            this._segmentInfomation = infomation;
            this._resultContainer = container;
            this._isInitialized = true;
        }

        #endregion

        #region private

        private async Task<IAttemptResult<byte[]>> DownloadInternalAsync(ISegmentInfomation infomation, int retryAttempt = 0)
        {
            var res = await this._http.GetAsync(new Uri(infomation.SegmentURL));
            if (!res.IsSuccessStatusCode)
            {
                //リトライ回数は3回
                if (retryAttempt < 3)
                {
                    retryAttempt++;
                    await Task.Delay(10 * 1000, infomation.CT);
                    return await this.DownloadInternalAsync(infomation, retryAttempt);
                }
                else
                {
                    this._errorHandler.HandleError(Err.FailedToFetch, infomation.Index, (int)res.StatusCode, infomation.SegmentURL, infomation.NiconicoID);
                    return AttemptResult<byte[]>.Fail(this._errorHandler.GetMessageForResult(Err.FailedToFetch, infomation.Index, (int)res.StatusCode, infomation.SegmentURL, infomation.NiconicoID));
                }
            }

            return AttemptResult<byte[]>.Succeeded(await res.Content.ReadAsByteArrayAsync());
        }

        #endregion
    }
}
