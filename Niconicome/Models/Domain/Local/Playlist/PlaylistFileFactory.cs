using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Playlist
{

    public interface IPlaylistFileFactory
    {
        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="filepaths"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetPlaylist(IEnumerable<string> filepaths, string name, PlaylistType type);

        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IAttemptResult<string> GetPlaylist(IEnumerable<IVideoInfo> videos, string name, PlaylistType type);

    }

    public enum PlaylistType
    {
        Aimp
    }

    public class PlaylistFileFactory : IPlaylistFileFactory
    {
        public PlaylistFileFactory(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        #endregion

        public string GetPlaylist(IEnumerable<string> filepaths, string name, PlaylistType type)
        {
            var playlist = new Playlist()
            {
                PlaylistName = name,
            };
            playlist.AddRange(filepaths.Select(p => new Video(p, "", "")));
            if (playlist.Count == 0) throw new InvalidOperationException("空のプレイリストを作成することは出来ません。");

            IPlaylistHandler handler = type switch
            {
                PlaylistType.Aimp => new AIMP.AimpPlaylisthandler(),
                _ => throw new InvalidOperationException($"不明なプレイリストです。({type})")
            };

            return handler.CreatePlaylist(playlist);
        }

        public IAttemptResult<string> GetPlaylist(IEnumerable<IVideoInfo> videos, string name, PlaylistType type)
        {

            var playlist = new Playlist()
            {
                PlaylistName = name,
            };

            playlist.AddRange(videos.Where(v => !string.IsNullOrEmpty(v.Mp4FilePath)).Select(v => new Video(v.Mp4FilePath, v.Title, v.OwnerName)));

            if (playlist.Count == 0)
            {
                this._errorHandler.HandleError(PlaylistFileFactoryError.CannotCreateEmptyPlaylist);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(PlaylistFileFactoryError.CannotCreateEmptyPlaylist));
            }

            IPlaylistHandler? handler = type switch
            {
                PlaylistType.Aimp => new AIMP.AimpPlaylisthandler(),
                _ => null,
            };

            if (handler is null)
            {
                this._errorHandler.HandleError(PlaylistFileFactoryError.PlaylistIsNotSupported, type.ToString());
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(PlaylistFileFactoryError.PlaylistIsNotSupported, type.ToString()));
            }

            return AttemptResult<string>.Succeeded(handler.CreatePlaylist(playlist));
        }


    }
}
