using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface IPlaylistInfoContainer
    {
        /// <summary>
        /// プレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ITreePlaylistInfo GetPlaylist(int id);
    }

    public class PlaylistInfoContainer : IPlaylistInfoContainer
    {
        #region field

        Dictionary<int, ITreePlaylistInfo> innerList = new();

        #endregion

        public ITreePlaylistInfo GetPlaylist(int id)
        {
            bool result = this.innerList.TryGetValue(id, out ITreePlaylistInfo? playlist);
            if (result)
            {
                return playlist!;
            }
            else
            {
                var newPlaylist = new BindableTreePlaylistInfo();
                this.innerList.Add(id, newPlaylist);
                return newPlaylist;
            }
        }
    }
}
