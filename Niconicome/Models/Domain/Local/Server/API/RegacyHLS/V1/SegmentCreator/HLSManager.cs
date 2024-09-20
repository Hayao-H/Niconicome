using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External.Software.FFmpeg.FFmpeg;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1.Error;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1.SegmentCreator
{
    public interface IHLSManager
    {
        /// <summary>
        /// インデックスファイルとセグメントファイルを生成する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        Task<IAttemptResult> CreateFilesAsync(string niconicoID, int playlistID);
    }

    public class HLSManager : IHLSManager
    {
        public HLSManager(INiconicomeDirectoryIO directoryIO, INiconicomeFileIO fileIO, ISettingsContainer settingsContainer, IErrorHandler errorHandler, IVideoStore store, IFFmpegManager ffmpegManager)
        {
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
            this._settingsContainer = settingsContainer;
            this._store = store;
            this._fFmpegManager = ffmpegManager;
        }

        #region field

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly INiconicomeFileIO _fileIO;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IErrorHandler _errorHandler;

        private readonly IVideoStore _store;

        private readonly IFFmpegManager _fFmpegManager;

        private bool _isRunnning;

        #endregion

        #region Method

        public async Task<IAttemptResult> CreateFilesAsync(string niconicoID, int playlistID)
        {
            if (this._isRunnning) return AttemptResult.Fail(this._errorHandler.HandleError(HLSManagerError.AlreadyRunning));

            string path = Path.Combine(AppContext.BaseDirectory, "data", "tmp", "hls", playlistID.ToString(), niconicoID, "master.m3u8");
            if (this._fileIO.Exists(path))
            {
                return AttemptResult.Succeeded();
            }


            IAttemptResult<IVideoInfo> vResult = this._store.GetVideo(niconicoID, playlistID);
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return AttemptResult.Fail(vResult.Message);
            }

            IVideoInfo video = vResult.Data;
            if (!video.IsDownloaded.Value)
            {
                this._errorHandler.HandleError(HLSManagerError.VideoIsNotDownloaded, niconicoID, playlistID);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(HLSManagerError.VideoIsNotDownloaded, niconicoID, playlistID));
            }

            if (video.IsDMS)
            {
                return AttemptResult.Fail(this._errorHandler.HandleError(HLSManagerError.VideoIsDMS, niconicoID, playlistID));
            }


            IAttemptResult dResult = this.CreateDirectory(playlistID, niconicoID);
            if (!dResult.IsSucceeded)
            {
                return dResult;
            }

            _isRunnning = true;
            var result = await this.CreateHLSFilesAsync(video.Mp4FilePath, playlistID, niconicoID);
            _isRunnning = false;

            return result;
        }


        #endregion

        #region private

        /// <summary>
        /// ディレクトリを作成
        /// </summary>
        /// <returns></returns>
        private IAttemptResult CreateDirectory(int playlistID, string niconicoID)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "data", "tmp", "hls", playlistID.ToString(), niconicoID);

            if (this._directoryIO.Exists(path))
            {
                IAttemptResult delResult = this._directoryIO.Delete(path);
                if (!delResult.IsSucceeded)
                {
                    return delResult;
                }
            }

            return this._directoryIO.CreateDirectory(path);
        }


        /// <summary>
        /// playlist.m3u8とtsファイルを作成
        /// </summary>
        /// <param name="videoFilePath"></param>
        /// <returns></returns>
        private async Task<IAttemptResult> CreateHLSFilesAsync(string videoFilePath, int playlistID, string niconicoID)
        {
            if (!this._fileIO.Exists(videoFilePath))
            {
                this._errorHandler.HandleError(HLSManagerError.FileDoesNotExist, videoFilePath);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(HLSManagerError.FileDoesNotExist, videoFilePath));
            }

            IAttemptResult<ISettingInfo<string>> ffmpeg = this._settingsContainer.GetSetting(SettingNames.FFmpegPath, Format.FFmpegPath);
            if (!ffmpeg.IsSucceeded || ffmpeg.Data is null)
            {
                return AttemptResult.Fail(ffmpeg.Message);
            }

            string output = Path.Combine(AppContext.BaseDirectory, "data", "tmp", "hls", playlistID.ToString(), niconicoID);
            var command = $"-i \"<source>\" -c:v copy -c:a copy -f hls -hls_time 9 -hls_playlist_type vod -hls_segment_filename \"{output}\\video%3d.ts\" \"<output>\"";

            IAttemptResult result = await this._fFmpegManager.EncodeAsync(videoFilePath, Path.Join(output, "playlist.m3u8"), command, CancellationToken.None);

            if (!result.IsSucceeded)
            {
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(HLSManagerError.FailedToEncodeFileToHLS));
            }

            return AttemptResult.Succeeded();
        }

        #endregion
    }
}
