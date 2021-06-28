using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Devices.Geolocation;

namespace Niconicome.Models.Playlist
{
    public interface IVideoInfoContainer
    {
        IListVideoInfo GetVideo(string id);
        int Count { get; }
        void Clear();
    }

    public class VideoInfoContainer : IVideoInfoContainer
    {
        #region field

        private readonly Dictionary<string, IListVideoInfo> videos = new();

        #endregion


        /// <summary>
        /// 動画を取得
        /// </summary>
        /// <param name="niconicoID"></param>
        public IListVideoInfo GetVideo(string niconicoID)
        {

            this.videos.TryGetValue(niconicoID, out IListVideoInfo? video);

            if (video is not null)
            {
                return video;
            }
            else
            {
                var newVideo = new NonBindableListVideoInfo();
                newVideo.NiconicoId.Value = niconicoID;
                this.videos.Add(niconicoID, newVideo);
                return newVideo;
            }
        }

        /// <summary>
        /// 動画数
        /// </summary>
        public int Count { get=>this.videos.Count; }


        /// <summary>
        /// キャッシュをクリア
        /// </summary>
        public void Clear()
        {
            this.videos.Clear();
        }


    }
}
