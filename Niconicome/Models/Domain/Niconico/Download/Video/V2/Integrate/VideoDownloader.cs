using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.HLS;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Session;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Utils.ParallelTaskV2;
using SC = Niconicome.Models.Domain.Niconico.Download.Video.V2.Integrate.VideoDownloaderSC;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Integrate
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
        Task<IAttemptResult<uint>> DownloadVideoAsync(IDownloadSettings settings, Action<string> OnMessage, IDomainVideoInfo videoInfo, CancellationToken token);

    }

    public class VideoDownloader : IVideoDownloader
    {
        public VideoDownloader(IPathOrganizer pathOrganizer, ISegmentDirectoryHandler segmentDirectoryHandler, IVideoEncoder videoEncoader, INiconicomeFileIO fileIO, IStringHandler stringHandler, IVideoFileStore fileStore,INiconicomeDirectoryIO directoryIO)
        {
            this._pathOrganizer = pathOrganizer;
            this._segmentDirectory = segmentDirectoryHandler;
            this._stringHandler = stringHandler;
            this._videoEncoder = videoEncoader;
            this._fileIO = fileIO;
            this._videoFileStore = fileStore;
            this._directoryIO = directoryIO;
        }

        #region field

        private readonly IPathOrganizer _pathOrganizer;

        private readonly ISegmentDirectoryHandler _segmentDirectory;

        private readonly IVideoEncoder _videoEncoder;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IStringHandler _stringHandler;

        private readonly IVideoFileStore _videoFileStore;

        private readonly INiconicomeDirectoryIO _directoryIO;

        #endregion

        #region Method

        public async Task<IAttemptResult<uint>> DownloadVideoAsync(IDownloadSettings settings, Action<string> OnMessage, IDomainVideoInfo videoInfo, CancellationToken token)
        {
            IWatchSession session = DIFactory.Resolve<IWatchSession>();

            //視聴セッションを確立
            OnMessage(this._stringHandler.GetContent(SC.EnsureSession));
            IAttemptResult sessionResult = await session.EnsureSessionAsync(videoInfo);

            if (!sessionResult.IsSucceeded)
            {
                return AttemptResult<uint>.Fail(sessionResult.Message);
            }

            //ストリーム情報を取得
            OnMessage(this._stringHandler.GetContent(SC.FetchingStreamInfo));
            IAttemptResult<IStreamsCollection> streamResult = await session.GetAvailableStreamsAsync();
            if (!streamResult.IsSucceeded || streamResult.Data is null)
            {
                return AttemptResult<uint>.Fail(streamResult.Message);
            }
            IStreamInfo stream = streamResult.Data.GetStream(settings.VerticalResolution);


            string filePath = this._pathOrganizer.GetFilePath(settings.FileNameFormat, videoInfo.DmcInfo, settings.SaveWithoutEncode ? FileFolder.TsFileExt : FileFolder.Mp4FileExt, settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite);

            //レジューム
            string folderPath;
            var targetSegments = new List<ISegmentURL>();

            IAttemptResult<ResumeInfomation> resumeResult = settings.ResumeEnable ? this.GetResumeInfomation(stream.SegmentUrls, videoInfo.Id, stream.VideoResolution.Vertical) : AttemptResult<ResumeInfomation>.Fail();

            if (!resumeResult.IsSucceeded || resumeResult.Data is null)
            {
                IAttemptResult<string> segmentResult = this._segmentDirectory.Create(videoInfo.Id, stream.VideoResolution.Vertical);
                if (!segmentResult.IsSucceeded || segmentResult.Data is null)
                {
                    return AttemptResult<uint>.Fail(segmentResult.Message);
                }


                folderPath = segmentResult.Data;
                targetSegments.AddRange(stream.SegmentUrls);
            }
            else
            {
                OnMessage(this._stringHandler.GetContent(SC.Resume));

                folderPath = resumeResult.Data.SegmentDirectoryPath;
                targetSegments.AddRange(resumeResult.Data.DownloadTargets);
            }

            //セグメントのDL
            IAttemptResult dlResult = await this.DownloadSegments(targetSegments, folderPath, videoInfo.Id, stream.VideoResolution.Vertical, settings.MaxParallelSegmentDLCount, OnMessage, token);
            if (!dlResult.IsSucceeded)
            {
                return AttemptResult<uint>.Fail(dlResult.Message);
            }

            //エンコード
            var option = new EncodeOption()
            {
                FilePath = filePath,
                FolderPath = folderPath,
                CommandFormat = settings.CommandFormat,
                IsOverrideDTEnable = settings.OverrideVideoFileDateToUploadedDT,
                IsStoreRawTSFileEnable = settings.SaveWithoutEncode,
                UploadedOn = videoInfo.DmcInfo.UploadedOn,
                TsFilePaths = targetSegments.Select(s => Path.Combine(folderPath, s.FileName))
            };

            OnMessage(this._stringHandler.GetContent(SC.Encode));
            IAttemptResult encodeResult = await this._videoEncoder.EncodeAsync(option, OnMessage, token);
            if (!encodeResult.IsSucceeded)
            {
                return AttemptResult<uint>.Fail(encodeResult.Message);
            }

            //エコノミーファイルを削除
            if (settings.DeleteExistingEconomyFile && !videoInfo.DmcInfo.IsEnonomy)
            {
                this.DeleteEconomyFile(settings.FilePath);
            }

            //ファイル情報を追加
            await this._videoFileStore.AddFileAsync(videoInfo.Id, filePath);

            //一時フォルダーを削除
            this._directoryIO.Delete(folderPath);

            //セッションを破棄
            if (session.IsSessionEnsured)
            {
                session.Dispose();
            }

            OnMessage(this._stringHandler.GetContent(SC.Completed));
            return AttemptResult<uint>.Succeeded(stream.VideoResolution.Vertical);
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
        private async Task<IAttemptResult> DownloadSegments(IEnumerable<ISegmentURL> targets, string folderPath, string videoID, uint verticalResoluiton, int parallelDLCount, Action<string> onMessage, CancellationToken token)
        {
            var handler = new ParallelTasksHandler(parallelDLCount);
            var container = new SegmentDLResultContainer(targets.Count());

            foreach (var streamURL in targets)
            {
                var info = new SegmentInfomation(onMessage, streamURL.AbsoluteUrl, streamURL.SequenceZero, Path.Combine(folderPath, streamURL.FileName), verticalResoluiton, videoID, token);

                var task = new ParallelTask(async _ =>
                {
                    var downloader = DIFactory.Resolve<ISegmentDownloader>();
                    downloader.Initialize(info, container);

                    await downloader.DownloadAsync();

                    onMessage(this._stringHandler.GetContent(SC.SegmentDownloadCompleted, container.CompletedCount, container.Length, verticalResoluiton));

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
        private IAttemptResult<ResumeInfomation> GetResumeInfomation(IEnumerable<ISegmentURL> source, string videoID, uint verticalResoluiton)
        {
            if (!this._segmentDirectory.Exists(videoID, verticalResoluiton))
            {
                return AttemptResult<ResumeInfomation>.Fail();
            }

            IAttemptResult<ISegmentDirectoryInfo> resumeResult = this._segmentDirectory.GetSegmentDirectoryInfo(videoID, verticalResoluiton);

            if (resumeResult.IsSucceeded && resumeResult.Data is not null)
            {
                var target = source.Where(s => !resumeResult.Data.ExistsFiles.Contains(s.FileName));
                return AttemptResult<ResumeInfomation>.Succeeded(new ResumeInfomation(target, resumeResult.Data.DirectoryPath));
            }
            else
            {
                return AttemptResult<ResumeInfomation>.Fail(resumeResult.Message);
            }
        }

        /// <summary>
        /// エコノミーファイルを削除
        /// </summary>
        /// <param name="path"></param>
        private void DeleteEconomyFile(string path)
        {
            if (!this._fileIO.Exists(path)) return;

            this._fileIO.Delete(path);
        }


        private record ResumeInfomation(IEnumerable<ISegmentURL> DownloadTargets, string SegmentDirectoryPath);

        #endregion
    }
}
