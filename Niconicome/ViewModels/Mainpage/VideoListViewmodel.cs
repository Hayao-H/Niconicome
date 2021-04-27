using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
using MaterialDesign = MaterialDesignThemes.Wpf;
using Playlist = Niconicome.Models.Domain.Local.Playlist;
using Utils = Niconicome.Models.Domain.Utils;
using WS = Niconicome.Workspaces;
using EnumSettings = Niconicome.Models.Local.Settings.EnumSettingsValue;

namespace Niconicome.ViewModels.Mainpage
{

    /// <summary>
    /// 動画一覧のVM
    /// </summary>
    class VideoListViewModel : BindableBase
    {
        public VideoListViewModel() : this((message, button, image) => MaterialMessageBox.Show(message, button, image))
        {

        }
        public VideoListViewModel(Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessageBox)
        {
            //プレイリスト選択変更イベントを購読する
            WS::Mainpage.CurrentPlaylist.SelectedPlaylistChanged += this.OnSelectedPlaylistChanged;

            //プレイリスト内容更新のイベントを購読する
            WS::Mainpage.VideoListContainer.ListChanged += (_, e) => this.VideoListUpdated(e);

            this.Videos = new ObservableCollection<IListVideoInfo>();

            this.showMessageBox = showMessageBox;

            this.SnackbarMessageQueue = WS::Mainpage.SnaclbarHandler.Queue;

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

            //メッセージハンドラーにイベントハンドラを追加する
            WS::Mainpage.Messagehandler.AddChangeHandler(() => this.OnPropertyChanged(nameof(this.Message)));

            //展開状況を引き継ぐ
            var inheritExpandedState = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.InheritExpandedState);
            var expandAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ExpandAll);
            WS::Mainpage.PlaylistTree.Refresh(expandAll, inheritExpandedState);

            #region コマンドの初期化
            this.AddVideoCommand = new CommandBase<object>(_ => WS::Mainpage.CurrentPlaylist.SelectedPlaylist is not null && !WS::Mainpage.VideoIDHandler.IsProcessing, async arg =>
              {
                  if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                  {
                      this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を追加できません");
                      return;
                  }
                  int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Id;

                  string niconicoId;
                  if (arg is not null and string)
                  {
                      niconicoId = (string)arg;
                  }
                  else if (this.InputString != string.Empty)
                  {
                      niconicoId = this.InputString;
                      this.InputString = string.Empty;
                  }
                  else
                  {
                      return;
                  }

                  var videos = new List<IListVideoInfo>();
                  var result = await WS::Mainpage.VideoIDHandler.TryGetVideoListInfosAsync(videos, niconicoId, this.Videos.Select(v => v.NiconicoId), m => this.SnackbarMessageQueue.Enqueue(m), m => WS::Mainpage.Messagehandler.AppendMessage(m));

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

                  if (!videos.First().ChannelID.IsNullOrEmpty())
                  {
                      var video = videos.First();
                      WS::Mainpage.SnaclbarHandler.Enqueue($"この動画のチャンネルは「{video.ChannelName}」です", "IDをコピー", () =>
                      {
                          Clipboard.SetText(video.ChannelID);
                          WS::Mainpage.SnaclbarHandler.Enqueue("コピーしました");
                      });
                  }
              });

