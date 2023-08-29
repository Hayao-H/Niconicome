using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream
{
    public interface IPlaylistInfo
    {
        /// <summary>
        /// URL
        /// </summary>
        string AbsoluteURL { get; }

        /// <summary>
        /// 解像度
        /// </summary>
        IResolution Resolution { get; }

        /// <summary>
        /// 帯域
        /// </summary>
        long BandWidth { get; }
    }


    public class PlaylistInfo : IPlaylistInfo
    {
        public PlaylistInfo(string absoluteURL, IResolution resolution, long bandWidth)
        {
            this.AbsoluteURL = absoluteURL;
            this.Resolution = resolution;
            this.BandWidth = bandWidth;
        }

        #region Props
        public string AbsoluteURL { get; init; }

        public IResolution Resolution { get; init; }

        public long BandWidth { get; init; }

        #endregion

    }

}
