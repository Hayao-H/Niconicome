using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Niconico.Dmc;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;

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
        Task DownloadAsync(IStreamInfo stream, IDownloadMessenger messenger, CancellationToken token);
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
        Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> OnMessage, CancellationToken token);
        Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> OnMessage, IWatchSession session, CancellationToken token);
    }

    public interface IVideoDownloadSettings
    {
        string NiconicoId { get; }
        string FolderName { get; }
        string FileNameFormat { get; }
        bool IsOverwriteEnable { get; }
        bool IsAutoDisposingEnable { get; }
        uint VerticalResolution { get; }
    }

    /// <summary>
    /// 外部から触るAPIを提供する
    /// </summary>
    public class VideoDownloader : IVideoDownloader
    {
        public VideoDownloader(IWatchSession session, ILogger logger, IVideoDownloadHelper videoDownloadHelper, IDownloadMessenger messenger, IVideoEncoader encorder, INiconicoUtils utils, IVideoFileStorehandler fileStorehandler)
        {
            this.session = session;
            this.logger = logger;
            this.videoDownloadHelper = videoDownloadHelper;
            this.messenger = messenger;
            this.encorder = encorder;
            this.utils = utils;
            this.fileStorehandler = fileStorehandler;
        }

        ~VideoDownloader()
        {
            if (!this.session.IsSessionExipired) this.session.Dispose();
        }

        public async Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> onMessage, IWatchSession session, CancellationToken token)
        {
            this.messenger.AddHandler(onMessage);

            if (session.IsSessionExipired)
            {
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Issucceeded = false, Message = "セッションが失効済のためダウンロード出来ません。" };
            }


            if (!session.IsSessionEnsured)
            {
                if (token.IsCancellationRequested)
                {
                    this.messenger.SendMessage("ダウンロードをキャンセル");
                    this.messenger.RemoveHandler(onMessage);
                    return this.GetCancelledResult();
                }
                await session.EnsureSessionAsync(settings.NiconicoId, true);

                if (!session.IsSessionEnsured)
                {
                    string message = session.State switch
                    {
                        WatchSessionState.HttpRequestFailure => "視聴ページの取得に失敗しました。",
                        WatchSessionState.PageAnalyzingFailure => "視聴ページの解析に失敗しました。",
                        WatchSessionState.EncryptedVideo => "暗号化された動画のため、ダウンロードできません。",
                        WatchSessionState.SessionEnsuringFailure => "セッションの確立に失敗しました。",
                        _ => "不明なエラーにより、セッションの確立に失敗しました。"
                    };

                    this.messenger.SendMessage(message);
                    this.messenger.RemoveHandler(onMessage);
                    return new DownloadResult() { Issucceeded = false, Message = message };
                }
            }


            string fileName = !settings.FileNameFormat.IsNullOrEmpty() ? this.utils.GetFileName(settings.FileNameFormat, session.Video!.DmcInfo, ".mp4")
                : $"[{session.Video!.Id}]{session.Video!.Title}.mp4"
                ;

            if (token.IsCancellationRequested)
            {
                this.messenger.SendMessage("ダウンロードをキャンセル");
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            var streams = await session.GetAvailableStreamsAsync();

            var targetStream = streams.GetStream(settings.VerticalResolution);

            if (token.IsCancellationRequested)
            {
                this.messenger.SendMessage("ダウンロードをキャンセル");
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            try
            {
                await this.videoDownloadHelper.DownloadAsync(targetStream, this.messenger, token);

            }
            catch (Exception e)
            {
                this.logger.Error("動画のダウンロード中にエラーが発生しました。", e);
                this.messenger.SendMessage("動画のダウンロードに失敗");
                this.messenger.RemoveHandler(onMessage);
                this.DeleteTmpFolder();
                return new DownloadResult() { Issucceeded = false, Message = $"動画のダウンロード中にエラーが発生しました。(詳細: {e.Message})" };
            }

            if (settings.IsAutoDisposingEnable) session.Dispose();

            if (token.IsCancellationRequested)
            {
                this.DeleteTmpFolder();
                this.messenger.SendMessage("ダウンロードをキャンセル");
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            this.messenger.SendMessage("動画を変換中...");
            var encodeSetting = new EncodeSettings()
            {
                FileName = fileName,
                FolderName = settings.FolderName,
                TsFilePaths = this.videoDownloadHelper.GetAllFileAbsPaths(),
                IsOverwriteEnable = settings.IsOverwriteEnable
            };

            try
            {
                await this.encorder.EncodeAsync(encodeSetting, this.messenger, token);
            }
            catch (TaskCanceledException)
            {
                this.messenger.SendMessage("動画の変換中にキャンセル");
                this.messenger.RemoveHandler(onMessage);
                this.DeleteTmpFolder();
                return new DownloadResult() { Issucceeded = false, Message = $"動画の変換中にキャンセルされました。" };
            }
            catch (Exception e)
            {
                this.logger.Error("ファイルの変換中にエラーが発生しました。", e);
                this.messenger.SendMessage("動画の変換中にエラー発生");
                this.messenger.RemoveHandler(onMessage);
                this.DeleteTmpFolder();
                return new DownloadResult() { Issucceeded = false, Message = $"ファイルの変換中にエラーが発生しました。(詳細: {e.Message})" };
            }
            this.messenger.SendMessage("動画の変換が完了");

            this.DeleteTmpFolder();

            this.fileStorehandler.Add(settings.NiconicoId, Path.Combine(settings.FolderName, fileName));

            this.messenger.RemoveHandler(onMessage);
            return new DownloadResult() { Issucceeded = true, VideoFileName = fileName, VerticalResolution = targetStream.Resolution!.Vertical };
        }

        public async Task<IDownloadResult> DownloadVideoAsync(IVideoDownloadSettings settings, Action<string> onMessage, CancellationToken token)
        {
            return await this.DownloadVideoAsync(settings, onMessage, this.session, token);
        }

        /// <summary>
        /// 一時フォルダーを削除
        /// </summary>
        private void DeleteTmpFolder()
        {
            string folderPath = this.videoDownloadHelper.FolderName;
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

        public VideoDownloadHelper(INicoHttp http, ISegmentWriter writer, IDownloadTaskHandler taskHandler, ILogger logger)
        {
            //最大並列ダウンロード数(既定:5)
            const int maxParallelDownloadCount = 5;

            this.maxParallelDownloadCount = maxParallelDownloadCount;
            this.http = http;
            this.writer = writer;
            this.taskHandler = taskHandler;
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

        /// <summary>
        /// タスクハンドラ
        /// </summary>
        private readonly IDownloadTaskHandler taskHandler;

        private readonly ILogger logger;

        private int currentParallelDownloadCount;

        private readonly int maxParallelDownloadCount;

        /// <summary>
        /// 非同期でダウンロードする
        /// </summary>
        /// <param name="taskHandler"></param>
        /// <returns></returns>
        public Task DownloadAsync(IStreamInfo stream, IDownloadMessenger messenger, CancellationToken token)
        {
            var taskCompletionSource = new TaskCompletionSource();

            var tasks = this.taskHandler.ConvertToTasks(stream);
            this.taskHandler.AddTasks(tasks);

            for (var i = 0; i < this.maxParallelDownloadCount; i++)
            {
                if (token.IsCancellationRequested)
                {
                    if (!taskCompletionSource.Task.IsCompleted) taskCompletionSource.SetResult();
                    break;
                }
                var _ = this.DownloadAndNextAsync(taskHandler, taskCompletionSource, messenger, token);
            }

            return taskCompletionSource.Task;
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
        /// DownloadTaskの内容を取得して可能であれば次のタスクを取得する
        /// </summary>
        /// <param name="taskHandler"></param>
        /// <param name="taskCompletionSource"></param>
        /// <returns></returns>
        private async Task DownloadAndNextAsync(IDownloadTaskHandler taskHandler, TaskCompletionSource taskCompletionSource, IDownloadMessenger messenger, CancellationToken token)
        {

            if (!taskHandler.IsCompleted && !token.IsCancellationRequested)
            {
                ++this.currentParallelDownloadCount;
                var task = taskHandler.RetrieveNextTask();
                byte[] data;

                if ((task.SequenceZero + 1) > this.maxParallelDownloadCount)
                {
                    this.isSleeping = true;
                    await Task.Delay(1 * 1000, token);
                    this.isSleeping = false;
                }

                try
                {
                    data = await this.DownloadInternalAsync(task.Url);
                }
                catch (Exception e)
                {
                    if (!taskCompletionSource.Task.IsCompleted || !taskCompletionSource.Task.IsCanceled)
                    {
                        taskCompletionSource.SetException(e);
                    }
                    this.logger.Error($"セグメント(idx:{task.SequenceZero})の取得に失敗しました。");
                    return;
                }

                this.logger.Log($"セグメント(idx:{task.SequenceZero})を取得しました。");

                if (token.IsCancellationRequested)
                {
                    if (!taskCompletionSource.Task.IsCompleted) taskCompletionSource.SetResult();
                    return;
                }


                try
                {
                    this.writer.Write(data, task);
                }
                catch (Exception e)
                {
                    if (!taskCompletionSource.Task.IsCompleted) taskCompletionSource.SetException(e);
                    return;
                }

                task.Completed = true;
                messenger.SendMessage($"完了: {taskHandler.CurrentCompletedTasksCount}/{taskHandler.OriginalTasksCount}");

                while (this.CanDownloadNext(taskHandler, token))
                {
                    var _ = this.DownloadAndNextAsync(taskHandler, taskCompletionSource, messenger, token);
                }

                --this.currentParallelDownloadCount;
            }

            if (!taskCompletionSource.Task.IsCompleted && (taskHandler.IsCompleted || token.IsCancellationRequested))
            {
                taskCompletionSource.SetResult();
            }

        }

        /// <summary>
        /// ダウンロード可能かどうかを取得する
        /// </summary>
        /// <returns></returns>
        private bool CanDownloadNext(IDownloadTaskHandler taskHandler, CancellationToken token)
        {
            return !this.isSleeping && !token.IsCancellationRequested && this.currentParallelDownloadCount <= this.maxParallelDownloadCount && taskHandler.HasNextTask;
        }

        /// <summary>
        /// フォルダー名
        /// </summary>
        public string FolderName => this.writer.FolderNameAbs;

        /// <summary>
        /// スリープフラグ
        /// </summary>
        private bool isSleeping;

        private async Task<byte[]> DownloadInternalAsync(string url, int retryAttempt = 0)
        {
            var res = await this.http.GetAsync(new Uri(url));
            if (!res.IsSuccessStatusCode)
            {
                //リトライ回数は3回
                if (retryAttempt < 3)
                {
                    retryAttempt++;
                    await Task.Delay(10 * 1000);
                    return await this.DownloadInternalAsync(url, retryAttempt);
                }
                else
                {
                    throw new HttpRequestException($"セグメントファイルの取得に失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase}, url: {url})");
                }
            }

            return await res.Content.ReadAsByteArrayAsync();
        }
    }

    /// <summary>
    /// ダウンロードタスクを管理する
    /// </summary>
    public class DownloadTaskHandler : IDownloadTaskHandler
    {
        public IDownloadTask RetrieveNextTask()
        {
            if (!this.HasNextTask) throw new InvalidOperationException("タスクが登録されていません。");
            var task = this.innerTasks.First();
            this.innerTasks.RemoveAt(0);
            return task;
        }

        /// <summary>
        /// ストリーム情報をタスクに変換する
        /// </summary>
        /// <param name="streamInfo"></param>
        /// <returns></returns>
        public IEnumerable<IDownloadTask> ConvertToTasks(IStreamInfo streamInfo)
        {
            var tasks = new List<IDownloadTask>();
            foreach (var url in streamInfo.StreamUrls)
            {
                var task = new DownloadTask(url.AbsoluteUrl, url.SequenceZero, url.FileName);
                tasks.Add(task);
            }
            return tasks;
        }


        /// <summary>
        /// タスクを追加する
        /// </summary>
        /// <param name="tasks"></param>
        public void AddTask(IDownloadTask task)
        {
            task.PostCompletedCallback = instance =>
            {
                this.innerTasks.RemoveAll(t => t.SequenceZero == task.SequenceZero);
                this.CurrentCompletedTasksCount++;
            };
            this.innerTasks.Add(task);
            this.OriginalTasksCount = this.CurrentTasksCount;
            this.CurrentCompletedTasksCount = 0;
        }

        /// <summary>
        /// 複数のタスクを追加する
        /// </summary>
        /// <param name="tasks"></param>
        public void AddTasks(IEnumerable<IDownloadTask> tasks)
        {
            foreach (var task in tasks)
            {
                this.AddTask(task);
            }
        }

        /// <summary>
        /// 元のタスク数
        /// </summary>
        public int OriginalTasksCount { get; private set; }

        /// <summary>
        /// タスク数
        /// </summary>
        public int CurrentTasksCount => this.innerTasks.Count;

        /// <summary>
        /// 完了したタスク数
        /// </summary>
        public int CurrentCompletedTasksCount { get; private set; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        public bool IsCompleted => this.OriginalTasksCount == this.CurrentCompletedTasksCount;

        /// <summary>
        /// 次のタスクの存在を確認
        /// </summary>
        public bool HasNextTask => this.CurrentTasksCount > 0;

        /// <summary>
        /// 内部でデータを保持する
        /// </summary>
        private readonly List<IDownloadTask> innerTasks = new();
    }

    /// <summary>
    /// タスク
    /// </summary>
    public class DownloadTask : IDownloadTask
    {
        public DownloadTask(string url, int sequenceZero, string filename)
        {
            this.Url = url;
            this.SequenceZero = sequenceZero;
            this.FileName = filename;
        }

        public string Url { get; init; }

        public string FileName { get; init; }

        /// <summary>
        /// ストリームのインデックス
        /// </summary>
        public int SequenceZero { get; init; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        public bool Completed
        {
            get => this.completedField;
            set
            {
                if (value)
                {
                    if (this.PostCompletedCallback is not null)
                    {
                        this.PostCompletedCallback(this);
                    }
                }
                this.IsDownloading = false;
                this.completedField = value;
            }
        }

        /// <summary>
        /// ダウンロード失敗
        /// </summary>
        public bool IsFailed { get; set; }

        /// <summary>
        /// コールバック
        /// </summary>
        public Action<IDownloadTask>? PostCompletedCallback { get; set; }

        private bool completedField;

        /// <summary>
        /// ダウンロード中フラグ
        /// </summary>
        public bool IsDownloading { get; set; }

    }


    /// <summary>
    /// ダウンロード設定
    /// </summary>
    public class VideoDownloadSettings : IVideoDownloadSettings
    {
        public string NiconicoId { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public bool IsOverwriteEnable { get; set; }

        public bool IsAutoDisposingEnable { get; set; }

        public uint VerticalResolution { get; set; }
    }
}
