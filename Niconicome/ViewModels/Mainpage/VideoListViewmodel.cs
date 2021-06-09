using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.Diagnostics;
using Niconicome.Extensions.System.List;
using Niconicome.Extensions.System.Windows;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Utils;
using Niconicome.ViewModels.Controls;
using Niconicome.Views;
using Niconicome.Views.Mainpage;
using Prism.Events;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using EnumSettings = Niconicome.Models.Local.Settings.EnumSettingsValue;
using MaterialDesign = MaterialDesignThemes.Wpf;
using Net = Niconicome.Models.Domain.Network;
using Playlist = Niconicome.Models.Domain.Local.Playlist;
using PlaylistPlaylist = Niconicome.Models.Playlist.Playlist;
using Utils = Niconicome.Models.Domain.Utils;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{

    /// <summary>
    /// 動画一覧のVM
    /// </summary>
    class VideoListViewModel : BindableBase, IDisposable
    {
        public VideoListViewModel(IEventAggregator ea) : this((message, button, image) => MaterialMessageBox.Show(message, button, image), ea)
        {

        }
        public VideoListViewModel(Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessageBox, IEventAggregator ea)
        {
            //プレイリスト選択変更イベントを購読する
            WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Subscribe(_ => this.OnSelectedPlaylistChanged());

            //プレイリスト内容更新のイベントを購読する
            WS::Mainpage.VideoListContainer.ListChanged += (_, _) => this.VideoListUpdated();

            this.Videos = WS::Mainpage.VideoListContainer.Videos.ToReadOnlyReactiveCollection(v => new VideoInfoViewModel(v)).AddTo(this.disposables);

            this.showMessageBox = showMessageBox;

            this.SnackbarMessageQueue = WS::Mainpage.SnaclbarHandler.Queue;

            this.isFetching = new ReactiveProperty<bool>(false);
            this.ea = ea;

            //アイコン
            this.RefreshCommandIcon = new ReactivePropertySlim<MaterialDesign.PackIconKind>(MaterialDesign::PackIconKind.Refresh);
            this.FilterIcon = new ReactivePropertySlim<MaterialDesign.PackIconKind>(MaterialDesign::PackIconKind.Filter);

            //幅
            var scWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWSelectColumnWid);
            var idWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWIDColumnWid);
            var titleWIdth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWTitleColumnWid);
            var uploadWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWUploadColumnWid);
            var vctWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWViewCountColumnWid);
            var dlfWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWDownloadedFlagColumnWid);
            var stWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWStateColumnWid);
            var tnWidth = WS::Mainpage.SettingHandler.GetIntSetting(SettingsEnum.MWThumbColumnWid);

            //展開状況を引き継ぐ
            var inheritExpandedState = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.InheritExpandedState);
            var expandAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ExpandAll);
            WS::Mainpage.PlaylistHandler.Refresh(expandAll, inheritExpandedState);

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

            #region クリップボード監視

            this.isClipbordMonitoring = new ReactiveProperty<bool>();

            this.ClipbordMonitorIcon = this.isClipbordMonitoring
                .Select(value => value ? MaterialDesign::PackIconKind.ClipboardRemove : MaterialDesign::PackIconKind.ClipboardPulse)
                .ToReactiveProperty()
                .AddTo(this.disposables);

            this.isClipbordMonitoring
                .Skip(1)
                .Subscribe(value =>
            {
                if (value)
                {

                    WS::Mainpage.SnaclbarHandler.Enqueue("クリップボードの監視を開始します。");
                    WS::Mainpage.Messagehandler.AppendMessage("クリップボードの監視を開始します。");
                }
                else
                {
                    WS::Mainpage.SnaclbarHandler.Enqueue("クリップボードの監視を終了します。");
                    WS::Mainpage.Messagehandler.AppendMessage("クリップボードの監視を終了します。");
                }
            }).AddTo(this.disposables);

            this.ClipboardMonitoringToolTip = this.isClipbordMonitoring
                .Select(value => value ? "クリップボードの監視を終了する" : "クリップボードを監視する")
                .ToReadOnlyReactiveProperty();

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
                  int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Id;

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

                  var videos = new List<IListVideoInfo>();
                  var result = await WS::Mainpage.VideoIDHandler.TryGetVideoListInfosAsync(videos, niconicoId, WS::Mainpage.VideoListContainer.Videos.Select(v => v.NiconicoId.Value), m => this.SnackbarMessageQueue.Enqueue(m), m => WS::Mainpage.Messagehandler.AppendMessage(m));

                  if (result.IsFailed)
                  {
                      this.SnackbarMessageQueue.Enqueue("動画情報の取得に失敗しました");
                      return;
                  }
                  else if (videos.Count == 0)
                  {

                      this.SnackbarMessageQueue.Enqueue("動画情報を1件も取得できませんでした");
                      return;
                  }

                  WS::Mainpage.VideoListContainer.AddRange(videos, playlistId,!WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);

                  WS::Mainpage.VideoListContainer.Refresh();

                  this.SnackbarMessageQueue.Enqueue($"{videos.Count}件の動画を追加しました");
                  WS::Mainpage.Messagehandler.AppendMessage($"{videos.Count}件の動画を追加しました");

                  if (!videos.First().ChannelID.Value.IsNullOrEmpty())
                  {
                      var video = videos.First();
                      WS::Mainpage.SnaclbarHandler.Enqueue($"この動画のチャンネルは「{video.ChannelName.Value}」です", "IDをコピー", () =>
                      {
                          Clipboard.SetText(video.ChannelID.Value);
                          WS::Mainpage.SnaclbarHandler.Enqueue("コピーしました");
                      });
                  }
              })
            .AddTo(this.disposables);

            this.RemoveVideoCommand = new CommandBase<VideoInfoViewModel>(_ => true, async arg =>
             {
                 if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                 {
                     this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を削除できません");
                     return;
                 }

                 var targetVideos = new List<IListVideoInfo>();

                 if (arg is not null && arg is VideoInfoViewModel videoVM) targetVideos.Add(videoVM.VideoInfo);

                 targetVideos.AddRange(WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value));
                 targetVideos = targetVideos.Distinct(v => v.Id).ToList();

                 string confirmMessage = targetVideos.Count == 1
                 ? $"本当に「[{targetVideos[0].NiconicoId.Value}]{targetVideos[0].Title.Value}」を削除しますか？"
                 : $"本当に「[{targetVideos[0].NiconicoId.Value}]{targetVideos[0].Title.Value}」ほか{targetVideos.Count - 1}件の動画を削除しますか？";


                 var confirm = await this.showMessageBox(confirmMessage, MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);
                 if (confirm != MaterialMessageBoxResult.Yes) return;

                 foreach (var video in targetVideos)
                 {
                     //取得失敗動画の場合
                     if (video.Title.Value == "取得失敗")
                     {
                         WS::Mainpage.VideoListContainer.Remove(video, null, false);
                     }
                     else
                     {
                         WS::Mainpage.VideoListContainer.Remove(video, commit: !WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);
                     }
                 }

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
                var window = new EditPlaylist
                {
                    Owner = Application.Current.MainWindow
                };
                window.Show();
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

                int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Id;

                var data = Clipboard.GetText();
                if (data == string.Empty) return;

                Utils::INiconicoUtils reader = new Utils::NiconicoUtils();
                var ids = reader.GetNiconicoIdsFromText(data).Where(i => !WS::Mainpage.PlaylistHandler.ContainsVideo(i, playlistId)).ToList();

                var videos = (await WS::Mainpage.NetworkVideoHandler.GetVideoListInfosAsync(ids)).ToList();
                var result = WS::Mainpage.VideoListContainer.AddRange(videos, playlistId,!WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);
                WS::Mainpage.VideoListContainer.Refresh();

                if (result.IsSucceeded)
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"{videos.Count}件の動画を登録しました。");
                    this.SnackbarMessageQueue.Enqueue($"{videos.Count}件の動画を登録しました。");

                    if (videos.Count < ids.Count)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage($"{ids.Count - videos.Count}件の動画の追加に失敗しました。");
                    }
                }
                else
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"{ids.Count}件の動画の追加に失敗しました。");
                }

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

                var window = new NetworkVideoSettings()
                {
                    Owner = Application.Current.MainWindow
                };
                window.Show();
            })
            .AddTo(this.disposables);

            this.UpdateVideoCommand = new[]
            {
                WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p=>p is not null),
                this.isFetching
                .Select(f=>!f)
            }.CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    if (this.isFetching.Value)
                    {
                        this.cts?.Cancel();
                        this.RefreshCommandIcon.Value = MaterialDesign::PackIconKind.Refresh;
                        this.cts = null;
                        this.isFetching.Value = false;
                        return;
                    }

                    if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                    {

                        this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を更新できません");
                        return;
                    }
                    int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Id;

                    var sourceVideos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value).ToList();
                    int sourceVideosCount = sourceVideos.Count;

                    if (sourceVideosCount < 1) return;

                    this.SnackbarMessageQueue.Enqueue($"{sourceVideosCount}件の動画を更新します。");
                    WS::Mainpage.Messagehandler.AppendMessage($"{sourceVideosCount}件の動画を更新します。");

                    this.RefreshCommandIcon.Value = MaterialDesign::PackIconKind.Close;
                    this.isFetching.Value = true;

                    this.cts = new CancellationTokenSource();

                    var videos = (await WS::Mainpage.NetworkVideoHandler.GetVideoListInfosAsync(sourceVideos, true, playlistId, true, this.cts.Token)).ToList();
                    var result = WS::Mainpage.VideoListContainer.UpdateRange(videos);

                    this.isFetching.Value = false;
                    this.cts = null;
                    this.RefreshCommandIcon.Value = MaterialDesign::PackIconKind.Refresh;

                    this.SnackbarMessageQueue.Enqueue($"{videos.Count}件の動画を更新しました。");
                    WS::Mainpage.Messagehandler.AppendMessage($"{videos.Count}件の動画を更新しました。");

                    if (sourceVideosCount > videos.Count)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage($"{sourceVideosCount - videos.Count}件の動画の更新に失敗しました。");
                    }

                })
            .AddTo(this.disposables);

            this.SyncWithNetowrkCommand = new[]
            {
                WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p=>p is not null),
                this.isFetching
                .Select(f=>!f),
                WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(p=>p?.IsRemotePlaylist??false),
            }.CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {

                    if (!WS::Mainpage.Session.IsLogin.Value)
                    {
                        this.SnackbarMessageQueue.Enqueue("リモートプレイリストと同期する為にはログインが必要です。");
                        return;
                    }

                    if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                    {
                        this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、同期できません");
                        return;
                    }

                    if (!WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.IsRemotePlaylist || (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteId.IsNullOrEmpty() && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteType != PlaylistPlaylist::RemoteType.WatchLater)) return;

                    this.isFetching.Value = true;

                    int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Id;
                    var videos = new List<IListVideoInfo>();

                    var result = await WS::Mainpage.RemotePlaylistHandler.TryGetRemotePlaylistAsync(WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteId, videos, WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteType, WS::Mainpage.VideoListContainer.Videos.Select(v => v.NiconicoId.Value), m => WS::Mainpage.Messagehandler.AppendMessage(m));

                    if (result.IsSucceeded)
                    {

                        videos = videos.Where(v => !WS::Mainpage.PlaylistHandler.ContainsVideo(v.NiconicoId.Value, playlistId)).ToList();

                        if (videos.Count == 0)
                        {
                            WS::Mainpage.Messagehandler.AppendMessage($"追加するものはありません。");
                            this.SnackbarMessageQueue.Enqueue($"追加するものはありません。");
                            this.isFetching.Value = false;
                            return;
                        }

                        WS::Mainpage.VideoListContainer.AddRange(videos, playlistId, !WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);

                        if (videos.Count > 1)
                        {
                            WS::Mainpage.Messagehandler.AppendMessage($"{videos[0].NiconicoId.Value}ほか{videos.Count - 1}件の動画を追加しました。");
                            this.SnackbarMessageQueue.Enqueue($"{videos[0].NiconicoId.Value}ほか{videos.Count - 1}件の動画を追加しました。");
                            WS::Mainpage.VideoListContainer.Refresh();
                        }
                        else
                        {
                            WS::Mainpage.Messagehandler.AppendMessage($"{videos[0].NiconicoId.Value}を追加しました。");
                            this.SnackbarMessageQueue.Enqueue($"{videos[0].NiconicoId.Value}を追加しました。");
                        }
                    }
                    else
                    {
                        string detail = result.Exception?.Message ?? "None";
                        WS::Mainpage.Messagehandler.AppendMessage($"情報の取得に失敗しました。(詳細: {detail})");
                        this.SnackbarMessageQueue.Enqueue($"情報の取得に失敗しました。(詳細: {detail})");
                    }

                    this.isFetching.Value = false;
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
                      var videos = this.IsFilteringFromAllVideos.Value ? WS::Mainpage.VideoHandler.GetAllVideos() : WS::Mainpage.VideoListContainer.Videos;
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
                var folderPath = WS::Mainpage.CurrentPlaylist.PlaylistFolderPath;
                var videos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value && v.CheckDownloaded(folderPath));
                if (!videos.Any()) return;

                var result = WS::Mainpage.PlaylistCreator.TryCreatePlaylist(videos, WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Name.Value, folderPath, type);

                if (result)
                {
                    WS::Mainpage.Messagehandler.AppendMessage("プレイリストを保存フォルダーに作成しました。");
                    this.SnackbarMessageQueue.Enqueue("プレイリストを保存フォルダーに作成しました。");
                }
                else
                {
                    WS::Mainpage.Messagehandler.AppendMessage("プレイリストの作成に失敗しました。詳しくはログファイルを参照して下さい。");
                    this.SnackbarMessageQueue.Enqueue("プレイリストの作成に失敗しました。詳しくはログファイルを参照して下さい。");
                }
            });

            this.VideoDoubleClickCommand = new ReactiveCommand<MouseEventArgs>()
                .WithSubscribe(e =>
                {
                    if (e.Source is not FrameworkElement source) return;
                    if (source.DataContext is not VideoInfoViewModel videoInfo) return;

                    var setting = WS::Mainpage.EnumSettingsHandler.GetSetting<EnumSettings::VideodbClickSettings>();

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
                    if (!this.isClipbordMonitoring.Value)
                    {
                        if (!Compatibility.IsOsVersionLargerThan(10, 0, 10240))
                        {
                            WS::Mainpage.SnaclbarHandler.Enqueue("この機能はOSでサポートされていません。");
                            WS::Mainpage.Messagehandler.AppendMessage("この機能は'Windows10 1507'以降のOSでのみ利用可能です。");
                            return;
                        }

                        Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += this.OnContextMenuChange;
                        this.isClipbordMonitoring.Value = true;

                    }
                    else
                    {
                        if (!Compatibility.IsOsVersionLargerThan(10, 0, 10240)) return;
                        Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged -= this.OnContextMenuChange;
                        this.isClipbordMonitoring.Value = false;

                    }
                }).AddTo(this.disposables);

            #endregion

            #region Width系プロパティー

            var isRestoreEnable = !WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.NoRestoreClumnWIdth);
            const int defaultWidth = 150;

            this.SelectColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? scWidth <= 0 ? defaultWidth : scWidth : defaultWidth);
            this.IDColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? idWidth <= 0 ? defaultWidth : idWidth : defaultWidth);
            this.TitleColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? titleWIdth <= 0 ? defaultWidth : titleWIdth : defaultWidth);
            this.UploadColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? uploadWidth <= 0 ? defaultWidth : uploadWidth : defaultWidth);
            this.ViewCountColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? vctWidth <= 0 ? defaultWidth : vctWidth : defaultWidth);
            this.DownloadedFlagColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? dlfWidth <= 0 ? defaultWidth : dlfWidth : defaultWidth);
            this.StateColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? stWidth <= 0 ? defaultWidth : stWidth : defaultWidth);
            this.ThumbColumnWidth = new ReactiveProperty<int>(isRestoreEnable ? tnWidth <= 0 ? defaultWidth : tnWidth : defaultWidth);

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
            #endregion
        }

        ~VideoListViewModel()
        {
            this.Dispose();
        }

        /// <summary>
        /// フィルターアイコン
        /// </summary>
        public ReactivePropertySlim<MaterialDesign::PackIconKind> FilterIcon { get; init; }

        /// <summary>
        /// 更新アイコン
        /// </summary>
        public ReactivePropertySlim<MaterialDesign::PackIconKind> RefreshCommandIcon { get; init; }

        #region コマンド
        /// <summary>
        /// 動画を追加する
        /// </summary>
        public ReactiveCommand<object> AddVideoCommand { get; init; }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        public CommandBase<VideoInfoViewModel> RemoveVideoCommand { get; init; }

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

        #endregion

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
        /// クリップボード監視アイコン
        /// </summary>
        public ReactiveProperty<MaterialDesign::PackIconKind> ClipbordMonitorIcon { get; init; } = new(MaterialDesign::PackIconKind.ClipboardPulse);

        /// <summary>
        /// クリップボード監視
        /// </summary>
        public ReadOnlyReactiveProperty<string?> ClipboardMonitoringToolTip { get; init; }


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

        #region private

        /// <summary>
        /// タスクキャンセラー
        /// </summary>
        private CancellationTokenSource? cts;

        private readonly ReactiveProperty<bool> isFetching;

        private bool isFiltered;

        private ReactiveProperty<bool> isClipbordMonitoring;

        private readonly IEventAggregator ea;

        /// <summary>
        /// 選択したプレイリストが変更された場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedPlaylistChanged()
        {
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Children.Count == 0)
            {
                WS::Mainpage.Messagehandler.ClearMessage();
                string name = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Name.Value;
                WS::Mainpage.Messagehandler.AppendMessage($"プレイリスト:{name}");
                WS::Mainpage.SnaclbarHandler.Enqueue($"プレイリスト:{name}");
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

        /// <summary>
        /// コンテキストメニュー監視
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextMenuChange(object? sender, object? e)
        {
            SystemSounds.Asterisk.Play();
            this.AddVideoFromClipboardCommand.Execute();
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
            var v1 = new NonBindableListVideoInfo();
            v1.Title.Value = "レッツゴー!陰陽師";
            v1.NiconicoId.Value = "sm9";
            v1.IsDownloaded.Value = true;

            var v2 = new NonBindableListVideoInfo();
            v2.Title.Value = "Bad Apple!! feat. nomico";
            v2.NiconicoId.Value = "sm8628149";
            v2.IsDownloaded.Value = false;

            this.Videos = new ReactiveCollection<VideoInfoViewModel>();

        }

        public ReactivePropertySlim<MaterialDesign::PackIconKind> FilterIcon { get; init; } = new(MaterialDesign::PackIconKind.Filter);

        public ReactivePropertySlim<MaterialDesign::PackIconKind> RefreshCommandIcon { get; init; } = new(MaterialDesign::PackIconKind.Refresh);

        public CommandBase<object> AddVideoCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<VideoInfoViewModel> RemoveVideoCommand { get; init; } = new(_ => true, _ => { });

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

        public CommandBase<string> CreatePlaylistCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand<MouseEventArgs> VideoDoubleClickCommand { get; init; } = new ReactiveCommand<MouseEventArgs>();

        public ReactiveCollection<VideoInfoViewModel> Videos { get; init; }

        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; } = new MaterialDesign::SnackbarMessageQueue();

        public ReactivePropertySlim<string> InputString { get; init; } = new("Hello World!!");

        public ReactivePropertySlim<bool> IsFilteringOnlyByTag { get; set; } = new();

        public ReactivePropertySlim<bool> IsFilteringFromAllVideos { get; set; } = new();

        public ReactivePropertySlim<string> PlaylistTitle { get; set; } = new("空白のプレイリスト");

        public ReactivePropertySlim<bool> IsSelectedAll { get; init; } = new();

        public ReactiveProperty<MaterialDesign::PackIconKind> ClipbordMonitorIcon { get; init; } = new(MaterialDesign::PackIconKind.ClipboardPulse);

        public ReactivePropertySlim<int> SelectColumnWidth { get; set; } = new();

        public ReactivePropertySlim<int> IDColumnWidth { get; set; } = new();

        public ReactivePropertySlim<int> TitleColumnWidth { get; set; } = new();

        public ReactivePropertySlim<int> UploadColumnWidth { get; set; } = new();

        public ReactivePropertySlim<int> ViewCountColumnWidth { get; set; } = new();

        public ReactivePropertySlim<int> DownloadedFlagColumnWidth { get; set; } = new();

        public ReactivePropertySlim<int> StateColumnWidth { get; set; } = new();

        public ReactivePropertySlim<int> ThumbColumnWidth { get; set; } = new();

        public ReactiveProperty<int> SelectedIndex { get; init; } = new();

        public ReactivePropertySlim<string> IdColumnTitle { get; init; } = new("ID");

        public ReactivePropertySlim<string> TitleColumnTitle { get; init; } = new("タイトル");

        public ReactivePropertySlim<string> UploadColumnTitle { get; init; } = new("投稿日時");

        public ReactivePropertySlim<string> ViewCountColumnTitle { get; init; } = new("再生回数");

        public ReactivePropertySlim<string> DlFlagColumnTitle { get; init; } = new("DL済み");

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
        private readonly double minimumVerticalOffsetChangeToFourceScroll = 50;

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
        public VideoInfoViewModel(IListVideoInfo video)
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

        public ReactiveProperty<bool> IsSelected { get; init; }

        public ReactiveProperty<bool> IsThumbDownloading { get; init; }

        public ReactiveProperty<DateTime> UploadedOn { get; init; }

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

}
