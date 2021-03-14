using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.Diagnostics;
using Niconicome.Extensions.System.List;
using Niconicome.Extensions.System.Windows;
using Niconicome.Models.Local;
using Niconicome.Models.Playlist;
using Niconicome.Views;
using MaterialDesign = MaterialDesignThemes.Wpf;
using Utils = Niconicome.Models.Domain.Utils;
using WS = Niconicome.Workspaces;
using Playlist = Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Network;
using Niconicome.ViewModels.Controls;
using System.Threading.Tasks;

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
            WS::Mainpage.CurrentPlaylist.SelectedItemChanged += this.OnSelectedPlaylistChanged;

            //プレイリスト内容更新のイベントを購読する
            WS::Mainpage.CurrentPlaylist.VideosChanged += (e, s) => this.ChangePlaylistTitle();

            this.showMessageBox = showMessageBox;

            this.SnackbarMessageQueue = WS::Mainpage.SnaclbarHandler.Queue;

            //メッセージハンドラーにイベントハンドラを追加する
            WS::Mainpage.Messagehandler.AddChangeHandler(() => this.OnPropertyChanged(nameof(this.Message)));

            //種々のコマンドを初期化する
            this.AddVideoCommand = new CommandBase<object>(_ => this.Playlist is not null, async arg =>
            {
                if (this.Playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を追加できません");
                    return;
                }
                int playlistId = this.Playlist.Id;

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

                await WS::Mainpage.NetworkVideoHandler.AddVideosAsync(new List<string>() { niconicoId }, this.Playlist.Id, (result) =>
                {
                    var video = new BindableVIdeoListInfo()
                    {
                        Title = "取得失敗",
                        ThumbUrl = "https://nicovideo.cdn.nimg.jp/web/img/common/video_deleted.jpg",
                        Message = result.Message
                    };
                    this.Videos.Add(video);

                }, video =>
                {
                    WS::Mainpage.PlaylistTree.Refresh();
                    WS::Mainpage.CurrentPlaylist.Update(playlistId);

                    if (!video.ChannelId.IsNullOrEmpty())
                    {
                        WS::Mainpage.SnaclbarHandler.Enqueue($"この動画のチャンネルは「{video.ChannelId}」です", "IDをコピー", () =>
                        {
                            Clipboard.SetText(video.ChannelId);
                            WS::Mainpage.SnaclbarHandler.Enqueue("コピーしました");
                        });
                    }
                });
            });

            this.RemoveVideoCommand = new CommandBase<IVideoListInfo>(_ => true, async arg =>
             {
                 if (this.Playlist is null)
                 {
                     this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を削除できません");
                     return;
                 }

                 var targetVideos = new List<IVideoListInfo>();

                 if (arg is not null && arg.AsNullable<IVideoListInfo>() is IVideoListInfo videoInfo && videoInfo is not null) targetVideos.Add(videoInfo);

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
                         this.Videos.Remove(video);
                     }
                     else
                     {
                         WS::Mainpage.VideoHandler.RemoveVideo(video.Id, this.Playlist.Id);
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

                 WS::Mainpage.CurrentPlaylist.Update(this.Playlist.Id);
             });

            this.WatchOnNiconicoCommand = new CommandBase<IVideoListInfo>(_ => true, arg =>
            {
                if (this.Playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を視聴できません");
                    return;
                }
                if (arg is null || arg.AsNullable<IVideoListInfo>() is not IVideoListInfo videoInfo || videoInfo is null) return;
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

            this.EditPlaylistCommand = new CommandBase<object>(_ => this.Playlist is not null, _ =>
            {
                if (this.Playlist is null)
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

            this.AddVideoFromClipboardCommand = new CommandBase<object>(_ => this.Playlist is not null, async _ =>
            {
                if (this.Playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を追加できません");
                    return;
                }
                int playlistId = this.Playlist.Id;

                var data = Clipboard.GetText();
                if (data == string.Empty) return;

                Utils::INiconicoUtils reader = new Utils::NiconicoUtils();
                var ids = reader.GetNiconicoIdsFromText(data).Where(i => !WS::Mainpage.PlaylistTree.ContainsVideo(i, playlistId)).ToList();

                var result = await WS::Mainpage.NetworkVideoHandler.AddVideosAsync(ids, this.Playlist.Id, (result) =>
                {
                    var video = new BindableVIdeoListInfo()
                    {
                        Title = "取得失敗",
                        ThumbUrl = "https://nicovideo.cdn.nimg.jp/web/img/common/video_deleted.jpg",
                        Message = result.Message
                    };
                    this.Videos.Add(video);

                }, video =>
                {
                    WS::Mainpage.PlaylistTree.Refresh();
                    WS::Mainpage.CurrentPlaylist.Update(playlistId);
                });

                WS::Mainpage.Messagehandler.AppendMessage($"{result.SucceededCount}件の動画を登録しました。");
                if (!result.IsSucceededAll)
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"{result.FailedCount}件の動画の追加に失敗しました。");
                }

                this.SnackbarMessageQueue.Enqueue($"{result.SucceededCount}件の動画を登録しました。");
            });

            this.OpenNetworkSettingsCommand = new CommandBase<object>(_ => this.Playlist is not null, _ =>
            {
                if (this.Playlist is null) return;

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

            this.UpdateVideoCommand = new CommandBase<IVideoListInfo>(_ => this.Playlist is not null, async _ =>
            {
                if (this.isFetching)
                {
                    this.cts?.Cancel();
                    this.RefreshCommandIcon = MaterialDesign::PackIconKind.Refresh;
                    this.FetchingCompleted();
                    return;
                }

                if (this.Playlist is null)
                {

                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、動画を更新できません");
                    return;
                }
                int playlistId = this.Playlist.Id;

                var videos = this.Videos.Where(v => v.IsSelected).Select((video, index) => new { video, index });
                int videoCount = videos.Count();

                if (videoCount < 1) return;

                this.SnackbarMessageQueue.Enqueue($"{videos.Count()}件の動画を更新します。");
                WS::Mainpage.Messagehandler.AppendMessage($"{videos.Count()}件の動画を更新します。");

                this.RefreshCommandIcon = MaterialDesign::PackIconKind.Close;
                this.StartFetching();

                this.cts = new CancellationTokenSource();

                var result = await WS::Mainpage.NetworkVideoHandler.UpdateVideosAsync(this.Videos.Where(v => v.IsSelected), this.Playlist.Folderpath, video => WS::Mainpage.CurrentPlaylist.Update(playlistId, video), this.cts.Token);

                WS::Mainpage.PlaylistTree.Refresh();
                WS::Mainpage.CurrentPlaylist.Update(playlistId);

                this.FetchingCompleted();
                this.RefreshCommandIcon = MaterialDesign::PackIconKind.Refresh;

                this.SnackbarMessageQueue.Enqueue($"{result.SucceededCount}件の動画を更新しました。");
                WS::Mainpage.Messagehandler.AppendMessage($"{result.SucceededCount}件の動画を更新しました。");

                if (!result.IsSucceededAll)
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"{result.FailedCount}件の動画の更新に失敗しました。");
                }

            });

            this.SyncWithNetowrkCommand = new CommandBase<object>(_ => !this.isFetching && (this.Playlist?.IsRemotePlaylist ?? false), async _ =>
                 {

                     if (!WS::Mainpage.Session.IsLogin)
                     {
                         this.SnackbarMessageQueue.Enqueue("リモートプレイリストと同期する為にはログインが必要です。");
                         return;
                     }

                     if (this.Playlist is null)
                     {
                         this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、同期できません");
                         return;
                     }

                     if (!this.Playlist.IsRemotePlaylist || (this.Playlist.RemoteId.IsNullOrEmpty() && this.Playlist.RemoteType != RemoteType.WatchLater)) return;

                     this.StartFetching();

                     int playlistId = this.Playlist.Id;
                     var videos = new List<IVideoListInfo>();
                     INetworkResult result = this.Playlist.RemoteType switch
                     {
                         RemoteType.Mylist => await WS::Mainpage.RemotePlaylistHandler.TryGetMylistVideosAsync(this.Playlist.RemoteId, videos),
                         RemoteType.UserVideos => await WS::Mainpage.RemotePlaylistHandler.TryGetUserVideosAsync(this.Playlist.RemoteId, videos),
                         RemoteType.WatchLater => await WS::Mainpage.RemotePlaylistHandler.TryGetWatchLaterAsync(videos),
                         RemoteType.Channel => await WS::Mainpage.RemotePlaylistHandler.TryGetChannelVideosAsync(this.Playlist.RemoteId, videos, this.Videos.Select(v => v.NiconicoId), m => { }),
                         _ => new NetworkResult(),
                     };

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

                         await WS::Mainpage.NetworkVideoHandler.AddVideosAsync(videos, playlistId);

                         if (videos.Count > 1)
                         {
                             WS::Mainpage.Messagehandler.AppendMessage($"{videos[0].NiconicoId}ほか{videos.Count - 1}件の動画を追加しました。");
                             this.SnackbarMessageQueue.Enqueue($"{videos[0].NiconicoId}ほか{videos.Count - 1}件の動画を追加しました。");
                             WS::Mainpage.PlaylistTree.Refresh();
                             WS::Mainpage.CurrentPlaylist.Update(playlistId);
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
                if (this.Playlist is null)
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

                if (this.Playlist is null)
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
                if (this.Playlist is null)
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
                if (this.Playlist is null)
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
                if (this.Playlist is null)
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
                if (this.Playlist is null)
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
                if (this.Playlist is null)
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

            this.FilterCommand = new CommandBase<object>(_ => this.Playlist is not null, _ =>
              {
                  if (this.Playlist is null)
                  {
                      this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                      return;
                  }

                  if (this.isFiltered)
                  {
                      this.Videos.Clear();
                      WS::Mainpage.CurrentPlaylist.Update(this.Playlist.Id);
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

            this.SearchCommand = new CommandBase<object>(_ => this.Playlist is not null, _ =>
             {
                 if (this.Playlist is null)
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

            this.OpenInPlayerAcommand = new CommandBase<object>(_ => true, arg =>
            {
                if (this.Playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }

                if (arg is null || arg.AsNullable<IVideoListInfo>() is not IVideoListInfo videoInfo || videoInfo is null) return;

                string folderPath = this.GetFolderPath();
                if (!videoInfo.CheckDownloaded(folderPath) || videoInfo.GetFilePath(folderPath).IsNullOrEmpty())
                {
                    this.SnackbarMessageQueue.Enqueue("動画が保存されていません。");
                    return;
                }

                string? appPath = WS::Mainpage.SettingHandler.GetStringSetting(Settings.PlayerAPath);

                if (appPath is not null)
                {
                    ProcessEx.StartWithShell(appPath, $"\"{videoInfo.GetFilePath(folderPath)}\"");
                }
            });

            this.OpenInPlayerBcommand = new CommandBase<object>(_ => true, arg =>
            {
                if (this.Playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }

                if (arg is null || arg.AsNullable<IVideoListInfo>() is not IVideoListInfo videoInfo || videoInfo is null) return;

                string folderPath = this.GetFolderPath();
                if (!videoInfo.CheckDownloaded(folderPath) || videoInfo.GetFilePath(folderPath).IsNullOrEmpty())
                {
                    this.SnackbarMessageQueue.Enqueue("動画が保存されていません。");
                    return;
                }

                string? appPath = WS::Mainpage.SettingHandler.GetStringSetting(Settings.PlayerBPath);

                if (appPath is not null)
                {
                    ProcessEx.StartWithShell(appPath, $"\"{videoInfo.GetFilePath(folderPath)}\"");
                }
            });

            this.SendIdToappCommand = new CommandBase<object>(_ => true, arg =>
            {
                if (this.Playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }
                if (arg is null || arg.AsNullable<IVideoListInfo>() is not IVideoListInfo videoInfo || videoInfo is null) return;

                string folderPath = this.GetFolderPath();
                if (!videoInfo.CheckDownloaded(folderPath) || videoInfo.GetFilePath(folderPath).IsNullOrEmpty())
                {
                    this.SnackbarMessageQueue.Enqueue("動画が保存されていません。");
                    return;
                }

                string? appPath = WS::Mainpage.SettingHandler.GetStringSetting(Settings.AppIdPath);
                string param = WS::Mainpage.SettingHandler.GetStringSetting(Settings.AppIdParam) ?? string.Empty;

                if (appPath is not null)
                {
                    ProcessEx.StartWithShell(appPath, $"{param} {videoInfo.NiconicoId}");
                }
            });

            this.SendUrlToappCommand = new CommandBase<object>(_ => true, arg =>
            {
                if (this.Playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、処理できません。");
                    return;
                }

                if (arg is null || arg.AsNullable<IVideoListInfo>() is not IVideoListInfo videoInfo || videoInfo is null) return;

                string folderPath = this.GetFolderPath();
                if (!videoInfo.CheckDownloaded(folderPath) || videoInfo.GetFilePath(folderPath).IsNullOrEmpty())
                {
                    this.SnackbarMessageQueue.Enqueue("動画が保存されていません。");
                    return;
                }

                string? appPath = WS::Mainpage.SettingHandler.GetStringSetting(Settings.AppIdPath);
                string param = WS::Mainpage.SettingHandler.GetStringSetting(Settings.AppIdParam) ?? string.Empty;

                if (appPath is not null)
                {
                    ProcessEx.StartWithShell(appPath, $"{param} {videoInfo.NiconicoId}");
                }
            });

            this.CreatePlaylistCommand = new CommandBase<string>(_ => true, arg =>
            {
                if (this.Playlist is null)
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

                var result = WS::Mainpage.PlaylistCreator.TryCreatePlaylist(videos, this.Playlist.Name, folderPath, type);

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

        /// <summary>
        /// 動画を追加する
        /// </summary>
        public CommandBase<object> AddVideoCommand { get; init; }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        public CommandBase<IVideoListInfo> RemoveVideoCommand { get; init; }

        /// <summary>
        /// ニコニコ動画で開く
        /// </summary>
        public CommandBase<IVideoListInfo> WatchOnNiconicoCommand { get; init; }

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
        public CommandBase<IVideoListInfo> UpdateVideoCommand { get; init; }

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
        public CommandBase<object> SendIdToappCommand { get; init; }

        /// <summary>
        /// アプリにIdを送る
        /// </summary>
        public CommandBase<object> SendUrlToappCommand { get; init; }

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

        /// <summary>
        /// メッセージボックスを表示するコマンド
        /// </summary>
        private readonly Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessageBox;

        /// <summary>
        /// 動画のリスト
        /// </summary>
        public ObservableCollection<IVideoListInfo> Videos { get; private set; } = WS::Mainpage.CurrentPlaylist.Videos;

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

        private string inputStringFIeld = string.Empty;

        private string playlistTitleFIeld = string.Empty;

        private bool isFilteringOnlyByTagEnableField;

        private bool isFilteringFromAllVideosEnableField;

        /// <summary>
        /// タスクキャンセラー
        /// </summary>
        private CancellationTokenSource? cts;

        /// <summary>
        /// プレイリストのタイトル
        /// </summary>
        public string PlaylistTitle { get => this.playlistTitleFIeld; set => this.SetProperty(ref this.playlistTitleFIeld, value); }

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

        public ITreePlaylistInfo? Playlist
        {
            get { return this.selectedList; }
            set { this.SetProperty(ref this.selectedList, value); }
        }

        private ITreePlaylistInfo? selectedList;

        /// <summary>
        /// 選択したプレイリストが変更された場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedPlaylistChanged(object? sender, EventArgs e)
        {
            if (WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist is not null && WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist.ChildrensIds.Count == 0)
            {
                this.Playlist = WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist;
                this.RaiseOverallCanExecuteChanged();
                WS::Mainpage.Messagehandler.ClearMessage();
                string name = WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist.Name;
                WS::Mainpage.Messagehandler.AppendMessage($"プレイリスト:{name}");
                WS::Mainpage.SnaclbarHandler.Enqueue($"プレイリスト:{name}");
                this.ChangePlaylistTitle();
            }
        }

        /// <summary>
        /// プレイリストのタイトルを変更する
        /// </summary>
        private void ChangePlaylistTitle()
        {
            if (this.Playlist is not null)
            {
                this.PlaylistTitle = $"{this.Playlist.Name}({this.Videos.Count}件)";
            }
        }

        /// <summary>
        /// 並び替える
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="orderBy"></param>
        public void SetOrder(SortType sortType, OrderBy orderBy)
        {
            var videos = new List<IVideoListInfo>(this.Videos);
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
                    _ => videos,
                });
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

        private string GetFolderPath()
        {
            if (this.Playlist is null) return string.Empty;

            return this.Playlist.Folderpath.IsNullOrEmpty() ? WS::Mainpage.SettingHandler.GetStringSetting(Settings.DefaultFolder) ?? "downloaded" : this.Playlist.Folderpath;
        }
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
            WS::Mainpage.CurrentPlaylist.SelectedItemChanged += this.OnSourceChanged;
            this.AssociatedObject.AddHandler(GridViewColumnHeader.ClickEvent, this.headerClickedHandler);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            WS::Mainpage.CurrentPlaylist.SelectedItemChanged -= this.OnSourceChanged;
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
            if (context.Playlist is null) return;
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
    }

    enum OrderBy
    {
        None,
        Ascending,
        Descending,
    }
}
