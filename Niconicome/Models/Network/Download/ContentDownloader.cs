using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Cdl = Niconicome.Models.Domain.Niconico.Download.Comment;
using DDL = Niconicome.Models.Domain.Niconico.Download.Description;
using IDl = Niconicome.Models.Domain.Niconico.Download.Ichiba;
using Tdl = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using Vdl = Niconicome.Models.Domain.Niconico.Download.Video;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Network.Download
{
    public interface IContentDownloader
    {
        Task<INetworkResult?> DownloadVideosFriendly(Action<string> onMessage, Action<string> onMessageShort);
        ReactiveProperty<bool> CanDownload { get; }
        void Cancel();
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
        bool DownloadIchibaInfo { get; }
        bool IsReplaceStrictedEnable { get; }
        bool OverrideVideoFileDateToUploadedDT { get; }
        bool ResumeEnable { get; }
        bool EnableUnsafeCommentHandle { get; }
        bool SaveWithoutEncode { get; }
        string NiconicoId { get; }
        string FolderPath { get; }
        string FileNameFormat { get; }
        string VideoInfoExt { get; }
        string IchibaInfoExt { get; }
        string ThumbnailExt { get; }
        string IchibaInfoSuffix { get; }
        string VideoInfoSuffix { get; }
        uint VerticalResolution { get; }
        int PlaylistID { get; }
        int MaxCommentsCount { get; }
        IchibaInfoTypeSettings IchibaInfoType { get; }
        VideoInfo::ThumbSize ThumbSize { get; }
        Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(bool autodispose, int maxParallelDLCount);
        Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting();
        Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(int commentOffset);
        DDL::IDescriptionSetting ConvertToDescriptionDownloadSetting(bool dlInJson, bool dlInXml, bool dlInText);
        IDl::IIchibaInfoDownloadSettings ConvertToIchibaInfoDownloadSettings();
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

    /// <summary>
    /// DL実処理
    /// </summary>
    class ContentDownloader : BindableBase, IContentDownloader
    {

        public ContentDownloader(ILocalSettingHandler settingHandler, ILogger logger, IMessageHandler messageHandler, IVideoHandler videoHandler, IDownloadTasksHandler downloadTasksHandler, IContentDownloadHelper downloadHelper, IPlaylistHandler playlistHandler, IVideoInfoContainer videoInfoContainer)
        {
            this.settingHandler = settingHandler;
            this.logger = logger;
            this.videoHandler = videoHandler;
            this.messageHandler = messageHandler;
            this.downloadTasksHandler = downloadTasksHandler;
            this.downloadHelper = downloadHelper;
            this.playlistHandler = playlistHandler;
            this.videoInfoContainer = videoInfoContainer;

            int maxParallel = this.settingHandler.GetIntSetting(SettingsEnum.MaxParallelDL);
            var sleepInterval = this.settingHandler.GetIntSetting(SettingsEnum.FetchSleepInterval);
            if (maxParallel <= 0)
            {
                maxParallel = 3;
            }
            if (sleepInterval <= 0)
            {
                sleepInterval = 5;
            }
            this.parallelTasksHandler = new ParallelTasksHandler<DownloadTaskParallel>(maxParallel, sleepInterval, 15, untilEmpty: true);
            this.downloadTasksHandler.DownloadTaskPool.TaskPoolChange += this.DownloadTaskPoolChangedEventHandler;
        }

        #region フィールド
        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IMessageHandler messageHandler;

        private readonly IDownloadTasksHandler downloadTasksHandler;

        private readonly IContentDownloadHelper downloadHelper;

        private readonly IPlaylistHandler playlistHandler;

        private readonly IVideoInfoContainer videoInfoContainer;

        private readonly ParallelTasksHandler<DownloadTaskParallel> parallelTasksHandler;

        private CancellationTokenSource? cts;
        #endregion

        public INetworkResult? CurrentResult { get; private set; }

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="setting"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<INetworkResult?> DownloadVideos()
        {
            this.CurrentResult = new NetworkResult();

            if (this.parallelTasksHandler.IsProcessing)
            {
                return new NetworkResult();
            }

            if (this.cts is null)
            {
                this.cts = new CancellationTokenSource();
            }

            await this.parallelTasksHandler.ProcessTasksAsync(() => this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing, ct: this.cts?.Token ?? CancellationToken.None);

            this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing;

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
                    onMessage($"{result.FirstVideo.NiconicoId.Value}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");
                    onMessageShort($"{result.FirstVideo.NiconicoId.Value}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");

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
                    onMessage($"{result.FirstVideo.NiconicoId.Value}をダウンロードしました。");
                    onMessageShort($"{result.FirstVideo.NiconicoId.Value}をダウンロードしました。");
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
        public ReactiveProperty<bool> CanDownload { get; init; } = new(true);

        /// <summary>
        /// ダウンロードをキャンセル
        /// </summary>
        public void Cancel()
        {
            this.parallelTasksHandler.CancellAllTasks();
            this.downloadTasksHandler.DownloadTaskPool.Clear();
            this.cts?.Cancel();
            this.cts = null;
            this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing;
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
                if (!e.Task.IsCanceled.Value && !e.Task.IsDone.Value)
                {

                    var setting = e.Task.DownloadSettings;
                    IListVideoInfo video = this.videoInfoContainer.GetVideo(e.Task.NiconicoID);

                    this.messageHandler.AppendMessage($"{e.Task.NiconicoID}のダウンロード処理を開始しました。");

                    string folderPath = setting.FolderPath;
                    bool skippedFlag = false;
                    e.Task.IsProcessing.Value = true;

                    IDownloadResult downloadResult = await this.downloadHelper.TryDownloadContentAsync(setting with { NiconicoId = e.Task.NiconicoID, Video = !skippedFlag && setting.Video }, msg => e.Task.Message.Value = msg, e.Task.CancellationToken);


                    if (downloadResult.IsCanceled || !downloadResult.IsSucceeded)
                    {
                        if (downloadResult.IsCanceled)
                        {
                            this.CurrentResult.FailedCount++;
                            this.messageHandler.AppendMessage($"{e.Task.NiconicoID}のダウンロードがキャンセルされました。");
                            e.Task.Message.Value = "ダウンロードをキャンセル";
                        }
                        else
                        {
                            this.CurrentResult.FailedCount++;
                            this.messageHandler.AppendMessage($"{e.Task.NiconicoID}のダウンロードに失敗しました。");
                            this.messageHandler.AppendMessage($"詳細: {downloadResult.Message}");
                        }

                        bool isFailedHIstoryDisabled = this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLFailedHistory);
                        if (video is not null && !isFailedHIstoryDisabled)
                        {
                            ITreePlaylistInfo? playlist = this.playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLFailedHistory);
                            if (playlist is not null)
                            {
                                this.playlistHandler.AddVideo(video, playlist.Id);
                            }
                        }
                    }
                    else
                    {
                        string rMessage = downloadResult.VideoVerticalResolution == 0 ? string.Empty : $"(vertical:{downloadResult.VideoVerticalResolution}px)";
                        this.messageHandler.AppendMessage($"{e.Task.NiconicoID}のダウンロードに成功しました。");

                        if (video is not null && downloadResult.VideoInfo is not null)
                        {
                            if (!downloadResult.VideoFileName.IsNullOrEmpty())
                            {
                                video.FileName.Value = downloadResult.VideoFileName;
                            }
                            video.SetNewData(downloadResult.VideoInfo);
                            this.videoHandler.Update(video);
                        }

                        LightVideoListinfoHandler.GetLightVideoListInfo(e.Task.VideoID, e.Task.PlaylistID).IsSelected.Value = false;
                        this.CurrentResult.SucceededCount++;

                        e.Task.Message.Value = $"ダウンロード完了{rMessage}";

                        bool isSucceededHIstoryDisabled = this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLSucceededHistory);
                        if (video is not null && !isSucceededHIstoryDisabled)
                        {
                            ITreePlaylistInfo? playlist = this.playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLSucceedeeHistory);
                            if (playlist is not null)
                            {
                                this.playlistHandler.AddVideo(video, playlist.Id);
                            }
                        }
                    }

                    if (this.CurrentResult.FirstVideo is null)
                    {
                        this.CurrentResult.FirstVideo = video;
                    }

                    e.Task.IsProcessing.Value = false;
                    if (!e.Task.IsCanceled.Value)
                    {
                        e.Task.IsDone.Value = true;
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
                e.Task.Message.Value = "待機中...(15s)";
                this.messageHandler.AppendMessage($"待機中...(15s)");

            });

            this.parallelTasksHandler.AddTaskToQueue(t);
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

        public bool DownloadIchibaInfo { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }

        public bool OverrideVideoFileDateToUploadedDT { get; set; }

        public bool ResumeEnable { get; set; }

        public bool EnableUnsafeCommentHandle { get; set; }

        public bool SaveWithoutEncode { get; set; }

        public uint VerticalResolution { get; set; }

        public int PlaylistID { get; set; }

        public int MaxCommentsCount { get; set; }

        public string NiconicoId { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public string VideoInfoExt { get; set; } = string.Empty;

        public string IchibaInfoExt { get; set; } = string.Empty;

        public string ThumbnailExt { get; set; } = string.Empty;

        public string IchibaInfoSuffix { get; set; } = string.Empty;

        public string VideoInfoSuffix { get; set; } = string.Empty;

        public string CommandFormat { get; set; } = string.Empty;

        public IchibaInfoTypeSettings IchibaInfoType { get; set; }

        public VideoInfo::ThumbSize ThumbSize { get; set; }

        public Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(bool autodispose, int maxParallelDLCount)
        {
            return new Vdl::VideoDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FileNameFormat = this.FileNameFormat,
                FolderName = this.FolderPath,
                IsAutoDisposingEnable = autodispose,
                IsOverwriteEnable = this.Overwrite,
                VerticalResolution = this.VerticalResolution,
                MaxParallelDownloadCount = maxParallelDLCount,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
                IsOvwrridingFileDTEnable = this.OverrideVideoFileDateToUploadedDT,
                IsResumeEnable = this.ResumeEnable,
                IsNoEncodeEnable = this.SaveWithoutEncode,
                CommandFormat = this.CommandFormat,
            };
        }

        public Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting()
        {
            return new Tdl::ThumbDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FolderName = this.FolderPath,
                FileNameFormat = this.FileNameFormat,
                IsOverwriteEnable = this.Overwrite,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
                Extension = this.ThumbnailExt,
                ThumbSize = this.ThumbSize,
            };
        }

        public Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(int commentOffset)
        {
            return new Cdl::CommentDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FolderName = this.FolderPath,
                FileNameFormat = this.FileNameFormat,
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

        public DDL::IDescriptionSetting ConvertToDescriptionDownloadSetting(bool dlInJson, bool dlInXml, bool dlInText)
        {
            return new DDL::DescriptionSetting()
            {
                IsOverwriteEnable = this.Overwrite,
                FolderName = this.FolderPath,
                IsReplaceRestrictedEnable = this.IsReplaceStrictedEnable,
                Format = this.FileNameFormat,
                IsSaveInJsonEnabled = dlInJson,
                IsSaveInXmlEnabled = dlInXml,
                IsSaveInTextEnabled = dlInText,
                Suffix = this.VideoInfoSuffix,
            };
        }

        public IDl::IIchibaInfoDownloadSettings ConvertToIchibaInfoDownloadSettings()
        {
            return new IDl::IchibaInfoDownloadSettings()
            {
                IsReplacingStrictedEnabled = this.IsReplaceStrictedEnable,
                IsXml = this.IchibaInfoType == IchibaInfoTypeSettings.Xml,
                IsJson = this.IchibaInfoType == IchibaInfoTypeSettings.Json,
                IsHtml = this.IchibaInfoType == IchibaInfoTypeSettings.Html,
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

        public int Index { get; set; }

        public Func<DownloadTaskParallel, object, IParallelTaskToken, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }

    }

}
