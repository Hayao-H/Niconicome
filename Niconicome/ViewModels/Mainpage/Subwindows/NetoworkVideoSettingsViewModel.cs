using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Network;
using Niconicome.Models.Playlist;
using Playlist = Niconicome.Models.Playlist.Playlist;
using WS = Niconicome.Workspaces;


namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class NetoworkVideoSettingsViewModel : BindableBase
    {
        public NetoworkVideoSettingsViewModel()
        {
            var mylist = new ComboBoxItem("マイリスト", Playlist::RemoteType.Mylist);
            var userVideo = new ComboBoxItem("投稿動画", Playlist::RemoteType.UserVideos);
            var watchLater = new ComboBoxItem("あとで見る", Playlist::RemoteType.WatchLater);
            var channel = new ComboBoxItem("チャンネル", Playlist::RemoteType.Channel);
            var none = new ComboBoxItem("設定しない", Playlist::RemoteType.None);

            this.NetworkSettings = new ObservableCollection<ComboBoxItem>()
            {
                userVideo,mylist,watchLater,channel,none
            };


            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null && WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.IsRemotePlaylist)
            {
                this.idField = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteId;

                this.currentSettingField = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.RemoteType switch
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
                this.idField = string.Empty;
                this.currentSettingField = none;
            }

            this.SetRemotePlaylistCommand = new CommandBase<Window>(arg => !this.isfetching, async arg =>
            {
                if ((this.Id == string.Empty && this.CurrentSetting.NetworkMode != Playlist::RemoteType.WatchLater) || this.CurrentSetting is null) return;

                if (arg is null) return;

                if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is null) return;


                if (arg is Window window)
                {

                    this.isfetching = true;
                    this.SetRemotePlaylistCommand?.RaiseCanExecutechanged();

                    string messagePropName = nameof(this.Message);
                    string id = this.Id;

                    this.Id = string.Empty;

                    this.messageField.Clear();
                    this.OnPropertyChanged(messagePropName);

                    var videos = new List<IListVideoInfo>();
                    int playlistID = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value.Id;

                    this.Message = "情報を取得中です...";

                    var result = await WS::Mainpage.RemotePlaylistHandler.TryGetRemotePlaylistAsync(id, videos, this.CurrentSetting.NetworkMode, new List<string>(), m =>
                    {
                        this.Message = m;
                        WS::Mainpage.Messagehandler.AppendMessage(m);
                    });


                    if (!result.IsSucceeded)
                    {
                        string detail = result.Exception?.Message ?? "None";
                        this.Message = $"{this.CurrentSetting.Name}(id:{id})の取得に失敗しました。({result.Message})";
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
                        WS::Mainpage.VideoListContainer.AddRange(videos, playlistID);
                        WS::Mainpage.VideoListContainer.Refresh();
                    }

                    if (this.CurrentSetting.NetworkMode == Playlist::RemoteType.None)
                    {
                        WS::Mainpage.PlaylistHandler.SetAsLocalPlaylist(playlistID);

                    }
                    else
                    {
                        WS::Mainpage.PlaylistHandler.SetAsRemotePlaylist(playlistID, id, result.Data!, this.CurrentSetting.NetworkMode);
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
        public ObservableCollection<ComboBoxItem> NetworkSettings { get; init; }

        private ComboBoxItem currentSettingField;

        private string idField;

        /// <summary>
        /// 同期中フラグ
        /// </summary>
        private bool isfetching;

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get => this.idField; set => this.SetProperty(ref this.idField, value); }

        /// <summary>
        /// 現在の設定
        /// </summary>
        public ComboBoxItem CurrentSetting
        {
            get => this.currentSettingField; set => this.SetProperty(ref this.currentSettingField, value);
        }

        private readonly StringBuilder messageField = new();

        /// <summary>
        /// 状態を表すメッセージ
        /// </summary>
        public string Message { get => this.messageField.ToString(); set { this.messageField.AppendLine(value); this.OnPropertyChanged(); } }
    }

    class ComboBoxItem
    {
        public ComboBoxItem(string name, Playlist::RemoteType networkMode)
        {
            this.Name = name;
            this.NetworkMode = networkMode;
        }

        /// <summary>
        /// 設定名
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 列挙値
        /// </summary>
        public Playlist::RemoteType NetworkMode { get; init; }
    }
}
