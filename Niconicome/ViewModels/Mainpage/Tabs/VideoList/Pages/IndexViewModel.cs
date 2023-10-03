using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Local.State.Style;
using Niconicome.Models.Local.State.Toast;
using Niconicome.Models.Playlist.V2.Manager;
using Niconicome.Models.Playlist.V2.Utils;
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
            this._disposable = new Disposable();

            WS::Mainpage.BlazorPageManager.RegisterNavigationManager(BlazorWindows.MainPage, navigation);
            this.InputText = new BindableProperty<string>("").AddTo(this.Bindables);
            this.IsProcessing = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.ConfirmMessage = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.IsIntegratedCheckboxChecked = new BindableProperty<bool>(false)
                .Subscribe(x =>
                {
                    foreach (var video in this.Videos)
                    {
                        video.IsSelected.Value = x;
                    }
                })
                .AddTo(this.Bindables);

            this.IsUpdating = WS::Mainpage.VideoListManager.IsUpdating.AsReadOnly().Subscribe(x => this.IsProcessing.Value = x).AddTo(this.Bindables).AddTo(this._disposable);

            this.ContextMenu = new ContextMenuViewModel();
            this.Bindables.Add(this.ContextMenu.Bindables);

            this.OutputViewModel = new OutputViewModel();
            this.Bindables.Add(this.OutputViewModel.Bindables);

            this.SortViewModel = new SortViewModel();
            this.SortViewModel.RegisterSortEventHandler(() =>
            {
                _ = this.LoadVideoAsync(false);
            });
            this.Bindables.Add(this.SortViewModel.Bindables);

            this.FilterViewModel = new FilterViewModel(this.InputText, this.Videos);
            this.FilterViewModel.RegisterFilterEventHandler(this.OnListChanged);

            this.InputContextMenuViewModel = new InputContextMenuViewModel(this.InputText);
            this.Bindables.Add(this.InputContextMenuViewModel.Bindables);

            this.ClipboardViewModel = new ClipboardViewModel();
            this.Bindables.Add(this.ClipboardViewModel.Bindables);
            this._disposable.Add(this.ClipboardViewModel);
            this.ClipboardViewModel.ClipboardContent.Subscribe(async x =>
            {
                SystemSounds.Asterisk.Play();
                await this.AddVideoAsync(x);
            });
        }


        #region field

        private readonly Disposable _disposable;

        private Action? _listChangedEventHandler;

        private Action? _toastMessageChangeHandler;

        private Action? _deletionHandler;

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
        /// プレイリストのID
        /// </summary>
        public string PlaylistID => WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist?.ID.ToString() ?? "";

        /// <summary>
        /// 入力値
        /// </summary>
        public IBindableProperty<string> InputText { get; init; }

        /// <summary>
        /// 削除の確認メッセージ
        /// </summary>
        public IBindableProperty<string> ConfirmMessage { get; init; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        public IBindableProperty<bool> IsProcessing { get; init; }

        /// <summary>
        /// 更新中フラグ
        /// </summary>
        public IReadonlyBindablePperty<bool> IsUpdating { get; init; }

        /// <summary>
        /// ヘッダーのチェックボックスのチェック状態
        /// </summary>
        public IBindableProperty<bool> IsIntegratedCheckboxChecked { get; init; }

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

        /// <summary>
        /// フィルター
        /// </summary>
        public FilterViewModel FilterViewModel { get; init; }

        /// <summary>
        /// 幅
        /// </summary>
        public VideoListWidthViewModel VideoListWidthViewModel { get; init; } = new();

        /// <summary>
        /// 入力欄のコンテキストメニュー
        /// </summary>
        public InputContextMenuViewModel InputContextMenuViewModel { get; init; }

        /// <summary>
        /// クリップボード
        /// </summary>
        public ClipboardViewModel ClipboardViewModel { get; init; }

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

        /// <summary>
        /// トーストメッセージハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterToastMessageChangeEventHandler(Action handler)
        {
            this._toastMessageChangeHandler += handler;
        }

        /// <summary>
        /// 削除確認ハンドラを登録
        /// </summary>
        /// <param name="landler"></param>
        public void RegisterConfirmOfDeletionHandler(Action landler)
        {
            this._deletionHandler += landler;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public async Task Initialize()
        {
            if (WS::Mainpage.VideoAndPlayListMigration.IsMigrationNeeded)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/migration/videos", BlazorWindows.MainPage);
            }

            WS::Mainpage.SnackbarHandler.RegisterToastHandler(this.ToastMessageChangeHandler);

            this._playlistChangeEventHandler = p =>
            {
                _ = this.LoadVideoAsync(true);
            };

            WS::Mainpage.PlaylistVideoContainer.AddPlaylistChangeEventHandler(this._playlistChangeEventHandler);

            await this.LoadVideoAsync(true);
        }

        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <returns></returns>
        public async Task AddVideoAsync(string content = "")
        {
            if (string.IsNullOrEmpty(content))
            {
                content = this.InputText.Value;
            }

            if (this.IsProcessing.Value || string.IsNullOrEmpty(content)) return;

            this.IsProcessing.Value = true;

            IAttemptResult<VideoRegistrationResult> result = await WS::Mainpage.VideoListManager.RegisterVideosAsync(content, (m, e) => WS::Mainpage.MessageHandler.AppendMessage(m, LocalConstant.SystemMessageDispacher, e));

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
        /// クリップボードから動画を登録
        /// </summary>
        /// <returns></returns>
        public async Task AddVideoFromClipbordAsync()
        {
            if (this.IsProcessing.Value) return;

            this.IsProcessing.Value = true;

            IAttemptResult<VideoRegistrationResult> result = await WS::Mainpage.VideoListManager.RegisterVideosFromClipbordAsync((m, e) => WS::Mainpage.MessageHandler.AppendMessage(m, LocalConstant.SystemMessageDispacher, e));

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
            }

            this.Videos.Clear();
            this.Videos.AddRange(WS::Mainpage.PlaylistVideoContainer.Videos.Select(v => this.Convert(v)));

            this.IsProcessing.Value = false;

        }

        /// <summary>
        /// ドロップ操作時
        /// </summary>
        /// <param name="idList"></param>
        public void OnDrop(string idList)
        {
            string message = WS.Mainpage.StringHandler.GetContent(IndexViewModelStringContent.ContentDropped);
            WS.Mainpage.SnackbarHandler.Enqueue(message);
            WS.Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);

            _ = this.AddVideoAsync(idList);
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
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null) return;
            var playlistID = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist.ID;

            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate($"/playlist/{playlistID}", BlazorWindows.MainPage);
        }

        /// <summary>
        /// プレイリスト編集ボタンがクリックされたとき
        /// </summary>
        public void OnSearchButtonClick()
        {
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate($"/search", BlazorWindows.MainPage);
        }

        /// <summary>
        /// 動画削除の確認を実施
        /// </summary>
        public void ConfirmBeforeDeletion()
        {
            this.ContextMenu.RemoveVideo();

            var selected = this.Videos.Where(v => v.IsSelected.Value).ToList();

            if (selected.Count == 0)
            {
                if (this.TryGetSelectedVideo(out VideoInfoViewModel? video))
                {
                    selected.Add(video);
                }
                else
                {
                    return;
                }
            }


            if (selected.Count == 1)
            {
                this.ConfirmMessage.Value = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.DeletionConfitmMessageSingle, selected[0].Title);
            }
            else
            {
                this.ConfirmMessage.Value = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.DeletionConfitmMessageSingle, selected[0].Title, selected.Count - 1);
            }

            try
            {
                this._deletionHandler?.Invoke();
            }
            catch { }
        }

        /// <summary>
        /// 動画を削除
        /// </summary>
        public void DeleteVideos()
        {
            var selected = this.Videos.Where(v => v.IsSelected.Value).ToList();

            if (selected.Count == 0)
            {
                if (this.TryGetSelectedVideo(out VideoInfoViewModel? video))
                {
                    selected.Add(video);
                }
                else
                {
                    return;
                }
            }

            IAttemptResult result = WS::Mainpage.VideoListManager.RemoveVideosFromCurrentPlaylist(selected.Select(v => v.VideoInfo).ToList().AsReadOnly());

            if (result.IsSucceeded)
            {
                string message = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.VideoDeleted, selected.Count);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
                WS::Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
            }
            else
            {
                WS::Mainpage.SnackbarHandler.Enqueue(result.Message ?? "");
                WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? "", LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }

            _ = this.LoadVideoAsync(false);

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

            await this.LoadVideoAsync(true);
        }

        /// <summary>
        /// 更新ボタンがクリックされたとき
        /// </summary>
        /// <returns></returns>
        public async Task OnUpdateButtonClick()
        {
            if (this.IsUpdating.Value) return;

            var selected = this.Videos.Where(v => v.IsSelected.Value).ToList();

            if (selected.Count == 0)
            {
                return;
            }

            IAttemptResult result = await WS::Mainpage.VideoListManager.UpdateVideosAsync(selected.Where(v => v.IsSelected.Value).Select(v => v.VideoInfo).ToList().AsReadOnly(), (s, e) => WS::Mainpage.MessageHandler.AppendMessage(s, LocalConstant.SystemMessageDispacher, e));

            if (result.IsSucceeded)
            {
                string message = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.VideoUpdated, selected.Count);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
                WS::Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
            }
            else
            {
                WS::Mainpage.SnackbarHandler.Enqueue(result.Message ?? "");
                WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? "", LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }

            _ = this.LoadVideoAsync(false);
        }

        public void OnCancelUpdateButtonClick()
        {
            WS::Mainpage.VideoListManager.CancelUpdate();
        }

        /// <summary>
        /// 動画詳細情報のボタンがクリックされたとき
        /// </summary>
        /// <param name="niconicoID"></param>
        public void OnVideoDetailButtonClick(string niconicoID)
        {
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate($"/video/{niconicoID}/", BlazorWindows.MainPage);
        }

        #endregion

        #region private

        /// <summary>
        /// 動画リストを更新する
        /// </summary>
        /// <returns></returns>
        private async Task LoadVideoAsync(bool setPath)
        {
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null) return;

            this.SortViewModel.SetValue(WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist);

            await WS::Mainpage.VideoListManager.LoadVideosAsync(setPath: setPath);

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

        /// <summary>
        /// コンテクストメニューで選択された動画を取得
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private bool TryGetSelectedVideo([NotNullWhen(true)] out VideoInfoViewModel? video)
        {
            if (this.ContextMenu.TargetNiconicoID is null)
            {
                video = null;
                return false;
            }

            video = this.Videos.FirstOrDefault(v => v.NiconicoId == this.ContextMenu.TargetNiconicoID);
            return video is not null;
        }

        #endregion

        public void Dispose()
        {
            this._disposable.Dispose();

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
            this.MouseTop = new BindableProperty<double>(0);
            this.MouseLeft = new BindableProperty<double>(0);
            this.IsMenuVisible = new BindableProperty<bool>(false).AddTo(this.Bindables);
        }

        #region field


        #endregion

        #region Prop

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<double> MouseTop { get; init; }

        public IBindableProperty<double> MouseLeft { get; init; }

        public IBindableProperty<bool> IsMenuVisible { get; init; }

        public string? TargetNiconicoID { get; private set; }

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
                this.TargetNiconicoID = niconicoID;
                return;
            }

            if (e.Button == 0)
            {
                this.IsMenuVisible.Value = false;
                this.TargetNiconicoID = null;
                return;
            }
        }

        public void RemoveVideo()
        {
            //実処理はIndexViewModelが担当
            this.HideMenu();
        }

        public void OpenInNiconico()
        {
            this.HideMenu();

            if (this.TargetNiconicoID is null)
            {
                return;
            }

            IAttemptResult result = WS::Mainpage.ExternalProcessUtils.StartProcess($"https://nico.ms/{this.TargetNiconicoID}");

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
            this.HideMenu();

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

        public void CopyDataToClipboardSingle(CopyTarget target)
        {
            if (this.TargetNiconicoID is null)
            {
                return;
            }

            IAttemptResult<IVideoInfo> vResult = WS::Mainpage.VideoListManager.GetVideoFromCurrentPlaylist(this.TargetNiconicoID);
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return;
            }

            var list = new List<IVideoInfo> { vResult.Data };

            this.CopyDataToClipboard(list.AsReadOnly(), target);

            this.HideMenu();

        }

        public void CopyDataToClipboardMulti(CopyTarget target)
        {
            IAttemptResult<IReadOnlyList<IVideoInfo>> vResult = WS::Mainpage.VideoListManager.GetSelectedVideoFromCurrentPlaylist();
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return;
            }

            if (vResult.Data.Count == 0)
            {
                this.CopyDataToClipboardSingle(target);
                return;
            }

            this.CopyDataToClipboard(vResult.Data, target);
        }

        #endregion

        #region prvate

        private void OpenInExternalApp(AppKind appKind)
        {

            if (this.TargetNiconicoID is null)
            {
                return;
            }

            IAttemptResult<IVideoInfo> videoResult = WS::Mainpage.VideoListManager.GetVideoFromCurrentPlaylist(this.TargetNiconicoID);
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

        private void CopyDataToClipboard(IReadOnlyList<IVideoInfo> source, CopyTarget target)
        {
            IAttemptResult result = WS::Mainpage.VideoInfoCopyManager.CopyInfomartion(source, target);

            if (result.IsSucceeded)
            {
                string message = WS::Mainpage.StringHandler.GetContent(IndexViewModelStringContent.InfomationCopied);
                WS::Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                WS::Mainpage.SnackbarHandler.Enqueue(message);
            }
            else
            {

                WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? "", LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                WS::Mainpage.SnackbarHandler.Enqueue(result.Message ?? "");
            }
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

    public class FilterViewModel
    {
        public FilterViewModel(IBindableProperty<string> inputText, List<VideoInfoViewModel> videos)
        {
            this._inputText = inputText;
            this._videos = videos;
        }

        #region field

        private readonly IBindableProperty<string> _inputText;

        private readonly List<VideoInfoViewModel> _videos;

        private Action? _filterEventHandler;

        #endregion

        #region Method

        public void RegisterFilterEventHandler(Action handler)
        {
            this._filterEventHandler = handler;
        }


        public void FilterByTag()
        {
            if (string.IsNullOrEmpty(this._inputText.Value))
            {
                return;
            }

            IReadOnlyList<IVideoInfo> filtered = WS::Mainpage.VideoInfoFilterManager.FilterByTags(this._videos.Select(v => v.VideoInfo).ToList().AsReadOnly(), this._inputText.Value);

            this._videos.Clear();
            this._videos.AddRange(filtered.Select(v => new VideoInfoViewModel(v)));

            this.OnFilter();
        }

        public void FilterByKeyword()
        {
            if (string.IsNullOrEmpty(this._inputText.Value))
            {
                return;
            }

            IReadOnlyList<IVideoInfo> filtered = WS::Mainpage.VideoInfoFilterManager.FilterByKeyWord(this._videos.Select(v => v.VideoInfo).ToList().AsReadOnly(), this._inputText.Value);

            this._videos.Clear();
            this._videos.AddRange(filtered.Select(v => new VideoInfoViewModel(v)));

            this.OnFilter();
        }

        public void ResetFilterState()
        {
            this._videos.Clear();
            this._videos.AddRange(WS::Mainpage.PlaylistVideoContainer.Videos.Select(v => new VideoInfoViewModel(v)));

            this.OnFilter();
        }

        #endregion

        #region private

        private void OnFilter()
        {
            try
            {
                this._filterEventHandler?.Invoke();
            }
            catch { }
        }


        #endregion
    }

    public class VideoListWidthViewModel
    {
        /// <summary>
        /// 幅を設定する
        /// </summary>
        public void SetWidth(string value, string name)
        {
            if (int.TryParse(value, out int width))
            {
                WS::Mainpage.VideoListWidthManager.SetWidth(width, this.Convert(name));
            }
        }

        /// <summary>
        /// 幅を取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetWidth(string name)
        {
            IAttemptResult<int> result = WS::Mainpage.VideoListWidthManager.GetWidth(this.Convert(name));
            if (result.IsSucceeded)
            {
                return result.Data;
            }
            else
            {
                return -1;
            }
        }

        private ColumnType Convert(string name)
        {
            return name switch
            {
                "CheckBoxColumn" => ColumnType.CheckBoxColumn,
                "ThumbnailColumn" => ColumnType.ThumbnailColumn,
                "TitleColumn" => ColumnType.TitleColumn,
                "UploadedDateTimeColumn" => ColumnType.UploadedDateTimeColumn,
                "IsDownloadedColumn" => ColumnType.IsDownloadedColumn,
                "ViewCountColumn" => ColumnType.ViewCountColumn,
                "CommentCountColumn" => ColumnType.CommentCountColumn,
                "MylistCountColumn" => ColumnType.MylistCountColumn,
                "LikeCountColumn" => ColumnType.LikeCountColumn,
                "MessageColumn" => ColumnType.MessageColumn,
                _ => ColumnType.MessageColumn,
            };
        }
    }

    public class InputContextMenuViewModel
    {
        public InputContextMenuViewModel(IBindableProperty<string> input)
        {
            this._input = input;
            this.MouseTop = new BindableProperty<double>(0);
            this.MouseLeft = new BindableProperty<double>(0);
            this.IsMenuVisible = new BindableProperty<bool>(false).AddTo(this.Bindables);
        }

        #region field

        private readonly IBindableProperty<string> _input;

        private Func<Task<string>>? _getSelectitonFunc;

        #endregion

        #region Prop

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<double> MouseTop { get; init; }

        public IBindableProperty<double> MouseLeft { get; init; }

        public IBindableProperty<bool> IsMenuVisible { get; init; }

        #endregion

        #region Method

        public void OnClick(MouseEventArgs e)
        {
            if (e.Button == 2)
            {
                this.MouseTop.Value = e.ClientY - 50;
                this.MouseLeft.Value = e.ClientX;
                this.IsMenuVisible.Value = true;
                return;
            }

            if (e.Button == 0)
            {
                this.IsMenuVisible.Value = false;
                return;
            }
        }

        /// <summary>
        /// 選択状況を取得する関数を登録
        /// </summary>
        /// <param name="func"></param>
        public void RegisterGetSelectionFunc(Func<Task<string>> func)
        {
            this._getSelectitonFunc = func;
        }

        /// <summary>
        /// 貼り付け
        /// </summary>
        public void OnPasteButtonClick()
        {
            this.IsMenuVisible.Value = false;

            IAttemptResult<string> clipBoardResult = WS.Mainpage.ClipbordManager.GetClipboardContent();
            if (!clipBoardResult.IsSucceeded || clipBoardResult.Data is null)
            {
                return;
            }

            string currentValue = this._input.Value;
            this._input.Value = currentValue + clipBoardResult.Data;
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <returns></returns>
        public async Task OnCopyClick()
        {
            this.IsMenuVisible.Value = false;

            if (this._getSelectitonFunc is null)
            {
                return;
            }

            string content;

            try
            {
                content = await this._getSelectitonFunc();
            }
            catch
            {
                return;
            }

            WS.Mainpage.ClipbordManager.SetToClipBoard(content);
        }

        #endregion

    }
}
