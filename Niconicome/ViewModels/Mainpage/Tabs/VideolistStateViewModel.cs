using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;
using Playlist = Niconicome.Models.Playlist;
using Videolist = Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Mainpage.Tabs;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.Models.Domain.Playlist;

namespace Niconicome.ViewModels.Mainpage
{
    class VideolistStateViewModel : TabViewModelBase
    {
        public VideolistStateViewModel() : base("状態", "")
        {
            WS::Mainpage.PlaylistVideoContainer.AddPlaylistChangeEventHandler(p =>
            {
                this.SetCount(p);

                if (this._alreadySubscribed.Contains(p.ID))
                {
                    return;
                }

                p.SelectedVideosCount.Subscribe(count => this.SelectedVideosCount.Value = count.ToString());
                p.VideosCount.Subscribe(count => this.AllVideosCount.Value = count.ToString());

                this._alreadySubscribed.Add(p.ID);
            });

            this.RefreshCommand = new BindableCommand(() =>
            {
                if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
                {
                    return;
                }

                IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;
                this.SetCount(playlist);

            }, new BindableProperty<bool>(true));

        }

        private List<int> _alreadySubscribed = new();

        /// <summary>
        /// 選択された動画数
        /// </summary>
        public IBindableProperty<string> SelectedVideosCount { get; init; } = new BindableProperty<string>(string.Empty);

        /// <summary>
        /// すべての動画数
        /// </summary>
        public IBindableProperty<string> AllVideosCount { get; init; } = new BindableProperty<string>(string.Empty);

        /// <summary>
        /// 更新コマンド
        /// </summary>
        public BindableCommand RefreshCommand { get; init; }

        /// <summary>
        /// 値を設定
        /// </summary>
        /// <param name="playlist"></param>
        private void SetCount(IPlaylistInfo playlist)
        {
            this.SelectedVideosCount.Value = playlist.SelectedVideosCount.Value.ToString();
            this.AllVideosCount.Value = playlist.VideosCount.Value.ToString();
        }

    }

    class VideolistStateViewModelD
    {
        public ReactiveProperty<string> SelectedVideosCount { get; init; } = new("5");

        public ReactiveProperty<string> AllVideosCount { get; init; } = new("10");

        public ReactiveCommand RefreshCommand { get; init; } = new();
    }
}
