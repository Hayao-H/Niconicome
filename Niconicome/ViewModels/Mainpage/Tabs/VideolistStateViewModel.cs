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

namespace Niconicome.ViewModels.Mainpage
{
    class VideolistStateViewModel : TabViewModelBase
    {
        public VideolistStateViewModel() : base("状態", "")
        {
            WS::Mainpage.VideoListContainer.ListChanged += this.OnListChanged;
            WS::Mainpage.CurrentPlaylist.SelectedVideos.Subscribe(value => this.SelectedVideosCount.Value = value.ToString());
            this.RefreshCommand.Subscribe(() =>
            {
                this.SelectedVideosCount.Value = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value).Count().ToString();
            });
        }

        ~VideolistStateViewModel()
        {
            WS::Mainpage.VideoListContainer.ListChanged -= this.OnListChanged;
        }

        private void OnListChanged(object? sender, ListChangedEventArgs<Playlist::IListVideoInfo> e)
        {
            this.AllVideosCount.Value = WS::Mainpage.VideoListContainer.Videos.Count.ToString();
        }

        /// <summary>
        /// 選択された動画数
        /// </summary>
        public ReactiveProperty<string> SelectedVideosCount { get; init; } = new();

        /// <summary>
        /// すべての動画数
        /// </summary>
        public ReactiveProperty<string> AllVideosCount { get; init; } = new();

        /// <summary>
        /// 更新コマンド
        /// </summary>
        public ReactiveCommand RefreshCommand { get; init; } = new();

    }

    class VideolistStateViewModelD
    {
        public ReactiveProperty<string> SelectedVideosCount { get; init; } = new("5");

        public ReactiveProperty<string> AllVideosCount { get; init; } = new("10");

        public ReactiveCommand RefreshCommand { get; init; } = new();
    }
}
