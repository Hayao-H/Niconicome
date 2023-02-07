using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Local.State.Toast;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Shared;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class IndexViewModel
    {
        public IndexViewModel(NavigationManager navigation)
        {
            this._navigation = navigation;
            this.InputText = new BindableProperty<string>("").AddTo(this.Bindables);
            this.IsProcessing = new BindableProperty<bool>(false).AddTo(this.Bindables);
        }

        ~IndexViewModel()
        {
            if (this._playlistChangeEventHandler is not null)
            {
                WS::Mainpage.PlaylistVideoContainer.RemovePlaylistChangeEventHandler(this._playlistChangeEventHandler);
            }

            WS::Mainpage.SnackbarHandler.UnRegisterToastHandler(this.ToastMessageChangeHandler);
        }


        #region field

        private readonly NavigationManager _navigation;

        private Action? _listChangedEventHandler;

        private Action? _toastMessageChangeHandler;

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

        /// <summary>
        /// 入力値
        /// </summary>
        public IBindableProperty<string> InputText { get; init; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        public IBindableProperty<bool> IsProcessing { get; init; }

        /// <summary>
        /// トーストのメッセージ
        /// </summary>
        public ToastMessageViewModel? ToastMessage { get; private set; }

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

        public void RegisterToastMessageChangeEventHandler(Action handler)
        {
            this._toastMessageChangeHandler += handler;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public async Task Initialize()
        {
            if (WS::Mainpage.VideoAndPlayListMigration.IsMigrationNeeded)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/migration/videos", BlazorWindows.MainPage);
                this._navigation.NavigateTo("/migration/videos");
            }

            WS::Mainpage.SnackbarHandler.RegisterToastHandler(this.ToastMessageChangeHandler);

            this._playlistChangeEventHandler = p =>
            {
                _ = this.LoadVideoAsync();
            };

            WS::Mainpage.PlaylistVideoContainer.AddPlaylistChangeEventHandler(this._playlistChangeEventHandler);

            await this.LoadVideoAsync();
        }

        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <returns></returns>
        public async Task AddVideoAsync()
        {
            if (this.IsProcessing.Value || string.IsNullOrEmpty(this.InputText.Value)) return;

            this.IsProcessing.Value = true;

            await WS::Mainpage.VideoListManager.RegisterVideoAsync(this.InputText.Value, m => WS::Mainpage.Messagehandler.AppendMessage(m));

            this.Videos.Clear();
            this.Videos.AddRange(WS::Mainpage.PlaylistVideoContainer.Videos.Select(v => this.Convert(v)));

            this.InputText.Value = string.Empty;
            this.IsProcessing.Value = false;
        }

        /// <summary>
        /// エンターキー入力時
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Code == "Enter")
            {
                _ = this.AddVideoAsync();
            }
        }

        /// <summary>
        /// プレイリスト編集ボタンがクリックされたとき
        /// </summary>
        public void OnPlaylistEditButtonClick()
        {
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/playlist",BlazorWindows.MainPage);
            this._navigation.NavigateTo("/playlist");
        }

        /// <summary>
        /// リモートプレイリスト同期ボタンがクリックされたとき
        /// </summary>
        public async Task OnSyncWithRemotePlaylistButtonClick()
        {
            this.IsProcessing.Value = true;

            IAttemptResult result = await WS::Mainpage.VideoListManager.SyncWithRemotePlaylistAsync(m =>
            {
                WS::Mainpage.Messagehandler.AppendMessage(m);
            });

            if (result.IsSucceeded) WS::Mainpage.SnackbarHandler.Enqueue(result.Message ?? "");

            this.IsProcessing.Value = false;

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

        /// <summary>
        /// トースト監視
        /// </summary>
        /// <param name="message"></param>
        private void ToastMessageChangeHandler(IToastMessage message)
        {
            this.ToastMessage = new ToastMessageViewModel(message);
            this._toastMessageChangeHandler?.Invoke();
        }

        #endregion
    }
}
