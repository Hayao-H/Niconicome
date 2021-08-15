using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Niconico.Dmc;
using Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Video
{

    public interface IDownloadTask
    {
        string Url { get; }
        string FileName { get; }
        int SequenceZero { get; }
        bool Completed { get; set; }
        Action<IDownloadTask>? PostCompletedCallback { get; set; }
    }


    public interface IVideoDownloadHelper
    {
        Task DownloadAsync(IStreamInfo stream, IDownloadMessenger messenger, IDownloadContext context, int maxParallelDownloadCount, CancellationToken token);
        IEnumerable<string> GetAllFileAbsPaths();
        string FolderName { get; }
    }

    public interface IDownloadTaskHandler
    {
        IDownloadTask RetrieveNextTask();
        IEnumerable<IDownloadTask> ConvertToTasks(IStreamInfo streamInfo);
        void AddTask(IDownloadTask task);
        void AddTasks(IEnumerable<IDownloadTask> task);
        bool IsCompleted { get; }
        bool HasNextTask { get; }
        int OriginalTasksCount { get; }
        int CurrentTasksCount { get; }
        int CurrentCompletedTasksCount { get; }
    }

    public interface IVideoDownloader
    {
        Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> OnMessage, IDownloadContext context, CancellationToken token);
        Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> OnMessage, IWatchSession session, IDownloadContext context, CancellationToken token);
    }

    public interface IVideoDownloadSettings
    {
        string NiconicoId { get; }
        string FolderName { get; }
        string FileNameFormat { get; }
        string CommandFormat { get; }
        string? EconomySuffix { get; }
        bool IsOverwriteEnable { get; }
        bool IsAutoDisposingEnable { get; }
        bool IsReplaceStrictedEnable { get; }
        bool IsOvwrridingFileDTEnable { get; }
        bool IsNoEncodeEnable { get; }
        bool IsResumeEnable { get; }
        uint VerticalResolution { get; }
        int MaxParallelDownloadCount { get; }
    }

    /// <summary>
    /// 外部から触るAPIを提供する
    /// </summary>
    public class VideoDownloader : IVideoDownloader
    {
        public VideoDownloader(IWatchSession session, ILogger logger, IVideoDownloadHelper videoDownloadHelper, IDownloadMessenger messenger, IVideoEncoader encorder, INiconicoUtils utils, IVideoFileStorehandler fileStorehandler, IStreamResumer streamResumer)
        {
            this.session = session;
            this.logger = logger;
            this.videoDownloadHelper = videoDownloadHelper;
            this.messenger = messenger;
            this.encorder = encorder;
            this.utils = utils;
            this.fileStorehandler = fileStorehandler;
            this.streamResumer = streamResumer;
        }

        ~VideoDownloader()
        {
            if (!this.session.IsSessionExipired) this.session.Dispose();
        }

        public async Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> onMessage, IWatchSession session, IDownloadContext context, CancellationToken token)
        {
            this.messenger.AddHandler(onMessage);
            this.logger.Log($"動画のダウンロードを開始しました。(context_id: {context.Id}, content_id: {context.NiconicoId})");

            if (session.IsSessionExipired)
            {
                this.logger.Log("セッションが失効していたため動画のダウンロードをキャンセルします。(context_id: {context.Id}, content_id: {context.NiconicoId})");
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Issucceeded = false, Message = "セッションが失効済のためダウンロード出来ません。" };
            }


            if (!session.IsSessionEnsured)
            {
                if (token.IsCancellationRequested)
                {
                    this.logger.Log("ユーザーの操作によって動画のダウンロード処理がキャンセルされました。(context_id: {context.Id}, content_id: {context.NiconicoId})");
                    this.messenger.SendMessage("ダウンロードをキャンセル");
                    this.messenger.RemoveHandler(onMessage);
                    return this.GetCancelledResult();
                }
                await session.EnsureSessionAsync(settings.NiconicoId, true);

                if (!session.IsSessionEnsured)
                {
                    string message = session.State switch
                    {
                        WatchSessionState.HttpRequestOrPageAnalyzingFailure => "視聴ページの取得、または解析に失敗しました。",
                        WatchSessionState.EncryptedVideo => "暗号化された動画のため、ダウンロードできません。",
                        WatchSessionState.SessionEnsuringFailure => "セッションの確立に失敗しました。",
                        _ => "不明なエラーにより、セッションの確立に失敗しました。"
                    };

                    this.messenger.SendMessage(message);
                    this.messenger.RemoveHandler(onMessage);
                    return new DownloadResult() { Issucceeded = false, Message = message };
                }
            }


            string? economySuffix = session.Video!.DmcInfo.IsEnonomy ? settings.EconomySuffix : null;
            string? suffix = economySuffix;
            string ext = settings.IsNoEncodeEnable ? ".ts" : ".mp4";
            string fileName = !settings.FileNameFormat.IsNullOrEmpty() ? this.utils.GetFileName(settings.FileNameFormat, session.Video!.DmcInfo, ext, settings.IsReplaceStrictedEnable, suffix)
                : $"[{session.Video!.Id}]{session.Video!.Title}{ext}"
                ;

            if (token.IsCancellationRequested)
            {
                this.logger.Log($"ユーザーの操作によって動画のダウンロード処理がキャンセルされました。(context_id: {context.Id}, content_id: {context.NiconicoId})");
                this.messenger.SendMessage("ダウンロードをキャンセル");
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            var streams = await session.GetAvailableStreamsAsync();

            var targetStream = streams.GetStream(settings.VerticalResolution);

            context.ActualVerticalResolution = targetStream.Resolution?.Vertical ?? 0;
            context.OriginalSegmentsCount = targetStream.StreamUrls.Count;

            if (token.IsCancellationRequested)
            {
                this.logger.Log($"ユーザーの操作によって動画のダウンロード処理がキャンセルされました。(context_id: {context.Id}, content_id: {context.NiconicoId})");
                this.messenger.SendMessage("ダウンロードをキャンセル");
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            ISegmentsDirectoryInfo? segmentsDirectoryInfo = null;
            var segmentFilePaths = targetStream.StreamUrls.Select(u => u.FileName).Copy();
            if (settings.IsResumeEnable)
            {
                try
                {
                    segmentsDirectoryInfo = this.GetSegmentsDirectoryInfo(settings.NiconicoId, targetStream.Resolution?.Vertical ?? 0);
                }
                catch (Exception e)
                {
                    this.logger.Error("セグメントディレクトリ情報の取得に失敗しました。", e);
                }
            }

            if (segmentsDirectoryInfo is not null)
            {
                var urls = targetStream.StreamUrls.Where(u => !segmentsDirectoryInfo.ExistsFileNames.Contains(u.FileName)).Copy();
                targetStream.StreamUrls.Clear();
                targetStream.StreamUrls.AddRange(urls);
                context.SegmentsDirectoryName = segmentsDirectoryInfo.DirectoryName;
                this.messenger.SendMessage("DLをレジューム");
            }
            else
            {
                context.SegmentsDirectoryName = $"{settings.NiconicoId}-{targetStream.Resolution?.Vertical ?? 0}-{DateTime.Now.ToString("yyyy-MM-dd")}";
            }

            segmentFilePaths = segmentFilePaths.Select(p => Path.Combine(AppContext.BaseDirectory, "tmp", context.SegmentsDirectoryName, p));

            try
            {
                await this.videoDownloadHelper.DownloadAsync(targetStream, this.messenger, context, settings.MaxParallelDownloadCount, token);

            }
            catch (Exception e)
            {
                this.logger.Error("動画のダウンロード中にエラーが発生しました。", e);
                this.messenger.SendMessage("動画のダウンロードに失敗");
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Issucceeded = false, Message = $"動画のダウンロード中にエラーが発生しました。(詳細: {e.Message})" };
            }

            if (settings.IsAutoDisposingEnable) session.Dispose();

            if (token.IsCancellationRequested)
            {
                this.logger.Log($"ユーザーの操作によって動画のダウンロード処理がキャンセルされました。(context_id: {context.Id}, content_id: {context.NiconicoId})");
                this.messenger.SendMessage("ダウンロードをキャンセル");
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            this.messenger.SendMessage("動画を変換中...");
            var encodeSetting = new EncodeSettings()
            {
                FileName = fileName,
                FolderName = settings.FolderName,
                CommandFormat = settings.CommandFormat,
                TsFilePaths = segmentFilePaths,
                IsOverwriteEnable = settings.IsOverwriteEnable,
                IsOverrideDTEnable = settings.IsOvwrridingFileDTEnable,
                UploadedOn = session.Video.DmcInfo.UploadedOn,
                IsNoEncodeEnable = settings.IsNoEncodeEnable,
            };

            try
            {
                await this.encorder.EncodeAsync(encodeSetting, this.messenger, token);
            }
            catch (TaskCanceledException)
            {
                this.logger.Log($"ユーザーの操作によって動画の変換処理がキャンセルされました。(context_id: {context.Id}, content_id: {context.NiconicoId})");
                this.messenger.SendMessage("動画の変換中にキャンセル");
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Issucceeded = false, Message = $"動画の変換中にキャンセルされました。" };
            }
            catch (Exception e)
            {
                this.logger.Error("ファイルの変換中にエラーが発生しました。", e);
                this.messenger.SendMessage("動画の変換中にエラー発生");
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Issucceeded = false, Message = $"ファイルの変換中にエラーが発生しました。(詳細: {e.Message})" };
            }
            this.messenger.SendMessage("動画の変換が完了");

            this.DeleteTmpFolder(context);

            this.fileStorehandler.Add(settings.NiconicoId, Path.Combine(settings.FolderName, fileName));

            this.messenger.RemoveHandler(onMessage);
            return new DownloadResult() { Issucceeded = true, VideoFileName = fileName, VerticalResolution = targetStream.Resolution!.Vertical };
        }

        public async Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> onMessage, IDownloadContext context, CancellationToken token)
        {
            return await this.DownloadVideoAsync(settings, onMessage, this.session, context, token);
        }

        /// <summary>
        /// セグメントディレクトリ情報を取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="verticalResolution"></param>
        /// <returns></returns>
        private ISegmentsDirectoryInfo? GetSegmentsDirectoryInfo(string niconicoID, uint verticalResolution)
        {
            if (!this.streamResumer.SegmentsDirectoryExists(niconicoID)) return null;

            var info = this.streamResumer.GetSegmentsDirectoryInfo(niconicoID);

            if (info.Resolution != verticalResolution)
            {
                this.streamResumer.RemoveSegmentsdirectory(niconicoID);
                return null;
            }

            return info;
        }

        /// <summary>
        /// 一時フォルダーを削除
        /// </summary>
        private void DeleteTmpFolder(IDownloadContext context)
        {
            string folderPath = Path.Combine(AppContext.BaseDirectory, "tmp", context.SegmentsDirectoryName);
            if (!Directory.Exists(folderPath)) return;
            try
            {
                Directory.Delete(folderPath, true);
            }
            catch (Exception e)
            {

                this.logger.Error("一時フォルダーの削除中にエラーが発生しました。", e);
                this.messenger.SendMessage("一時フォルダーの削除中にエラー発生");
            }
        }

        private readonly IWatchSession session;

        private readonly ILogger logger;

        private readonly IVideoDownloadHelper videoDownloadHelper;

        private readonly IDownloadMessenger messenger;

        private readonly IVideoEncoader encorder;

        private readonly INiconicoUtils utils;

        private readonly IVideoFileStorehandler fileStorehandler;

        private readonly IStreamResumer streamResumer;

        /// <summary>
        /// キャンセル時のメッセージを取得する
        /// </summary>
        /// <returns></returns>
        private IDownloadResult GetCancelledResult()
        {
            return new DownloadResult() { Issucceeded = false, Message = "処理がキャンセルされました" };
        }
    }

    /// <summary>
    /// ダウンロードの実処理を担当する
    /// </summary>
    public class VideoDownloadHelper : IVideoDownloadHelper
    {

        public VideoDownloadHelper(INicoHttp http, ISegmentWriter writer, ILogger logger)
        {
            this.http = http;
            this.writer = writer;
            this.logger = logger;
        }

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;

        /// <summary>
        /// データハンドラ
        /// </summary>
        private readonly ISegmentWriter writer;

        private readonly ILogger logger;

        /// <summary>
        /// 非同期でダウンロードする
        /// </summary>
        /// <param name="taskHandler"></param>
        /// <returns></returns>
        public async Task DownloadAsync(IStreamInfo stream, IDownloadMessenger messenger, IDownloadContext context, int maxParallelDownloadCount, CancellationToken token)
        {
            var taskHandler = new ParallelTasksHandler<IParallelDownloadTask>(maxParallelDownloadCount, false);
            var completed = context.OriginalSegmentsCount - stream.StreamUrls.Count;
            var all = context.OriginalSegmentsCount;
            var resolution = context.ActualVerticalResolution == 0 ? string.Empty : $"（{context.ActualVerticalResolution}px）";
            Exception? ex = null;

            var tasks = stream.StreamUrls.Select(url => new ParallelDownloadTask(async (self, _, pToken) =>
            {
                if (token.IsCancellationRequested || ex is not null)
                {
                    pToken.IsSkipped = true;
                    return;
                }

                byte[] data;

                try
                {
                    data = await this.DownloadInternalAsync(self.Url, self.Context, token);
                }
                catch (Exception e)
                {
                    ex = e;
                    this.logger.Error($"セグメント(idx:{self.SequenceZero})の取得に失敗しました。(context_id:{context.Id},content_id:{context.NiconicoId})", e);
                    return;
                }

                this.logger.Log($"セグメント(idx:{self.SequenceZero})を取得しました。(context_id:{context.Id},content_id:{context.NiconicoId})");

                if (token.IsCancellationRequested)
                {
                    return;
                }


                try
                {
                    this.writer.Write(data, self, context);
                }
                catch (Exception e)
                {
                    ex = e;
                    return;
                }

                completed++;
                messenger.SendMessage($"完了: {completed}/{all}{resolution}");

                await Task.Delay(1 * 1000, token);


            }, context, url.AbsoluteUrl, url.SequenceZero, url.FileName));

            foreach (var task in tasks)
            {
                taskHandler.AddTaskToQueue(task);
            }

            await taskHandler.ProcessTasksAsync(ct: token);

            if (ex is not null)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 全てのファイルの完全パスを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllFileAbsPaths()
        {
            return this.writer.FilesPathAbs;
        }

        /// <summary>
        /// フォルダー名
        /// </summary>
        public string FolderName => this.writer.FolderNameAbs;

        /// <summary>
        /// 内部ダウンロードメソッド
        /// </summary>
        /// <param name="url"></param>
        /// <param name="context"></param>
        /// <param name="retryAttempt"></param>
        /// <returns></returns>
        private async Task<byte[]> DownloadInternalAsync(string url, IDownloadContext context, CancellationToken token, int retryAttempt = 0)
        {
            var res = await this.http.GetAsync(new Uri(url));
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
                    throw new HttpRequestException($"セグメントファイルの取得に失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase}, url: {url}, context_id:{context.Id},content_id:{context.NiconicoId})");
                }
            }

            return await res.Content.ReadAsByteArrayAsync();
        }
    }

    public interface IParallelDownloadTask : IParallelTask<IParallelDownloadTask>
    {
        IDownloadContext Context { get; init; }
        string FileName { get; init; }
        int SequenceZero { get; init; }
        string Url { get; init; }
    }

    /// <summary>
    /// 並列DLタスク
    /// </summary>
    public class ParallelDownloadTask : IParallelDownloadTask
    {
        public ParallelDownloadTask(Func<IParallelDownloadTask, object, IParallelTaskToken, Task> taskFunc, IDownloadContext context, string url, int sequenceZero, string filename)
        {
            this.TaskFunction = taskFunc;
            this.TaskId = Guid.NewGuid();
            this.OnWait += (_) => { };
            this.Context = context;
            this.Url = url;
            this.SequenceZero = sequenceZero;
            this.FileName = filename;
        }

        /// <summary>
        /// コンテクスト
        /// </summary>
        public IDownloadContext Context { get; init; }

        public string Url { get; init; }

        public string FileName { get; init; }

        /// <summary>
        /// ストリームのインデックス
        /// </summary>
        public int SequenceZero { get; init; }

        public int Index { get; set; }

        public Guid TaskId { get; init; }

        public Func<IParallelDownloadTask, object, IParallelTaskToken, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }
    }

    /// <summary>
    /// ダウンロード設定
    /// </summary>
    public class VideoDownloadSettings : IVideoDownloadSettings
    {
        public string NiconicoId { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public string CommandFormat { get; set; } = Format.DefaultFFmpegFormat;

        public string? EconomySuffix { get; set; }

        public bool IsOverwriteEnable { get; set; }

        public bool IsAutoDisposingEnable { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }

        public bool IsOvwrridingFileDTEnable { get; set; }

        public bool IsResumeEnable { get; set; }

        public bool IsNoEncodeEnable { get; set; }

        public uint VerticalResolution { get; set; }

        public int MaxParallelDownloadCount { get; set; }

    }
}
