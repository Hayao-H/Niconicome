using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation.Text;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Utils.Reactive;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Playlist
{
    public interface IPlaylistInfo : IUpdatable
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
        string FolderPath { get; set; }

        /// <summary>
        /// 起動中のみ保持されるフォルダーパス
        /// </summary>
        string TemporaryFolderPath { get; set; }

        /// <summary>
        /// プレイリスト名
        /// </summary>
        BindableProperty<string> Name { get; }

        /// <summary>
        /// プレイリストの種別
        /// </summary>
        PlaylistType PlaylistType { get; set; }

        /// <summary>
        /// リモートプレイリストの場合はパラメーター
        /// </summary>
        string RemoteParameter { get; set; }

        /// <summary>
        /// 並び替え形式
        /// </summary>
        SortType SortType { get; set; }

        /// <summary>
        /// 昇順・降順
        /// </summary>
        bool IsAscendant { get; set; }

        /// <summary>
        /// 展開フラグ
        /// </summary>
        IBindableProperty<bool> IsExpanded { get; }

        /// <summary>
        /// 選択されている動画数
        /// </summary>
        IReadonlyBindablePperty<int> SelectedVideosCount { get; }

        /// <summary>
        /// 動画数
        /// </summary>
        IReadonlyBindablePperty<int> VideosCount { get; }

        /// <summary>
        /// 子プレイリスト(デフォルトでは空)
        /// </summary>
        ReadOnlyObservableCollection<IPlaylistInfo> Children { get; }

        /// <summary>
        /// 子プレイリストのID
        /// </summary>
        IReadOnlyList<int> ChildrenID { get; }

        /// <summary>
        /// 親プレイリストの名前一覧
        /// </summary>
        IReadOnlyList<string> ParentNames { get; }

        /// <summary>
        /// 動画一覧
        /// </summary>
        IReadOnlyList<IVideoInfo> Videos { get; }

        /// <summary>
        /// 子プレイリストを追加
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <param name="commit"></param>
        IAttemptResult AddChild(IPlaylistInfo playlistInfo, bool commit = true);

        /// <summary>
        /// 子プレイリストを削除
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <returns></returns>
        IAttemptResult RemoveChild(IPlaylistInfo playlistInfo);

        /// <summary>
        /// 動画を追加
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult AddVideo(IVideoInfo video);

        /// <summary>
        /// 動画を削除
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult RemoveVideo(IVideoInfo video);

        /// <summary>
        /// 動画を並び替える(移動先はtargetの直前)
        /// </summary>
        /// <param name="sourceVideo"></param>
        /// <param name="TargetVideo"></param>
        void MoveVideo(string sourceVideoID, string targetVideoID);

        /// <summary>
        /// 親プレイリストの名前一覧をセットする
        /// </summary>
        /// <param name="names"></param>
        void SetParentNamesList(List<string> names);
    }

    public class PlaylistInfo : UpdatableInfoBase<IPlaylistStore, IPlaylistInfo>, IPlaylistInfo
    {
        public PlaylistInfo(string name, List<IVideoInfo> videos, IPlaylistStore playlistStore) : base(playlistStore)
        {
            this.Children = new ReadOnlyObservableCollection<IPlaylistInfo>(this._children);

            this.Name = new BindableProperty<string>(name);
            this.Name.RegisterPropertyChangeHandler(_ =>
            {
                if (this.IsAutoUpdateEnabled) this.Update(this);
            });
            this._videos = videos;
            this.Videos = videos.AsReadOnly();

            this._videosCount = new BindableProperty<int>(videos.Count);
            this.VideosCount = this._videosCount.AsReadOnly();

            this.SelectedVideosCount = this._selectedVideosCount.AsReadOnly();
            foreach (var video in videos)
            {
                video.IsSelected.Subscribe(this.OnSelectedChange);
            }

            this.IsExpanded = new BindableProperty<bool>(false);
            this.IsExpanded.Subscribe(v =>
            {
                if (this.IsAutoUpdateEnabled)
                {
                    this.Update(this);
                }
            });

        }

        #region field

        private readonly ObservableCollection<IPlaylistInfo> _children = new();

        private readonly List<IVideoInfo> _videos = new();

        private List<string> _parentNames = new();

        private string _folderPath = string.Empty;

        private string _remoteParameter = string.Empty;

        private SortType _sortType;

        private bool _isAscendant;

        private PlaylistType _playlistType = PlaylistType.Local;

        private readonly BindableProperty<int> _selectedVideosCount = new(0);

        private readonly BindableProperty<int> _videosCount;


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
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public string TemporaryFolderPath { get; set; } = string.Empty;

        public BindableProperty<string> Name { get; init; }

        public PlaylistType PlaylistType
        {
            get => this._playlistType;
            set
            {
                this._playlistType = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public SortType SortType
        {
            get => this._sortType;
            set
            {
                this._sortType = value;
                this.SortVideos();
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public bool IsAscendant
        {
            get => this._isAscendant;
            set
            {
                this._isAscendant = value;
                this.SortVideos();
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public IBindableProperty<bool> IsExpanded { get; init; }


        public IReadonlyBindablePperty<int> SelectedVideosCount { get; init; }

        public IReadonlyBindablePperty<int> VideosCount { get; init; }


        public string RemoteParameter
        {
            get => this._remoteParameter;
            set
            {
                this._remoteParameter = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }


        public ReadOnlyObservableCollection<IPlaylistInfo> Children { get; init; }

        public IReadOnlyList<int> ChildrenID { get; init; } = (new List<int>()).AsReadOnly();

        public IReadOnlyList<IVideoInfo> Videos { get; init; }

        public IReadOnlyList<string> ParentNames => this._parentNames.AsReadOnly();

        #endregion

        #region Method

        public IAttemptResult AddChild(IPlaylistInfo playlistInfo, bool commit = true)
        {
            this._children.Add(playlistInfo);

            return commit && this.IsAutoUpdateEnabled ? this.Update(this) : AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveChild(IPlaylistInfo playlistInfo)
        {
            this._children.Remove(playlistInfo);

            return this.IsAutoUpdateEnabled ? this.Update(this) : AttemptResult.Succeeded();
        }

        public IAttemptResult AddVideo(IVideoInfo video)
        {
            this._videosCount.Value++;
            video.IsSelected.Subscribe(this.OnSelectedChange);
            this.SetSelectedVideos();

            this._videos.Add(video);
            return this.IsAutoUpdateEnabled ? this.Update(this) : AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveVideo(IVideoInfo video)
        {
            this._videosCount.Value--;
            video.IsSelected.UnRegisterPropertyChangeHandler(this.OnSelectedChange);
            this.SetSelectedVideos();

            this._videos.RemoveAll(v => v.NiconicoId == video.NiconicoId);
            return this.IsAutoUpdateEnabled ? this.Update(this) : AttemptResult.Succeeded();
        }

        public void MoveVideo(string sourceVideoID, string targetVideoID)
        {
            IVideoInfo source = this.Videos.First(v => v.NiconicoId == sourceVideoID);
            IVideoInfo target = this.Videos.First(v => v.NiconicoId == targetVideoID);

            int sourceIndex = this._videos.IndexOf(source);
            int targetIndex = this._videos.IndexOf(target);

            this._videos.Remove(source);

            //上⇒下
            if (sourceIndex < targetIndex)
            {
                this._videos.Insert(targetIndex - 1, source);
            }
            //下⇒上
            else
            {
                this._videos.Insert(targetIndex, source);
            }

            this.SortType = SortType.Custom;
            this.Update(this);

        }

        public void SetParentNamesList(List<string> names)
        {
            this._parentNames = names;
        }

        #endregion

        #region private

        private void SortVideos()
        {
            if (this.SortType == SortType.Custom)
            {
                return;
            }

            var sortedList = new List<IVideoInfo>();

            if (this.IsAscendant)
            {
                var sorted = this.SortType switch
                {
                    SortType.Title => this.Videos.OrderBy(v => v.Title),
                    SortType.UploadedOn => this.Videos.OrderBy(v => v.UploadedOn),
                    SortType.AddedAt => this.Videos.OrderBy(v => v.AddedAt),
                    SortType.ViewCount => this.Videos.OrderBy(v => v.ViewCount),
                    SortType.CommentCount => this.Videos.OrderBy(v => v.CommentCount),
                    SortType.MylistCount => this.Videos.OrderBy(v => v.MylistCount),
                    SortType.LikeCount => this.Videos.OrderBy(v => v.LikeCount),
                    SortType.IsDownlaoded => this.Videos.OrderBy(v => v.IsDownloaded.Value ? 1 : 0),
                    _ => this.Videos.OrderBy(v => int.Parse(Regex.Replace(v.NiconicoId, @"\D", "")))
                };

                sortedList.AddRange(sorted.ToList());
            }
            else
            {
                var sorted = this.SortType switch
                {
                    SortType.Title => this.Videos.OrderByDescending(v => v.Title),
                    SortType.UploadedOn => this.Videos.OrderByDescending(v => v.UploadedOn),
                    SortType.AddedAt => this.Videos.OrderByDescending(v => v.AddedAt),
                    SortType.ViewCount => this.Videos.OrderByDescending(v => v.ViewCount),
                    SortType.CommentCount => this.Videos.OrderByDescending(v => v.CommentCount),
                    SortType.MylistCount => this.Videos.OrderByDescending(v => v.MylistCount),
                    SortType.LikeCount => this.Videos.OrderByDescending(v => v.LikeCount),
                    SortType.IsDownlaoded => this.Videos.OrderByDescending(v => v.IsDownloaded.Value ? 1 : 0),
                    _ => this.Videos.OrderByDescending(v => int.Parse(Regex.Replace(v.NiconicoId, @"\D", "")))
                };

                sortedList.AddRange(sorted.ToList());
            }

            this._videos.Clear();
            this._videos.AddRange(sortedList);

            this.Update(this);
        }

        private void OnSelectedChange(bool value)
        {
            if (value)
            {
                this._selectedVideosCount.Value++;
            }
            else
            {
                this._selectedVideosCount.Value--;
            }
        }

        private void SetSelectedVideos()
        {
            this._selectedVideosCount.Value = this.Videos.Select(v => v.IsSelected.Value).Count();
        }

        #endregion

        public override string ToString()
        {
            return this.Name.Value;
        }
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

    public enum SortType
    {
        NiconicoID,
        Title,
        UploadedOn,
        AddedAt,
        ViewCount,
        CommentCount,
        MylistCount,
        LikeCount,
        IsDownlaoded,
        Custom,
    }
}
