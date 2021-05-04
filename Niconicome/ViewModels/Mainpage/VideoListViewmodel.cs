using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.Diagnostics;
using Niconicome.Extensions.System.List;
using Niconicome.Extensions.System.Windows;
using Niconicome.Models.Const;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist;
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
            WS::Mainpage.VideoListContainer.ListChanged += (_, e) => this.VideoListUpdated(e);

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
            this.selectColumnWidthField = scWidth <= 0 ? 150 : scWidth;
            this.iDColumnWidthField = idWidth <= 0 ? 150 : idWidth;
            this.titleColumnWidthField = titleWIdth <= 0 ? 150 : titleWIdth;
            this.uploadColumnWidthField = uploadWidth <= 0 ? 150 : uploadWidth;
            this.viewCountColumnWidthField = vctWidth <= 0 ? 150 : vctWidth;
            this.downloadedFlagColumnWidthField = dlfWidth <= 0 ? 150 : dlfWidth;
            this.stateColumnWidthField = stWidth <= 0 ? 150 : stWidth;
            this.thumbColumnWidthField = tnWidth <= 0 ? 150 : tnWidth;

            //展開状況を引き継ぐ
            var inheritExpandedState = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.InheritExpandedState);
            var expandAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ExpandAll);
            WS::Mainpage.PlaylistTree.Refresh(expandAll, inheritExpandedState);

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

                  WS::Mainpage.VideoListContainer.AddRange(videos, playlistId);

                  WS::Mainpage.PlaylistTree.Refresh();

                  this.SnackbarMessageQueue.Enqueue($"{videos.Count}件の動画を追加しました");

                  if (!videos.First().ChannelID.Value.IsNullOrEmpty())
                  {
                      var video = videos.First();
                      WS::Mainpage.SnaclbarHandler.Enqueue($"この動画のチャンネルは「{video.ChannelName}」です", "IDをコピー", () =>
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
                         WS::Mainpage.VideoListContainer.Remove(video);
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

            this.WatchOnNiconicoCommand = new CommandBase<IListVideoInfo>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を視聴できません");
                    return;
                }
                if (arg is null || arg.AsNullable<IListVideoInfo>() is not IListVideoInfo videoInfo || videoInfo is null) return;
                try
                {
                    ProcessEx.StartWithShell($"https://nico.ms/{videoInfo.NiconicoId}");
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
                var ids = reader.GetNiconicoIdsFromText(data).Where(i => !WS::Mainpage.PlaylistTree.ContainsVideo(i, playlistId)).ToList();

                var videos = (await WS::Mainpage.NetworkVideoHandler.GetVideoListInfosAsync(ids)).ToList();
                var result = WS::Mainpage.VideoListContainer.AddRange(videos, playlistId);

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

                if (!WS::Mainpage.Session.IsLogin)
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

                    var sourceVideos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value);
                    int sourceVideosCount = sourceVideos.Count();

                    if (sourceVideosCount < 1) return;

                    this.SnackbarMessageQueue.Enqueue($"{sourceVideosCount}件の動画を更新します。");
                    WS::Mainpage.Messagehandler.AppendMessage($"{sourceVideosCount}件の動画を更新します。");

                    this.RefreshCommandIcon.Value = MaterialDesign::PackIconKind.Close;
                    this.isFetching.Value = true;

                    this.cts = new CancellationTokenSource();

                    var videos = (await WS::Mainpage.NetworkVideoHandler.GetVideoListInfosAsync(sourceVideos, true, playlistId, this.cts.Token)).ToList();
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
                .Select(f=>!f)
            }.CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {

                    if (!WS::Mainpage.Session.IsLogin)
                    {
                        this.SnackbarMessageQueue.Enqueue("リモートプレイリストと同期する為にはログインが必要です。");
                        return;
                    }

                    if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null)
                    {
                        this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、同期できません");
                        return;
                    }

                    if (!WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.IsRemotePlaylist || (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteId.IsNullOrEmpty() && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteType != RemoteType.WatchLater)) return;

                    this.isFetching.Value = true;

                    int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Id;
                    var videos = new List<IListVideoInfo>();

                    var result = await WS::Mainpage.RemotePlaylistHandler.TryGetRemotePlaylistAsync(WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteId, videos, WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteType, WS::Mainpage.VideoListContainer.Videos.Select(v => v.NiconicoId.Value), m => WS::Mainpage.Messagehandler.AppendMessage(m));

                    if (result.IsSucceeded)
                    {

                        videos = videos.Where(v => !WS::Mainpage.PlaylistTree.ContainsVideo(v.NiconicoId.Value, playlistId)).ToList();

                        if (videos.Count == 0)
                        {
                            WS::Mainpage.Messagehandler.AppendMessage($"追加するものはありません。");
                            this.SnackbarMessageQueue.Enqueue($"追加するものはありません。");
                            this.isFetching.Value = false;
                            return;
                        }

                        WS::Mainpage.VideoListContainer.AddRange(videos, playlistId);

                        if (videos.Count > 1)
                        {
                            WS::Mainpage.Messagehandler.AppendMessage($"{videos[0].NiconicoId.Value}ほか{videos.Count - 1}件の動画を追加しました。");
                            this.SnackbarMessageQueue.Enqueue($"{videos[0].NiconicoId.Value}ほか{videos.Count - 1}件の動画を追加しました。");
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
                foreach (var video in WS::Mainpage.VideoListContainer.Videos.Where(v => !v.CheckDownloaded(this.GetFolderPath())))
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
                foreach (var video in WS::Mainpage.VideoListContainer.Videos.Where(v => v.CheckDownloaded(this.GetFolderPath())))
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
                foreach (var video in WS::Mainpage.VideoListContainer.Videos.Where(v => !v.CheckDownloaded(this.GetFolderPath())))
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
                foreach (var video in WS::Mainpage.VideoListContainer.Videos.Where(v => v.CheckDownloaded(this.GetFolderPath())))
                {
                    video.IsSelected.Value = true;
                }
            });

            this.OpenPlaylistFolder = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                string folderPath = this.GetFolderPath();
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
                var folderPath = this.GetFolderPath();
                var videos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value && v.CheckDownloaded(folderPath));
                if (!videos.Any()) return;

                var result = WS::Mainpage.PlaylistCreator.TryCreatePlaylist(videos, WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Name, folderPath, type);

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
        public CommandBase<IListVideoInfo> WatchOnNiconicoCommand { get; init; }

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
        public CommandBase<object> OpenPlaylistFolder { get; init; }

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

        #region Width
        public int selectColumnWidthField;

        public int iDColumnWidthField;

        public int titleColumnWidthField;

        public int uploadColumnWidthField;

        public int viewCountColumnWidthField;

        public int downloadedFlagColumnWidthField;

        public int stateColumnWidthField;

        public int thumbColumnWidthField;

        /// <summary>
        /// 選択
        /// </summary>
        public int SelectColumnWidth { get => this.selectColumnWidthField; set => this.SetProperty(ref this.selectColumnWidthField, value); }

        /// <summary>
        /// ID
        /// </summary>
        public int IDColumnWidth { get => this.iDColumnWidthField; set => this.SetProperty(ref this.iDColumnWidthField, value); }

        /// <summary>
        /// タイトル
        /// </summary>
        public int TitleColumnWidth { get => this.titleColumnWidthField; set => this.SetProperty(ref this.titleColumnWidthField, value); }

        /// <summary>
        /// 投稿日時
        /// </summary>
        public int UploadColumnWidth { get => this.uploadColumnWidthField; set => this.SetProperty(ref this.uploadColumnWidthField, value); }

        /// <summary>
        /// 再生回数
        /// </summary>
        public int ViewCountColumnWidth { get => this.viewCountColumnWidthField; set => this.SetProperty(ref this.viewCountColumnWidthField, value); }

        /// <summary>
        /// DL済み
        /// </summary>
        public int DownloadedFlagColumnWidth { get => this.downloadedFlagColumnWidthField; set => this.SetProperty(ref this.downloadedFlagColumnWidthField, value); }

        /// <summary>
        /// 状態
        /// </summary>
        public int StateColumnWidth { get => this.stateColumnWidthField; set => this.SetProperty(ref this.stateColumnWidthField, value); }

        /// <summary>
        /// サムネイル
        /// </summary>
        public int ThumbColumnWidth { get => this.thumbColumnWidthField; set => this.SetProperty(ref this.thumbColumnWidthField, value); }

        #endregion

        /// <summary>
        /// 並び替える
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="orderBy"></param>
        public void SetOrder(SortType sortType, OrderBy orderBy)
        {
            var videos = new List<IListVideoInfo>(WS::Mainpage.VideoListContainer.Videos);
            WS::Mainpage.VideoListContainer.Clear();
            if (orderBy != OrderBy.Descending)
            {
                WS::Mainpage.VideoListContainer.AddRange(sortType switch
                {
                    SortType.DateTime => videos.OrderBy(v => v.UploadedOn.Value),
                    SortType.Id => videos.OrderBy(v => v.NiconicoId.Value),
                    SortType.Title => videos.OrderBy(v => v.Title.Value),
                    SortType.Selected => videos.OrderBy(v => !v.IsSelected.Value ? 1 : 0),
                    SortType.ViewCount => videos.OrderBy(v => v.ViewCount.Value),
                    SortType.Downloaded => videos.OrderBy(v => v.IsDownloaded.Value ? 1 : 0),
                    _ => videos,
                }, null, false); ;
            }
            else
            {
                WS::Mainpage.VideoListContainer.AddRange(sortType switch
                {
                    SortType.DateTime => videos.OrderByDescending(v => v.UploadedOn.Value),
                    SortType.Id => videos.OrderByDescending(v => v.NiconicoId.Value),
                    SortType.Title => videos.OrderByDescending(v => v.Title.Value),
                    SortType.Selected => videos.OrderByDescending(v => !v.IsSelected.Value ? 1 : 0),
                    SortType.ViewCount => videos.OrderByDescending(v => v.ViewCount.Value),
                    SortType.Downloaded => videos.OrderByDescending(v => v.IsDownloaded.Value ? 1 : 0),
                    _ => videos,
                }, null, false);
            }

            string sortTypeStr = sortType switch
            {
                SortType.DateTime => "投稿日時",
                SortType.Id => "ID",
                SortType.Title => "タイトル",
                SortType.Selected => "選択",
                SortType.ViewCount => "再生回数",
                SortType.Downloaded => "DL済み",
                _ => "並び替えなし"
            };
            string orderStr = orderBy switch
            {
                OrderBy.Ascending => "昇順",
                OrderBy.Descending => "降順",
                _ => "昇順",
            };

            WS::Mainpage.Messagehandler.AppendMessage($"動画を{sortTypeStr}の順に{orderStr}で並び替えました。");
            this.SnackbarMessageQueue.Enqueue($"動画を{sortTypeStr}の順に{orderStr}で並び替えました。");
        }

        /// <summary>
        /// 幅を保持する
        /// </summary>
        public void SaveColumnWidth()
        {
            WS::Mainpage.SettingHandler.SaveSetting(this.SelectColumnWidth, SettingsEnum.MWSelectColumnWid);
            WS::Mainpage.SettingHandler.SaveSetting(this.IDColumnWidth, SettingsEnum.MWIDColumnWid);
            WS::Mainpage.SettingHandler.SaveSetting(this.TitleColumnWidth, SettingsEnum.MWTitleColumnWid);
            WS::Mainpage.SettingHandler.SaveSetting(this.UploadColumnWidth, SettingsEnum.MWUploadColumnWid);
            WS::Mainpage.SettingHandler.SaveSetting(this.ViewCountColumnWidth, SettingsEnum.MWViewCountColumnWid);
            WS::Mainpage.SettingHandler.SaveSetting(this.DownloadedFlagColumnWidth, SettingsEnum.MWDownloadedFlagColumnWid);
            WS::Mainpage.SettingHandler.SaveSetting(this.StateColumnWidth, SettingsEnum.MWStateColumnWid);
            WS::Mainpage.SettingHandler.SaveSetting(this.ThumbColumnWidth, SettingsEnum.MWThumbColumnWid);
        }

        /// <summary>
        /// インスタンスを破棄する
        /// </summary>
        public void Dispose()
        {
            if (this.hasDisposed) return;
            this.disposables.Dispose();
            this.hasDisposed = true;
            GC.SuppressFinalize(this);
        }

        #region private

        /// <summary>
        /// タスクキャンセラー
        /// </summary>
        private CancellationTokenSource? cts;

        private ReactiveProperty<bool> isFetching;

        private bool isFiltered;

        private CompositeDisposable disposables = new();

        private bool hasDisposed;

        private IEventAggregator ea;

        /// <summary>
        /// 動画リストを更新する
        /// </summary>
        /// <param name="e"></param>
        private void UpdateList(ListChangedEventArgs<IListVideoInfo> e)
        {
            if ((e.ChangeType is ChangeType.Add or ChangeType.Remove) && e.Data is null)
            {
                WS::Mainpage.Messagehandler.AppendMessage("動画リストの更新に失敗しました。(VIDEO_DATA_IS_NULL)");
                return;
            }

            //if (e.ChangeType == ChangeType.Add)
            //{
            //    this.Videos.Add(e.Data!);
            //}
            //else if (e.ChangeType == ChangeType.Remove)
            //{
            //    this.Videos.Remove(e.Data!);
            //}
            //else if (e.ChangeType == ChangeType.Clear)
            //{
            //    this.Videos.Clear();
            //}
            //else if (e.ChangeType == ChangeType.Overall)
            //{
            //    //this.Videos.Addrange(WS::Mainpage.VideoListContainer.GetVideos());
            //}
        }

        /// <summary>
        /// 選択したプレイリストが変更された場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedPlaylistChanged()
        {
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.ChildrensIds.Count == 0)
            {
                WS::Mainpage.Messagehandler.ClearMessage();
                string name = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Name;
                WS::Mainpage.Messagehandler.AppendMessage($"プレイリスト:{name}");
                WS::Mainpage.SnaclbarHandler.Enqueue($"プレイリスト:{name}");
                WS::Mainpage.VideoListContainer.Refresh();
            }
        }

        /// <summary>
        /// プレイリストのタイトルを変更する
        /// </summary>
        private void VideoListUpdated(ListChangedEventArgs<IListVideoInfo>? e = null)
        {
            if (e is not null)
            {
                this.UpdateList(e);
            }

            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null)
            {
                string name = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Name;
                var count = WS::Mainpage.VideoListContainer.Count;
                this.PlaylistTitle.Value = $"{name}({count}件)";
            }
        }

        /// <summary>
        /// フォルダーパスを取得する
        /// </summary>
        /// <returns></returns>
        private string GetFolderPath()
        {
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null) return string.Empty;

            return WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Folderpath.IsNullOrEmpty() ? WS::Mainpage.SettingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? FileFolder.DefaultDownloadDir : WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Folderpath;
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

        public CommandBase<object> OpenPlaylistFolder { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> OpenInPlayerAcommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> OpenInPlayerBcommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SendToappACommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SendToappBCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand FilterCommand { get; init; } = new();

        public ReactiveCommand SearchCommand { get; init; } = new();

        public CommandBase<string> CreatePlaylistCommand { get; init; } = new(_ => true, _ => { });

        public ReactiveCommand<MouseEventArgs> VideoDoubleClickCommand { get; init; } = new ReactiveCommand<MouseEventArgs>();

        public ReactiveCollection<VideoInfoViewModel> Videos { get; init; }

        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; } = new MaterialDesign::SnackbarMessageQueue();

        public ReactivePropertySlim<string> InputString { get; init; } = new("Hello World!!");

        public ReactivePropertySlim<bool> IsFilteringOnlyByTag { get; set; } = new();

        public ReactivePropertySlim<bool> IsFilteringFromAllVideos { get; set; } = new();

        public ReactivePropertySlim<string> PlaylistTitle { get; set; } = new("空白のプレイリスト");

        public int SelectColumnWidth { get; set; }

        public int IDColumnWidth { get; set; }

        public int TitleColumnWidth { get; set; }

        public int UploadColumnWidth { get; set; }

        public int ViewCountColumnWidth { get; set; }

        public int DownloadedFlagColumnWidth { get; set; }

        public int StateColumnWidth { get; set; }

        public int ThumbColumnWidth { get; set; }

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

            SortType sortType = this.GetSortType(headerString);
            OrderBy orderBy = this.GetOrderBy(sortType);

            this.ResetHeaderState();

            header.Tag = "Selected";
            this.currentSortHeader = header;
            this.CurrentSortType = sortType;
            this.CurrentOrder = orderBy;

            context.SetOrder(sortType, orderBy);

        }

        private GridViewColumnHeader? currentSortHeader;

        private OrderBy CurrentOrder;

        private SortType CurrentSortType;

        /// <summary>
        /// ヘッダーの状態をリセットする
        /// </summary>
        private void ResetHeaderState()
        {
            if (this.currentSortHeader is not null)
            {
                this.currentSortHeader.Tag = null;
            }
        }

        /// <summary>
        /// 並び替えのタイプを取得する
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private SortType GetSortType(string header)
        {
            return header switch
            {
                "ID" => SortType.Id,
                "投稿日" => SortType.DateTime,
                "タイトル" => SortType.Title,
                "選択" => SortType.Selected,
                "再生回数" => SortType.ViewCount,
                "DL済み" => SortType.Downloaded,
                _ => SortType.None
            };
        }

        /// <summary>
        /// 昇順・降順を取得する
        /// </summary>
        /// <param name="sortType"></param>
        /// <returns></returns>
        private OrderBy GetOrderBy(SortType sortType)
        {
            if (sortType == this.CurrentSortType)
            {
                return this.CurrentOrder switch
                {
                    OrderBy.Ascending => OrderBy.Descending,
                    OrderBy.Descending => OrderBy.Ascending,
                    _ => OrderBy.Ascending,
                };
            }
            else
            {
                return OrderBy.Ascending;
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

        public ReactiveProperty<DateTime> UploadedOn { get; init; }

        public void Dispose()
        {
            if (this.hasDisposed) return;
            this.disposables.Dispose();
            this.hasDisposed = true;
            GC.SuppressFinalize(this);
        }
        private bool hasDisposed;
        private readonly CompositeDisposable disposables = new();
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

    enum SortType
    {
        None,
        DateTime,
        Id,
        Title,
        Selected,
        ViewCount,
        Downloaded,
    }

    enum OrderBy
    {
        None,
        Ascending,
        Descending,
    }
}
