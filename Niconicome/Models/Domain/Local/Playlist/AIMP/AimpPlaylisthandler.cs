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
            var paths = playlistData.GetAllFile();
            var listdata = new List<string>
            {
                "#-----SUMMARY-----#",
                $"Name={playlistData.PlaylistName}",
                string.Empty,
                "#-----CONTENT-----#",
                $"-{Path.GetDirectoryName(paths.First())}",
            };
            listdata.AddRange(paths);
            listdata.Add(string.Empty);
            return string.Join(Environment.NewLine, listdata);
        }

    }
}
