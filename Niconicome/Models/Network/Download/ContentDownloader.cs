using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Cdl = Niconicome.Models.Domain.Niconico.Download.Comment;
using DDL = Niconicome.Models.Domain.Niconico.Download.Description;
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
        bool DownloadVideoInfo { get; }
        bool IsReplaceStrictedEnable { get; }
        bool OverrideVideoFileDateToUploadedDT { get; }
        bool ResumeEnable { get; }
        bool EnableUnsafeCommentHandle { get; }
        string NiconicoId { get; }
        string FolderPath { get; }
        uint VerticalResolution { get; }
        int PlaylistID { get; }
        int MaxCommentsCount { get; }
        Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(string filenameFormat, bool autodispose, int maxParallelDLCount);
        Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting(string fileFormat);
        Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(string fileFormat, int commentOffset);
        DDL::IDescriptionSetting ConvertToDescriptionDownloadSetting(string fileFormat, bool dlInJson);
    }

    public interface IDownloadResult
    {
        bool IsSucceeded { get; }
        bool IsCanceled { get; }
        string? Message { get; }
        string? VideoFileName { get; }
        IListVideoInfo VideoInfo { get; }
        uint VideoVerticalResolution { get; }
    }

    public interface IVideoToDownload
    {
        IListVideoInfo Video { get; }
        int Index { get; }
    }

    public interface ILocalContentHandler
    {
        ILocalContentInfo GetLocalContentInfo(string folderPath, string format, IDmcInfo dmcInfo, bool replaceStricted, bool downloadInJson);
        IDownloadResult MoveDownloadedFile(string niconicoId, string downloadedFilePath, string destinationPath);
    }

    public interface ILocalContentInfo
    {
        bool CommentExist { get; init; }
        bool ThumbExist { get; init; }
        bool VideoExist { get; init; }
        bool VIdeoExistInOnotherFolder { get; init; }
        bool VideoInfoExist { get; init; }
        string? LocalPath { get; init; }
    }

    /// <summary>
    /// DL実処理
    /// </summary>
    class ContentDownloader : IContentDownloader
    {

        public ContentDownloader(ILocalSettingHandler settingHandler, ILogger logger, IMessageHandler messageHandler, IVideoHandler videoHandler, IDownloadTasksHandler downloadTasksHandler, IVideoListContainer videoListContainer,IContentDownloadHelper downloadHelper)
        {
            this.settingHandler = settingHandler;
            this.logger = logger;
            this.videoHandler = videoHandler;
            this.messageHandler = messageHandler;
            this.downloadTasksHandler = downloadTasksHandler;
            this.videoListContainer = videoListContainer;
            this.downloadHelper = downloadHelper;

            int maxParallel = this.settingHandler.GetIntSetting(SettingsEnum.MaxParallelDL);
            if (maxParallel <= 0)
            {
                maxParallel = 3;
            }
            this.parallelTasksHandler = new ParallelTasksHandler<DownloadTaskParallel>(maxParallel, 5, 15);
            this.downloadTasksHandler.DownloadTaskPool.TaskPoolChange += this.DownloadTaskPoolChangedEventHandler;
        }

        #region フィールド
        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IMessageHandler messageHandler;

        private readonly IDownloadTasksHandler downloadTasksHandler;

        private readonly IVideoListContainer videoListContainer;

        private readonly IContentDownloadHelper downloadHelper;

        private readonly ParallelTasksHandler<DownloadTaskParallel> parallelTasksHandler;
        #endregion

        public INetworkResult? CurrentResult { get; private set; }

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="setting"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<INetworkResult?> DownloadVideos()
        {
            this.CurrentResult = new NetworkResult();

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
                this.logger.Error("ダウンロード中にエラーが発生しました", e);
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

        #region private

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

            var t = new DownloadTaskParallel(async (parallelTask, lockObj, pToken) =>
            {
                if (!e.Task.IsCanceled && !e.Task.IsDone)
                {

                    var setting = e.Task.DownloadSettings;
                    var task = e.Task;
                    IListVideoInfo? video;
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

                    var downloadResult = await this.downloadHelper.TryDownloadContentAsync(setting with { NiconicoId = task.NiconicoID, Video = !skippedFlag && setting.Video }, msg => task.Message = msg, e.Task.CancellationToken);

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
                        if (video is not null && downloadResult.VideoInfo is not null)
                        {
                            if (!downloadResult.VideoFileName.IsNullOrEmpty())
                            {
                                video.FileName.Value = downloadResult.VideoFileName;
                            }
                            NonBindableListVideoInfo.SetData(video, downloadResult.VideoInfo);
                            this.videoHandler.Update(video);
                        }
                        this.videoListContainer.Uncheck(task.VideoID, task.PlaylistID);
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

        #endregion
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

        public IListVideoInfo VideoInfo { get; set; } = new NonBindableListVideoInfo();

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

        public bool DownloadVideoInfo { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }

        public bool OverrideVideoFileDateToUploadedDT { get; set; }

        public bool ResumeEnable { get; set; }

        public bool EnableUnsafeCommentHandle { get; set; }

        public uint VerticalResolution { get; set; }

        public int PlaylistID { get; set; }

        public int MaxCommentsCount { get; set; }

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
                IsResumeEnable = this.ResumeEnable,
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
                MaxcommentsCount = this.MaxCommentsCount,
                IsUnsafeHandleEnable = this.EnableUnsafeCommentHandle,
            };
        }

        public DDL::IDescriptionSetting ConvertToDescriptionDownloadSetting(string fileFormat, bool dlInJson)
        {
            return new DDL::DescriptionSetting()
            {
                IsOverwriteEnable = this.Overwrite,
                FolderName = this.FolderPath,
                IsReplaceRestrictedEnable = this.IsReplaceStrictedEnable,
                Format = fileFormat,
                IsSaveInJsonEnabled = dlInJson,
            };
        }


    }

    /// <summary>
    /// DL処理の単位
    /// </summary>
    public class DownloadTaskParallel : IParallelTask<DownloadTaskParallel>
    {
        public DownloadTaskParallel(Func<DownloadTaskParallel, object, IParallelTaskToken, Task> taskFunction, Action<int> onwait)
        {
            this.TaskFunction = taskFunction;
            this.OnWait = onwait;
        }

        public Guid TaskId { get; init; }

        public Func<DownloadTaskParallel, object, IParallelTaskToken, Task> TaskFunction { get; init; }

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
        public ILocalContentInfo GetLocalContentInfo(string folderPath, string format, IDmcInfo dmcInfo, bool replaceStricted, bool downloadInJson)
        {
            string videoFIlename = this.niconicoUtils.GetFileName(format, dmcInfo, ".mp4", replaceStricted);
            string commentFIlename = this.niconicoUtils.GetFileName(format, dmcInfo, ".xml", replaceStricted);
            string thumbFIlename = this.niconicoUtils.GetFileName(format, dmcInfo, ".jpg", replaceStricted);
            string videoInfoFilename = this.niconicoUtils.GetFileName(format, dmcInfo, downloadInJson ? ".json" : ".txt", replaceStricted);
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
                VideoInfoExist = File.Exists(Path.Combine(folderPath, videoInfoFilename)),
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

        public bool VideoInfoExist { get; init; }

        public string? LocalPath { get; init; }
    }
}
