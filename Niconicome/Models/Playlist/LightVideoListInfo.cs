using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Reactive.Bindings;

namespace Niconicome.Models.Playlist
{
    public interface ILightVideoListInfo
    {
        int ID { get; }
        ReactiveProperty<bool> IsSelected { get; set; }

        ReactiveProperty<string> Message { get; set; }
    }

    /// <summary>
    /// ハンドラ
    /// </summary>
    public static class LightVideoListinfoHandler
    {
        private static readonly Dictionary<int, List<ILightVideoListInfo>> videoListInfos = new();


        public static ILightVideoListInfo GetLightVideoListInfo(int videoId, int playlistId)
        {
            List<ILightVideoListInfo> list = LightVideoListinfoHandler.videoListInfos[playlistId];

            ILightVideoListInfo? video = list.Find(v=>v.ID==videoId);
            if (video is null)
            {
                video = new LightVideoListInfo() { ID = videoId };
                list.Add(video);
            }

            return video;
         }

        /// <summary>
        /// プレイリストを追加する
        /// </summary>
        /// <param name="playlistID"></param>
        public static void AddPlaylist(int playlistID)
        {
            if (!LightVideoListinfoHandler.videoListInfos.ContainsKey(playlistID))
            {
                LightVideoListinfoHandler.videoListInfos.Add(playlistID, new List<ILightVideoListInfo>());
            }
        }
    }

    /// <summary>
    /// データを保持しておくための軽量なクラス
    /// </summary>
    public class LightVideoListInfo : ILightVideoListInfo
    {

        public int ID { get; init; }

        public ReactiveProperty<bool> IsSelected { get; set; } = new();

        public ReactiveProperty<string> Message { get; set; } = new();


    }
}
