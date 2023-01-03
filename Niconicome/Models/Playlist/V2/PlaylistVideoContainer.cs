using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;

namespace Niconicome.Models.Playlist.V2
{
    public interface IPlaylistVideoContainer
    {
        /// <summary>
        /// プレイリストのツリー
        /// </summary>
        ObservableCollection<IPlaylistInfo> Playlist { get; }

        /// <summary>
        /// 現在のプレイリストに登録されている動画 
        /// </summary>
        List<IVideoInfo> Videos { get; }

        /// <summary>
        /// 現在選択されているプレイリスト
        /// </summary>
        IPlaylistInfo? CurrentSelectedPlaylist { get; set; }

        /// <summary>
        /// プレイリスト変更イベントのハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        void AddPlaylistChangeEventHandler(Action<IPlaylistInfo> handler);

        /// <summary>
        /// プレイリスト変更イベントのハンドラを削除
        /// </summary>
        /// <param name="handler"></param>
        void RemovePlaylistChangeEventHandler(Action<IPlaylistInfo> handler);

    }

    public class PlaylistVideoContainer : IPlaylistVideoContainer
    {
        #region field

        private readonly List<Action<IPlaylistInfo>> _playlistChangeHandlers = new();

        private IPlaylistInfo? _currentSelectedPlaylist;

        #endregion

        #region Props

        public ObservableCollection<IPlaylistInfo> Playlist { get; init; } = new();

        public List<IVideoInfo> Videos { get; init; } = new();

        public IPlaylistInfo? CurrentSelectedPlaylist
        {
            get => this._currentSelectedPlaylist;
            set
            {
                this._currentSelectedPlaylist = value;
                this.OnPlaylistChange(value);
            }
        }

        #endregion

        #region Method

        public void AddPlaylistChangeEventHandler(Action<IPlaylistInfo> handler)
        {
            this._playlistChangeHandlers.Add(handler);
        }

        public　void RemovePlaylistChangeEventHandler(Action<IPlaylistInfo> handler)
        {
            this._playlistChangeHandlers.RemoveAll(h => h == handler);
        }


        #endregion

        #region private

        /// <summary>
        /// プレイリストが変更されたとき
        /// </summary>
        /// <param name="playlist"></param>
        private void OnPlaylistChange(IPlaylistInfo? playlist)
        {
            if (playlist is not null)
            {
                foreach (var handler in this._playlistChangeHandlers)
                {
                    try
                    {
                        handler(playlist);
                    }
                    catch { }
                }
            }
        }

        #endregion
    }
}
