using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Utils.Reactive;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class IndexViewModel
    {
        public IndexViewModel(NavigationManager navigation)
        {
            this._navigation = navigation;
        }

        ~IndexViewModel()
        {
            if (this._playlistChangeEventHandler is not null)
            {
                WS::Mainpage.PlaylistVideoContainer.RemovePlaylistChangeEventHandler(this._playlistChangeEventHandler);
            }
        }


        #region field

        private readonly NavigationManager _navigation;

        private Action? _listChangedEventHandler;

        private Action<IPlaylistInfo>? _playlistChangeEventHandler;

        private SynchronizationContext? _context;

        #endregion

        #region Props

        public List<VideoInfoViewModel> Videos { get; init; } = new List<VideoInfoViewModel>();

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// プレイリスト名
        /// </summary>
        public string PlaylistName => WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist?.Name.Value ?? "";

        #endregion

        #region Method

        /// <summary>
        /// 動画リスト変更イベントハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterListChangedEventHandler(Action handler)
        {
            if (this._context is null)
            {
                this._context = SynchronizationContext.Current;
            }
            this._listChangedEventHandler += handler;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public async Task Initialize()
        {
            if (WS::Mainpage.VideoAndPlayListMigration.IsMigrationNeeded)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/migration/videos");
                this._navigation.NavigateTo("/migration/videos");
            }

            this._playlistChangeEventHandler = p =>
            {
                _ = this.LoadVideoAsync();
            };

            WS::Mainpage.PlaylistVideoContainer.AddPlaylistChangeEventHandler(this._playlistChangeEventHandler);

            await this.LoadVideoAsync();
        }

        #endregion

        #region private

        /// <summary>
        /// 動画リストを更新する
        /// </summary>
        /// <returns></returns>
        private async Task LoadVideoAsync()
        {
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null) return;

            await WS::Mainpage.VideoListManager.LoadVideosAsync();

            this.Videos.Clear();
            this.Videos.AddRange(WS::Mainpage.PlaylistVideoContainer.Videos.Select(v => this.Convert(v)));

            this.OnListChanged();
        }

        /// <summary>
        /// 動画リスト変更イベントハンドラ
        /// </summary>
        private void OnListChanged()
        {
            if (this._context is null) return;

            try
            {
                this._context.Post(_ =>
                {
                    this._listChangedEventHandler?.Invoke();
                }, null);
            }
            catch { }
        }

        /// <summary>
        /// 動画情報をVMに変換
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private VideoInfoViewModel Convert(IVideoInfo video)
        {
            var vm = new VideoInfoViewModel(video);
            this.Bindables.Add(vm.Bindable);
            return vm;
        }

        #endregion
    }
}
