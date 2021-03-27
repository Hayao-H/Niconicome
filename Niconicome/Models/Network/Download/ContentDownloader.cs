using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Utils;
using Cdl = Niconicome.Models.Domain.Niconico.Download.Comment;
using Download = Niconicome.Models.Domain.Niconico.Download;
using Tdl = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using Vdl = Niconicome.Models.Domain.Niconico.Download.Video;

namespace Niconicome.Models.Network.Download
{
    public interface IContentDownloader
    {
        Task<INetworkResult?> DownloadVideos();
        Task<INetworkResult?> DownloadVideosFriendly(Action<string> onMessage, Action<string> onMessageShort);
        bool CanDownload { get; }
        void Cancel();
        event EventHandler? CanDownloadChange;
    }

    public interface IDownloadSettings
    {
        bool Video { get; }
        bool Comment { get; }
        bool Thumbnail { get; }
        bool Overwrite { get; }
        bool FromAnotherFolder { get; }
        bool Skip { get; }
        bool DownloadEasy { get; }
        bool DownloadLog { get; }
        bool DownloadOwner { get; }
        bool IsReplaceStrictedEnable { get; }
        bool OverrideVideoFileDateToUploadedDT { get; }
        string NiconicoId { get; }
        string FolderPath { get; }
        uint VerticalResolution { get; }
        int PlaylistID { get; }
        Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(string filenameFormat, bool autodispose, int maxParallelDLCount);
        Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting(string fileFormat);
        Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(string fileFormat, int commentOffset);
    }

    public interface IDownloadResult
    {
        bool IsSucceeded { get; }
        bool IsCanceled { get; }
        string? Message { get; }
        string? VideoFileName { get; }
        uint VideoVerticalResolution { get; }
    }

    public interface IVideoToDownload
    {
        IVideoListInfo Video { get; }
        int Index { get; }
    }

    public interface ILocalContentHandler
    {
        ILocalContentInfo GetLocalContentInfo(string folderPath, string format, IDmcInfo dmcInfo, bool replaceStricted);
        IDownloadResult MoveDownloadedFile(string niconicoId, string downloadedFilePath, string destinationPath);
    }

    public interface ILocalContentInfo
    {
        bool CommentExist { get; init; }
        bool ThumbExist { get; init; }
        bool VideoExist { get; init; }
        bool VIdeoExistInOnotherFolder { get; init; }
        string? LocalPath { get; init; }
    }

    /// <summary>
    /// DL実処理
    /// </summary>
    class ContentDownloader : IContentDownloader
    {

        public ContentDownloader(ILocalSettingHandler settingHandler, ILogger logger, IMessageHandler messageHandler, IVideoHandler videoHandler, ILocalContentHandler localContentHandler, IDownloadTasksHandler downloadTasksHandler, ICurrent current)
        {
            this.settingHandler = settingHandler;
            this.logger = logger;
            this.videoHandler = videoHandler;
            this.messageHandler = messageHandler;
            this.localContentHandler = localContentHandler;
            this.downloadTasksHandler = downloadTasksHandler;
            this.current = current;

            int maxParallel = this.settingHandler.GetIntSetting(Settings.MaxParallelDL);
            if (maxParallel <= 0)
            {
                maxParallel = 3;
            }
            this.parallelTasksHandler = new ParallelTasksHandler<DownloadTaskParallel>(maxParallel, 5, 15);
            this.downloadTasksHandler.DownloadTaskPool.TaskPoolChange += this.DownloadTaskPoolChangedEventHandler;
        }

        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IMessageHandler messageHandler;

        private readonly ILocalContentHandler localContentHandler;

        private readonly IDownloadTasksHandler downloadTasksHandler;

        private readonly ICurrent current;

        private readonly ParallelTasksHandler<DownloadTaskParallel> parallelTasksHandler;

        public INetworkResult? CurrentResult { get; private set; }

