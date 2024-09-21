using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using PlaylistV2 = Niconicome.Models.Domain.Playlist;

namespace Niconicome.Models.Local.External.Playlist
{
    public interface IPlaylistCreator
    {

        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="name"></param>
        /// <param name="folderPath"></param>
        /// <param name="type"></param>
        /// <returns>失敗した動画数</returns>
        IAttemptResult<int> TryCreatePlaylist(IReadOnlyList<PlaylistV2::IVideoInfo> videos, string playlistName, string folderPath, PlaylistType type);
    }


    class PlaylistCreator : IPlaylistCreator
    {
        public PlaylistCreator(IPlaylistFileFactory fileFactory, IVideoFileStorehandler videoFileStorehandler, INiconicomeFileIO fileIO, INiconicomeDirectoryIO directoryIO, IErrorHandler errorHandler)
        {
            this._fileFactory = fileFactory;
            this._videoFileStorehandler = videoFileStorehandler;
            this._fileIO = fileIO;
            this._directoryIO = directoryIO;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IPlaylistFileFactory _fileFactory;

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly IVideoFileStorehandler _videoFileStorehandler;

        private readonly IErrorHandler _errorHandler;

        #endregion

        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="directoryPath"></param>
        /// <param name="type"></param>
        /// <returns></returns>        
        public IAttemptResult<int> TryCreatePlaylist(IReadOnlyList<PlaylistV2::IVideoInfo> videos, string playlistName, string folderPath, PlaylistType type)
        {


            videos = videos.Where(v => v.IsDownloaded.Value).ToList().AsReadOnly();
            if (videos.Count == 0)
            {
                this._errorHandler.HandleError(PlaylistCreatorError.DownloadedVideoDoesNotExist);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(PlaylistCreatorError.DownloadedVideoDoesNotExist));
            }

            int allVideos = videos.Count;
            List<PlaylistV2::IVideoInfo> existingVideos = new();

            existingVideos = videos.Where(p => this._fileIO.Exists(p.Mp4FilePath)).ToList();

            if (existingVideos.Count == 0)
            {
                this._errorHandler.HandleError(PlaylistCreatorError.DownloadedVideoDoesNotExist);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(PlaylistCreatorError.DownloadedVideoDoesNotExist));
            }

            IAttemptResult<string> result = this._fileFactory.GetPlaylist(existingVideos, playlistName, type);

            if (!result.IsSucceeded||result.Data  is null)
            {
                return AttemptResult<int>.Fail(result.Message);
            }

            if (!this._directoryIO.Exists(folderPath))
            {
                IAttemptResult dirResult = this._directoryIO.CreateDirectory(folderPath);
                if (!dirResult.IsSucceeded)
                {
                    return AttemptResult<int>.Fail(dirResult.Message);
                }
            }

            IAttemptResult writeResult = this._fileIO.Write(Path.Combine(folderPath, $"playlist.{this.GetExt(type)}"), result.Data, encoding: this.GetEncording(type));
            if (!writeResult.IsSucceeded)
            {
                return AttemptResult<int>.Fail(writeResult.Message);
            }

            return AttemptResult<int>.Succeeded(allVideos - existingVideos.Count);
        }


        /// <summary>
        /// 拡張子を取得する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetExt(PlaylistType type)
        {
            return type switch
            {
                PlaylistType.Aimp => "aimppl4",
                _ => string.Empty,
            };
        }

        private Encoding GetEncording(PlaylistType type)
        {
            return type switch
            {
                PlaylistType.Aimp => Encoding.GetEncoding("utf-16"),
                _ => Encoding.UTF8
            };
        }

    }
}
