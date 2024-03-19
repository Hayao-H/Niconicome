using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class VideoDetailViewModel
    {
        public VideoDetailViewModel()
        {
            this.CanPlay = new BindableProperty<bool>(false).AddTo(this.Bindables);
        }

        #region Props

        /// <summary>
        /// 動画情報
        /// </summary>
        public VideoInfoViewModel? VideoInfo { get; private set; }

        /// <summary>
        /// 動画URL
        /// </summary>
        public string VideoUrl { get; private set; } = string.Empty;

        /// <summary>
        /// HLSで動画を再生できるかどうか
        /// </summary>
        public IBindableProperty<bool> CanPlay { get; init; }

        /// <summary>
        /// 変更監視
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// ローカルサーバーのポート
        /// </summary>
        public int ServerPort => WS::Mainpage.LocalState.Port;

        #endregion

        #region Method

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        public void Initialize(string niconicoID)
        {
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
                return;
            }

            IAttemptResult<IVideoInfo> result = WS::Mainpage.VideoListManager.GetVideoFromCurrentPlaylist(niconicoID);
            if (!result.IsSucceeded || result.Data is null)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
                return;
            }

            IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;

            var port = WS::Mainpage.LocalState.Port;
            this.VideoInfo = new VideoInfoViewModel(result.Data);
            this.VideoUrl = $"http://localhost:{port}/niconicome/video/{playlist.ID}/{niconicoID}/video.mp4";


            if (result.Data.IsDownloaded.Value)
            {
                if (result.Data.IsDMS)
                {
                    this.CanPlay.Value = true;
                    this.VideoUrl = $"http://localhost:{port}/niconicome/watch/v1/{playlist.ID}/{niconicoID}/main.m3u8";
                } else
                {
                    _ = Task.Run(async () =>
                    {
                        IAttemptResult hlsResult = await WS::Mainpage.HLSManager.CreateFilesAsync(niconicoID, playlist.ID);
                        if (hlsResult.IsSucceeded)
                        {
                            this.CanPlay.Value = true;
                            this.VideoUrl = $"http://localhost:{port}/niconicome/hls/playlist.m3u8";
                        }
                    });
                }
                
            }

        }

        public void OnReturnButtonClick()
        {
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
        }

        #endregion
    }
}
