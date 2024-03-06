using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.Fetch.Segment;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.Session;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Utils.ParallelTaskV2;
using SC = Niconicome.Models.Domain.Niconico.Download.Video.V3.Integrate.VideoDownloaderSC;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.External;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.DMS;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.Fetch.Key;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.StreamJson;
using System.Text.RegularExpressions;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Integrate
{
    public interface IVideoDownloader
    {
        /// <summary>
        /// 動画をダウンロード
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="OnMessage"></param>
        /// <param name="videoInfo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult<int>> DownloadVideoAsync(IDownloadSettings settings, Action<string> OnMessage, IDomainVideoInfo videoInfo, CancellationToken token);

    }

    public class VideoDownloader : IVideoDownloader
    {
        public VideoDownloader(IPathOrganizer pathOrganizer, ISegmentDirectoryHandler segmentDirectory, INiconicomeFileIO fileIO, IStringHandler stringHandler, INiconicomeDirectoryIO directoryIO, IExternalDownloaderHandler external, IKeyDownlaoder keyDownlaoder, IStreamJsonHandler streamJsonHandler)
        {
            this._pathOrganizer = pathOrganizer;
            this._segmentDirectory = segmentDirectory;
            this._fileIO = fileIO;
            this._stringHandler = stringHandler;
            this._directoryIO = directoryIO;
            this._external = external;
            this._keyDownlaoder = keyDownlaoder;
            this._streamJsonHandler = streamJsonHandler;
        }

        #region field

        private readonly IPathOrganizer _pathOrganizer;

        private readonly ISegmentDirectoryHandler _segmentDirectory;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IStringHandler _stringHandler;

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly IExternalDownloaderHandler _external;

        private readonly IKeyDownlaoder _keyDownlaoder;

        private readonly IStreamJsonHandler _streamJsonHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<int>> DownloadVideoAsync(IDownloadSettings settings, Action<string> OnMessage, IDomainVideoInfo videoInfo, CancellationToken token)
        {
            //DLするかどうかを判定
            if (!this.ShouldDownladVideo(videoInfo, settings))
            {
                OnMessage(this._stringHandler.GetContent(SC.SkipEconomy));
                return AttemptResult<int>.Succeeded(0);
            }

            //ファイルパス
            string filePath = this._pathOrganizer.GetFilePath(settings.FileNameFormat, videoInfo.DmcInfo, string.Empty, settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite);

            if (!this._directoryIO.Exists(filePath))
            {
                this._directoryIO.CreateDirectory(filePath);
            }

            //外部ダウンローダー
            if (this._external.CheckCondition(videoInfo, settings))
            {
                IAttemptResult externalResult = await this._external.DownloadVideoByExtarnalDownloaderAsync(settings, videoInfo.Id, filePath, OnMessage, token);
                if (!externalResult.IsSucceeded)
                {
                    return AttemptResult<int>.Fail(externalResult.Message);
                }

                IAttemptResult<int> resolutionResult = await this._fileIO.GetVerticalResolutionAsync(filePath);
                if (!resolutionResult.IsSucceeded)
                {
                    return AttemptResult<int>.Succeeded(0);
                }
                else
                {
                    return AttemptResult<int>.Succeeded(resolutionResult.Data);
                }
            }

            using IWatchSession session = DIFactory.Resolve<IWatchSession>();

            //視聴セッションを確立
            OnMessage(this._stringHandler.GetContent(SC.EnsureSession));
            IAttemptResult sessionResult = await session.EnsureSessionAsync(videoInfo);

            if (!sessionResult.IsSucceeded)
            {
                return AttemptResult<int>.Fail(sessionResult.Message);
            }

            //ストリーム情報を取得
            OnMessage(this._stringHandler.GetContent(SC.FetchingStreamInfo));
            IAttemptResult<IStreamCollection> streamResult = await session.GetAvailableStreamsAsync();
            if (!streamResult.IsSucceeded || streamResult.Data is null)
            {
                return AttemptResult<int>.Fail(streamResult.Message);
            }
            IStreamInfo stream = streamResult.Data.GetStream((int)settings.VerticalResolution);

            IAttemptResult streamInfoResult = await stream.GetStreamInfo();
            if (!streamInfoResult.IsSucceeded)
            {
                return AttemptResult<int>.Fail(streamInfoResult.Message);
            }

            //レジューム
            string tempFolderPath;
            IEnumerable<string> existingVideoFileNames;
            IEnumerable<string> existingAudioFileNames;

            IAttemptResult<ResumeInfomation> resumeResult = settings.ResumeEnable ? this.GetResumeInfomation(videoInfo.Id, stream.VerticalResolution) : AttemptResult<ResumeInfomation>.Fail();

            if (!resumeResult.IsSucceeded || resumeResult.Data is null)
            {
                IAttemptResult<string> segmentResult = this._segmentDirectory.Create(videoInfo.Id, stream.VerticalResolution);
                if (!segmentResult.IsSucceeded || segmentResult.Data is null)
                {
                    return AttemptResult<int>.Fail(segmentResult.Message);
                }


                tempFolderPath = segmentResult.Data;
                existingAudioFileNames = new List<string>();
                existingVideoFileNames = new List<string>();
            }
            else
            {
                OnMessage(this._stringHandler.GetContent(SC.Resume));

                tempFolderPath = resumeResult.Data.SegmentDirectoryPath;
                existingAudioFileNames = resumeResult.Data.ExistingAudioFileNames;
                existingVideoFileNames = resumeResult.Data.ExistingVideoFileNames;
            }


            //セグメントのDL
            IAttemptResult dlResult = await this.DownloadSegments(stream.VideoSegmentURLs, stream.AudioSegmentURLs, existingVideoFileNames, existingAudioFileNames, tempFolderPath, videoInfo.Id, stream.VerticalResolution, settings.MaxParallelSegmentDLCount, OnMessage, token);
            if (!dlResult.IsSucceeded)
            {
                return AttemptResult<int>.Fail(dlResult.Message);
            }

            //ファイルを移動
            string destination = Path.Combine(filePath, stream.VerticalResolution.ToString());
            if (!this._directoryIO.Exists(destination))
            {
                this._directoryIO.CreateDirectory(destination);
            }
            IAttemptResult moveResult = this.MoveFiles(tempFolderPath, Path.Combine(filePath, stream.VerticalResolution.ToString()));
            if (!moveResult.IsSucceeded)
            {
                return AttemptResult<int>.Fail(moveResult.Message);
            }

            //キー
            IAttemptResult<KeyInfomation> keyResult = await this._keyDownlaoder.DownloadKeyASync(stream.VideoKeyURL, stream.AudioKeyURL);
            if (!keyResult.IsSucceeded || keyResult.Data is null)
            {
                return AttemptResult<int>.Fail(keyResult.Message);
            }

            IAttemptResult jsonResult = this._streamJsonHandler.AddStream(filePath, stream.VerticalResolution, keyResult.Data.VideoKey, keyResult.Data.AudioKey, stream.VideoIV, stream.AudioIV,stream.VideoSegmentDurations,stream.AudioSegmentDurations);
            if (!jsonResult.IsSucceeded)
            {
                return AttemptResult<int>.Fail(jsonResult.Message);
            }


            //一時フォルダーを削除
            this._directoryIO.Delete(tempFolderPath);

            OnMessage(this._stringHandler.GetContent(SC.Completed));
            return AttemptResult<int>.Succeeded(stream.VerticalResolution);
        }


        #endregion

        #region field

        /// <summary>
        /// セグメントをダウンロードする
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="folderPath"></param>
        /// <param name="videoID"></param>
        /// <param name="verticalResoluiton"></param>
        /// <param name="parallelDLCount"></param>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<IAttemptResult> DownloadSegments(IEnumerable<string> videos, IEnumerable<string> audios, IEnumerable<string> existingVideoFileNames, IEnumerable<string> existingAudioFileNames, string folderPath, string videoID, int verticalResoluiton, int parallelDLCount, Action<string> onMessage, CancellationToken token)
        {
            var targets = videos.Concat(audios).ToList();
            var handler = new ParallelTasksHandler(parallelDLCount);
            var container = new SegmentDLResultContainer(targets.Count);

            foreach (var streamURL in targets)

            {
                var fileNameWIthoutExt = Path.GetFileNameWithoutExtension(streamURL);
                var fileName = Path.GetFileName(new Uri(streamURL).AbsolutePath);
                var index = int.Parse(fileNameWIthoutExt.TrimStart('0')) - 1;
                string localPath;

                if (streamURL.Contains(".cmfv"))
                {
                    localPath = Path.Combine(folderPath, "video", fileName);
                    if (existingVideoFileNames.Contains(fileName))
                    {
                        container.SetResult(true, index);
                    }
                }
                else
                {
                    localPath = Path.Combine(folderPath, "audio", fileName);
                    index += videos.Count();
                    if (existingAudioFileNames.Contains(fileName))
                    {
                        container.SetResult(true, index);
                    }
                }

                var info = new SegmentInfomation(onMessage, streamURL, index, localPath, verticalResoluiton, videoID, token);

                var task = new ParallelTask(async _ =>
                {
                    var downloader = DIFactory.Resolve<ISegmentDownloader>();
                    downloader.Initialize(info, container);

                    await downloader.DownloadAsync();

                }, _ => { });

                handler.AddTaskToQueue(task);
            }

            await handler.ProcessTasksAsync();

            if (!container.IsAllSucceeded)
            {
                return AttemptResult<string>.Fail();
            }
            else
            {
                return AttemptResult<string>.Succeeded(folderPath);
            }
        }

        /// <summary>
        /// レジューム情報を取得する
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="verticalResoluiton"></param>
        /// <returns></returns>
        private IAttemptResult<ResumeInfomation> GetResumeInfomation(string videoID, int verticalResoluiton)
        {
            if (!this._segmentDirectory.Exists(videoID, verticalResoluiton))
            {
                return AttemptResult<ResumeInfomation>.Fail();
            }

            IAttemptResult<ISegmentDirectoryInfo> resumeResult = this._segmentDirectory.GetSegmentDirectoryInfo(videoID, verticalResoluiton);

            if (resumeResult.IsSucceeded && resumeResult.Data is not null)
            {
                return AttemptResult<ResumeInfomation>.Succeeded(new ResumeInfomation(resumeResult.Data.ExistingVideoFileNames, resumeResult.Data.ExistingAudioFileNames, resumeResult.Data.DirectoryPath));
            }
            else
            {
                return AttemptResult<ResumeInfomation>.Fail(resumeResult.Message);
            }
        }

        /// <summary>
        /// ファイルを移動する
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private IAttemptResult MoveFiles(string directoryPath, string destination)
        {
            IAttemptResult resultV = this._directoryIO.Move(Path.Combine(directoryPath, "video"), Path.Combine(destination, "video"));

            if (!resultV.IsSucceeded) return resultV;

            IAttemptResult resultA = this._directoryIO.Move(Path.Combine(directoryPath, "audio"), Path.Combine(destination, "audio"));

            return resultA;
        }

        /// <summary>
        /// DLするかどうかを判定する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private bool ShouldDownladVideo(IDomainVideoInfo videoInfo, IDownloadSettings settings)
        {
            if (!videoInfo.DmcInfo.IsEconomy) return true;

            if (string.IsNullOrEmpty(settings.FilePath)) return true;

            if (!this._fileIO.Exists(settings.FilePath)) return true;

            if (settings.SkipEconomyDownloadIfPremiumExists && !settings.FilePath.Contains(settings.EconomySuffix)) return false;

            if (settings.AlwaysSkipEconomyDownload) return false;

            return true;
        }


        private record ResumeInfomation(IEnumerable<string> ExistingVideoFileNames, IEnumerable<string> ExistingAudioFileNames, string SegmentDirectoryPath);

        #endregion
    }
}
