﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Niconicome.Models.Domain.Playlist;
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
        public void Initialize()
        {
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos");
                this._navigation.NavigateTo("/videos");
                return;
            }

            IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;

            this.Name = playlist.Name.Value;
            this.VideosCount = playlist.Videos.Count;
            this.DirectoryPath = playlist.FolderPath;
            this.RemotePlaylistParam = playlist.RemoteParameter;
            this.CurrentPlaylistType.Value = this.Convert(playlist.PlaylistType);
            this.IsRemotePlaylist.Value = this.CurrentPlaylistType.Value != PlaylistTypeString.Local;
            this._canEditPlaylistType.Value = playlist.PlaylistType != PlaylistType.Root && playlist.PlaylistType != PlaylistType.Temporary && playlist.PlaylistType != PlaylistType.PlaybackHistory && playlist.PlaylistType != PlaylistType.DownloadSucceededHistory && playlist.PlaylistType != PlaylistType.DownloadFailedHistory;
        }

        /// <summary>
        /// 変更を保存
        /// </summary>
        public void Update()
        {
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos");
                this._navigation.NavigateTo("/videos");
                return;
            }

            IPlaylistInfo playlist = WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist;
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
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos");
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
