﻿using System.Linq;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Domain.Local.External.Inport.Xeno
{
    public interface IXenoPlaylistConverter
    {
        ITreePlaylistInfo ConvertToTreePlaylistInfo(IXenoPlaylist playlist, bool recurse = true);
    }

    public class XenoPlaylistConverter : IXenoPlaylistConverter
    {
        /// <summary>
        /// Xenoのプレイリストをアプリケーション側で使えるデータに変換する
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public ITreePlaylistInfo ConvertToTreePlaylistInfo(IXenoPlaylist playlist, bool recurse = true)
        {
            var converted = new NonBindableTreePlaylistInfo()
            {
                Name = playlist.Name,
                IsRemotePlaylist = playlist.IsChannel,
                RemoteType = playlist.IsChannel ? RemoteType.Channel : RemoteType.None
            };

            converted.Videos.AddRange(playlist.Videos.Select(v => new NonBindableTreeVideoInfo() { NiconicoId = v }));

            if (playlist.ChildPlaylists.Count > 0 && recurse)
            {
                converted.Children.AddRange(playlist.ChildPlaylists.Select(c => this.ConvertToTreePlaylistInfo(c, true)));
            }

            return converted;
        }
    }
}
