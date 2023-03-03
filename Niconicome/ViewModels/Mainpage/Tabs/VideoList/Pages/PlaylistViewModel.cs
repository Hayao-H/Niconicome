using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class PlaylistViewModel
    {
        public PlaylistViewModel(NavigationManager navigation)
        {
            this._navigation = navigation;
            this.Bindables = new Bindables();

            this.SelectablePlaylistType = new List<string>() { PlaylistTypeString.WatchLater, PlaylistTypeString.Mylist, PlaylistTypeString.Series, PlaylistTypeString.UserVideos, PlaylistTypeString.Channel, PlaylistTypeString.Local }.AsReadOnly();
            this.CurrentPlaylistType = new BindableProperty<string>(string.Empty).Subscribe(v => this.IsRemotePlaylist.Value = v != PlaylistTypeString.Local).AddTo(this.Bindables);
            this.CanEditPlaylisType = this._canEditPlaylistType.AsReadOnly().AddTo(this.Bindables);
        }

        #region field

        private IPlaylistInfo? _playlist;

        private readonly NavigationManager _navigation;

        private readonly BindableProperty<bool> _canEditPlaylistType = new BindableProperty<bool>(false);

        #endregion

        #region Props

        /// <summary>
        /// プレイリスト名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 動画数
        /// </summary>
        public int VideosCount { get; private set; }

        /// <summary>
        /// プレイリストID
        /// </summary>
        public int PlaylistID { get; private set; }

        /// <summary>
        /// 保存先パス
        /// </summary>
        public string DirectoryPath { get; set; } = string.Empty;

        /// <summary>
        /// プレイリスト形式
        /// </summary>
        public IBindableProperty<string> CurrentPlaylistType { get; init; }

        /// <summary>
        /// リモートプレイリストのパラメーター
        /// </summary>
        public string RemotePlaylistParam { get; set; } = string.Empty;

        /// <summary>
        /// リモートプレイリストであるかどうか
        /// </summary>
        public IBindableProperty<bool> IsRemotePlaylist { get; init; } = new BindableProperty<bool>(false);

        /// <summary>
        /// プレイリストの種類を変更できるかどうか
        /// </summary>
        public IReadonlyBindablePperty<bool> CanEditPlaylisType { get; init; }

        /// <summary>
        /// 選択可能なプレイリスト形式
        /// </summary>
        public IReadOnlyList<string> SelectablePlaylistType { get; init; }

        /// <summary>
        /// 変更監視用オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; }

        #endregion

        #region Method

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(string playlistID)
        {
            if (!int.TryParse(playlistID, out int id))
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
                this._navigation.NavigateTo("/videos");
                return;
            }

            IAttemptResult<IPlaylistInfo> pResult = WS::Mainpage.PlaylistManager.GetPlaylist(id);
            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
                this._navigation.NavigateTo("/videos");
                return;
            }

            this._playlist = pResult.Data;

            this.Name = this._playlist.Name.Value;
            this.VideosCount = this._playlist.Videos.Count;
            this.PlaylistID = this._playlist.ID;
            this.DirectoryPath = this._playlist.FolderPath;
            this.RemotePlaylistParam = this._playlist.RemoteParameter;
            this.CurrentPlaylistType.Value = this.Convert(this._playlist.PlaylistType);
            this.IsRemotePlaylist.Value = this.CurrentPlaylistType.Value != PlaylistTypeString.Local;
            this._canEditPlaylistType.Value = this._playlist.PlaylistType != PlaylistType.Root && this._playlist.PlaylistType != PlaylistType.Temporary && this._playlist.PlaylistType != PlaylistType.PlaybackHistory && this._playlist.PlaylistType != PlaylistType.DownloadSucceededHistory && this._playlist.PlaylistType != PlaylistType.DownloadFailedHistory;
        }

        /// <summary>
        /// 変更を保存
        /// </summary>
        public void Update()
        {
            if (this._playlist is null)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
                this._navigation.NavigateTo("/videos");
                return;
            }

            IPlaylistInfo playlist = this._playlist;
            playlist.Name.Value = this.Name;
            playlist.FolderPath = this.DirectoryPath;
            playlist.PlaylistType = this.Convert(this.CurrentPlaylistType.Value, playlist.PlaylistType);
            playlist.RemoteParameter = this.RemotePlaylistParam;
        }

        /// <summary>
        /// 動画一覧に戻る
        /// </summary>
        public void ReurnToIndex()
        {
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos", BlazorWindows.MainPage);
            this._navigation.NavigateTo("/videos");
        }

        #endregion

        #region private

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="playlistType"></param>
        /// <returns></returns>
        private string Convert(PlaylistType playlistType)
        {
            return playlistType switch
            {
                PlaylistType.WatchLater => PlaylistTypeString.WatchLater,
                PlaylistType.Mylist => PlaylistTypeString.Mylist,
                PlaylistType.Series => PlaylistTypeString.Series,
                PlaylistType.UserVideos => PlaylistTypeString.UserVideos,
                PlaylistType.Channel => PlaylistTypeString.Channel,
                _ => PlaylistTypeString.Local,
            };
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="playlistType"></param>
        /// <returns></returns>
        private PlaylistType Convert(string playlistType, PlaylistType currentType)
        {
            return playlistType switch
            {
                PlaylistTypeString.WatchLater => PlaylistType.WatchLater,
                PlaylistTypeString.Mylist => PlaylistType.Mylist,
                PlaylistTypeString.Series => PlaylistType.Series,
                PlaylistTypeString.UserVideos => PlaylistType.UserVideos,
                PlaylistTypeString.Channel => PlaylistType.Channel,
                _ => currentType,
            };

        }

        private static class PlaylistTypeString
        {
            public const string WatchLater = "後で見る";

            public const string Mylist = "マイリスト";

            public const string Series = "シリーズ";

            public const string UserVideos = "ユーザー投稿動画";

            public const string Channel = "チャンネル";

            public const string Local = "ローカル";
        }

        #endregion
    }
}