        /// <summary>
        /// 非同期で動画をダウンロードする
        /// </summary>
        /// <param name="niconicoid"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadVideoAsync(IDownloadSettings settings, Action<string> onMessage, IWatchSession session, IDownloadContext context, CancellationToken token)
        {
            string fileNameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
            int maxParallel = this.settingHandler.GetIntSetting(Settings.MaxParallelSegDl);
            if (maxParallel <= 0)
            {
                maxParallel = 1;
            }
            else if (maxParallel > 5)
            {
                maxParallel = 5;
            }

            var vSettings = settings.ConvertToVideoDownloadSettings(fileNameFormat, false, maxParallel);
            var videoDownloader = DIFactory.Provider.GetRequiredService<Vdl::IVideoDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await videoDownloader.DownloadVideoAsync(vSettings, onMessage, session, context, token);
            }
            catch (Exception e)
            {
                this.logger.Error("動画のダウンロードに失敗しました。", e);
                return new DownloadResult() { IsSucceeded = false, Message = e.Message };
            }
            return new DownloadResult() { IsSucceeded = result.Issucceeded, Message = result.Message ?? null, VideoFileName = result.VideoFileName, VideoVerticalResolution = result.VerticalResolution };
        }

        /// <summary>
        /// サムネイルをダウンロードする
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadThumbAsync(IDownloadSettings setting, IWatchSession session)
        {
            string fileNameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
            var tSettings = setting.ConvertToThumbDownloadSetting(fileNameFormat);
            var thumbDownloader = DIFactory.Provider.GetRequiredService<Tdl::IThumbDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await thumbDownloader.DownloadThumbnailAsync(tSettings, session);
            }
            catch (Exception e)
            {
                this.logger.Error("サムネイルのダウンロードに失敗しました。", e);
                return new DownloadResult() { IsSucceeded = false, Message = e.Message };
            }
            return new DownloadResult() { IsSucceeded = result.Issucceeded, Message = result.Message ?? null };
        }

        /// <summary>
        /// コメントをダウンロードする
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="session"></param>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadCommentAsync(IDownloadSettings settings, IWatchSession session, Action<string> onMessage, IDownloadContext context, CancellationToken token)
        {
            var cOffset = this.settingHandler.GetIntSetting(Settings.CommentOffset);
            var autoSwicth = this.settingHandler.GetBoolSetting(Settings.SwitchOffset);

            if (cOffset < 0) cOffset = Cdl::CommentCollection.NumberToThrough;
            if (autoSwicth && session.Video!.DmcInfo.IsOfficial) cOffset = 0;

            string fileNameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
            var cSettings = settings.ConvertToCommentDownloadSetting(fileNameFormat, cOffset);
            var commentDownloader = DIFactory.Provider.GetRequiredService<Cdl::ICommentDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await commentDownloader.DownloadComment(session, cSettings, onMessage, context, token);
            }
            catch (Exception e)
            {
                this.logger.Error("コメントのダウンロードに失敗しました。", e);
                return new DownloadResult() { IsSucceeded = false, Message = e.Message };
            }
            return new DownloadResult() { IsSucceeded = result.Issucceeded, Message = result.Message ?? null };
        }

        /// <summary>
        /// 非同期でコンテンツをダウンロードする
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="OnMessage"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadContentAsync(IDownloadSettings setting, Action<string> OnMessage, CancellationToken token)
        {
            var result = new DownloadResult();
            var context = new DownloadContext(setting.NiconicoId);
            var session = DIFactory.Provider.GetRequiredService<IWatchSession>();

            await session.GetVideoDataAsync(setting.NiconicoId, setting.Video);

            if (session.Video?.DmcInfo.DownloadStartedOn is not null)
            {
                session.Video.DmcInfo.DownloadStartedOn = DateTime.Now;
            }

            if (session.State != WatchSessionState.GotPage || session.Video is null)
            {
                result.IsSucceeded = false;
                result.Message = session.State switch
                {
                    WatchSessionState.PaymentNeeded => "視聴ページの解析に失敗しました。",
                    WatchSessionState.HttpRequestFailure => "視聴ページの取得に失敗しました。",
                    WatchSessionState.PageAnalyzingFailure => "視聴ページの解析に失敗しました。",
                    _ => "不明なエラーにより、視聴ページの取得に失敗しました。"
                };
                return result;
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            ILocalContentInfo? info = null;
            if (setting.Skip)
            {
                string fileNameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
                info = this.localContentHandler.GetLocalContentInfo(setting.FolderPath, fileNameFormat, session.Video.DmcInfo, setting.IsReplaceStrictedEnable);
            }

            if (!Directory.Exists(setting.FolderPath))
            {
                Directory.CreateDirectory(setting.FolderPath);
            }

            //動画

            if (setting.Video)
            {
                if (!info?.VideoExist ?? true)
                {
                    var vResult = await this.TryDownloadVideoAsync(setting, OnMessage, session, context, token);
                    if (!vResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = vResult.Message ?? "None";
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                        result.VideoFileName = vResult.VideoFileName ?? string.Empty;
                        result.VideoVerticalResolution = vResult.VideoVerticalResolution;
                    }
                }
                else if (info?.VideoExist ?? false)
                {
                    OnMessage("動画を保存済みのためスキップしました。");
                    result.IsSucceeded = true;
                }
                else if (setting.FromAnotherFolder && (info?.VIdeoExistInOnotherFolder ?? false) && info?.LocalPath is not null)
                {
                    var vResult = this.localContentHandler.MoveDownloadedFile(setting.NiconicoId, info.LocalPath, setting.FolderPath);
                    if (!vResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = vResult.Message ?? "None";
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                        result.VideoFileName = vResult.VideoFileName ?? string.Empty;
                        OnMessage("別フォルダーに保存済みの動画をコピーしました。");
                    }
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //サムネイル
            if (setting.Thumbnail)
            {
                if (!info?.ThumbExist ?? true)
                {
                    var tResult = await this.TryDownloadThumbAsync(setting, session);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!tResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = tResult.Message ?? "None";
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                    }
                }
                else if (info?.ThumbExist ?? false)
                {
                    OnMessage("サムネイルを保存済みのためスキップしました。");
                    result.IsSucceeded = true;
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //コメント
            if (setting.Comment)
            {
                if (!info?.CommentExist ?? true)
                {

                    var cResult = await this.TryDownloadCommentAsync(setting, session, OnMessage, context, token);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!cResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = cResult.Message ?? "None";
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                    }
                }
                else if (info?.CommentExist ?? false)
                {
                    OnMessage("コメントを保存済みのためスキップしました。");
                    result.IsSucceeded = true;
                }
            }

            if (session.IsSessionEnsured) session.Dispose();

            return result;
        }

        /// <summary>
        /// DLをキャンセルする
        /// </summary>
        /// <returns></returns>
        private IDownloadResult CancelledDownloadAndGetResult()
        {

            return new DownloadResult() { Message = "キャンセルされました。", IsCanceled = true };
        }

        /// <summary>
        /// イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadTaskPoolChangedEventHandler(object? sender, TaskPoolChangeEventargs e)
        {
            if (e.ChangeType != TaskPoolChangeType.Add)
            {
                return;
            }

            if (this.CurrentResult is null)
            {
                this.CurrentResult = new NetworkResult();
            }

            var t = new DownloadTaskParallel(async (parallelTask, lockObj) =>
            {
                if (!e.Task.IsCanceled && !e.Task.IsDone)
                {

                    var setting = e.Task.DownloadSettings;
                    var task = e.Task;
                    IVideoListInfo? video;
                    if (this.videoHandler.Exist(task.VideoID))
                    {
                        video = this.videoHandler.GetVideo(task.VideoID);
                    }
                    else
                    {
                        video = null;
                    }

                    this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロード処理を開始しました。");

                    string folderPath = setting.FolderPath;
                    bool skippedFlag = false;
                    e.Task.IsProcessing = true;

                    var downloadResult = await this.TryDownloadContentAsync(setting with { NiconicoId = task.NiconicoID, Video = !skippedFlag && setting.Video }, msg => task.Message = msg, e.Task.CancellationToken);

                    if (downloadResult.IsCanceled)
                    {
                        this.CurrentResult.FailedCount++;
                        this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロードがキャンセルされました。");
                        task.Message = "ダウンロードをキャンセル";
                    }
                    else if (!downloadResult.IsSucceeded)
                    {
                        this.CurrentResult.FailedCount++;
                        this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロードに失敗しました。");
                        this.messageHandler.AppendMessage($"詳細: {downloadResult.Message}");
                    }
                    else
                    {
                        string rMessage = downloadResult.VideoVerticalResolution == 0 ? string.Empty : $"(vertical:{downloadResult.VideoVerticalResolution}px)";
                        this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロードに成功しました。");

                        task.Message = $"ダウンロード完了{rMessage}";
                        if (!downloadResult.VideoFileName.IsNullOrEmpty() && video is not null)
                        {
                            video.FileName = downloadResult.VideoFileName;
                            this.videoHandler.Update(video);
                        }
                        this.current.Uncheck(task.PlaylistID, task.VideoID);
                        this.CurrentResult.SucceededCount++;
                    }

                    if (this.CurrentResult.FirstVideo is null)
                    {
                        this.CurrentResult.FirstVideo = video;
                    }

                    e.Task.IsProcessing = false;
                    if (!e.Task.IsCanceled)
                    {
                        e.Task.IsDone = true;
                    }
                    //this.downloadTasksHandler.DownloadTaskPool.RemoveTask(e.Task);

                }
                else
                {
                    this.parallelTasksHandler.CancellAllTasks();
                    //this.downloadTasksHandler.DownloadTaskPool.Clear();
                }
            }, _ =>
            {
                e.Task.Message = "待機中...(15s)";
                this.messageHandler.AppendMessage($"待機中...(15s)");

            });

            this.parallelTasksHandler.AddTaskToQueue(t);

            if (!this.parallelTasksHandler.IsProcessing)
            {
            }
        }

        /// <summary>
        /// DL可能フラグ変更イベントを発火させる
        /// </summary>
        private void RaiseCanDownloadChange()
        {
            this.CanDownloadChange?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="setting"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<INetworkResult?> DownloadVideos()
        {

            if (this.parallelTasksHandler.IsProcessing)
            {
                return new NetworkResult();
            }

            await this.parallelTasksHandler.ProcessTasksAsync(() => this.RaiseCanDownloadChange());

            this.RaiseCanDownloadChange();

            return this.CurrentResult;

        }

        /// <summary>
        /// メッセージを表示しながらDL
        /// </summary>
        /// <param name="onMessage"></param>
        /// <param name="onMessageShort"></param>
        /// <returns></returns>
        public async Task<INetworkResult?> DownloadVideosFriendly(Action<string> onMessage, Action<string> onMessageShort)
        {
            var videoCount = this.parallelTasksHandler.PallarelTasks.Count;

            onMessage($"動画のダウンロードを開始します。({videoCount}件)");
            onMessageShort($"動画のダウンロードを開始します。({videoCount}件)");

            INetworkResult? result = null;

            try
            {
                result = await this.DownloadVideos();

            }
            catch (Exception e)
            {
                onMessage($"ダウンロード中にエラーが発生しました。(詳細: {e.Message})");
                onMessageShort($"ダウンロード中にエラーが発生しました。");
            }

            if (result?.SucceededCount > 1)
            {
                if (result?.FirstVideo is not null)
                {
                    onMessage($"{result.FirstVideo.NiconicoId}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");
                    onMessageShort($"{result.FirstVideo.NiconicoId}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");

                    if (!result.IsSucceededAll)
                    {
                        onMessage($"{result.FailedCount}件の動画のダウンロードに失敗しました。");
                    }
                }
            }
            else if (result?.SucceededCount == 1)
            {
                if (result?.FirstVideo is not null)
                {
                    onMessage($"{result.FirstVideo.NiconicoId}をダウンロードしました。");
                    onMessageShort($"{result.FirstVideo.NiconicoId}をダウンロードしました。");
                }
            }
            else
            {
                onMessage($"ダウンロード出来ませんでした。");
                onMessageShort($"ダウンロード出来ませんでした。");
            }

            return result;
        }


        /// <summary>
        /// DL可能フラグ
        /// </summary>
        public bool CanDownload { get => !this.parallelTasksHandler.IsProcessing; }

        /// <summary>
        /// DL可能フラグ変更イベント
        /// </summary>
        public event EventHandler? CanDownloadChange;

        /// <summary>
        /// ダウンロードをキャンセル
        /// </summary>
        public void Cancel()
        {
            this.parallelTasksHandler.CancellAllTasks();
            this.downloadTasksHandler.DownloadTaskPool.Clear();
        }

    }

    /// <summary>
    /// DL結果
    /// </summary>
    class DownloadResult : IDownloadResult
    {
        public bool IsSucceeded { get; set; }
        public bool IsCanceled { get; set; }

        public string? Message { get; set; }
        public string? VideoFileName { get; set; }

        public uint VideoVerticalResolution { get; set; }


    }

    /// <summary>
    /// DL設定
    /// </summary>
    public record DownloadSettings : IDownloadSettings
    {
        public bool Video { get; set; }

        public bool Comment { get; set; }

        public bool Thumbnail { get; set; }

        public bool Overwrite { get; set; }

        public bool FromAnotherFolder { get; set; }

        public bool Skip { get; set; }

        public bool DownloadEasy { get; set; }

        public bool DownloadLog { get; set; }

        public bool DownloadOwner { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }

        public bool OverrideVideoFileDateToUploadedDT { get; set; }

        public uint VerticalResolution { get; set; }

        public int PlaylistID { get; set; }

        public string NiconicoId { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;

        public Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(string filenameFormat, bool autodispose, int maxParallelDLCount)
        {
            return new Vdl::VideoDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FileNameFormat = filenameFormat,
                FolderName = this.FolderPath,
                IsAutoDisposingEnable = autodispose,
                IsOverwriteEnable = this.Overwrite,
                VerticalResolution = this.VerticalResolution,
                MaxParallelDownloadCount = maxParallelDLCount,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
                IsOvwrridingFileDTEnable = this.OverrideVideoFileDateToUploadedDT,
            };
        }

        public Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting(string fileFormat)
        {
            return new Tdl::ThumbDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FolderName = this.FolderPath,
                FileNameFormat = fileFormat,
                IsOverwriteEnable = this.Overwrite,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
            };
        }

        public Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(string fileFormat, int commentOffset)
        {
            return new Cdl::CommentDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FolderName = this.FolderPath,
                FileNameFormat = fileFormat,
                IsOverwriteEnable = this.Overwrite,
                IsDownloadingEasyCommentEnable = this.DownloadEasy,
                IsDownloadingLogEnable = this.DownloadLog,
                IsDownloadingOwnerCommentEnable = this.DownloadOwner,
                CommentOffset = commentOffset,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
            };
        }

    }

    /// <summary>
    /// インデックス付きの動画情報
    /// </summary>
    public class VideoToDownload : IVideoToDownload
    {
        public VideoToDownload(IVideoListInfo video, int index)
        {
            this.Video = video;
            this.Index = index;
        }

        public IVideoListInfo Video { get; set; }

        public int Index { get; set; }
    }

    /// <summary>
    /// DL処理の単位
    /// </summary>
    public class DownloadTaskParallel : IParallelTask<DownloadTaskParallel>
    {
        public DownloadTaskParallel(Func<DownloadTaskParallel, object, Task> taskFunction, Action<int> onwait)
        {
            this.TaskFunction = taskFunction;
            this.OnWait = onwait;
        }

        public Guid TaskId { get; init; }

        public Func<DownloadTaskParallel, object, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }

    }

    /// <summary>
    /// ローカルデータの処理
    /// </summary>
    public class LocalContentHandler : ILocalContentHandler
    {
        public LocalContentHandler(INiconicoUtils niconicoUtils, IVideoFileStorehandler videoFileStorehandler, ILogger logger)
        {
            this.niconicoUtils = niconicoUtils;
            this.videoFileStorehandler = videoFileStorehandler;
            this.logger = logger;
        }

        private readonly INiconicoUtils niconicoUtils;

        private readonly IVideoFileStorehandler videoFileStorehandler;

        private readonly ILogger logger;

        /// <summary>
        /// ローカルの保存状況を取得する
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="format"></param>
        /// <param name="dmcInfo"></param>
        /// <returns></returns>
        public ILocalContentInfo GetLocalContentInfo(string folderPath, string format, IDmcInfo dmcInfo, bool replaceStricted)
        {
            string videoFIlename = this.niconicoUtils.GetFileName(format, dmcInfo, ".mp4", replaceStricted);
            string commentFIlename = this.niconicoUtils.GetFileName(format, dmcInfo, ".xml", replaceStricted);
            string thumbFIlename = this.niconicoUtils.GetFileName(format, dmcInfo, ".jpg", replaceStricted);
            bool videoExist = this.videoFileStorehandler.Exists(dmcInfo.Id);
            string? localPath = null;

            if (videoExist)
            {
                localPath = this.videoFileStorehandler.GetFilePath(dmcInfo.Id);
            }

            return new LocalContentInfo()
            {
                VideoExist = File.Exists(Path.Combine(folderPath, videoFIlename)),
                CommentExist = File.Exists(Path.Combine(folderPath, commentFIlename)),
                ThumbExist = File.Exists(Path.Combine(folderPath, thumbFIlename)),
                VIdeoExistInOnotherFolder = videoExist,
                LocalPath = localPath,
            };
        }

        /// <summary>
        /// ダウンロード済のファイルをコピーする
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="downloadedFilePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public IDownloadResult MoveDownloadedFile(string niconicoId, string downloadedFilePath, string destinationPath)
        {
            if (!File.Exists(downloadedFilePath))
            {
                return new DownloadResult() { Message = "そのようなファイルは存在しません。" };
            }

            if (!Directory.Exists(destinationPath))
            {
                try
                {
                    Directory.CreateDirectory(destinationPath);
                }
                catch (Exception e)
                {
                    this.logger.Error("移動先フォルダーの作成に失敗しました。", e);
                    return new DownloadResult() { Message = "移動先フォルダーの作成に失敗しました。" };
                }
            }

            string filename = Path.GetFileName(downloadedFilePath);
            try
            {
                File.Copy(downloadedFilePath, Path.Combine(destinationPath, filename));
            }
            catch (Exception e)
            {
                this.logger.Error("ファイルのコピーに失敗しました。", e);
                return new DownloadResult() { Message = $"ファイルのコピーに失敗しました。" };
            }

            this.videoFileStorehandler.Add(niconicoId, Path.Combine(destinationPath, filename));

            return new DownloadResult() { IsSucceeded = true };

        }

    }

    /// <summary>
    /// ローカル情報
    /// </summary>
    public class LocalContentInfo : ILocalContentInfo
    {
        public LocalContentInfo(string? localPath)
        {
            this.LocalPath = localPath;
        }

        public LocalContentInfo() : this(null) { }

        public bool VideoExist { get; init; }

        public bool VIdeoExistInOnotherFolder { get; init; }

        public bool CommentExist { get; init; }

        public bool ThumbExist { get; init; }

        public string? LocalPath { get; init; }
    }
}
