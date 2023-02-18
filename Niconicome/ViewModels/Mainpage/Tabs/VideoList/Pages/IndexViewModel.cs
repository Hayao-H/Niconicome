using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MS.WindowsAPICodePack.Internal;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Local.State.Toast;
using Niconicome.Models.Playlist.V2.Manager;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent;
using Niconicome.ViewModels.Shared;
using ExternalPlaylist = Niconicome.Models.Domain.Local.Playlist;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class IndexViewModel : IDisposable
    {
        public IndexViewModel(NavigationManager navigation)
        {
            this._navigation = navigation;
            this.InputText = new BindableProperty<string>("").AddTo(this.Bindables);
            this.IsProcessing = new BindableProperty<bool>(false).AddTo(this.Bindables);

            this.ContextMenu = new ContextMenuViewModel();
            this.Bindables.Add(this.ContextMenu.Bindables);

            this.OutputViewModel = new OutputViewModel();
            this.Bindables.Add(this.OutputViewModel.Bindables);

            this.SortViewModel = new SortViewModel();
            this.SortViewModel.RegisterSortEventHandler(() =>
            {
                _ = this.LoadVideoAsync(true);
            });
            this.Bindables.Add(this.SortViewModel.Bindables);

        }


        #region field

        private readonly NavigationManager _navigation;

        private Action? _listChangedEventHandler;

        private Action? _toastMessageChangeHandler;

        private Action<IPlaylistInfo>? _playlistChangeEventHandler;

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

        /// <summary>
        /// コンテクストメニュー
        /// </summary>
        public ContextMenuViewModel ContextMenu { get; init; }

        /// <summary>
        /// 出力
        /// </summary>
        public OutputViewModel OutputViewModel { get; init; }

        /// <summary>
        /// 並び替え
        /// </summary>
        public SortViewModel SortViewModel { get; init; }

        #endregion

        #region Method

        /// <summary>
        /// 動画リスト変更イベントハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterListChangedEventHandler(Action handler)
        {
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
                _ = this.LoadVideoAsync(false);
            };

            WS::Mainpage.PlaylistVideoContainer.AddPlaylistChangeEventHandler(this._playlistChangeEventHandler);

            await this.LoadVideoAsync(false);
        }

        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <returns></returns>
        public async Task AddVideoAsync()
        {
            if (this.IsProcessing.Value || string.IsNullOrEmpty(this.InputText.Value)) return;

            this.IsProcessing.Value = true;

            IAttemptResult<VideoRegistrationResult> result = await WS::Mainpage.VideoListManager.RegisterVideoAsync(this.InputText.Value, (m, e) => WS::Mainpage.MessageHandler.AppendMessage(m, LocalConstant.SystemMessageDispacher, e));

            if (!result.IsSucceeded || result.Data is null)
            {
                WS::Mainpage.SnackbarHandler.Enqueue(result.Message ?? string.Empty);
                WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? string.Empty, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
            }
            else
            {
                string message = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.VideoAdded, result.Data.VideosCount);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
                WS::Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);

                if (result.Data.IsChannelVideo)
                {
                    string channelMessage = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.NotifyChannelID, result.Data.ChannelName);
                    string action = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.CopyChannelIDAction);
                    WS::Mainpage.SnackbarHandler.Enqueue(channelMessage, action, () => WS::Mainpage.ClipbordManager.SetToClipBoard(result.Data.ChannelID));
                }
            }

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
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/playlist", BlazorWindows.MainPage);
            this._navigation.NavigateTo("/playlist");
        }

        /// <summary>
        /// リモートプレイリスト同期ボタンがクリックされたとき
        /// </summary>
        public async Task OnSyncWithRemotePlaylistButtonClick()
        {
            this.IsProcessing.Value = true;

            IAttemptResult result = await WS::Mainpage.VideoListManager.SyncWithRemotePlaylistAsync((m, e) => WS::Mainpage.MessageHandler.AppendMessage(m, LocalConstant.SystemMessageDispacher, e));

            if (!result.IsSucceeded) WS::Mainpage.SnackbarHandler.Enqueue(result.Message ?? "");

            this.IsProcessing.Value = false;

            await this.LoadVideoAsync(false);
        }

        #endregion

        #region private

        /// <summary>
        /// 動画リストを更新する
        /// </summary>
        /// <returns></returns>
        private async Task LoadVideoAsync(bool afterSort)
        {
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null) return;

            this.SortViewModel.SetValue(WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist);

            await WS::Mainpage.VideoListManager.LoadVideosAsync(setPath: !afterSort);

            this.Videos.Clear();
            this.Videos.AddRange(WS::Mainpage.PlaylistVideoContainer.Videos.Select(v => this.Convert(v)));

            this.OnListChanged();
        }

        /// <summary>
        /// 動画リスト変更イベントハンドラ
        /// </summary>
        private void OnListChanged()
        {

            try
            {
                this._listChangedEventHandler?.Invoke();
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

        public void Dispose()
        {
            if (this._playlistChangeEventHandler is not null)
            {
                WS::Mainpage.PlaylistVideoContainer.RemovePlaylistChangeEventHandler(this._playlistChangeEventHandler);
            }

            WS::Mainpage.SnackbarHandler.UnRegisterToastHandler(this.ToastMessageChangeHandler);
        }
    }

    public class ContextMenuViewModel
    {
        public ContextMenuViewModel()
        {
            this.MouseTop = new BindableProperty<double>(0).AddTo(this.Bindables);
            this.MouseLeft = new BindableProperty<double>(0).AddTo(this.Bindables);
            this.IsMenuVisible = new BindableProperty<bool>(false).AddTo(this.Bindables);
        }

        #region field

        private string? targetNiconicoID;

        #endregion

        #region Prop

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<double> MouseTop { get; init; }

        public IBindableProperty<double> MouseLeft { get; init; }

        public IBindableProperty<bool> IsMenuVisible { get; init; }

        #endregion

        #region Method

        public void OnClick(MouseEventArgs e, string niconicoID, int bodyHeight)
        {
            if (e.Button == 2)
            {
                if (bodyHeight < e.ClientY + 37 * 9)
                {
                    this.MouseTop.Value = bodyHeight - 38 * 9;
                }
                else
                {
                    this.MouseTop.Value = e.ClientY - 100;
                }
                this.MouseLeft.Value = e.ClientX;
                this.IsMenuVisible.Value = true;
                this.targetNiconicoID = niconicoID;
                return;
            }

            if (e.Button == 0)
            {
                this.IsMenuVisible.Value = false;
                this.targetNiconicoID = null;
                return;
            }
        }

        public void OpenInNiconico()
        {
            this.HideMenu();

            if (this.targetNiconicoID is null)
            {
                return;
            }

            IAttemptResult result = WS::Mainpage.ExternalProcessUtils.StartProcess($"https://nico.ms/{this.targetNiconicoID}");

            if (result.IsSucceeded)
            {
                string message = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.VideoOpenedInBrowser);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
                return;
            }
            else
            {
                string message = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.FailedToOpenVideoInBrowser);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
                return;
            }
        }

        public void OpenInPlayerA()
        {
            this.HideMenu();
            this.OpenInExternalApp(AppKind.PlayerA);
        }

        public void OpenInPlayerB()
        {
            this.HideMenu();
            this.OpenInExternalApp(AppKind.PlayerB);
        }

        public void SendToAppA()
        {
            this.HideMenu();
            this.OpenInExternalApp(AppKind.AppA);
        }

        public void SendToAppB()
        {
            this.HideMenu();
            this.OpenInExternalApp(AppKind.AppB);
        }

        public void Select(SelectTarget target)
        {
            this.HideMenu();

            foreach (var video in WS::Mainpage.PlaylistVideoContainer.Videos.Where(video => target switch
            {
                SelectTarget.NotDownloaded => !video.IsDownloaded.Value,
                SelectTarget.Downloaded => video.IsDownloaded.Value,
                _ => true
            }))
            {
                video.IsSelected.Value = true;
            }
        }

        public void UnSelect(SelectTarget target)
        {
            this.HideMenu();

            foreach (var video in WS::Mainpage.PlaylistVideoContainer.Videos.Where(video => target switch
            {
                SelectTarget.NotDownloaded => !video.IsDownloaded.Value,
                SelectTarget.Downloaded => video.IsDownloaded.Value,
                _ => true
            }))
            {
                video.IsSelected.Value = false;
            }
        }

        public void OpenDownloadDirectory()
        {
            this.HideMenu();

            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null) return;

            IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;
            if (this.TryGetFolderPath(playlist, out string path))
            {
                IAttemptResult result = WS::Mainpage.ExternalAppUtilsV2.OpenExplorer(path);
                if (result.IsSucceeded)
                {
                    WS::Mainpage.MessageHandler.AppendMessage(WS::Mainpage.StringHandler.GetContent(ContextMenuViewModelStringContent.SucceededToOpenExplorer), LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                    WS::Mainpage.SnackbarHandler.Enqueue(WS::Mainpage.StringHandler.GetContent(ContextMenuViewModelStringContent.SucceededToOpenExplorer));
                }
                else
                {
                    WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? string.Empty, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                }
            }

        }

        public void CreatePlaylist(PlaylistType type)
        {

            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null) return;

            IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;
            if (this.TryGetFolderPath(playlist, out string path))
            {
                ExternalPlaylist::PlaylistType pType = type switch
                {
                    PlaylistType.AIMP => ExternalPlaylist.PlaylistType.Aimp,
                    _ => ExternalPlaylist.PlaylistType.Aimp,
                };



                IAttemptResult<int> result = WS::Mainpage.PlaylistCreator.TryCreatePlaylist(WS::Mainpage.PlaylistVideoContainer.Videos.Where(v => v.IsSelected.Value).ToList().AsReadOnly(), playlist.Name.Value, path, pType);

                if (result.IsSucceeded)
                {
                    WS::Mainpage.MessageHandler.AppendMessage(WS::Mainpage.StringHandler.GetContent(ContextMenuViewModelStringContent.PlaylistCreated, result.Data), LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                    WS::Mainpage.SnackbarHandler.Enqueue(WS::Mainpage.StringHandler.GetContent(ContextMenuViewModelStringContent.PlaylistCreated, result.Data));
                }
                else
                {
                    WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? string.Empty, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                }
            }

        }

        #endregion

        #region prvate

        private void OpenInExternalApp(AppKind appKind)
        {

            if (this.targetNiconicoID is null)
            {
                return;
            }

            IAttemptResult<IVideoInfo> videoResult = WS::Mainpage.VideoListManager.GetVideoFromCurrentPlaylist(this.targetNiconicoID);
            if (!videoResult.IsSucceeded || videoResult.Data is null)
            {
                string message = WS::Mainpage.StringHandler.GetContent(OutputViewModelStringContent.VideoIsNotDownloaded);
                WS::Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
                return;
            }

            IAttemptResult result = appKind switch
            {
                AppKind.PlayerA => WS::Mainpage.ExternalAppUtilsV2.OpenInPlayerA(videoResult.Data),
                AppKind.PlayerB => WS::Mainpage.ExternalAppUtilsV2.OpenInPlayerB(videoResult.Data),
                AppKind.AppA => WS::Mainpage.ExternalAppUtilsV2.SendToAppA(videoResult.Data),
                _ => WS::Mainpage.ExternalAppUtilsV2.SendToAppB(videoResult.Data),
            };
            if (!result.IsSucceeded)
            {
                string message = WS::Mainpage.StringHandler.GetContent(appKind switch
                {
                    AppKind.PlayerA or AppKind.PlayerA => OutputViewModelStringContent.FailedToOpenInPlayer,
                    _ => OutputViewModelStringContent.FailedToLaunchApp,
                });
                WS::Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
                return;
            }
        }

        private void HideMenu()
        {
            this.IsMenuVisible.Value = false;
        }

        private bool TryGetFolderPath(IPlaylistInfo playlistInfo, out string path)
        {
            if (!playlistInfo.FolderPath.IsNullOrEmpty())
            {
                path = playlistInfo.FolderPath;
                return true;
            }

            if (!playlistInfo.TemporaryFolderPath.IsNullOrEmpty())
            {
                path = playlistInfo.TemporaryFolderPath;
                return true;
            }

            path = string.Empty;
            return false;
        }

        #endregion

        enum AppKind
        {
            PlayerA,
            PlayerB,
            AppA,
            AppB,
        }

        public enum SelectTarget
        {
            All,
            Downloaded,
            NotDownloaded,
        }

        public enum PlaylistType
        {
            AIMP
        }
    }

    public class SortViewModel
    {
        public SortViewModel()
        {
            var bindables = new Bindables();

            this.Visibility = new Dictionary<SortType, IBindableProperty<bool>>
            {
                { SortType.Title, new BindableProperty<bool>(false).AddTo(bindables) },
                { SortType.UploadedOn, new BindableProperty<bool>(false).AddTo(bindables) },
                { SortType.ViewCount, new BindableProperty<bool>(false).AddTo(bindables) },
                { SortType.CommentCount, new BindableProperty<bool>(false).AddTo(bindables) },
                { SortType.MylistCount, new BindableProperty<bool>(false).AddTo(bindables) },
                { SortType.LikeCount, new BindableProperty<bool>(false).AddTo(bindables) },
                { SortType.IsDownlaoded, new BindableProperty<bool>(false).AddTo(bindables) },
            };

            this.IsAscending = new BindableProperty<bool>(false).AddTo(bindables);
            this.SortOption = this.IsAscending.Select(x => x ? "Ascendant" : "Descendant").AsReadOnly();
            this.IsMenuVisible = new BindableProperty<bool>(false).AddTo(bindables);
            this.MenuLeft = new BindableProperty<double>(0);

            this.Bindables = bindables;
        }

        #region field

        private Action? _sortEventHandler;

        #endregion

        #region Props

        public Dictionary<SortType, IBindableProperty<bool>> Visibility { get; init; }

        public IReadonlyBindablePperty<string> SortOption { get; init; }

        public IBindableProperty<bool> IsAscending { get; init; }

        public IBindableProperty<bool> IsMenuVisible { get; init; }

        public IBindableProperty<double> MenuLeft { get; init; }

        public Bindables Bindables { get; init; }

        #endregion

        #region Method

        /// <summary>
        /// ヘッダークリック時
        /// </summary>
        /// <param name="target"></param>
        public void OnHeaderClick(SortType target)
        {
            this.IsMenuVisible.Value = false;

            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                return;
            }

            IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;

            if (playlist.SortType == target)
            {
                playlist.IsAscendant = !playlist.IsAscendant;
            }
            else
            {
                playlist.SortType = target;
            }

            this.SetValue(playlist);
            this.OnSort();
        }

        /// <summary>
        /// ヘッダーの表示を更新
        /// </summary>
        /// <param name="playlist"></param>
        public void SetValue(IPlaylistInfo playlist)
        {

            foreach (var visibility in this.Visibility)
            {
                if (!visibility.Value.Value)
                {
                    continue;
                }

                visibility.Value.Value = false;
            }

            if (this.Visibility.ContainsKey(playlist.SortType))
            {
                this.Visibility[playlist.SortType].Value = true;
            }

            this.IsAscending.Value = playlist.IsAscendant;

        }

        /// <summary>
        /// 動画を移動
        /// </summary>
        /// <param name="sourceID"></param>
        /// <param name="targetID"></param>
        public void MoveVideo(string sourceID, string targetID)
        {

            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                return;
            }

            WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist.MoveVideo(sourceID, targetID);
        }

        /// <summary>
        /// ソート状態を監視
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterSortEventHandler(Action handler)
        {
            this._sortEventHandler += handler;
        }

        /// <summary>
        /// ヘッダークリック時(コンテキストメニュー)
        /// </summary>
        /// <param name="e"></param>
        public void OnMenuClick(MouseEventArgs e)
        {
            if (e.Button == 2)
            {
                this.IsMenuVisible.Value = true;
                this.MenuLeft.Value = e.ClientX - 50;
            }
            else
            {
                this.IsMenuVisible.Value = false;
            }
        }

        /// <summary>
        /// ソート順を変更
        /// </summary>
        /// <param name="isAscendant"></param>
        public void SetSortDirection(bool isAscendant)
        {
            this.IsMenuVisible.Value = false;

            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                return;
            }

            IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;

            playlist.IsAscendant = isAscendant;

            this.SetValue(playlist);
            this.OnSort();
        }

        #endregion

        #region private

        private void OnSort()
        {
            this._sortEventHandler?.Invoke();
        }

        #endregion

    }


}
