using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Dmc;
using Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;

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

    public interface IVideoDownloader
    {
        Task<IAttemptResult> DownloadVideoAsync(IDownloadSettings settings, Action<string> OnMessage, IDownloadContext context, IWatchSession session, CancellationToken token);
    }

    /// <summary>
    /// 外部から触るAPIを提供する
    /// </summary>
    public class VideoDownloader : IVideoDownloader
    {
        public VideoDownloader(ILogger logger, IVideoDownloadHelper videoDownloadHelper, IVideoEncoader encorder, IStreamResumer streamResumer, INicoFileIO fileIO, IPathOrganizer pathOrganizer, IVideoFileStore videoFileStore)
        {
            this._logger = logger;
            this._videoDownloadHelper = videoDownloadHelper;
            this._videoFileStore = videoFileStore;
            this._encorder = encorder;
            this._streamResumer = streamResumer;
            this._fileIO = fileIO;
            this._pathOrganizer = pathOrganizer;
        }

        #region field

        private readonly ILogger _logger;

        private readonly IVideoDownloadHelper _videoDownloadHelper;

        private readonly IVideoEncoader _encorder;

        private readonly IVideoFileStore _videoFileStore;

        private readonly IStreamResumer _streamResumer;

        private readonly INicoFileIO _fileIO;

        private readonly IPathOrganizer _pathOrganizer;

        private IDownloadContext? context;

        #endregion

        public async Task<IAttemptResult> DownloadVideoAsync(IDownloadSettings settings, Action<string> onMessage, IDownloadContext context, IWatchSession session, CancellationToken token)
        {
            IAttemptResult OnCanceled()
            {
                this._logger.Log($"ユーザーの操作によって動画のダウンロード処理がキャンセルされました。({context.GetLogContent()})");
                onMessage("ダウンロードをキャンセル");
                return AttemptResult.Fail("処理がキャンセルされました");
            }

            this.context = context;
            this._logger.Log($"動画のダウンロードを開始しました。({context.GetLogContent()})");

            if (session.IsSessionExipired)
            {
                this._logger.Log($"セッションが失効していたため動画のダウンロードをキャンセルします。({context.GetLogContent()})");
                return AttemptResult<string>.Fail("セッションが失効済のためダウンロード出来ません。");
            }

            if (!session.IsSessionEnsured)
            {
                if (token.IsCancellationRequested)
                {
                    return OnCanceled();
                }

                IAttemptResult sessionR = await this.EnsureSessionAsync(session, settings);

                if (!sessionR.IsSucceeded)
                {
                    onMessage(sessionR.Message ?? "");
                    return sessionR;
                }
            }


            //ファイル名取得
            string fileName = this.GetFileName(session, settings);
            context.FileName = fileName;


            if (token.IsCancellationRequested)
            {
                return OnCanceled();
            }


            //ストリーム系
            IAttemptResult<IStreamInfo> streamResult = await this.GetStream(session, settings.VerticalResolution);

            if (!streamResult.IsSucceeded || streamResult.Data is null)
            {
                return AttemptResult.Fail(streamResult.Message, streamResult.Exception);
            }

            IStreamInfo targetStream = streamResult.Data;
            List<string> rawsegmentFilePaths = targetStream.StreamUrls.Select(u => u.FileName).ToList();
            context.ActualVerticalResolution = targetStream.Resolution?.Vertical ?? 0;
            context.OriginalSegmentsCount = targetStream.StreamUrls.Count;


            if (token.IsCancellationRequested)
            {
                return OnCanceled();
            }


            //レジューム系
            string segmentDirectoryName;
            IAttemptResult<string> sResult = settings.ResumeEnable ? this.GetAndSetSegmentsDirectoryInfoIfExists(settings.NiconicoId, targetStream.Resolution?.Vertical ?? 0, targetStream) : AttemptResult<string>.Fail();

            if (sResult.IsSucceeded && sResult.Data is not null)
            {
                segmentDirectoryName = sResult.Data;
                onMessage("DLをレジューム");
                this._logger.Log($"ダウンロードをレジュームします。({context.GetLogContent()})");
            }
            else
            {
                segmentDirectoryName = $"{settings.NiconicoId}-{targetStream.Resolution?.Vertical ?? 0}-{DateTime.Now.ToString("yyyy-MM-dd")}";
            }
            List<string> segmentFilePaths = rawsegmentFilePaths.Select(p => Path.Combine(AppContext.BaseDirectory, "tmp", segmentDirectoryName, p)).ToList();


            //DL系
            IAttemptResult downloadresult = await this.DownloadVideoInternalAsync(targetStream, onMessage, context, settings.MaxParallelSegmentDLCount, segmentDirectoryName, token);
            if (!downloadresult.IsSucceeded)
            {
                return downloadresult;
            }

            if (token.IsCancellationRequested)
            {
                return OnCanceled();
            }

            onMessage("動画を変換中...");
            IAttemptResult encodeResult = await this.EncodeVideosAsync(session, context, settings, onMessage, segmentFilePaths, token);
            if (!encodeResult.IsSucceeded)
            {
                return encodeResult;
            }
            onMessage("動画の変換が完了");

            this.DeleteTmpFolder(segmentDirectoryName, onMessage);


            bool isEconomy = session.Video!.DmcInfo.IsEconomy;
            if (settings.IsEconomy && settings.DeleteExistingEconomyFile && !isEconomy)
            {
                this.RemoveEconomyFile(settings.FilePath);
            }

            await this._videoFileStore.AddFileAsync(settings.NiconicoId, Path.Combine(settings.FolderPath, fileName));

            this._logger.Log($"動画のダウンロードが完了しました。({context.GetLogContent()})");

            return AttemptResult.Succeeded();
        }

        #region private

        private async Task<IAttemptResult> EncodeVideosAsync(IWatchSession session, IDownloadContext context, IDownloadSettings settings, Action<string> onMessage, IEnumerable<string> segmentFilePaths, CancellationToken token)
        {

            var encodeSetting = new EncodeSettings()
            {
                FilePath = context.FileName,
                CommandFormat = settings.CommandFormat,
                TsFilePaths = segmentFilePaths,
                IsOverwriteEnable = settings.Overwrite,
                IsOverrideDTEnable = settings.OverrideVideoFileDateToUploadedDT,
                UploadedOn = session.Video!.DmcInfo.UploadedOn,
                IsNoEncodeEnable = settings.SaveWithoutEncode,
            };

            try
            {
                await this._encorder.EncodeAsync(encodeSetting, onMessage, token);
            }
            catch (Exception e)
            {
                this._logger.Error($"ファイルの変換中にエラーが発生しました。({this.context!.GetLogContent()})", e);
                onMessage("動画の変換中にエラー発生");
                return AttemptResult.Fail($"ファイルの変換中にエラーが発生しました。(詳細: {e.Message})");
            }

            return AttemptResult.Succeeded();
        }

        private async Task<IAttemptResult> DownloadVideoInternalAsync(IStreamInfo targetStream, Action<string> onMessage, IDownloadContext context, int maxParallelSegmentDLCount, string segmentDirectoryName, CancellationToken token)
        {

            try
            {
                await this._videoDownloadHelper.DownloadAsync(targetStream, onMessage, context, maxParallelSegmentDLCount, segmentDirectoryName, token);

            }
            catch (Exception e)
            {
                this._logger.Error($"動画のダウンロード中にエラーが発生しました。({this.context!.GetLogContent()})", e);
                onMessage("動画のダウンロードに失敗");
                return AttemptResult.Fail($"動画のダウンロード中にエラーが発生しました。(詳細: {e.Message})");
            }

            return AttemptResult.Succeeded();
        }

        private async Task<IAttemptResult<IStreamInfo>> GetStream(IWatchSession session, uint resolution)
        {
            IStreamInfo targetStream;

            try
            {
                IStreamsCollection streams = await session.GetAvailableStreamsAsync();
                targetStream = streams.GetStream(resolution);
            }
            catch (Exception e)
            {
                this._logger.Error($"ストリームの取得に失敗しました。({this.context!.GetLogContent()})", e);
                return AttemptResult<IStreamInfo>.Fail("ストリームの取得に失敗しました。", e);
            }

            return AttemptResult<IStreamInfo>.Succeeded(targetStream);
        }

        private async Task<IAttemptResult> EnsureSessionAsync(IWatchSession session, IDownloadSettings settings)
        {

            await session.EnsureSessionAsync(settings.NiconicoId);

            if (!session.IsSessionEnsured)
            {
                string message = session.State switch
                {
                    WatchSessionState.HttpRequestOrPageAnalyzingFailure => "視聴ページの取得、または解析に失敗しました。",
                    WatchSessionState.EncryptedVideo => "暗号化された動画のため、ダウンロードできません。",
                    WatchSessionState.SessionEnsuringFailure => "セッションの確立に失敗しました。",
                    _ => "不明なエラーにより、セッションの確立に失敗しました。"
                };

                return AttemptResult.Fail(message);
            }
            else
            {
                this._logger.Log($"視聴セッションを確立しました。({this.context!.GetLogContent()})");
                return AttemptResult.Succeeded();
            }
        }

        private IAttemptResult<string> GetAndSetSegmentsDirectoryInfoIfExists(string niconicoID, uint verticalResolution, IStreamInfo stream)
        {
            if (!this._streamResumer.SegmentsDirectoryExists(niconicoID)) return AttemptResult<string>.Fail();

            var info = this._streamResumer.GetSegmentsDirectoryInfo(niconicoID);

            if (info.Resolution != verticalResolution)
            {
                this._streamResumer.RemoveSegmentsdirectory(niconicoID);
                return AttemptResult<string>.Fail();
            }

            var urls = stream.StreamUrls.Where(u => !info.ExistsFileNames.Contains(u.FileName)).Copy();
            stream.StreamUrls.Clear();
            stream.StreamUrls.AddRange(urls);
            return AttemptResult<string>.Succeeded(info.DirectoryName);
        }

        private string GetFileName(IWatchSession session, IDownloadSettings settings)
        {
            string? economySuffix = session.Video!.DmcInfo.IsEconomy ? settings.EconomySuffix : null;
            string? suffix = economySuffix;
            string ext = settings.SaveWithoutEncode ? FileFolder.TsFileExt : FileFolder.Mp4FileExt;
            string format = string.IsNullOrEmpty(settings.FileNameFormat) ? Format.DefaultFileNameFormat : settings.FileNameFormat;

            return this._pathOrganizer.GetFilePath(format, session.Video!.DmcInfo, ext, settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite, suffix);
        }

        private void DeleteTmpFolder(string segmentDirectoryName, Action<string> onMessage)
        {
            string folderPath = Path.Combine(AppContext.BaseDirectory, "tmp", segmentDirectoryName);
            if (!Directory.Exists(folderPath)) return;

            try
            {
                Directory.Delete(folderPath, true);
            }
            catch (Exception e)
            {
                this._logger.Error($"一時フォルダーの削除中にエラーが発生しました。({this.context!.GetLogContent()})", e);
                onMessage("一時フォルダーの削除中にエラー発生");
            }
        }

        private void RemoveEconomyFile(string path)
        {
            try
            {
                this._fileIO.Delete(path);
            }
            catch (Exception e)
            {
                this._logger.Error($"エコノミーファイルの削除中にエラーが発生しました。({this.context!.GetLogContent()})", e);
            }
        }

        #endregion

    }
}
