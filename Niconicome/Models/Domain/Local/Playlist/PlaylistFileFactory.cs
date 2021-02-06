using System;
using System.Collections.Generic;

namespace Niconicome.Models.Domain.Local.Playlist
{

    public interface IPlaylistFileFactory
    {
        string GetPlaylist(IEnumerable<string> filepaths, string name, PlaylistType type);
    }

    public enum PlaylistType
    {
        Aimp
    }

    public class PlaylistFileFactory : IPlaylistFileFactory
    {

        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="filepaths"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetPlaylist(IEnumerable<string> filepaths, string name, PlaylistType type)
        {
            var playlist = new Playlist()
            {
                PlaylistName = name,
            };
            playlist.AddRange(filepaths);
            if (playlist.Count == 0) throw new InvalidOperationException("空のプレイリストを作成することは出来ません。");

            IPlaylistHandler handler = type switch
            {
                PlaylistType.Aimp => new AIMP.AimpPlaylisthandler(),
                _ => throw new InvalidOperationException($"不明なプレイリストです。({type})")
            };

            return handler.CreatePlaylist(playlist);
        }

    }
}