            this.RemoveVideoCommand = new CommandBase<IListVideoInfo>(_ => true, async arg =>
             {
                 if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                 {
                     this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を削除できません");
                     return;
                 }

                 var targetVideos = new List<IListVideoInfo>();

                 if (arg is not null && arg.AsNullable<IListVideoInfo>() is IListVideoInfo videoInfo && videoInfo is not null) targetVideos.Add(videoInfo);

                 targetVideos.AddRange(this.Videos.Where(v => v.IsSelected));
                 targetVideos = targetVideos.Distinct(v => v.Id).ToList();

                 string confirmMessage = targetVideos.Count == 1
                 ? $"本当に「[{targetVideos[0].NiconicoId}]{targetVideos[0].Title}」を削除しますか？"
                 : $"本当に「[{targetVideos[0].NiconicoId}]{targetVideos[0].Title}」ほか{targetVideos.Count - 1}件の動画を削除しますか？";


                 var confirm = await this.showMessageBox(confirmMessage, MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);
                 if (confirm != MaterialMessageBoxResult.Yes) return;

                 foreach (var video in targetVideos)
                 {
                     //取得失敗動画の場合
                     if (video.Title == "取得失敗")
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

                     WS::Mainpage.Messagehandler.AppendMessage($"{targetVideos.First().NiconicoId}ほか{targetVideos.Count - 1}件の動画を削除しました。");
                     this.SnackbarMessageQueue.Enqueue($"{targetVideos.First().NiconicoId}ほか{targetVideos.Count - 1}件の動画を削除しました。");
                 }
                 else
                 {
                     WS::Mainpage.Messagehandler.AppendMessage($"{targetVideos.First().NiconicoId}を削除しました。");
                     this.SnackbarMessageQueue.Enqueue($"{targetVideos.First().NiconicoId}を削除しました。");
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

            this.EditPlaylistCommand = new CommandBase<object>(_ => WS::Mainpage.CurrentPlaylist.SelectedPlaylist is not null, _ =>
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
            });

            this.AddVideoFromClipboardCommand = new CommandBase<object>(_ => WS::Mainpage.CurrentPlaylist.SelectedPlaylist is not null, async _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を追加できません");
                    return;
                }

                int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Id;

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

            });

