using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.Diagnostics;
using Niconicome.Extensions.System.Windows;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.ViewModels.Controls;
using Niconicome.ViewModels.Converter.KeyDown;
using Niconicome.ViewModels.Mainpage.Tabs;
using Niconicome.Views;
using Niconicome.Views.Mainpage;
using Prism.Events;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using EnumSettings = Niconicome.Models.Local.Settings.EnumSettingsValue;
using MaterialDesign = MaterialDesignThemes.Wpf;
using Net = Niconicome.Models.Domain.Network;
using Playlist = Niconicome.Models.Domain.Local.Playlist;
using Utils = Niconicome.Models.Domain.Utils;
using WS = Niconicome.Workspaces;
using Settings = Niconicome.Models.Domain.Local.Settings;

namespace Niconicome.ViewModels.Mainpage
{

    /// <summary>
    /// 動画一覧のVM
    /// </summary>
    class VideoListViewModel : TabViewModelBase, IDisposable
    {
        public VideoListViewModel(IEventAggregator ea) : this((message, button, image) => MaterialMessageBox.Show(message, button, image), ea)
        {

        }
        public VideoListViewModel(Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessageBox, IEventAggregator ea) : base("動画一覧", "")
        {
            //プレイリスト選択変更イベントを購読する
            WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Subscribe(_ => this.OnSelectedPlaylistChanged());

            //プレイリスト内容更新のイベントを購読する
            WS::Mainpage.VideoListContainer.ListChanged += (_, _) => this.VideoListUpdated();

            this.Videos = WS::Mainpage.VideoListContainer.Videos.ToReadOnlyReactiveCollection(v => new VideoInfoViewModel(v, (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value?.BookMarkedVideoID ?? -1) == v.Id.Value)).AddTo(this.disposables);

            this.IsTemporaryPlaylist = WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.ToReadOnlyReactiveProperty();

            this.showMessageBox = showMessageBox;

            this.SnackbarMessageQueue = WS::Mainpage.SnackbarHandler.Queue;

            this.ea = ea;


            //アイコン
            this.RefreshCommandIcon = WS::Mainpage.VideoRefreshManager.IsFetching.Select(value => value ? MaterialDesign::PackIconKind.Close : MaterialDesign::PackIconKind.Refresh).ToReadOnlyReactivePropertySlim();
            this.FilterIcon = new ReactivePropertySlim<MaterialDesign.PackIconKind>(MaterialDesign::PackIconKind.Filter);

            //幅
            int scWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWSelectColumnWid);
            int idWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWIDColumnWid);
            int titleWIdth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWTitleColumnWid);
            int uploadWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWUploadColumnWid);
            int vctWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWViewCountColumnWid);
            int dlfWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWDownloadedFlagColumnWid);
            int stWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWStateColumnWid);
            int tnWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWThumbColumnWid);
            int bmWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWBookMarkColumnWid);
            int economyWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWEconomyColumnWid);

            //展開状況を引き継ぐ
            var inheritExpandedState = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.InheritExpandedState);
            var expandAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ExpandAll);
            WS::Mainpage.PlaylistHandler.Refresh(expandAll, inheritExpandedState, true);
            WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value = WS::Mainpage.PlaylistTreeHandler.GetTmp();

            //インデックス
            this.SelectedIndex = WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.ToReactivePropertyAsSynchronized(x => x.Value);

            //すべて選択する
            this.IsSelectedAll = new ReactivePropertySlim<bool>().AddTo(this.disposables);
            this.IsSelectedAll.Subscribe(value =>
            {
                WS::Mainpage.VideoListContainer.ForEach(v => v.IsSelected.Value = value);
            });

            //カラム
            this.IdColumnTitle = WS::Mainpage.SortInfoHandler.IdColumnTitle.ToReadOnlyReactiveProperty().AddTo(this.disposables);
            this.TitleColumnTitle = WS::Mainpage.SortInfoHandler.TitleColumnTitle.ToReadOnlyReactiveProperty().AddTo(this.disposables);
            this.UploadColumnTitle = WS::Mainpage.SortInfoHandler.UploadColumnTitle.ToReadOnlyReactiveProperty().AddTo(this.disposables);
            this.ViewCountColumnTitle = WS::Mainpage.SortInfoHandler.ViewCountColumnTitle.ToReadOnlyReactiveProperty().AddTo(this.disposables);
            this.DlFlagColumnTitle = WS::Mainpage.SortInfoHandler.DlFlagColumnTitle.ToReadOnlyReactiveProperty().AddTo(this.disposables);
            this.StateColumnTitle = WS::Mainpage.SortInfoHandler.StateColumnTitle.ToReadOnlyReactiveProperty().AddTo(this.disposables);
            this.EconomyColumnTitle = WS::Mainpage.SortInfoHandler.EconomyColumnTitle.ToReadOnlyReactiveProperty().AddTo(this.disposables);

            #region Width系プロパティー

            var isRestoreEnable = !WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.NoRestoreClumnWIdth);

            this.SelectColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? scWidth <= 0 ? defaultWidth : scWidth : defaultWidth);
            this.IDColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? idWidth <= 0 ? defaultWidth : idWidth : defaultWidth);
            this.TitleColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? titleWIdth <= 0 ? defaultWidth : titleWIdth : defaultWidth);
            this.UploadColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? uploadWidth <= 0 ? defaultWidth : uploadWidth : defaultWidth);
            this.ViewCountColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? vctWidth <= 0 ? defaultWidth : vctWidth : defaultWidth);
            this.DownloadedFlagColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? dlfWidth <= 0 ? defaultWidth : dlfWidth : defaultWidth);
            this.StateColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? stWidth <= 0 ? defaultWidth : stWidth : defaultWidth);
            this.ThumbColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? tnWidth <= 0 ? defaultWidth : tnWidth : defaultWidth);
            this.BookMarkColumnWidth = new ReactivePropertySlim<int>(isRestoreEnable ? bmWidth < 0 ? defaultWidth : bmWidth : defaultWidth);
            this.EconomyColumnWidth = new ReactivePropertySlim<int>(isRestoreEnable ? economyWidth < 0 ? defaultWidth : economyWidth : defaultWidth);

            this.SelectColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWSelectColumnWid))
                .AddTo(this.disposables);
            this.IDColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWIDColumnWid))
                .AddTo(this.disposables);
            this.TitleColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWTitleColumnWid))
                .AddTo(this.disposables);
            this.UploadColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWUploadColumnWid))
                .AddTo(this.disposables);
            this.ViewCountColumnWidth
                .Throttle(TimeSpan.FromSeconds(5))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWViewCountColumnWid))
                .AddTo(this.disposables);
            this.DownloadedFlagColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWDownloadedFlagColumnWid))
                .AddTo(this.disposables);
            this.StateColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWStateColumnWid))
                .AddTo(this.disposables);
            this.ThumbColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWThumbColumnWid))
                .AddTo(this.disposables);
            this.BookMarkColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWBookMarkColumnWid))
                .AddTo(this.disposables);
            this.EconomyColumnWidth
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(value => WS::Mainpage.SettingHandler.SaveSetting(value, SettingsEnum.MWEconomyColumnWid))
                .AddTo(this.disposables);
            #endregion

            #region UI系要素
            this.ListItemHeight = WS::Mainpage.StyleHandler.UserChrome.Select(value => value?.MainPage.VideoList.ItemHeight ?? 100).ToReadOnlyReactiveProperty();
            this.TitleHeight = WS::Mainpage.StyleHandler.UserChrome.Select(value => value?.MainPage.VideoList.TitleHeight ?? 40).ToReadOnlyReactiveProperty();
            this.ButtonsHeight = WS::Mainpage.StyleHandler.UserChrome.Select(value => value?.MainPage.VideoList.ButtonsHeight ?? 50).ToReadOnlyReactiveProperty();

            WS::Mainpage.StyleHandler.UserChrome.Subscribe(value =>
            {
                //サムネ
                if (!(value?.MainPage.VideoList.Column.Thumbnail ?? true))
                {
                    this.ThumbColumnWidth.Value = 0;
                }
                else if (this.ThumbColumnWidth.Value == 0)
                {
                    this.ThumbColumnWidth.Value = defaultWidth;
                }

                //ID
                if (!(value?.MainPage.VideoList.Column.NiconicoID ?? true))
                {
                    this.IDColumnWidth.Value = 0;
                }
                else if (this.IDColumnWidth.Value == 0)
                {
                    this.IDColumnWidth.Value = defaultWidth;
                }

                //タイトル
                if (!(value?.MainPage.VideoList.Column.Title ?? true))
                {
                    this.TitleColumnWidth.Value = 0;
                }
                else if (this.TitleColumnWidth.Value == 0)
                {
                    this.TitleColumnWidth.Value = defaultWidth;
                }

                //投稿日時
                if (!(value?.MainPage.VideoList.Column.UploadedDT ?? true))
                {
                    this.UploadColumnWidth.Value = 0;
                }
                else if (this.UploadColumnWidth.Value == 0)
                {
                    this.UploadColumnWidth.Value = defaultWidth;
                }

                //再生回数
                if (!(value?.MainPage.VideoList.Column.ViewCount ?? true))
                {
                    this.ViewCountColumnWidth.Value = 0;
                }
                else if (this.ViewCountColumnWidth.Value == 0)
                {
                    this.ViewCountColumnWidth.Value = defaultWidth;
                }

                //DL済み
                if (!(value?.MainPage.VideoList.Column.DLFlag ?? true))
                {
                    this.DownloadedFlagColumnWidth.Value = 0;
                }
                else if (this.DownloadedFlagColumnWidth.Value == 0)
                {
                    this.DownloadedFlagColumnWidth.Value = defaultWidth;
                }

                //ブックマーク
                if (!(value?.MainPage.VideoList.Column.BookMark ?? true))
                {
                    this.BookMarkColumnWidth.Value = 0;
                }
                else if (this.BookMarkColumnWidth.Value == 0)
                {
                    this.BookMarkColumnWidth.Value = defaultWidth;
                }

                if (!(value?.MainPage.VideoList.Column.Economy ?? true))
                {
                    this.EconomyColumnWidth.Value = 0;
                }
                else if (this.EconomyColumnWidth.Value == 0)
                {
                    this.EconomyColumnWidth.Value = defaultWidth;
                }
            });
            #endregion

            #region クリップボード監視

            this.ClipbordMonitorIcon = WS::Mainpage.ClipbordManager.IsMonitoring
                .Select(value => value ? MaterialDesign::PackIconKind.ClipboardRemove : MaterialDesign::PackIconKind.ClipboardPulse)
                .ToReactiveProperty()
                .AddTo(this.disposables);

            WS::Mainpage.ClipbordManager.IsMonitoring
                .Skip(1)
                .Subscribe(value =>
            {
                if (value)
                {

                    WS::Mainpage.SnackbarHandler.Enqueue("クリップボードの監視を開始します。");
                    WS::Mainpage.Messagehandler.AppendMessage("クリップボードの監視を開始します。");
                }
                else
                {
                    WS::Mainpage.SnackbarHandler.Enqueue("クリップボードの監視を終了します。");
                    WS::Mainpage.Messagehandler.AppendMessage("クリップボードの監視を終了します。");
                }
            }).AddTo(this.disposables);

            this.ClipboardMonitoringToolTip = WS::Mainpage.ClipbordManager.IsMonitoring
                .Select(value => value ? "クリップボードの監視を終了する" : "クリップボードを監視する")
                .ToReadOnlyReactiveProperty();

            WS::Mainpage.ClipbordManager.RegisterClipboardChangeHandler(_ =>
            {
                SystemSounds.Asterisk.Play();
                this.AddVideoFromClipboardCommand?.Execute();
            });

            #endregion

            #region コマンドの初期化
            this.AddVideoCommand = new[] {
            WS::Mainpage.CurrentPlaylist.SelectedPlaylist
            .Select(p=>p is not null),
            WS::Mainpage.VideoIDHandler.IsProcessing
            .Select(f=>!f)
            }
            .CombineLatestValuesAreAllTrue()
            .ToReactiveCommand<object>()
            .WithSubscribe
            (async arg =>
              {
                  if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                  {
                      this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を追加できません");
                      return;
                  }

                  string niconicoId;
                  if (arg is not null and string)
                  {
                      niconicoId = (string)arg;
                  }
                  else if (this.InputString.Value != string.Empty)
                  {
                      niconicoId = this.InputString.Value;
                      this.InputString.Value = string.Empty;
                  }
                  else
                  {
                      return;
                  }

                  await this.RegisterVideoAsync(niconicoId, false);
              })
            .AddTo(this.disposables);

            this.OnKeyDownCommand = new ReactiveCommand<KeyEventInfo>()
                .WithSubscribe((info) =>
                {
                    if (info.Key == Key.Enter && this.AddVideoCommand.CanExecute())
                    {
                        this.AddVideoCommand.Execute(new object());
                    }
                });

            this.RemoveVideoCommand = new ReactiveCommand()
            .WithSubscribe(async () =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を削除できません");
                    return;
                }

                List<IListVideoInfo> targetVideos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value).ToList();

                if (targetVideos.Count == 0)
                {
                    int index = WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.Value;
                    if (index < 0 || WS::Mainpage.VideoListContainer.Count <= index) return;
                    targetVideos.Add(WS::Mainpage.VideoListContainer.Videos[index]);
                }


                string confirmMessage = targetVideos.Count == 1
                ? $"本当に「[{targetVideos[0]}」を削除しますか？"
                : $"本当に「[{targetVideos[0]}」ほか{targetVideos.Count - 1}件の動画を削除しますか？";


                var confirm = await this.showMessageBox(confirmMessage, MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);
                if (confirm != MaterialMessageBoxResult.Yes) return;

                WS::Mainpage.VideoListContainer.RemoveRange(targetVideos, commit: !WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);

                if (targetVideos.Count > 1)
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"{targetVideos.First().NiconicoId.Value}ほか{targetVideos.Count - 1}件の動画を削除しました。");
                    this.SnackbarMessageQueue.Enqueue($"{targetVideos.First().NiconicoId.Value}ほか{targetVideos.Count - 1}件の動画を削除しました。");
                }
                else
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"{targetVideos.First().NiconicoId.Value}を削除しました。");
                    this.SnackbarMessageQueue.Enqueue($"{targetVideos.First().NiconicoId.Value}を削除しました。");
                }

            });

            this.WatchOnNiconicoCommand = new CommandBase<VideoInfoViewModel>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を視聴できません");
                    return;
                }
                if (arg is null || arg is not VideoInfoViewModel videoInfo) return;
                try
                {
                    ProcessEx.StartWithShell($"https://nico.ms/{videoInfo.NiconicoId.Value}");
                }
                catch (Exception ex)
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"ブラウザー起動中に問題が発生しました。(詳細: {ex.Message})");
                    this.SnackbarMessageQueue.Enqueue($"ブラウザー起動中に問題が発生しました。(詳細: {ex.Message})");
                    return;
                }
            });

            this.EditPlaylistCommand = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p => p is not null)
                .ToReadOnlyReactivePropertySlim()
                .ToReactiveCommand()
                .WithSubscribe(() =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、プレイリスト情報を編集できません");
                    return;
                }

                WS::Mainpage.WindowsHelper.OpenWindow(() => new EditPlaylist
                {
                    Owner = Application.Current.MainWindow
                });
            })
                .AddTo(this.disposables);

            this.AddVideoFromClipboardCommand = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p => p is not null)
                .ToReadOnlyReactivePropertySlim()
                .ToReactiveCommand()
                .WithSubscribe
                (async () =>
                {
                    if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                    {
                        this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を追加できません");
                        return;
                    }

                    IAttemptResult<string> result = WS::Mainpage.ClipbordManager.GetClipboardContent();

                    if (!result.IsSucceeded || result.Data is null)
                    {
                        this.SnackbarMessageQueue.Enqueue("クリップボードの読み込みに失敗しました。");
                        return;
                    }
                    else if (result.Data.IsNullOrEmpty())
                    {
                        this.SnackbarMessageQueue.Enqueue("クリップボードが空です。");
                        return;
                    }

                    await this.RegisterVideoAsync(result.Data, true);
                })
                .AddTo(this.disposables);

            this.OpenNetworkSettingsCommand = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p => p is not null)
                .ToReadOnlyReactivePropertySlim()
                .ToReactiveCommand()
                .WithSubscribe(() =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null) return;

                if (!WS::Mainpage.Session.IsLogin.Value)
                {
                    this.SnackbarMessageQueue.Enqueue("リモートプレイリストを登録する為にはログインが必要です。");
                    return;
                }

                WS::Mainpage.WindowsHelper.OpenWindow(() => new NetworkVideoSettings()
                {
                    Owner = Application.Current.MainWindow
                });
            })
            .AddTo(this.disposables);

            this.UpdateVideoCommand = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p => p is not null)
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    if (WS::Mainpage.VideoRefreshManager.IsFetching.Value)
                    {
                        WS::Mainpage.VideoRefreshManager.Cancel();
                        return;
                    }

                    if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                    {

                        this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を更新できません");
                        return;
                    }

                    List<IListVideoInfo> videos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value).ToList();

                    if (videos.Count < 1) return;

                    this.SnackbarMessageQueue.Enqueue($"{videos.Count}件の動画を更新します。");
                    WS::Mainpage.Messagehandler.AppendMessage($"{videos.Count}件の動画を更新します。");

                    IAttemptResult<int> result = await WS::Mainpage.VideoRefreshManager.RefreshAndSaveAsync(videos);

                    this.SnackbarMessageQueue.Enqueue($"動画を更新しました。（{result.Data}件失敗）");
                    WS::Mainpage.Messagehandler.AppendMessage($"動画を更新しました。（{result.Data}件失敗）");

                })
            .AddTo(this.disposables);

            this.SyncWithNetowrkCommand = new[]
            {
                WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p=>p is not null),
                WS::Mainpage.VideoRefreshManager.IsFetching
                .Select(f=>!f),
                WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p=>p?.IsRemotePlaylist??false),
            }.CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    ITreePlaylistInfo? playlistInfo = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value;

                    if (!WS::Mainpage.Session.IsLogin.Value)
                    {
                        this.SnackbarMessageQueue.Enqueue("リモートプレイリストと同期する為にはログインが必要です。");
                        return;
                    }
                    else if (playlistInfo is null)
                    {
                        this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、同期できません");
                        return;
                    }
                    else if (!WS::Mainpage.VideoRefreshManager.CheckIfRemotePlaylistCanBeFetched(playlistInfo))
                    {
                        return;
                    }

                    IAttemptResult<string> result = await WS::Mainpage.VideoRefreshManager.RefreshRemoteAndSaveAsync();

                    if (result.IsSucceeded)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage($"同期が完了しました。({result.Message})");
                        this.SnackbarMessageQueue.Enqueue($"同期が完了しました。({result.Message})");
                    }
                    else
                    {
                        WS::Mainpage.Messagehandler.AppendMessage("情報の取得に失敗しました。");
                        this.SnackbarMessageQueue.Enqueue($"情報の取得に失敗しました。(詳細: {result.Message})");
                    }

                })
            .AddTo(this.disposables);

            this.FilterCommand = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p => p is not null)
                .ToReadOnlyReactivePropertySlim()
                .ToReactiveCommand()
                .WithSubscribe
                (() =>
              {
                  if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                  {
                      this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                      return;
                  }

                  if (this.isFiltered)
                  {
                      WS::Mainpage.VideoListContainer.Refresh();
                      this.isFiltered = false;
                      this.FilterIcon.Value = MaterialDesign::PackIconKind.Filter;
                  }
                  else if (!this.InputString.Value.IsNullOrEmpty())
                  {
                      FilterringOptions option = this.IsFilteringOnlyByTag.Value ? FilterringOptions.OnlyByTag : FilterringOptions.None;
                      IEnumerable<IListVideoInfo> videos;
                      if (this.IsFilteringFromAllVideos.Value)
                      {
                          IAttemptResult<IEnumerable<IListVideoInfo>> result = WS::Mainpage.VideoHandler.GetAllVideos();
                          if (!result.IsSucceeded || result.Data is null)
                          {
                              this.SnackbarMessageQueue.Enqueue("データベースからの動画情報の取得に失敗しました。");
                              return;
                          }
                          videos = result.Data;
                      }
                      else
                      {
                          videos = WS::Mainpage.VideoListContainer.Videos;
                      }

                      videos = WS::Mainpage.VideoFilter.FilterVideos(this.InputString.Value, videos, option);

                      WS::Mainpage.VideoListContainer.Clear();
                      WS::Mainpage.VideoListContainer.AddRange(videos, null, false);
                      this.isFiltered = true;
                      this.FilterIcon.Value = MaterialDesign::PackIconKind.FilterOff;
                  }
              })
            .AddTo(this.disposables);

            this.SearchCommand = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p => p is not null)
                .ToReadOnlyReactiveProperty()
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                    {
                        this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、検索画面を表示できません。");
                        return;
                    }

                    var window = new SearchPage()
                    {
                        Owner = Application.Current.MainWindow,
                    };
                    window.Show();
                })
            .AddTo(this.disposables);

            this.SelectAllVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos)
                {
                    video.IsSelected.Value = true;
                }
            });

            this.DisSelectAllVideosCommand = new CommandBase<object>(_ => true, _ =>
            {

                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos)
                {
                    video.IsSelected.Value = false;
                }
            });

            this.SelectAllNotDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => !v.VideoInfo.IsDownloaded.Value))
                {
                    video.IsSelected.Value = true;
                }
            });

            this.DisSelectAllDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => v.VideoInfo.IsDownloaded.Value))
                {
                    video.IsSelected.Value = false;
                }
            });

            this.DisSelectAllNotDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => !v.VideoInfo.IsDownloaded.Value))
                {
                    video.IsSelected.Value = false;
                }
            });

            this.SelectAllDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => v.VideoInfo.IsDownloaded.Value))
                {
                    video.IsSelected.Value = true;
                }
            });

            this.OpenPlaylistFolder = new ReactiveCommand<VideoInfoViewModel>()
                .WithSubscribe(video =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                string folderPath = video.VideoInfo.FolderPath.Value;
                if (folderPath.IsNullOrEmpty() || !Directory.Exists(folderPath)) return;
                try
                {
                    Process.Start("explorer.exe", folderPath);
                }
                catch { }
            });

            this.SendToappACommand = new CommandBase<object>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                if (arg is null || arg is not VideoInfoViewModel videoInfo || videoInfo is null) return;

                var result = WS::Mainpage.ExternalAppUtils.SendToAppA(videoInfo.VideoInfo);

                if (!result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue("コマンドの実行に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage(result.Message ?? "コマンドの実行に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage($"詳細:{result?.Exception?.Message ?? "None"}");
                }
            });

            this.SendToappBCommand = new CommandBase<object>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }

                if (arg is null || arg is not VideoInfoViewModel videoInfo || videoInfo is null) return;

                var result = WS::Mainpage.ExternalAppUtils.SendToAppB(videoInfo.VideoInfo);

                if (!result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue("コマンドの実行に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage(result.Message ?? "コマンドの実行に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage($"詳細:{result?.Exception?.Message ?? "None"}");
                }
            });

            this.OpenInPlayerAcommand = new CommandBase<object>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }

                if (arg is null || arg is not VideoInfoViewModel videoInfo || videoInfo is null) return;

                if (!videoInfo.VideoInfo.IsDownloaded.Value || videoInfo.VideoInfo.FileName.Value.IsNullOrEmpty())
                {
                    var reAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ReAllocateCommands);
                    if (reAll) this.SendToappACommand.Execute(arg);
                    return;
                }

                var result = WS::Mainpage.ExternalAppUtils.OpenInPlayerA(videoInfo.VideoInfo);

                if (!result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue("動画の再生に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage(result.Message ?? "動画の再生に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage($"詳細:{result?.Exception?.Message ?? "None"}");
                }
            });

            this.OpenInPlayerBcommand = new CommandBase<object>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }

                if (arg is null || arg is not VideoInfoViewModel videoInfo || videoInfo is null) return;

                if (!videoInfo.VideoInfo.IsDownloaded.Value || videoInfo.VideoInfo.FileName.Value.IsNullOrEmpty())
                {
                    var reAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ReAllocateCommands);
                    if (reAll) this.SendToappBCommand.Execute(arg);
                    return;
                }

                var result = WS::Mainpage.ExternalAppUtils.OpenInPlayerB(videoInfo.VideoInfo);

                if (!result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue("動画の再生に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage(result.Message ?? "動画の再生に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage($"詳細:{result?.Exception?.Message ?? "None"}");
                }
            });

            this.CreatePlaylistCommand = new CommandBase<string>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }

                if (arg is null || arg is not string || arg.IsNullOrEmpty()) return;
                Playlist::PlaylistType type = arg switch
                {
                    "aimp" => Playlist::PlaylistType.Aimp,
                    _ => Playlist::PlaylistType.Aimp,
                };

                IEnumerable<IListVideoInfo> videos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value && v.IsDownloaded.Value);

                if (!videos.Any()) return;

                IAttemptResult<int> result = WS::Mainpage.PlaylistCreator.TryCreatePlaylist(videos, type);

                if (result.IsSucceeded)
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"プレイリストを保存フォルダーに作成しました。({result.Data}件失敗)");
                    this.SnackbarMessageQueue.Enqueue($"プレイリストを保存フォルダーに作成しました。({result.Data}件失敗)");
                }
                else
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"プレイリストの作成に失敗しました。");
                    this.SnackbarMessageQueue.Enqueue($"プレイリストの作成に失敗しました。（{result.Message}）");
                }
            });

            this.VideoDoubleClickCommand = new ReactiveCommand<MouseEventArgs>()
                .WithSubscribe(e =>
                {
                    if (e.Source is not FrameworkElement source) return;
                    if (source.DataContext is not VideoInfoViewModel videoInfo) return;

                    IAttemptResult<ISettingInfo<EnumSettings::VideodbClickSettings>> result = WS::Mainpage.SettingsConainer.GetSetting(Settings::SettingNames.VideoListItemdbClickAction, EnumSettings::VideodbClickSettings.NotConfigured);

                    if (!result.IsSucceeded || result.Data is null) return;

                    var setting = result.Data.Value;

                    if (setting == EnumSettings::VideodbClickSettings.OpenInPlayerA)
                    {
                        this.OpenInPlayerAcommand.Execute(videoInfo);
                    }
                    else if (setting == EnumSettings::VideodbClickSettings.OpenInPlayerB)
                    {
                        this.OpenInPlayerBcommand.Execute(videoInfo);
                    }
                    else if (setting == EnumSettings::VideodbClickSettings.SendToAppA)
                    {
                        this.SendToappACommand.Execute(videoInfo);
                    }
                    else if (setting == EnumSettings::VideodbClickSettings.SendToAppB)
                    {
                        this.SendToappBCommand.Execute(videoInfo);
                    }
                    else if (setting == EnumSettings::VideodbClickSettings.Download)
                    {
                        this.ea.GetEvent<PubSubEvent<MVVMEvent<VideoInfoViewModel>>>().Publish(new MVVMEvent<VideoInfoViewModel>(videoInfo, typeof(DownloadSettingsViewModel), EventType.Download));
                    }
                }).AddTo(this.disposables)
            .AddTo(this.disposables);

            this.MonitorClipbordCommand = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.ClipbordManager.IsMonitoring.Value)
                    {
                        WS::Mainpage.ClipbordManager.StopMonitoring();
                    }
                    else
                    {
                        IAttemptResult result = WS::Mainpage.ClipbordManager.StartMonitoring();
                        if (!result.IsSucceeded)
                        {
                            this.SnackbarMessageQueue.Enqueue("クリップボードの監視に失敗しました。");
                            WS::Mainpage.Messagehandler.AppendMessage(result.Message ?? "クリップボードの監視に失敗しました。");
                        }
                    }
                }).AddTo(this.disposables);

            this.SavePlaylistCommand = this.IsTemporaryPlaylist
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    WS::Mainpage.SnackbarHandler.Enqueue("一時プレイリストの内容を保存します。");
                    List<IListVideoInfo> videos = this.Videos.Select(v => v.VideoInfo).ToList();
                    IAttemptResult<int> result = WS::Mainpage.PlaylistHandler.AddPlaylistToRoot();

                    if (!result.IsSucceeded)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage("プレイリストの新規作成に失敗しました。");
                        WS::Mainpage.SnackbarHandler.Enqueue("保存に失敗しました。");
                        return;
                    }

                    IAttemptResult vResult = WS::Mainpage.VideoListContainer.AddRange(videos, result.Data);

                    if (!vResult.IsSucceeded)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage($"動画の保存に失敗しました。(詳細:{result.Message})");
                        WS::Mainpage.SnackbarHandler.Enqueue("保存に失敗しました。");
                        return;
                    }

                    WS::Mainpage.Messagehandler.AppendMessage($"一時プレイリストの内容しました。");
                    WS::Mainpage.SnackbarHandler.Enqueue("保存に失敗しました。");
                });

            this.CopyOne = new ReactiveCommand<VideoProperties>()
                .WithSubscribe(type =>
                {
                    int index = WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.Value;
                    if (index < 0 || WS::Mainpage.VideoListContainer.Count <= index) return;

                    IListVideoInfo video = WS::Mainpage.VideoListContainer.Videos[index];
                    string source;

                    if (type == VideoProperties.NiconicoId)
                    {
                        source = video.NiconicoId.Value;
                    }
                    else if (type == VideoProperties.Title)
                    {
                        source = video.Title.Value;
                    }
                    else
                    {
                        source = Niconicome.Models.Const.NetConstant.NiconicoShortUrl + video.NiconicoId.Value;
                    }

                    try
                    {
                        Clipboard.SetText(source);
                    }
                    catch (Exception e)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage($"情報のコピーに失敗しました。(詳細:{e.Message})");
                        WS::Mainpage.SnackbarHandler.Enqueue("情報のコピーに失敗しました。");
                    }
                    WS::Mainpage.SnackbarHandler.Enqueue("情報をコピーしました。");
                });

            this.CopyAll = new ReactiveCommand<VideoProperties>()
                .WithSubscribe(type =>
                {
                    List<IListVideoInfo> videos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value).ToList();
                    var builder = new StringBuilder();

                    if (type == VideoProperties.NiconicoId)
                    {
                        foreach (IListVideoInfo video in videos)
                        {
                            builder.AppendLine(video.NiconicoId.Value);
                        }
                    }
                    else if (type == VideoProperties.Title)
                    {
                        foreach (IListVideoInfo video in videos)
                        {
                            builder.AppendLine(video.Title.Value);
                        }
                    }
                    else
                    {
                        foreach (IListVideoInfo video in videos)
                        {
                            builder.AppendLine(Niconicome.Models.Const.NetConstant.NiconicoShortUrl + video.NiconicoId.Value);
                        }
                    }

                    try
                    {
                        Clipboard.SetText(builder.ToString());
                    }
                    catch (Exception e)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage($"情報のコピーに失敗しました。(詳細:{e.Message})");
                        WS::Mainpage.SnackbarHandler.Enqueue("情報のコピーに失敗しました。");
                    }

                    WS::Mainpage.SnackbarHandler.Enqueue("情報をコピーしました。");
                });
            #endregion

        }

        ~VideoListViewModel()
        {
            this.Dispose();
        }


        #region コマンド
        /// <summary>
        /// 動画を追加する
        /// </summary>
        public ReactiveCommand<object> AddVideoCommand { get; init; }

        /// <summary>
        /// キー押下時
        /// </summary>
        public ReactiveCommand<KeyEventInfo> OnKeyDownCommand { get; init; }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        public ReactiveCommand RemoveVideoCommand { get; init; }

        /// <summary>
        /// ニコニコ動画で開く
        /// </summary>
        public CommandBase<VideoInfoViewModel> WatchOnNiconicoCommand { get; init; }

        /// <summary>
        /// クリップボードから動画を追加する
        /// </summary>
        public ReactiveCommand AddVideoFromClipboardCommand { get; init; }

        /// <summary>
        /// プレイリスト情報を編集する
        /// </summary>
        public ReactiveCommand EditPlaylistCommand { get; init; }

        /// <summary>
        /// ネットワーク上の動画を設定する
        /// </summary>
        public ReactiveCommand OpenNetworkSettingsCommand { get; init; }

        /// <summary>
        /// 動画情報を更新する
        /// </summary>
        public ReactiveCommand UpdateVideoCommand { get; init; }

        /// <summary>
        /// オンライン上のプレイリストと同期する
        /// </summary>
        public ReactiveCommand SyncWithNetowrkCommand { get; init; }

        /// <summary>
        /// 全ての動画を選択する
        /// </summary>
        public CommandBase<object> SelectAllVideosCommand { get; init; }

        /// <summary>
        /// 選択解除
        /// </summary>
        public CommandBase<object> DisSelectAllVideosCommand { get; init; }

        /// <summary>
        /// 未ダウンロードの動画を選択する
        /// </summary>
        public CommandBase<object> SelectAllNotDownloadedVideosCommand { get; init; }

        /// <summary>
        /// 未ダウンロードの動画を選択解除する
        /// </summary>
        public CommandBase<object> DisSelectAllNotDownloadedVideosCommand { get; init; }

        /// <summary>
        /// ダウンロード済の動画を選択する
        /// </summary>
        public CommandBase<object> SelectAllDownloadedVideosCommand { get; init; }

        /// <summary>
        /// 選択解除
        /// </summary>
        public CommandBase<object> DisSelectAllDownloadedVideosCommand { get; init; }

        /// <summary>
        /// 保存フォルダーを開く
        /// </summary>
        public ReactiveCommand<VideoInfoViewModel> OpenPlaylistFolder { get; init; }

        /// <summary>
        /// プレイヤーAで開く
        /// </summary>
        public CommandBase<object> OpenInPlayerAcommand { get; init; }

        /// <summary>
        /// プレイヤーBで開く
        /// </summary>
        public CommandBase<object> OpenInPlayerBcommand { get; init; }

        /// <summary>
        /// アプリにIdを送る
        /// </summary>
        public CommandBase<object> SendToappACommand { get; init; }

        /// <summary>
        /// アプリにIdを送る
        /// </summary>
        public CommandBase<object> SendToappBCommand { get; init; }

        /// <summary>
        /// フィルターコマンド
        /// </summary>
        public ReactiveCommand FilterCommand { get; init; }

        /// <summary>
        /// 検索コマンド
        /// </summary>
        public ReactiveCommand SearchCommand { get; init; }

        /// <summary>
        /// プレイリスト作成コマンド
        /// </summary>
        public CommandBase<string> CreatePlaylistCommand { get; init; }

        /// <summary>
        /// ダブルクリック
        /// </summary>
        public ReactiveCommand<MouseEventArgs> VideoDoubleClickCommand { get; init; }

        /// <summary>
        /// クリップボード監視コマンド
        /// </summary>
        public ReactiveCommand MonitorClipbordCommand { get; init; }

        /// <summary>
        /// 一時プレイリストを保存するコマンド
        /// </summary>
        public ReactiveCommand SavePlaylistCommand { get; init; }

        /// <summary>
        /// 動画情報をコピー
        /// </summary>
        public ReactiveCommand<VideoProperties> CopyOne { get; init; }

        /// <summary>
        /// 選択した動画情報をコピー
        /// </summary>
        public ReactiveCommand<VideoProperties> CopyAll { get; init; }

        #endregion

        #region Props

        /// <summary>
        /// フィルターアイコン
        /// </summary>
        public ReactivePropertySlim<MaterialDesign::PackIconKind> FilterIcon { get; init; }

        /// <summary>
        /// 更新アイコン
        /// </summary>
        public ReadOnlyReactivePropertySlim<MaterialDesign::PackIconKind> RefreshCommandIcon { get; init; }

        /// <summary>
        /// メッセージボックスを表示するコマンド
        /// </summary>
        private readonly Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessageBox;

        /// <summary>
        /// 動画のリスト
        /// </summary>
        public ReadOnlyReactiveCollection<VideoInfoViewModel> Videos { get; init; }

        /// <summary>
        /// スナックバー
        /// </summary>
        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }

        /// <summary>
        /// ユーザーの入力値
        /// </summary>
        public ReactivePropertySlim<string> InputString { get; init; } = new();

        /// <summary>
        /// 選択されている動画のインデックス
        /// </summary>
        public ReactiveProperty<int> SelectedIndex { get; init; }


        /// <summary>
        /// フィルターの設定
        /// </summary>
        public ReactivePropertySlim<bool> IsFilteringOnlyByTag { get; init; } = new();

        /// <summary>
        /// 全ての動画から検索する
        /// </summary>
        public ReactivePropertySlim<bool> IsFilteringFromAllVideos { get; init; } = new();

        /// <summary>
        /// プレイリストのタイトル
        /// </summary>
        public ReactivePropertySlim<string> PlaylistTitle { get; init; } = new();

        /// <summary>
        /// すべて選択
        /// </summary>
        public ReactivePropertySlim<bool> IsSelectedAll { get; init; }

        /// <summary>
        /// 一時プレイリスト
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsTemporaryPlaylist { get; init; }

        /// <summary>
        /// クリップボード監視アイコン
        /// </summary>
        public ReactiveProperty<MaterialDesign::PackIconKind> ClipbordMonitorIcon { get; init; } = new(MaterialDesign::PackIconKind.ClipboardPulse);

        /// <summary>
        /// クリップボード監視
        /// </summary>
        public ReadOnlyReactiveProperty<string?> ClipboardMonitoringToolTip { get; init; }

        #endregion

        #region UI系

        /// <summary>
        /// リストアイテムの高さ
        /// </summary>
        public ReadOnlyReactiveProperty<int> ListItemHeight { get; init; }

        /// <summary>
        /// タイトルの高さ
        /// </summary>
        public ReadOnlyReactiveProperty<int> TitleHeight { get; init; }

        /// <summary>
        /// ボタンの高さ
        /// </summary>
        public ReadOnlyReactiveProperty<int> ButtonsHeight { get; init; }


        #endregion

        #region カラムタイトル
        /// <summary>
        /// ID
        /// </summary>
        public ReadOnlyReactiveProperty<string?> IdColumnTitle { get; init; }

        /// <summary>
        /// タイトル
        /// </summary>
        public ReadOnlyReactiveProperty<string?> TitleColumnTitle { get; init; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        public ReadOnlyReactiveProperty<string?> UploadColumnTitle { get; init; }

        /// <summary>
        /// 再生回数
        /// </summary>
        public ReadOnlyReactiveProperty<string?> ViewCountColumnTitle { get; init; }

        /// <summary>
        /// DLフラグ
        /// </summary>
        public ReadOnlyReactiveProperty<string?> DlFlagColumnTitle { get; init; }

        /// <summary>
        /// 状態
        /// </summary>
        public ReadOnlyReactiveProperty<string?> StateColumnTitle { get; init; }

        /// <summary>
        /// エコノミー
        /// </summary>
        public ReadOnlyReactiveProperty<string?> EconomyColumnTitle { get; init; }

        #endregion

        #region Width

        /// <summary>
        /// 選択
        /// </summary>
        public ReactiveProperty<int> SelectColumnWidth { get; init; }

        /// <summary>
        /// ID
        /// </summary>
        public ReactiveProperty<int> IDColumnWidth { get; init; }

        /// <summary>
        /// タイトル
        /// </summary>
        public ReactiveProperty<int> TitleColumnWidth { get; init; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        public ReactiveProperty<int> UploadColumnWidth { get; init; }

        /// <summary>
        /// 再生回数
        /// </summary>
        public ReactiveProperty<int> ViewCountColumnWidth { get; init; }
        /// <summary>
        /// DL済み
        /// </summary>
        public ReactiveProperty<int> DownloadedFlagColumnWidth { get; init; }

        /// <summary>
        /// 状態
        /// </summary>
        public ReactiveProperty<int> StateColumnWidth { get; init; }

        /// <summary>
        /// サムネイル
        /// </summary>
        public ReactiveProperty<int> ThumbColumnWidth { get; init; }

        /// <summary>
        /// ブックマーク
        /// </summary>
        public ReactivePropertySlim<int> BookMarkColumnWidth { get; set; }

        /// <summary>
        /// エコノミー情報
        /// </summary>
        public ReactivePropertySlim<int> EconomyColumnWidth { get; set; }

        #endregion

        /// <summary>
        /// 並び替える
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="orderBy"></param>
        public void SetOrder(VideoSortType sortType, bool isDesscending)
        {
            if (WS::Mainpage.SortInfoHandler.SortType.Value == sortType)
            {
                WS::Mainpage.SortInfoHandler.IsDescending.Value = isDesscending;
            }
            else
            {
                WS::Mainpage.SortInfoHandler.SortType.Value = sortType;
            }

            var sortTypeStr = WS::Mainpage.SortInfoHandler.SortTypeStr.Value;
            var orderStr = WS::Mainpage.SortInfoHandler.IsDescendingStr.Value;

            WS::Mainpage.Messagehandler.AppendMessage($"動画を{sortTypeStr}の順に{orderStr}で並び替えました。");
            this.SnackbarMessageQueue.Enqueue($"動画を{sortTypeStr}の順に{orderStr}で並び替えました。");
        }

        #region field

        private const int defaultWidth = 150;

        #endregion

        #region private

        /// <summary>
        /// タスクキャンセラー
        /// </summary>

        private bool isFiltered;

        private readonly IEventAggregator ea;

        /// <summary>
        /// 動画を非同期に登録する
        /// </summary>
        /// <param name="data">入力値</param>
        /// <param name="isTextIsFromClipBoard">入力値がクリップボードからの入力であるかどうか</param>
        /// <returns></returns>
        private async Task RegisterVideoAsync(string data, bool isTextIsFromClipBoard)
        {
            this.SnackbarMessageQueue.Enqueue("動画を追加します");

            IAttemptResult<IEnumerable<IListVideoInfo>> result = await WS::Mainpage.VideoRegistrationHandler.ResgisterVideoAsync(data, isTextIsFromClipBoard);

            if (!result.IsSucceeded || result.Data is null)
            {
                this.SnackbarMessageQueue.Enqueue(result.Message?? "動画情報の取得に失敗しました");
                return;
            }

            List<IListVideoInfo> videos = result.Data.ToList();

            if (videos.Count == 0)
            {
                this.SnackbarMessageQueue.Enqueue("動画情報を1件も取得できませんでした");
                return;
            }

            this.SnackbarMessageQueue.Enqueue($"{videos.Count}件の動画を追加しました");
            WS::Mainpage.Messagehandler.AppendMessage($"{videos.Count}件の動画を追加しました");

            if (!videos[0].ChannelID.Value.IsNullOrEmpty())
            {
                IListVideoInfo firstVideo = videos[0];
                WS::Mainpage.SnackbarHandler.Enqueue($"この動画のチャンネルは「{firstVideo.ChannelName.Value}」です", "IDをコピー", () =>
                {
                    Clipboard.SetText(firstVideo.ChannelID.Value);
                    WS::Mainpage.SnackbarHandler.Enqueue("コピーしました");
                });
            }
        }

        /// <summary>
        /// 選択したプレイリストが変更された場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedPlaylistChanged()
        {
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Children.Count == 0)
            {
                string name = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Name.Value;
                WS::Mainpage.Messagehandler.AppendMessage($"プレイリスト:{name}");
                WS::Mainpage.SnackbarHandler.Enqueue($"プレイリスト:{name}");
                WS::Mainpage.VideoListContainer.Refresh();
            }
        }

        /// <summary>
        /// プレイリストのタイトルを変更する
        /// </summary>
        private void VideoListUpdated()
        {

            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null)
            {
                string name = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Name.Value;
                var count = WS::Mainpage.VideoListContainer.Count;
                this.PlaylistTitle.Value = $"{name}({count}件)";
            }
        }

        #endregion

    }

    /// <summary>
    /// デザイナー用のVM
    /// </summary>
    class VideoListViewModelD
    {
        public VideoListViewModelD()
        {
            var v1 = new VideoInfoViewModel("レッツゴー!陰陽師", "sm9", @"https://nicovideo.cdn.nimg.jp/thumbnails/9/9");
            var v2 = new VideoInfoViewModel("Bad Apple!! feat.nomico", "sm8628149", @"https://nicovideo.cdn.nimg.jp/thumbnails/8628149/8628149");

            this.Videos = new ReactiveCollection<VideoInfoViewModel>() { v1, v2 };

        }

        public ReactivePropertySlim<MaterialDesign::PackIconKind> FilterIcon { get; init; } = new(MaterialDesign::PackIconKind.Filter);

        public ReactivePropertySlim<MaterialDesign::PackIconKind> RefreshCommandIcon { get; init; } = new(MaterialDesign::PackIconKind.Refresh);

        public CommandBase<object> AddVideoCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand RemoveVideoCommand { get; init; } = new();

        public ReactiveCommand<KeyEventInfo> OnKeyDownCommand { get; init; } = new();

        public CommandBase<IListVideoInfo> WatchOnNiconicoCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand AddVideoFromClipboardCommand { get; init; } = new();

        public ReactiveCommand EditPlaylistCommand { get; init; } = new();

        public ReactiveCommand OpenNetworkSettingsCommand { get; init; } = new();

        public ReactiveCommand UpdateVideoCommand { get; init; } = new();

        public ReactiveCommand SyncWithNetowrkCommand { get; init; } = new();

        public CommandBase<object> SelectAllVideosCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> DisSelectAllVideosCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SelectAllNotDownloadedVideosCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> DisSelectAllNotDownloadedVideosCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SelectAllDownloadedVideosCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> DisSelectAllDownloadedVideosCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand<VideoInfoViewModel> OpenPlaylistFolder { get; init; } = new();

        public CommandBase<object> OpenInPlayerAcommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> OpenInPlayerBcommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SendToappACommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SendToappBCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand FilterCommand { get; init; } = new();

        public ReactiveCommand SearchCommand { get; init; } = new();

        public ReactiveCommand MonitorClipbordCommand { get; init; } = new();

        public ReactiveCommand SavePlaylistCommand { get; init; } = new();

        public ReactiveCommand BookMark { get; init; } = new();

        public CommandBase<string> CreatePlaylistCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand<MouseEventArgs> VideoDoubleClickCommand { get; init; } = new ReactiveCommand<MouseEventArgs>();

        public ReactiveCommand<VideoProperties> CopyOne { get; init; } = new();

        public ReactiveCommand<VideoProperties> CopyAll { get; init; } = new();

        public ReactiveCollection<VideoInfoViewModel> Videos { get; init; }

        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; } = new MaterialDesign::SnackbarMessageQueue();

        public ReactivePropertySlim<string> InputString { get; init; } = new("Hello World!!");

        public ReactivePropertySlim<bool> IsFilteringOnlyByTag { get; set; } = new();

        public ReactivePropertySlim<bool> IsFilteringFromAllVideos { get; set; } = new();

        public ReactivePropertySlim<string> PlaylistTitle { get; set; } = new("空白のプレイリスト");

        public ReactivePropertySlim<bool> IsSelectedAll { get; init; } = new();

        public ReactiveProperty<bool> IsTemporaryPlaylist { get; init; } = new(true);

        public ReactiveProperty<MaterialDesign::PackIconKind> ClipbordMonitorIcon { get; init; } = new(MaterialDesign::PackIconKind.ClipboardPulse);

        public ReactivePropertySlim<int> SelectColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> IDColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> TitleColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> UploadColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> ViewCountColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> DownloadedFlagColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> StateColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> ThumbColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> BookMarkColumnWidth { get; set; } = new(150);

        public ReactivePropertySlim<int> EconomyColumnWidth { get; set; } = new(150);

        public ReactiveProperty<int> ListItemHeight { get; init; } = new(100);

        public ReactiveProperty<int> TitleHeight { get; init; } = new(40);

        public ReactiveProperty<int> ButtonsHeight { get; init; } = new(50);

        public ReactiveProperty<bool> ThumbnailVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> NiconicoIDVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> TitleVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> UploadedDTVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> ViewCountVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> DLFlagVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> BookMarkVisibility { get; init; } = new(true);

        public ReactiveProperty<int> SelectedIndex { get; init; } = new();

        public ReactivePropertySlim<string> IdColumnTitle { get; init; } = new("ID");

        public ReactivePropertySlim<string> TitleColumnTitle { get; init; } = new("タイトル");

        public ReactivePropertySlim<string> UploadColumnTitle { get; init; } = new("投稿日時");

        public ReactivePropertySlim<string> ViewCountColumnTitle { get; init; } = new("再生回数");

        public ReactivePropertySlim<string> DlFlagColumnTitle { get; init; } = new("DL済み");

        public ReactivePropertySlim<string> StateColumnTitle { get; init; } = new("状態");

        public ReactivePropertySlim<string> EconomyColumnTitle { get; init; } = new("エコノミー");

        public ReactiveProperty<string> ClipboardMonitoringToolTip { get; init; } = new("クリップボードを監視する");

    }

    /// <summary>
    /// 動画一覧のビヘビアー
    /// </summary>
    class VideoListBehavior : Behavior<ListView>
    {
        public VideoListBehavior()
        {
            this.headerClickedHandler = new RoutedEventHandler(this.OnHeaderClicked);
        }

        /// <summary>
        /// 強制的に元のスクロール位置に戻すスクロール変更値の最小値
        /// </summary>
        private readonly double minimumVerticalOffsetChangeToFourceScroll = 100;

        protected override void OnAttached()
        {
            base.OnAttached();
            WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Subscribe(_ => this.OnSourceChanged());
            this.AssociatedObject.AddHandler(GridViewColumnHeader.ClickEvent, this.headerClickedHandler);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RemoveHandler(GridViewColumnHeader.ClickEvent, this.headerClickedHandler);
        }

        private readonly RoutedEventHandler headerClickedHandler;

        /// <summary>
        /// スクロール時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScroll(object? sender, ScrollChangedEventArgs e)
        {
            if (sender is null || sender is not ScrollViewer scrollViewer) return;
            if (WS::Mainpage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.DisableScrollRestore).Value) return;


            if (scrollViewer is null) return;

            double verticalChange = Math.Abs(this.scrollPos - e.VerticalOffset);

            if (e.VerticalOffset <= 0 && verticalChange > this.minimumVerticalOffsetChangeToFourceScroll && this.scrollPos > 0)
            {
                scrollViewer.ScrollToVerticalOffset(this.scrollPos);
            }
            else
            {
                this.scrollPos = e.VerticalOffset;
            }

        }

        /// <summary>
        /// スクロールイベントを登録する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSourceChanged()
        {
            var child = this.AssociatedObject.GetChildrenList<ScrollViewer>().FirstOrDefault();
            if (child is not null)
            {
                child.ScrollChanged -= this.OnScroll;
                child.ScrollChanged += this.OnScroll;
            }

            Debug.Write(this.AssociatedObject.View);
        }

        /// <summary>
        /// ヘッダーがクリックされたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHeaderClicked(object? sender, RoutedEventArgs e)
        {
            if (sender is not ListView listView) return;
            if (listView.DataContext is not VideoListViewModel context) return;
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null) return;
            if (e.OriginalSource is not GridViewColumnHeader header) return;
            if (header.Column.Header is null) return;

            string? headerString = header.Column.Header.ToString();
            if (headerString.IsNullOrEmpty()) return;

            var sortType = this.GetSortType(headerString);
            bool isDecending = sortType == WS::Mainpage.SortInfoHandler.SortType.Value && !WS::Mainpage.SortInfoHandler.IsDescending.Value;

            header.Tag = "Selected";

            context.SetOrder(sortType, isDecending);

        }


        /// <summary>
        /// 並び替えのタイプを取得する
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private VideoSortType GetSortType(string header)
        {
            if (header.StartsWith(SortInfoHandler.DefaultIdColumnTitle))
            {
                return VideoSortType.NiconicoID;
            }
            else if (header.StartsWith(SortInfoHandler.DefaultTitleColumnTitle))
            {
                return VideoSortType.Title;
            }
            else if (header.StartsWith(SortInfoHandler.DefaultUploadColumnTitle))
            {
                return VideoSortType.UploadedDT;
            }
            else if (header.StartsWith(SortInfoHandler.DefaultViewCountColumnTitle))
            {
                return VideoSortType.ViewCount;
            }
            else if (header.StartsWith(SortInfoHandler.DefaultDlFlagColumnTitle))
            {
                return VideoSortType.DownloadedFlag;
            }
            else if (header.StartsWith(SortInfoHandler.DefaultStateColumnTitle))
            {
                return VideoSortType.State;
            }
            else if (header.StartsWith(SortInfoHandler.DefaultEconomyColumnTitle))
            {
                return VideoSortType.Economy;
            }
            else
            {
                return VideoSortType.Register;
            }

        }

        private double scrollPos;
    }

    /// <summary>
    /// 動画情報のVM
    /// </summary>
    class VideoInfoViewModel : BindableBase, IDisposable
    {
        public VideoInfoViewModel(IListVideoInfo video, bool isBookMarked)
        {
            this.VideoInfo = video;
            this.Id = video.Id.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(this.disposables);
            this.ViewCount = video.ViewCount.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(this.disposables);
            this.NiconicoId = video.NiconicoId.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(this.disposables);
            this.Title = video.Title.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(this.disposables);
            this.Message = video.Message.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(this.disposables);
            this.ThumbPath = video.ThumbPath.ToReactivePropertyAsSynchronized(p => p.Value, origin =>
            {
                var dir = AppContext.BaseDirectory;
                if (video.ThumbPath.Value is null || video.ThumbPath.Value == string.Empty)
                {
                    var cacheHandler = Utils::DIFactory.Provider.GetRequiredService<Net::ICacheHandler>();
                    string cachePath = cacheHandler.GetCachePath("0", Net.CacheType.Thumbnail);
                    return Path.Combine(dir, cachePath);
                }
                else
                {
                    return Path.Combine(dir, video.ThumbPath.Value);
                }
            }, x => x).AddTo(this.disposables);
            this.IsSelected = video.IsSelected.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(this.disposables);
            this.IsDownloaded = video.IsDownloaded.ToReactivePropertyAsSynchronized(p => p.Value, p => p ? "○" : "×", x => x == "○").AddTo(this.disposables);
            this.UploadedOn = video.UploadedOn.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(this.disposables);
            this.IsThumbDownloading = video.IsThumbDownloading.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.disposables);
            this.BookMarkColor = new ReactiveProperty<System.Windows.Media.Brush>(isBookMarked ? Models.Domain.Utils.Utils.ConvertToBrush("#4580BC") : Models.Domain.Utils.Utils.ConvertToBrush("#E2EFFC"));
            this.IsEconomy = video.IsEconomy.CombineLatest(video.IsDownloaded,
                (isEconomy, isDownloaded) => isDownloaded ? isEconomy ? "エコノミー" : "非エコノミー" : "-")
                .ToReactiveProperty();

            this.BookMark = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    int index = WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.Value;
                    ITreePlaylistInfo? playlist = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value;

                    if (index < 0 || playlist is null) return;
                    IListVideoInfo video = WS::Mainpage.VideoListContainer.Videos[index];
                    int videoID = video.Id.Value;

                    if (video.Id.Value == playlist.BookMarkedVideoID)
                    {
                        videoID = -1;
                    }

                    WS::Mainpage.PlaylistHandler.BookMark(videoID, playlist.Id);
                    WS::Mainpage.VideoListContainer.Refresh();
                })
                .AddTo(this.disposables);

        }

        /// <summary>
        /// テスト用のコンストラクタ
        /// </summary>
        /// <param name="title"></param>
        /// <param name="ID"></param>
        /// <param name="thumbUrl"></param>
        public VideoInfoViewModel(string title, string ID, string thumbUrl)
        {
            this.Id = new ReactiveProperty<int>();
            this.ViewCount = new ReactiveProperty<int>();
            this.NiconicoId = new ReactiveProperty<string>(ID);
            this.Title = new ReactiveProperty<string>(title);
            this.Message = new ReactiveProperty<string>("テスト");
            this.ThumbPath = new ReactiveProperty<string>(thumbUrl);
            this.IsSelected = new ReactiveProperty<bool>(true);
            this.IsDownloaded = new ReactiveProperty<string>("〇");
            this.UploadedOn = new ReactiveProperty<DateTime>(DateTime.Now);
            this.IsThumbDownloading = new ReactiveProperty<bool>();
            this.BookMarkColor = new ReactiveProperty<System.Windows.Media.Brush>(Models.Domain.Utils.Utils.ConvertToBrush("#E2EFFC"));
            this.VideoInfo = new NonBindableListVideoInfo();
            this.IsEconomy = new ReactiveProperty<string?>("○");

            this.BookMark = new ReactiveCommand();
        }

        ~VideoInfoViewModel()
        {
            this.Dispose();
        }

        public IListVideoInfo VideoInfo { get; init; }

        public ReactiveProperty<int> Id { get; init; }

        public ReactiveProperty<int> ViewCount { get; init; }

        public ReactiveProperty<string> NiconicoId { get; init; }

        public ReactiveProperty<string> Title { get; init; }

        public ReactiveProperty<string> Message { get; init; }

        public ReactiveProperty<string> ThumbPath { get; init; }

        public ReactiveProperty<string> IsDownloaded { get; init; }

        public ReactiveProperty<string?> IsEconomy { get; init; }

        public ReactiveProperty<bool> IsSelected { get; init; }

        public ReactiveProperty<bool> IsThumbDownloading { get; init; }

        public ReactiveProperty<DateTime> UploadedOn { get; init; }

        public ReactiveProperty<System.Windows.Media.Brush> BookMarkColor { get; init; }

        /// <summary>
        /// ブックマークする
        /// </summary>
        public ReactiveCommand BookMark { get; init; }

    }

    /// <summary>
    /// プレイリストの並び替えパターン
    /// </summary>
    enum InsertType
    {
        Before,
        After,
        Child,
        None
    }

    enum VideoProperties
    {
        NiconicoId,
        Title,
        Url,
    }
}
