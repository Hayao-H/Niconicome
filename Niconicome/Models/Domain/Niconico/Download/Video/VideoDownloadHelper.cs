using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Dmc;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Video
{
    public interface IVideoDownloadHelper
    {
        /// <summary>
        /// ストリームをダウンロードする
        /// </summary>
        /// <param name="stream"></param>s
        /// <param name="onMessage"></param>
        /// <param name="context"></param>
        /// <param name="maxParallelDownloadCount"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult> DownloadAsync(IStreamInfo stream, Action<string> onMessage, IDownloadContext context, int maxParallelDownloadCount, string segmentDiectoryNames, CancellationToken token);
    }



    /// <summary>
    /// ダウンロードの実処理を担当する
    /// </summary>
    public class VideoDownloadHelper : IVideoDownloadHelper
    {

        public VideoDownloadHelper(INicoHttp http, ISegmentWriter writer, ILogger logger)
        {
            this._http = http;
            this._writer = writer;
            this._logger = logger;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ISegmentWriter _writer;

        private readonly ILogger _logger;

        #endregion

        #region Method

        public async Task<IAttemptResult> DownloadAsync(IStreamInfo stream, Action<string> onMessage, IDownloadContext context, int maxParallelDownloadCount, string segmentDirectoryName, CancellationToken token)
        {
            //変数の初期化
            var taskHandler = new ParallelTasksHandler<IParallelDownloadTask>(maxParallelDownloadCount, false);
            int completed = context.OriginalSegmentsCount - stream.StreamUrls.Count;
            int all = context.OriginalSegmentsCount;
            string resolution = context.ActualVerticalResolution == 0 ? string.Empty : $"（{context.ActualVerticalResolution}px）";
            var failedInOne = false;

            var tasks = stream.StreamUrls.Select(url => new ParallelDownloadTask(async (self, _) =>
            {
                if (token.IsCancellationRequested || failedInOne)
                {
                    self.SetResult(AttemptResult.Fail());
                    return;
                }

                byte[] data;

                try
                {
                    data = await this.DownloadInternalAsync(self.Url, self.Context, token);
                }
                catch (Exception e)
                {
                    self.SetResult(AttemptResult.Fail(null, e));
                    this._logger.Error($"セグメント(idx:{self.SequenceZero})の取得に失敗しました。({context.GetLogContent()})", e);
                    failedInOne = true;
                    return;
                }

                this._logger.Log($"セグメント(idx:{self.SequenceZero})を取得しました。({context.GetLogContent()})");
                if (token.IsCancellationRequested)
                {
                    self.SetResult(AttemptResult.Fail());
                    return;
                }

                try
                {
                    this._writer.Write(data, self, segmentDirectoryName);
                }
                catch (Exception e)
                {
                    self.SetResult(AttemptResult.Fail(null, e));
                    this._logger.Error($"セグメント(idx:{self.SequenceZero})の書き込みに失敗しました。({context.GetLogContent()})", e);
                    failedInOne = true;
                    return;
                }

                completed++;
                onMessage($"完了: {completed}/{all}{resolution}");
                self.SetResult(AttemptResult.Succeeded());

                await Task.Delay(1 * 1000, token);


            }, context, url.AbsoluteUrl, url.SequenceZero, url.FileName));

            foreach (var task in tasks)
            {
                taskHandler.AddTaskToQueue(task);
            }

            await taskHandler.ProcessTasksAsync(ct: token);

            foreach (var task in tasks)
            {
                if (task.Result is not null && !task.Result.IsSucceeded)
                {
                    return task.Result;
                }
            }

            return AttemptResult.Succeeded();
        }

        #endregion

        #region private

        private async Task<byte[]> DownloadInternalAsync(string url, IDownloadContext context, CancellationToken token, int retryAttempt = 0)
        {
            var res = await this._http.GetAsync(new Uri(url));
            if (!res.IsSuccessStatusCode)
            {
                //リトライ回数は3回
                if (retryAttempt < 3)
                {
                    retryAttempt++;
                    await Task.Delay(10 * 1000, token);
                    return await this.DownloadInternalAsync(url, context, token, retryAttempt);
                }
                else
                {
                    throw new HttpRequestException($"セグメントファイルの取得に失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase}, url: {url}, {context.GetLogContent()})");
                }
            }

            return await res.Content.ReadAsByteArrayAsync();
        }

        #endregion
    }
}
