using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Cdl = Niconicome.Models.Domain.Niconico.Download.Comment;
using Download = Niconicome.Models.Domain.Niconico.Download;
using Tdl = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using Vdl = Niconicome.Models.Domain.Niconico.Download.Video;

namespace Niconicome.Models.Network
{
    public interface IContentDownloader
    {
        Task<INetworkResult> DownloadVideos(IEnumerable<ITreeVideoInfo> videos, DownloadSettings setting, CancellationToken token);
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
        string NiconicoId { get; }
        string FolderPath { get; }
        uint VerticalResolution { get; }
        Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(string filenameFormat, bool autodispose);
        Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting(string fileFormat);
        Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(string fileFormat);
    }

    public interface IDownloadResult
    {
        bool IsSucceeded { get; }
        string? Message { get; }
        string? VideoFileName { get; }
        uint VideoVerticalResolution { get; }
    }

    public interface IVideoToDownload
    {
        ITreeVideoInfo Video { get; }
        int Index { get; }
    }

    class ContentDownloader : IContentDownloader
    {

        public ContentDownloader(ILocalSettingHandler settingHandler, ILogger logger, IVideoFileStorehandler fileStorehandler, INetworkVideoHandler networkVideoHandler, IMessageHandler messageHandler,IVideoHandler videoHandler)
        {
            this.settingHandler = settingHandler;
            this.logger = logger;
            this.fileStorehandler = fileStorehandler;
            this.networkVideoHandler = networkVideoHandler;
            this.videoHandler = videoHandler;
            this.messageHandler = messageHandler;
        }

        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IVideoFileStorehandler fileStorehandler;

        private readonly IVideoHandler videoHandler;

        private readonly INetworkVideoHandler networkVideoHandler;

        private readonly IMessageHandler messageHandler;


        /// <summary>
        /// 最大並列ダウンロード数
        /// </summary>
        private readonly int maxParallelDownloadingCount = 3;

        private int currentParallelDownloadingCount;

