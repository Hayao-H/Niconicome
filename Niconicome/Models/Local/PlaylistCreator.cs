using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network;
using Niconicome.Models.Playlist.VideoList;
using Windows.System.Profile;
using Playlist = Niconicome.Models.Playlist;

namespace Niconicome.Models.Local
{
    public interface IPlaylistCreator
    {
        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="playlistName"></param>
        /// <param name="directoryPath"></param>
        /// <param name="type"></param>
        /// <returns>失敗した動画の数</returns>
        IAttemptResult<int> TryCreatePlaylist(IEnumerable<Playlist::IListVideoInfo> videos, PlaylistType type);
    }


    class PlaylistCreator : IPlaylistCreator
    {
        public PlaylistCreator(IPlaylistFileFactory fileFactory, ILogger logger, IVideoFileStorehandler videoFileStorehandler, INicoFileIO fileIO, INicoDirectoryIO directoryIO, ICurrent current)
        {
            this._fileFactory = fileFactory;
            this._logger = logger;
            this._videoFileStorehandler = videoFileStorehandler;
            this._fileIO = fileIO;
            this._directoryIO = directoryIO;
            this._current = current;
        }

        #region field

        private readonly IPlaylistFileFactory _fileFactory;

        private readonly INicoFileIO _fileIO;

        private readonly INicoDirectoryIO _directoryIO;

        private readonly IVideoFileStorehandler _videoFileStorehandler;

        private readonly ILogger _logger;

        private readonly ICurrent _current;

        #endregion

        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="directoryPath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IAttemptResult<int> TryCreatePlaylist(IEnumerable<Playlist::IListVideoInfo> videos, PlaylistType type)
        {
            if (this._current.SelectedPlaylist.Value is null) return AttemptResult<int>.Fail("プレイリストが選択されていません。");

            string playlistName = this._current.SelectedPlaylist.Value.Name.Value;
            string directoryPath = this._current.PlaylistFolderPath;

            videos = videos.Where(v => v.IsDownloaded.Value);
            if (!videos.Any()) return AttemptResult<int>.Fail("ダウンロードされた動画がありません。");

            int allVideos = videos.Count();
            List<string> videosPath = new();
            string data;
            try
            {
                videosPath = videos.Select(v => v.FileName.Value).Where(p => this._fileIO.Exists(p)).ToList();
                data = this._fileFactory.GetPlaylist(videosPath, playlistName, type);
            }
            catch (Exception e)
            {
                this._logger.Error("プレイリストの作成に失敗しました。", e);
                return AttemptResult<int>.Fail("プレイリストの作成に失敗しました。");
            }

            if (!this._directoryIO.Exists(directoryPath))
            {
                try
                {
                    this._directoryIO.Create(directoryPath);
                }
                catch (Exception e)
                {
                    this._logger.Error("ディレクトリーの作成に失敗しました。", e);
                    return AttemptResult<int>.Fail("ディレクトリーの作成に失敗しました。");
                }
            }

            try
            {
                this._fileIO.Write(Path.Combine(directoryPath, $"playlist.{this.GetExt(type)}"), data, encoding: this.GetEncording(type));
            }
            catch (Exception e)
            {
                this._logger.Error("プレイリストへの書き込みに失敗しました。", e);
                return AttemptResult<int>.Fail("プレイリストへの書き込みに失敗しました。");
            }

            return new AttemptResult<int>() { IsSucceeded = true, Data = allVideos - videosPath.Count };
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
