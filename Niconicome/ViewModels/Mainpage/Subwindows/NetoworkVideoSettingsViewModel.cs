using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Mainpage.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Playlist = Niconicome.Models.Playlist.Playlist;
using WS = Niconicome.Workspaces;


namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class NetoworkVideoSettingsViewModel : BindableBase
    {
        public NetoworkVideoSettingsViewModel()
        {
            var mylist = new ComboboxItem<Playlist::RemoteType>(Playlist::RemoteType.Mylist, "マイリスト");
            var userVideo = new ComboboxItem<Playlist::RemoteType>(Playlist::RemoteType.UserVideos, "投稿動画");
            var watchLater = new ComboboxItem<Playlist::RemoteType>(Playlist::RemoteType.WatchLater, "あとで見る");
            var channel = new ComboboxItem<Playlist::RemoteType>(Playlist::RemoteType.Channel, "チャンネル");
            var series = new ComboboxItem<Playlist::RemoteType>(Playlist::RemoteType.Series, "シリーズ");
            var none = new ComboboxItem<Playlist::RemoteType>(Playlist::RemoteType.None, "設定しない");

            this.NetworkSettings = new List<ComboboxItem<Playlist.RemoteType>>()
            {
                userVideo,mylist,watchLater,channel,series,none
            };


            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.IsRemotePlaylist)
            {
                this.Id.Value = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteId;

                this.RemoteType.Value = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteType switch
                {
                    Playlist::RemoteType.Mylist => mylist,
                    Playlist::RemoteType.UserVideos => userVideo,
                    Playlist::RemoteType.WatchLater => watchLater,
                    Playlist::RemoteType.Channel => channel,
                    _ => none
                };

            }
            else
            {
                this.Id.Value = string.Empty;
                this.RemoteType.Value = none;
            }



            this.SetRemotePlaylistCommand = new CommandBase<Window>(arg => !this.isfetching, async arg =>
            {
                if ((this.Id.Value == string.Empty && this.RemoteType.Value.Value != Playlist::RemoteType.WatchLater)) return;

                if (arg is null) return;

                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null) return;


                if (arg is Window window)
                {

                    this.isfetching = true;
                    this.SetRemotePlaylistCommand?.RaiseCanExecutechanged();

                    string messagePropName = nameof(this.Message);
                    string id = this.Id.Value;

                    this.Id.Value = string.Empty;

                    this.messageField.Clear();
                    this.OnPropertyChanged(messagePropName);

                    var videos = new List<IListVideoInfo>();
                    int playlistID = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Id;

                    this.Message = "情報を取得中です...";

                    var result = await WS::Mainpage.RemotePlaylistHandler.TryGetRemotePlaylistAsync(id, videos, this.RemoteType.Value.Value, new List<string>(), m =>
                    {
                        this.Message = m;
                        WS::Mainpage.Messagehandler.AppendMessage(m);
                    });


                    if (!result.IsSucceeded)
                    {
                        string detail = result.Exception?.Message ?? "None";
                        this.Message = $"{this.RemoteType.Value.DisplayValue}(id:{id})の取得に失敗しました。({result.Message})";
                        this.Message = $"詳細情報:{detail}";

                        this.isfetching = false;
                        this.SetRemotePlaylistCommand?.RaiseCanExecutechanged();

                        return;
                    }
                    else if (videos.Count == 0)
                    {
                        this.Message = "取得に成功しましたが、動画は一件も存在しませんでした。";
                    }



                    if (videos.Count > 0)
                    {
                        WS::Mainpage.VideoListContainer.AddRange(videos, playlistID, commit: !WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);
                        WS::Mainpage.VideoListContainer.Refresh();
                    }

                    if (this.RemoteType.Value.Value == Playlist::RemoteType.None)
                    {
                        WS::Mainpage.PlaylistHandler.SetAsLocalPlaylist(playlistID);

                    }
                    else
                    {
                        WS::Mainpage.PlaylistHandler.SetAsRemotePlaylist(playlistID, id, result.Data!, this.RemoteType.Value.Value);
                    }

                    this.Message = "取得処理が完了しました。";
                    this.isfetching = false;
                    this.SetRemotePlaylistCommand?.RaiseCanExecutechanged();
                }




            });

        }

        public CommandBase<Window> SetRemotePlaylistCommand { get; init; }

        /// <summary>
        /// 設定一覧
        /// </summary>
        public List<ComboboxItem<Playlist::RemoteType>> NetworkSettings { get; init; }

        public ReactiveProperty<ComboboxItem<Playlist::RemoteType>> RemoteType { get; init; } = new();


        /// <summary>
        /// 同期中フラグ
        /// </summary>
        private bool isfetching;

        /// <summary>
        /// ID
        /// </summary>
        public ReactiveProperty<string> Id { get; init; } = new();

        private readonly StringBuilder messageField = new();

        /// <summary>
        /// 状態を表すメッセージ
        /// </summary>
        public string Message { get => this.messageField.ToString(); set { this.messageField.AppendLine(value); this.OnPropertyChanged(); } }
    }

}
