using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Playlist
{
    public interface IPlaylistInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        int ID { get; }

        /// <summary>
        /// 親プレイリストのID
        /// </summary>
        int ParentID { get; set; }

        /// <summary>
        /// フォルダーパス
        /// </summary>
        string FolderPath { get; }

        /// <summary>
        /// プレイリスト名
        /// </summary>
        ReactiveProperty<string> Name { get; }

        /// <summary>
        /// プレイリストの種別
        /// </summary>
        PlaylistType PlaylistType { get; set; }

        /// <summary>
        /// リモートプレイリストの場合はパラメーター
        /// </summary>
        string RemoteParameter { get; set; }

        /// <summary>
        /// 子プレイリスト(デフォルトでは空)
        /// </summary>
        ReadOnlyObservableCollection<IPlaylistInfo> Children { get; }

        /// <summary>
        /// 子プレイリストのID
        /// </summary>
        IReadOnlyList<int> ChildrenID { get; }

        /// <summary>
        /// 動画一覧
        /// </summary>
        IReadOnlyList<IVideoInfo> Videos { get; }

        /// <summary>
        /// 子プレイリストを追加
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <param name="commit"></param>
        void AddChild(IPlaylistInfo playlistInfo, bool commit = true);

        /// <summary>
        /// 子プレイリストを削除
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <returns></returns>
        IAttemptResult RemoveChild(IPlaylistInfo playlistInfo);
    }

    public class PlaylistInfo : UpdatableInfoBase<IPlaylistStore, IPlaylistInfo>, IPlaylistInfo
    {
        public PlaylistInfo(string name, List<IVideoInfo> videos, IPlaylistStore playlistStore) : base(playlistStore)
        {
            this.Children = new ReadOnlyObservableCollection<IPlaylistInfo>(this._children);
            this.Name = new ReactiveProperty<string>(name);
            this.Name.Subscribe(_ => this.Update(this));
            this.Videos = videos.AsReadOnly();
        }

        #region field

        private readonly ObservableCollection<IPlaylistInfo> _children = new();

        private string _folderPath = string.Empty;

        private string _remoteParameter = string.Empty;

        private PlaylistType _playlistType = PlaylistType.Local;

        #endregion

        #region Props

        public int ID { get; init; }

        public int ParentID { get; set; }

        public string FolderPath
        {
            get => this._folderPath;
            set
            {
                this._folderPath = value;
                this.Update(this);
            }
        }

        public ReactiveProperty<string> Name { get; init; } = new();

        public PlaylistType PlaylistType
        {
            get => this._playlistType;
            set
            {
                this._playlistType = value;
                this.Update(this);
            }
        }


        public string RemoteParameter
        {
            get => this._remoteParameter;
            set
            {
                this._remoteParameter = value;
                this.Update(this);
            }
        }

        public ReadOnlyObservableCollection<IPlaylistInfo> Children { get; init; }

        public IReadOnlyList<int> ChildrenID { get; init; } = new List<int>().AsReadOnly();

        public IReadOnlyList<IVideoInfo> Videos { get; init; }


        #endregion

        #region Method

        public void AddChild(IPlaylistInfo playlistInfo, bool commit = true)
        {
            this._children.Add(playlistInfo);
            if (commit) this.Update(this);
        }

        public IAttemptResult RemoveChild(IPlaylistInfo playlistInfo)
        {
            this._children.Remove(playlistInfo);
            return this.Update(this);
        }

        #endregion
    }

    public enum PlaylistType
    {
        Local,
        Mylist,
        Series,
        WatchLater,
        UserVideos,
        Channel,
        Root,
        Temporary,
        DownloadSucceededHistory,
        DownloadFailedHistory,
        PlaybackHistory,
    }
}
