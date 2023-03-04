using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Playlist.AIMP
{
    /// <summary>
    /// aimpで再生できるプレイリストを作成する
    /// </summary>
    class AimpPlaylisthandler : IPlaylistHandler
    {
        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="playlistData"></param>
        /// <returns></returns>
        public string CreatePlaylist(IPlaylist playlistData)
        {
            var listdata = new List<string>
            {
                "#-----SUMMARY-----#",
                $"Name={playlistData.PlaylistName}",
                string.Empty,
                "#-----CONTENT-----#",
                $"-{Path.GetDirectoryName(playlistData.Videos.First().Path)}",
            };
            listdata.AddRange(playlistData.Videos.Select(v => $"{v.Path}|{v.Title}|{v.OwnerName}|"));
            listdata.Add(string.Empty);
            return string.Join(Environment.NewLine, listdata);
        }

    }
}
