using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.ViewModels.Mainpage.Utils;
using Reactive.Bindings;
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
                    Playlist::RemoteType.Series => series,
                    _ => none
                };

            }
            else
            {
                this.Id.Value = string.Empty;
                this.RemoteType.Value = none;
            }



            this.SetRemotePlaylistCommand = WS::Mainpage.VideoRefreshManager.IsFetching.Select(x => !x).ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
           {
               //変数定義
               string remoteID = this.Id.Value;
               Playlist::RemoteType remoteType = this.RemoteType.Value.Value;
               ITreePlaylistInfo? playlistInfo = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value;

               if (string.IsNullOrEmpty(remoteID) && remoteType != Playlist::RemoteType.WatchLater && remoteType != Playlist::RemoteType.None)
               {
                   this.AppendMessage("「あとで見る」以外のリモートプレイリストを設定するには、IDの入力が必要です。");
                   return;
               }
               else if (playlistInfo is null)
               {

                   this.AppendMessage("「プレイリストが選択されていません。");
                   return;
               }

               //そもそもリモートじゃない
               if (remoteType == Playlist::RemoteType.None)
               {
                   WS::Mainpage.PlaylistHandler.SetAsLocalPlaylist(playlistInfo.Id);
                   this.AppendMessage("リモートプレイリストとの同期を解除しました。");
                   return;
               }

               //一時的に情報を設定
               playlistInfo.RemoteId = remoteID;
               playlistInfo.RemoteType = remoteType;
               playlistInfo.IsRemotePlaylist = true;

               //更新処理
               this.AppendMessage("情報を取得中です...");
               IAttemptResult<string> result = await WS::Mainpage.VideoRefreshManager.RefreshRemoteAndSaveAsync();

               if (!result.IsSucceeded)
               {
                   this.AppendMessage($"情報の取得に失敗しました。(詳細:{result.Message})");
                   return;
               }
               else
               {
                   this.AppendMessage($"リモートプレイリストと同期しました。({result.Message})");
               }

               WS::Mainpage.PlaylistHandler.SetAsRemotePlaylist(playlistInfo.Id, remoteID, result.Data ?? String.Empty, remoteType);

           });

        }

        #region field

        private readonly StringBuilder builder = new();

        #endregion


        #region Commands

        /// <summary>
        /// 設定コマンド
        /// </summary>
        public AsyncReactiveCommand SetRemotePlaylistCommand { get; init; }

        #endregion

        #region Props

        /// <summary>
        /// 設定一覧
        /// </summary>
        public List<ComboboxItem<Playlist::RemoteType>> NetworkSettings { get; init; }

        /// <summary>
        /// 選択可能なタイプ
        /// </summary>
        public ReactiveProperty<ComboboxItem<Playlist::RemoteType>> RemoteType { get; init; } = new();

        /// <summary>
        /// ID
        /// </summary>
        public ReactiveProperty<string> Id { get; init; } = new();

        /// <summary>
        /// 状態を表すメッセージ
        /// </summary>
        public ReactiveProperty<string> Message { get; init; } = new();

        #endregion

        #region private

        private void AppendMessage(string message)
        {
            this.builder.AppendLine(message);
            this.Message.Value = this.builder.ToString();
        }

        private void ClearMessage()
        {
            this.builder.Clear();
            this.Message.Value = "";
        }

        #endregion

    }

}