            this.OpenNetworkSettingsCommand = new CommandBase<object>(_ => WS::Mainpage.CurrentPlaylist.SelectedPlaylist is not null, _ =>
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
            });

            this.UpdateVideoCommand = new CommandBase<IListVideoInfo>(_ => WS::Mainpage.CurrentPlaylist is not null, async _ =>
            {
                if (this.isFetching)
                {
                    this.cts?.Cancel();
                    this.RefreshCommandIcon = MaterialDesign::PackIconKind.Refresh;
                    this.FetchingCompleted();
                    return;
                }

                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {

                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を更新できません");
                    return;
                }
                int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Id;

                var sourceVideos = this.Videos.Where(v => v.IsSelected);
                int sourceVideosCount = sourceVideos.Count();

                if (sourceVideosCount < 1) return;

                this.SnackbarMessageQueue.Enqueue($"{sourceVideosCount}件の動画を更新します。");
                WS::Mainpage.Messagehandler.AppendMessage($"{sourceVideosCount}件の動画を更新します。");

                this.RefreshCommandIcon = MaterialDesign::PackIconKind.Close;
                this.StartFetching();

                this.cts = new CancellationTokenSource();

                var videos = (await WS::Mainpage.NetworkVideoHandler.GetVideoListInfosAsync(sourceVideos, true, playlistId, cts.Token)).ToList();
                var result = WS::Mainpage.VideoListContainer.UpdateRange(videos);

                this.FetchingCompleted();
                this.RefreshCommandIcon = MaterialDesign::PackIconKind.Refresh;

                this.SnackbarMessageQueue.Enqueue($"{videos.Count}件の動画を更新しました。");
                WS::Mainpage.Messagehandler.AppendMessage($"{videos.Count}件の動画を更新しました。");

                if (sourceVideosCount > videos.Count)
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"{sourceVideosCount - videos.Count}件の動画の更新に失敗しました。");
                }

            });

            this.SyncWithNetowrkCommand = new CommandBase<object>(_ => !this.isFetching && (WS::Mainpage.CurrentPlaylist.SelectedPlaylist?.IsRemotePlaylist ?? false), async _ =>
                 {

                     if (!WS::Mainpage.Session.IsLogin)
                     {
                         this.SnackbarMessageQueue.Enqueue("リモートプレイリストと同期する為にはログインが必要です。");
                         return;
                     }

                     if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                     {
                         this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、同期できません");
                         return;
                     }

                     if (!WS::Mainpage.CurrentPlaylist.SelectedPlaylist.IsRemotePlaylist || (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.RemoteId.IsNullOrEmpty() && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.RemoteType != RemoteType.WatchLater)) return;

                     this.StartFetching();

                     int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Id;
                     var videos = new List<IListVideoInfo>();

                     var result = await WS::Mainpage.RemotePlaylistHandler.TryGetRemotePlaylistAsync(WS::Mainpage.CurrentPlaylist.SelectedPlaylist.RemoteId, videos, WS::Mainpage.CurrentPlaylist.SelectedPlaylist.RemoteType, this.Videos.Select(v => v.NiconicoId), m => WS::Mainpage.Messagehandler.AppendMessage(m));

                     if (!result.IsFailed)
                     {
                         if (!result.IsSucceededAll)
                         {
                             WS::Mainpage.Messagehandler.AppendMessage($"{result.FailedCount}件の取得に失敗しました。");
                         }

                         videos = videos.Where(v => !WS::Mainpage.PlaylistTree.ContainsVideo(v.NiconicoId, playlistId)).ToList();

                         if (videos.Count == 0)
                         {
                             WS::Mainpage.Messagehandler.AppendMessage($"追加するものはありません。");
                             this.SnackbarMessageQueue.Enqueue($"追加するものはありません。");
                             this.FetchingCompleted();
                             return;
                         }

                         WS::Mainpage.VideoListContainer.AddRange(videos, playlistId);

                         if (videos.Count > 1)
                         {
                             WS::Mainpage.Messagehandler.AppendMessage($"{videos[0].NiconicoId}ほか{videos.Count - 1}件の動画を追加しました。");
                             this.SnackbarMessageQueue.Enqueue($"{videos[0].NiconicoId}ほか{videos.Count - 1}件の動画を追加しました。");
                             WS::Mainpage.PlaylistTree.Refresh();
                         }
                         else
                         {
                             WS::Mainpage.Messagehandler.AppendMessage($"{videos[0].NiconicoId}を追加しました。");
                             this.SnackbarMessageQueue.Enqueue($"{videos[0].NiconicoId}を追加しました。");
                         }
                     }
                     else
                     {
                         string detail = WS::Mainpage.RemotePlaylistHandler.ExceptionDetails ?? "None";
                         WS::Mainpage.Messagehandler.AppendMessage($"情報の取得に失敗しました。(詳細: {detail})");
                         this.SnackbarMessageQueue.Enqueue($"情報の取得に失敗しました。(詳細: {detail})");
                     }

                     this.FetchingCompleted();
                 });

            this.ClearMessageCommand = new CommandBase<object>(_ => true, _ =>
            {
                WS::Mainpage.Messagehandler.ClearMessage();
            });

            this.CopyMessageCommand = new CommandBase<object>(_ => true, _ =>
            {
                string content = WS::Mainpage.Messagehandler.Message;
                Clipboard.SetText(content);
                WS::Mainpage.Messagehandler.AppendMessage("出力をクリップボードにコピーしました。");
                this.SnackbarMessageQueue.Enqueue("出力をクリップボードにコピーしました。");
            });

            this.OpenLogWindowCommand = new CommandBase<object>(_ => true, _ =>
            {
                var window = new LogWindow()
                {
                    Owner = Application.Current.MainWindow,
                };
                window.Show();
            });

            this.SelectAllVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos)
                {
                    video.IsSelected = true;
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
                    video.IsSelected = false;
                }
            });

            this.SelectAllNotDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => !v.CheckDownloaded(this.GetFolderPath())))
                {
                    video.IsSelected = true;
                }
            });

            this.DisSelectAllDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => v.CheckDownloaded(this.GetFolderPath())))
                {
                    video.IsSelected = false;
                }
            });

            this.DisSelectAllNotDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => !v.CheckDownloaded(this.GetFolderPath())))
                {
                    video.IsSelected = false;
                }
            });

            this.SelectAllDownloadedVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                foreach (var video in this.Videos.Where(v => v.CheckDownloaded(this.GetFolderPath())))
                {
                    video.IsSelected = true;
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

            this.FilterCommand = new CommandBase<object>(_ => WS::Mainpage.CurrentPlaylist is not null, _ =>
              {
                  if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                  {
                      this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                      return;
                  }

                  if (this.isFiltered)
                  {
                      this.Videos.Clear();
                      WS::Mainpage.VideoListContainer.Refresh();
                      this.isFiltered = false;
                      this.FilterIcon = MaterialDesign::PackIconKind.Filter;
                  }
                  else if (!this.InputString.IsNullOrEmpty())
                  {
                      FilterringOptions option = this.IsFilteringOnlyByTag ? FilterringOptions.OnlyByTag : FilterringOptions.None;
                      var videos = this.IsFilteringFromAllVideos ? WS::Mainpage.VideoHandler.GetAllVideos() : this.Videos;
                      videos = WS::Mainpage.VideoFilter.FilterVideos(this.InputString, videos, option);

                      this.Videos.Clear();
                      this.Videos.Addrange(videos);
                      this.isFiltered = true;
                      this.FilterIcon = MaterialDesign::PackIconKind.FilterOff;
                  }
              });

            this.SearchCommand = new CommandBase<object>(_ => WS::Mainpage.CurrentPlaylist is not null, _ =>
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
             });

            this.SendToappACommand = new CommandBase<object>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                if (arg is null || arg.AsNullable<IListVideoInfo>() is not IListVideoInfo videoInfo || videoInfo is null) return;

                var result = WS::Mainpage.ExternalAppUtils.SendToAppA(videoInfo);

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

                if (arg is null || arg.AsNullable<IListVideoInfo>() is not IListVideoInfo videoInfo || videoInfo is null) return;

                var result = WS::Mainpage.ExternalAppUtils.SendToAppB(videoInfo);

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

                if (arg is null || arg.AsNullable<IListVideoInfo>() is not IListVideoInfo videoInfo || videoInfo is null) return;

                if (!videoInfo.IsDownloaded || videoInfo.FileName.IsNullOrEmpty())
                {
                    var reAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ReAllocateCommands);
                    if (reAll) this.SendToappACommand.Execute(arg);
                    return;
                }

                var result = WS::Mainpage.ExternalAppUtils.OpenInPlayerA(videoInfo);

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

                if (arg is null || arg.AsNullable<IListVideoInfo>() is not IListVideoInfo videoInfo || videoInfo is null) return;

                if (!videoInfo.IsDownloaded || videoInfo.FileName.IsNullOrEmpty())
                {
                    var reAll = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ReAllocateCommands);
                    if (reAll) this.SendToappBCommand.Execute(arg);
                    return;
                }

                var result = WS::Mainpage.ExternalAppUtils.OpenInPlayerB(videoInfo);

                if (!result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue("動画の再生に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage(result.Message ?? "動画の再生に失敗しました。");
                    WS::Mainpage.Messagehandler.AppendMessage($"詳細:{result?.Exception?.Message ?? "None"}");
                }
            });

            this.CreatePlaylistCommand = new CommandBase<string>(_ => true, arg =>
            {
                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null)
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
                var videos = this.Videos.Where(v => v.IsSelected && v.CheckDownloaded(folderPath));
                if (!videos.Any()) return;

                var result = WS::Mainpage.PlaylistCreator.TryCreatePlaylist(videos, WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Name, folderPath, type);

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
            #endregion

            //動画情報取得クラスの実行可能状態変更イベントを購読する
            WS::Mainpage.VideoIDHandler.StateChange += (_, _) => this.AddVideoCommand.RaiseCanExecutechanged();
        }

        private MaterialDesign::PackIconKind refreshCommandIconField = MaterialDesign::PackIconKind.Refresh;

        private MaterialDesign::PackIconKind filterIconFIeld = MaterialDesign::PackIconKind.Filter;

        /// <summary>
        /// フィルターアイコン
        /// </summary>
        public MaterialDesign::PackIconKind FilterIcon { get => this.filterIconFIeld; set => this.SetProperty(ref this.filterIconFIeld, value); }

        /// <summary>
        /// 更新アイコン
        /// </summary>
        public MaterialDesign::PackIconKind RefreshCommandIcon { get => this.refreshCommandIconField; set => this.SetProperty(ref this.refreshCommandIconField, value); }

        #region コマンド
        /// <summary>
        /// 動画を追加する
        /// </summary>
        public CommandBase<object> AddVideoCommand { get; init; }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        public CommandBase<IListVideoInfo> RemoveVideoCommand { get; init; }

        /// <summary>
        /// ニコニコ動画で開く
        /// </summary>
        public CommandBase<IListVideoInfo> WatchOnNiconicoCommand { get; init; }

        /// <summary>
        /// クリップボードから動画を追加する
        /// </summary>
        public CommandBase<object> AddVideoFromClipboardCommand { get; init; }

        /// <summary>
        /// プレイリスト情報を編集する
        /// </summary>
        public CommandBase<object> EditPlaylistCommand { get; init; }

        /// <summary>
        /// ネットワーク上の動画を設定する
        /// </summary>
        public CommandBase<object> OpenNetworkSettingsCommand { get; init; }

        /// <summary>
        /// 動画情報を更新する
        /// </summary>
        public CommandBase<IListVideoInfo> UpdateVideoCommand { get; init; }

        /// <summary>
        /// オンライン上のプレイリストと同期する
        /// </summary>
        public CommandBase<object> SyncWithNetowrkCommand { get; init; }

        /// <summary>
        /// メッセージ出力をクリアする
        /// </summary>
        public CommandBase<object> ClearMessageCommand { get; init; }

        /// <summary>
        /// メッセージを
        /// </summary>
        public CommandBase<object> CopyMessageCommand { get; init; }

        /// <summary>
        /// 出力を別窓で開く
        /// </summary>
        public CommandBase<object> OpenLogWindowCommand { get; init; }

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
        public CommandBase<object> FilterCommand { get; init; }

        /// <summary>
        /// 検索コマンド
        /// </summary>
        public CommandBase<object> SearchCommand { get; init; }

        /// <summary>
        /// プレイリスト作成コマンド
        /// </summary>
        public CommandBase<string> CreatePlaylistCommand { get; init; }
        #endregion

        /// <summary>
        /// メッセージボックスを表示するコマンド
        /// </summary>
        private readonly Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessageBox;

        /// <summary>
        /// 動画のリスト
        /// </summary>
        public ObservableCollection<IListVideoInfo> Videos { get; init; }

        /// <summary>
        /// スナックバー
        /// </summary>
        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }

        /// <summary>
        /// ユーザーの入力値
        /// </summary>
        public string InputString { get => this.inputStringFIeld; set => this.SetProperty(ref this.inputStringFIeld, value); }

        /// <summary>
        /// フィルターの設定
        /// </summary>
        public bool IsFilteringOnlyByTag { get => this.isFilteringOnlyByTagEnableField; set => this.SetProperty(ref this.isFilteringOnlyByTagEnableField, value); }

        /// <summary>
        /// 全ての動画から検索する
        /// </summary>
        public bool IsFilteringFromAllVideos { get => this.isFilteringFromAllVideosEnableField; set => this.SetProperty(ref this.isFilteringFromAllVideosEnableField, value); }

        /// <summary>
        /// プレイリストのタイトル
        /// </summary>
        public string PlaylistTitle { get => this.playlistTitleFIeld; set => this.SetProperty(ref this.playlistTitleFIeld, value); }

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

        private string inputStringFIeld = string.Empty;

        private string playlistTitleFIeld = string.Empty;

        private bool isFilteringOnlyByTagEnableField;

        private bool isFilteringFromAllVideosEnableField;


        /// <summary>
        /// 並び替える
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="orderBy"></param>
        public void SetOrder(SortType sortType, OrderBy orderBy)
        {
            var videos = new List<IListVideoInfo>(this.Videos);
            this.Videos.Clear();
            if (orderBy != OrderBy.Descending)
            {
                this.Videos.Addrange(sortType switch
                {
                    SortType.DateTime => videos.OrderBy(v => v.UploadedOn),
                    SortType.Id => videos.OrderBy(v => v.NiconicoId),
                    SortType.Title => videos.OrderBy(v => v.Title),
                    SortType.Selected => videos.OrderBy(v => !v.IsSelected ? 1 : 0),
                    SortType.ViewCount => videos.OrderBy(v => v.ViewCount),
                    SortType.Downloaded => videos.OrderBy(v => v.IsDownloaded ? 1 : 0),
                    _ => videos,
                }); ;
            }
            else
            {
                this.Videos.Addrange(sortType switch
                {
                    SortType.DateTime => videos.OrderByDescending(v => v.UploadedOn),
                    SortType.Id => videos.OrderByDescending(v => v.NiconicoId),
                    SortType.Title => videos.OrderByDescending(v => v.Title),
                    SortType.Selected => videos.OrderByDescending(v => !v.IsSelected ? 1 : 0),
                    SortType.ViewCount => videos.OrderByDescending(v => v.ViewCount),
                    SortType.Downloaded => videos.OrderByDescending(v => v.IsDownloaded ? 1 : 0),
                    _ => videos,
                });
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

        public void OnDoubleClick(object? sender)
        {
            if (sender is not ListViewItem item) return;
            if (item.DataContext is not BindableListVIdeoInfo videoInfo) return;
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
                this.OpenInPlayerAcommand.Execute(videoInfo);
            }
        }

        #region private

        /// <summary>
        /// タスクキャンセラー
        /// </summary>
        private CancellationTokenSource? cts;

        private bool isFetching;

        private bool isFiltered;

        /// <summary>
        /// リソースの取得を開始する
        /// </summary>
        private void StartFetching()
        {
            this.isFetching = true;
            this.RaiseOverallCanExecuteChanged();
        }

        /// <summary>
        /// リソースの取得が完了
        /// </summary>
        private void FetchingCompleted()
        {
            this.isFetching = false;
            this.cts = null;
            this.RaiseOverallCanExecuteChanged();
        }

        /// <summary>
        /// コマンドの実行可能状態を変更する
        /// </summary>
        private void RaiseOverallCanExecuteChanged()
        {
            //this.GetThumbnailAgainCommand.RaiseCanExecutechanged();
            this.SyncWithNetowrkCommand.RaiseCanExecutechanged();
            this.AddVideoCommand.RaiseCanExecutechanged();
            this.AddVideoFromClipboardCommand.RaiseCanExecutechanged();
            this.EditPlaylistCommand.RaiseCanExecutechanged();
            this.OpenNetworkSettingsCommand.RaiseCanExecutechanged();
            this.UpdateVideoCommand.RaiseCanExecutechanged();
            this.FilterCommand.RaiseCanExecutechanged();
            this.SearchCommand.RaiseCanExecutechanged();
        }

        /// <summary>
        /// 出力メッセージ
        /// </summary>
        public string Message
        {
            get => WS::Mainpage.Messagehandler.Message;
        }

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

            if (e.ChangeType == ChangeType.Add)
            {
                this.Videos.Add(e.Data!);
            }
            else if (e.ChangeType == ChangeType.Remove)
            {
                this.Videos.Remove(e.Data!);
            }
            else if (e.ChangeType == ChangeType.Clear)
            {
                this.Videos.Clear();
            }
            else if (e.ChangeType == ChangeType.Overall)
            {
                this.Videos.Addrange(WS::Mainpage.VideoListContainer.GetVideos());
            }
        }

        /// <summary>
        /// 選択したプレイリストが変更された場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedPlaylistChanged(object? sender, EventArgs e)
        {
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is not null && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.ChildrensIds.Count == 0)
            {
                this.RaiseOverallCanExecuteChanged();
                WS::Mainpage.Messagehandler.ClearMessage();
                string name = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Name;
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

            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is not null)
            {
                string name = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Name;
                this.PlaylistTitle = $"{name}({this.Videos.Count}件)";
            }
        }

        /// <summary>
        /// フォルダーパスを取得する
        /// </summary>
        /// <returns></returns>
        private string GetFolderPath()
        {
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist is null) return string.Empty;

            return WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Folderpath.IsNullOrEmpty() ? WS::Mainpage.SettingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? FileFolder.DefaultDownloadDir : WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Folderpath;
        }
        #endregion

    }

    class VideoListViewModelD
    {
        public VideoListViewModelD()
        {
            var v1 = new BindableListVIdeoInfo()
            {
                Title = "レッツゴー!陰陽師",
                NiconicoId = "sm9",
                IsDownloaded = true,
            };

            var v2 = new BindableListVIdeoInfo()
            {
                Title = "Bad Apple!! feat. nomico",
                NiconicoId = "sm8628149",
                IsDownloaded = false,
            };

            this.Videos = new ObservableCollection<IListVideoInfo>() { v1, v2 };

        }

        public MaterialDesign::PackIconKind FilterIcon { get; set; } = MaterialDesign::PackIconKind.Filter;

        public MaterialDesign::PackIconKind RefreshCommandIcon { get; set; } = MaterialDesign::PackIconKind.Refresh;

        public CommandBase<object> AddVideoCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<IListVideoInfo> RemoveVideoCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<IListVideoInfo> WatchOnNiconicoCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> AddVideoFromClipboardCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> EditPlaylistCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> OpenNetworkSettingsCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<IListVideoInfo> UpdateVideoCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SyncWithNetowrkCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> ClearMessageCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> CopyMessageCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> OpenLogWindowCommand { get; init; } = new(_ => true, _ => { });

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

        public CommandBase<object> FilterCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<object> SearchCommand { get; init; } = new(_ => true, _ => { });

        public CommandBase<string> CreatePlaylistCommand { get; init; } = new(_ => true, _ => { });

        public ObservableCollection<IListVideoInfo> Videos { get; private set; }

        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; } = new MaterialDesign::SnackbarMessageQueue();

        public string InputString { get; set; } = string.Empty;

        public bool IsFilteringOnlyByTag { get; set; } = false;

        public bool IsFilteringFromAllVideos { get; set; } = false;

        public string PlaylistTitle { get; set; } = "空白のプレイリスト";

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
        private readonly double minimumVerticalOffsetChangeToFourceScroll = 5;

        protected override void OnAttached()
        {
            base.OnAttached();
            WS::Mainpage.CurrentPlaylist.SelectedPlaylistChanged += this.OnSourceChanged;
            this.AssociatedObject.AddHandler(GridViewColumnHeader.ClickEvent, this.headerClickedHandler);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            WS::Mainpage.CurrentPlaylist.SelectedPlaylistChanged -= this.OnSourceChanged;
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
        private void OnSourceChanged(object? sender, EventArgs e)
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

    class VideoListItemBehavior : Behavior<ListViewItem>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.MouseLeftButtonDown += this.OnClick;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.MouseLeftButtonDown -= this.OnClick;
            base.OnDetaching();
        }

        private void OnClick(object? sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.AssociatedObject.Tag is not VideoListViewModel vm) return;
                var setting = WS::Mainpage.EnumSettingsHandler.GetSetting<EnumSettings::VideodbClickSettings>();

                if (setting == EnumSettings::VideodbClickSettings.OpenInPlayerA)
                {
                    vm.OpenInPlayerAcommand.Execute(this.AssociatedObject);
                }
                else if (setting == EnumSettings::VideodbClickSettings.OpenInPlayerB)
                {
                    vm.OpenInPlayerBcommand.Execute(this.AssociatedObject);
                }
                else if (setting == EnumSettings::VideodbClickSettings.SendToAppA)
                {
                    vm.SendToappACommand.Execute(this.AssociatedObject);
                }
                else if (setting == EnumSettings::VideodbClickSettings.SendToAppB)
                {
                    vm.SendToappBCommand.Execute(this.AssociatedObject);
                }
                else if (setting == EnumSettings::VideodbClickSettings.Download)
                {
                    vm.OpenInPlayerAcommand.Execute(this.AssociatedObject);
                }
            }
        }
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
