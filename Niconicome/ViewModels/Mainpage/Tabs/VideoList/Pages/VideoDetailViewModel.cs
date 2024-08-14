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
using Const = Niconicome.Models.Const.NetConstant;
using Niconicome.Models.Domain.Niconico.Net.Json;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class VideoDetailViewModel
    {
        public VideoDetailViewModel()
        {
            this.JsWatchInfo = new BindableProperty<string>("").AddTo(this.Bindables);
        }

        #region Props

        public IBindableProperty<string> JsWatchInfo { get; init; }

        /// <summary>
        /// 変更監視
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        #endregion

        #region Method

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="niconicoID"></param>
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
            int port = WS::Mainpage.LocalState.Port;

            string info = WS.Mainpage.VideoInfoHandler.GetVideoInfoJson(port, niconicoID, playlist.ID);
            this.JsWatchInfo.Value = info;

        }

        public void OnReturnButtonClick()
        {
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
        }

        #endregion



    }
}