        /// <summary>
        /// 非同期で動画をダウンロードする
        /// </summary>
        /// <param name="niconicoid"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadVideoAsync(IDownloadSettings settings, Action<string> onMessage, IWatchSession session, CancellationToken token)
        {
            string fileNameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
            var vSettings = settings.ConvertToVideoDownloadSettings(fileNameFormat, false);
            var videoDownloader = DIFactory.Provider.GetRequiredService<Vdl::IVideoDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await videoDownloader.DownloadVideoAsync(vSettings, onMessage, session, token);
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
        private async Task<IDownloadResult> TryDownloadCommentAsync(IDownloadSettings settings, IWatchSession session, Action<string> onMessage, CancellationToken token)
        {
            string fileNameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
            var cSettings = settings.ConvertToCommentDownloadSetting(fileNameFormat);
            var commentDownloader = DIFactory.Provider.GetRequiredService<Cdl::ICommentDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await commentDownloader.DownloadComment(session, cSettings, onMessage, token);
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
            var session = DIFactory.Provider.GetRequiredService<IWatchSession>();

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();
            if (setting.Video)
            {
                var vResult = await this.TryDownloadVideoAsync(setting, OnMessage, session, token);
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

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();
            if (setting.Thumbnail)
            {
                var tResult = await this.TryDownloadThumbAsync(setting, session);
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

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();
            if (setting.Comment)
            {
                var cResult = await this.TryDownloadCommentAsync(setting, session, OnMessage, token);
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

            if (session.IsSessionEnsured) session.Dispose();

            return result;
        }

        /// <summary>
        /// ダウンロード済のファイルをコピーする
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="downloadedFilePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        private IDownloadResult MoveDownloadedFile(string niconicoId, string downloadedFilePath, string destinationPath)
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

            this.fileStorehandler.Add(niconicoId, Path.Combine(destinationPath, filename));

            return new DownloadResult() { IsSucceeded = true };

        }

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="setting"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<INetworkResult> DownloadVideos(IEnumerable<ITreeVideoInfo> videos, DownloadSettings setting, CancellationToken token)
        {
            this.totalVideos = 0;
            this.downloadVideos.Clear();
            this.currentParallelDownloadingCount = 0;
            this.AddVideos(videos);
            var tcs = new TaskCompletionSource<INetworkResult>();
            var result = new NetworkResult();

            while (this.CanDownloadNext(token))
            {
                var _ = this.DownloadAndNext(setting, tcs,result, token);
            }

            return tcs.Task;
        }

        private IDownloadResult CancelledDownloadAndGetResult()
        {
            this.downloadVideos.Clear();
            this.currentParallelDownloadingCount = 0;
            return new DownloadResult() { Message = "キャンセルされました。" };
        }

        /// <summary>
        /// ダウンロードする動画のリスト
        /// </summary>
        private readonly List<IVideoToDownload> downloadVideos = new();

        /// <summary>
        /// ダウンロードする動画の数
        /// </summary>
        private int totalVideos;

        /// <summary>
        /// ダウンロードする動画を追加する
        /// </summary>
        /// <param name="video"></param>
        private void AddVideo(ITreeVideoInfo video)
        {
            this.totalVideos ++ ;
            if (this.downloadVideos.Any(v => v.Video.Id == video.Id)) return;
            this.downloadVideos.Add(new VideoToDownload(video, this.downloadVideos.Count));
        }

        /// <summary>
        /// ダウンロードする動画を追加する
        /// </summary>
        /// <param name="video"></param>
        private void AddVideos(IEnumerable<ITreeVideoInfo> videos)
        {
            foreach (var video in videos)
            {
                this.AddVideo(video);
            }
        }

        /// <summary>
        /// 次のタスクを取得する
        /// </summary>
        /// <returns></returns>
        private IVideoToDownload GetNext()
        {
            var first = this.downloadVideos.First();
            this.downloadVideos.RemoveAt(0);
            return first;
        }

        /// <summary>
        /// ダウンロードの実処理
        /// </summary>
        /// <param name="tcs"></param>
        /// <param name="token"></param>
        /// <returns></returns>this.messageHandler
        private async Task DownloadAndNext(DownloadSettings setting, TaskCompletionSource<INetworkResult> tcs, INetworkResult currentResult, CancellationToken token)
        {
            if (!token.IsCancellationRequested || this.CanDownloadNext(token))
            {
                this.currentParallelDownloadingCount++;
                var video = this.GetNext().Video;

                this.messageHandler.AppendMessage($"{video.NiconicoId}のダウンロード処理を開始しました。");

                string folderPath = setting.FolderPath;
                bool isDownloaded = this.networkVideoHandler.IsVideoDownloaded(video.NiconicoId);
                bool skippedFlag = false;

                if (isDownloaded)
                {
                    IDownloadResult? moveResult = null;
                    bool isSameFolder = video.CheckDownloaded(folderPath);

                    if (!isSameFolder && setting.FromAnotherFolder)
                    {
                        string downloadedPath = this.networkVideoHandler.GetFilePath(video.NiconicoId);
                        moveResult = this.MoveDownloadedFile(video.NiconicoId, downloadedPath, folderPath);
                    }

                    if ((isSameFolder && setting.Skip) || (moveResult?.IsSucceeded ?? false))
                    {
                        video.Message = "既にダウンロード済の為動画のダウンロードをスキップ";
                        this.messageHandler.AppendMessage($"{video.NiconicoId}は既にダウンロード済の為動画のダウンロードをスキップしました。");
                        skippedFlag = true;
                        if (setting.Video && !setting.Thumbnail)
                        {
                            video.IsSelected = false;
                            video.Message = "既にダウンロード済の為スキップ";
                            this.currentParallelDownloadingCount--;
                            currentResult.SucceededCount++;
                            if (currentResult.FirstVideo is null)
                            {
                                currentResult.FirstVideo = video;
                            }
                            this.StartNextDownload(setting, tcs,currentResult, token);
                            return;
                        }
                    }
                }


                var result = await this.TryDownloadContentAsync(setting with { NiconicoId = video.NiconicoId, Video = !skippedFlag && setting.Video }, msg => video.Message = msg, token);

                if (!result.IsSucceeded)
                {
                    currentResult.FailedCount++;
                    this.messageHandler.AppendMessage($"{video.NiconicoId}のダウンロードに失敗しました。");
                    this.messageHandler.AppendMessage($"詳細: {result.Message}");
                }
                else
                {
                    string rMessage = result.VideoVerticalResolution == 0 ? string.Empty : $"(vertical:{result.VideoVerticalResolution}px)";
                    this.messageHandler.AppendMessage($"{video.NiconicoId}のダウンロードに成功しました。");
                    video.IsSelected = false;
                    video.Message = $"ダウンロード完了{rMessage}";
                    if (!result.VideoFileName.IsNullOrEmpty())
                    {
                        video.FileName = result.VideoFileName;
                        this.videoHandler.Update(video);
                    }
                }

                this.currentParallelDownloadingCount--;
                currentResult.SucceededCount++;
                if (currentResult.FirstVideo is null)
                {
                    currentResult.FirstVideo = video;
                }
                this.StartNextDownload(setting, tcs,currentResult, token);
            }
            else
            {
                if (currentResult.SucceededCount == this.totalVideos)
                {
                    currentResult.IsSucceededAll = true;
                }
                if (!tcs.Task.IsCompleted) tcs.SetResult(currentResult);

            }


        }

        /// <summary>
        /// 次のダウンロードを開始する
        /// </summary>
        /// <param name="tcs"></param>
        /// <param name="token"></param>
        private async void StartNextDownload(DownloadSettings setting, TaskCompletionSource<INetworkResult> tcs, INetworkResult currentResult, CancellationToken token)
        {
            if (!this.CanDownloadNext(token) && this.IsDownloadCompleted())
            {
                if (currentResult.SucceededCount == this.totalVideos)
                {
                    currentResult.IsSucceededAll = true;
                }
                if (!tcs.Task.IsCompleted) tcs.SetResult(currentResult);
                return;
            }
            else if (this.NeedWait())
            {
                this.messageHandler.AppendMessage($"待機中(15s)");
                await Task.Delay(5 * 1000, token);
            }

            if (token.IsCancellationRequested)
            {
                if (!tcs.Task.IsCompleted) tcs.SetResult(currentResult);
                return;
            }

            while (this.CanDownloadNext(token))
            {
                var _ = this.DownloadAndNext(setting, tcs,currentResult, token);
            }
        }

        private bool CanDownloadNext(CancellationToken token)
        {
            return this.currentParallelDownloadingCount < this.maxParallelDownloadingCount && this.downloadVideos.Count > 0 && !token.IsCancellationRequested;
        }

        private bool NeedWait()
        {
            var video = this.downloadVideos.FirstOrDefault();
            if (video is null)
            {
                return false;
            }
            else
            {
                return (video.Index + 1) % 5 == 0;
            }
        }

        private bool IsDownloadCompleted()
        {
            return this.currentParallelDownloadingCount == 0;
        }

    }

    class DownloadResult : IDownloadResult
    {
        public bool IsSucceeded { get; set; }
        public string? Message { get; set; }
        public string? VideoFileName { get; set; }

        public uint VideoVerticalResolution { get; set; }


    }

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

        public uint VerticalResolution { get; set; }

        public string NiconicoId { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;

        public Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(string filenameFormat, bool autodispose)
        {
            return new Vdl::VideoDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FileNameFormat = filenameFormat,
                FolderName = this.FolderPath,
                IsAutoDisposingEnable = autodispose,
                IsOverwriteEnable = this.Overwrite,
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
            };
        }

        public Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(string fileFormat)
        {
            return new Cdl::CommentDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FolderName = this.FolderPath,
                FileNameFormat = fileFormat,
                IsOverwriteEnable = this.Overwrite,
                IsDownloadingEasyCommentEnable = this.DownloadEasy,
                IsDownloadingLogEnable = this.DownloadLog,
                IsDownloadingOwnerCommentEnable = this.DownloadOwner
            };
        }


    }

    /// <summary>
    /// インデックス付きの動画情報
    /// </summary>
    public class VideoToDownload : IVideoToDownload
    {
        public VideoToDownload(ITreeVideoInfo video, int index)
        {
            this.Video = video;
            this.Index = index;
        }

        public ITreeVideoInfo Video { get; set; }

        public int Index { get; set; }
    }
}
