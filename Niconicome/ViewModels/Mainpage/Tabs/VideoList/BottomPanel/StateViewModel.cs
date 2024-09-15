using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.BottomPanel
{
    public class StateViewModel
    {
        public StateViewModel()
        {
            WS.PlaylistVideoContainer.AddPlaylistChangeEventHandler(p =>
            {
                this.SetCount(p);

                if (this._watched.Contains(p.ID))
                {
                    return;
                }

                p.SelectedVideosCount.Subscribe(count => this.SelectedVideosCount.Value = count);
                p.VideosCount.Subscribe(count => this.AllVideosCount.Value = count);

                this._watched.Add(p.ID);
            });

            this.Bindables.Add(this.SelectedVideosCount);
            this.Bindables.Add(this.AllVideosCount);

        }

        private readonly List<int> _watched = new();

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<int> SelectedVideosCount { get; init; } = new BindableProperty<int>(0);

        public IBindableProperty<int> AllVideosCount { get; init; } = new BindableProperty<int>(0);

        public void OnRefreshButtonClicked()
        {
            if (WS.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                return;
            }

            IPlaylistInfo playlist = WS.PlaylistVideoContainer.CurrentSelectedPlaylist;
            this.SetCount(playlist);
        }

        /// <summary>
        /// 動画数を設定する
        /// </summary>
        /// <param name="playlist"></param>
        private void SetCount(IPlaylistInfo playlist)
        {
            this.SelectedVideosCount.Value = playlist.SelectedVideosCount.Value;
            this.AllVideosCount.Value = playlist.VideosCount.Value;
        }

        public void OnRefreshButtonClick()
        {
            if (WS.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                return;
            }

            this.SetCount(WS.PlaylistVideoContainer.CurrentSelectedPlaylist);
        }
    }
}
