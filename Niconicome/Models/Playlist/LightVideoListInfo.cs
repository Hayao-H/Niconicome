using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Playlist.VideoList;
using Reactive.Bindings;

namespace Niconicome.Models.Playlist
{
    public interface ILightVideoListInfo
    {
        string NiconicoID { get; }

        int PlaylistID { get; }

        ReactiveProperty<bool> IsSelected { get; set; }

        ReactiveProperty<string> Message { get; set; }
    }

    public interface ILightVideoListinfoHandler
    {
        ILightVideoListInfo GetLightVideoListInfo(string niconicoID, int playlistId);
        void AddPlaylist(int playlistID);
    }

    /// <summary>
    /// ハンドラ
    /// </summary>
    public class LightVideoListinfoHandler : ILightVideoListinfoHandler
    {
        public LightVideoListinfoHandler(ICurrent current)
        {
            this.current = current;
        }

        #region field

        private readonly Dictionary<int, List<ILightVideoListInfo>> videoListInfos = new();

        private readonly ICurrent current;

        #endregion

        /// <summary>
        /// 動画譲歩を取得する
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        public ILightVideoListInfo GetLightVideoListInfo(string niconicoID, int playlistId)
        {
            List<ILightVideoListInfo> list = this.videoListInfos[playlistId];

            ILightVideoListInfo? video = list.Find(v => v.NiconicoID == niconicoID);
            if (video is null)
            {
                video = new LightVideoListInfo() { NiconicoID = niconicoID, PlaylistID = playlistId };
                video.IsSelected.Skip(1).Subscribe(value =>
                {
                    if ((this.current.SelectedPlaylist.Value?.Id ?? -1) == video.PlaylistID)
                    {
                        if (value)
                        {
                            this.current.SelectedVideos.Value++;
                        }
                        else
                        {
                            this.current.SelectedVideos.Value--;
                        }
                    }

                });
                list.Add(video);
            }

            return video;
        }

        /// <summary>
        /// プレイリストを追加する
        /// </summary>
        /// <param name="playlistID"></param>
        public void AddPlaylist(int playlistID)
        {
            if (!this.videoListInfos.ContainsKey(playlistID))
            {
                this.videoListInfos.Add(playlistID, new List<ILightVideoListInfo>());
            }
        }
    }

    /// <summary>
    /// データを保持しておくための軽量なクラス
    /// </summary>
    public class LightVideoListInfo : ILightVideoListInfo
    {

        public string NiconicoID { get; init; } = string.Empty;

        public int PlaylistID { get; init; }


        public ReactiveProperty<bool> IsSelected { get; set; } = new();

        public ReactiveProperty<string> Message { get; set; } = new();


    }
}
