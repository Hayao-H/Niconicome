using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface IVideosUnchecker
    {
        /// <summary>
        /// 動画のチェックを外す
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        void Uncheck(string niconicoID, int playlistID);
    }

    public class VideosUnchecker : IVideosUnchecker
    {
        public VideosUnchecker(ILightVideoListinfoHandler handler)
        {
            this._handler = handler;
        }

        #region field

        private readonly ILightVideoListinfoHandler _handler;

        #endregion

        #region Method

        public void Uncheck(string niconicoID, int playlistID)
        {
            ILightVideoListInfo video = this._handler.GetLightVideoListInfo(niconicoID, playlistID);
            video.IsSelected.Value = false;
        }

        #endregion
    }
}
