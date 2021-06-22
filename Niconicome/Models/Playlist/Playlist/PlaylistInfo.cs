using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using LiteDB;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.ViewModels;
using Reactive.Bindings;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface ITreePlaylistInfo : IClonable<ITreePlaylistInfo>
    {
        int Id { get; set; }
        ReactiveProperty<string> Name { get;  }
        string RemoteId { get; set; }
        string Folderpath { get; set; }
        ObservableCollection<ITreePlaylistInfo> Children { get; }
        List<IListVideoInfo> Videos { get; }
        List<int> CustomSortSequence { get; }
        int ParentId { get; set; }
        int BookMarkedVideoID { get; set; }
        bool IsExpanded { get; set; }
        bool IsRoot { get; set; }
        bool IsConcrete { get; }
        bool IsVideoDescending { get; set; }
        bool IsDownloadSucceededHistory { get; set; }
        bool IsDownloadFailedHistory { get; set; }
        bool IsTemporary { get; set; }
        Brush BackgroundColor { get; set; }
        Visibility BeforeSeparatorVisibility { get; set; }
        Visibility AfterSeparatorVisibility { get; set; }
        bool HasChildPlaylist { get; }
        bool HasVideo(string niconicoId);
        bool IsRemotePlaylist { get; set; }
        RemoteType RemoteType { get; set; }
        int GetLayer(IPlaylistTreeHandler handler);
        VideoSortType VideoSortType { get; set; }
        void UpdateData(ITreePlaylistInfo newPlaylist);

    }


    /// <summary>
    /// バインド不可能なTreePlaylistInfo
    /// </summary>
    public class NonBindableTreePlaylistInfo : BindableBase, ITreePlaylistInfo
    {
        /// <summary>
        /// プレイリストID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// プレイリスト名
        /// </summary>
        public ReactiveProperty<string> Name { get; init; } = new(string.Empty);

        /// <summary>
        /// リモートID
        /// </summary>
        public string RemoteId { get; set; } = string.Empty;

        /// <summary>
        /// フォルダー名
        /// </summary>
        public string Folderpath { get; set; } = string.Empty;

        /// <summary>
        /// 子プレイリスト一覧
        /// 通常は空のリスト
        /// </summary>
        public ObservableCollection<ITreePlaylistInfo> Children { get; private set; } = new();

        /// <summary>
        /// 動画情報のリスト
        /// </summary>
        public List<IListVideoInfo> Videos { get; init; } = new();

        /// <summary>
        /// 動画の並び替え順
        /// </summary>
        public List<int> CustomSortSequence { get; init; } = new();

        /// <summary>
        /// 親プレイリストのID
        /// </summary>
        public int ParentId { get; set; } = -1;

        /// <summary>
        /// ブックマークされた動画
        /// </summary>
        public int BookMarkedVideoID { get; set; }

        /// <summary>
        /// 開閉状態
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// ルートフラグ
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// 末端プレイリストであるかどうかを返す
        /// </summary>
        public bool IsConcrete
        {
            get { return this.Videos.Count > 0; }
        }

        /// <summary>
        /// リモートプレイリストフラグ
        /// </summary>
        public bool IsRemotePlaylist { get; set; }

        /// <summary>
        /// 選択フラグ
        /// </summary>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// 降順フラグ
        /// </summary>
        public bool IsVideoDescending { get; set; }

        /// <summary>
        /// DL成功履歴
        /// </summary>
        public bool IsDownloadSucceededHistory { get; set; }

        /// <summary>
        /// DL失敗履歴
        /// </summary>
        public bool IsDownloadFailedHistory { get; set; }

        /// <summary>
        /// 一時プレイリスト
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// リモート設定
        /// </summary>
        public RemoteType RemoteType { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public virtual Brush BackgroundColor { get; set; } = Brushes.White;

        /// <summary>
        /// 上部セパレーターの可視性
        /// </summary>
        public virtual Visibility BeforeSeparatorVisibility { get; set; } = Visibility.Hidden;

        /// <summary>
        /// 下部セパレーターの可視性
        /// </summary>
        public virtual Visibility AfterSeparatorVisibility { get; set; } = Visibility.Hidden;

        /// <summary>
        /// 並び替え順
        /// </summary>
        public VideoSortType VideoSortType { get; set; }

        /// <summary>
        /// 子プレイリストが存在する場合
        /// </summary>
        public bool HasChildPlaylist
        {
            get
            {
                return this.Children.Count > 0;
            }
        }

        /// <summary>
        /// 動画が存在するかどうかを返す
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public bool HasVideo(string niconicoId)
        {
            if (this.Videos is null) return false;

            return this.Videos.Any(v => v.NiconicoId.Value == niconicoId);
        }

        /// <summary>
        /// レイヤーを取得する
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public int GetLayer(IPlaylistTreeHandler handler)
        {

            int layer = 1;

            if (this.ParentId == -1)
            {
                return layer;
            }

            int parentId = this.Id;
            while ((parentId = handler.GetParent(parentId)?.Id ?? -1) != -1)
            {
                ++layer;
            }
            return layer;
        }

        /// <summary>
        /// DBの型をNonBindableTreePlaylistInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public static ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist)
        {

            var converted = new NonBindableTreePlaylistInfo();
            SetData(playlist, converted);
            return converted;
        }

        /// <summary>
        /// 別のプレイリストのデータを移行する
        /// </summary>
        /// <param name="newPlaylist"></param>
        public void UpdateData(ITreePlaylistInfo newPlaylist)
        {
            this.Name.Value= newPlaylist.Name.Value;
            this.IsRemotePlaylist= newPlaylist.IsRemotePlaylist;
            this.RemoteType= newPlaylist.RemoteType;
            this.RemoteId= newPlaylist.RemoteId;
            this.Folderpath= newPlaylist.Folderpath;
            this.IsExpanded= newPlaylist.IsExpanded;
            this.VideoSortType= newPlaylist.VideoSortType;
            this.CustomSortSequence.Clear();
            this.CustomSortSequence.AddRange(newPlaylist.CustomSortSequence);
            this.IsVideoDescending= newPlaylist.IsVideoDescending;
            this.IsDownloadFailedHistory= newPlaylist.IsDownloadFailedHistory;
            this.IsDownloadSucceededHistory= newPlaylist.IsDownloadSucceededHistory;
            this.IsTemporary= newPlaylist.IsTemporary;
            this.BookMarkedVideoID = newPlaylist.BookMarkedVideoID;
        }

        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="dbPlaylist"></param>
        /// <param name="playlistInfo"></param>
        protected static void SetData(STypes::Playlist dbPlaylist, ITreePlaylistInfo playlistInfo)
        {
            playlistInfo.Id = dbPlaylist.Id;
            playlistInfo.ParentId = dbPlaylist.ParentPlaylist?.Id ?? -1;
            playlistInfo.IsRoot = dbPlaylist.IsRoot;
            playlistInfo.Name.Value = dbPlaylist.PlaylistName ?? string.Empty;
            playlistInfo.IsRemotePlaylist = dbPlaylist.IsRemotePlaylist;
            playlistInfo.RemoteType = dbPlaylist.IsMylist ? RemoteType.Mylist : dbPlaylist.IsUserVideos ? RemoteType.UserVideos : dbPlaylist.IsWatchLater ? RemoteType.WatchLater : dbPlaylist.IsChannel ? RemoteType.Channel : RemoteType.None;
            playlistInfo.RemoteId = dbPlaylist.RemoteId ?? string.Empty;
            playlistInfo.Folderpath = dbPlaylist.FolderPath ?? string.Empty;
            playlistInfo.IsExpanded = dbPlaylist.IsExpanded;
            playlistInfo.VideoSortType = dbPlaylist.SortType;
            playlistInfo.CustomSortSequence.AddRange(dbPlaylist.CustomVideoSequence);
            playlistInfo.IsVideoDescending = dbPlaylist.IsVideoDescending;
            playlistInfo.IsDownloadFailedHistory = dbPlaylist.IsDownloadFailedHistory;
            playlistInfo.IsDownloadSucceededHistory = dbPlaylist.IsDownloadSucceededHistory;
            playlistInfo.IsTemporary = dbPlaylist.IsTemporary;
            playlistInfo.BookMarkedVideoID = dbPlaylist.BookMarkedVideoID;
        }

        /// <summary>
        /// インスタンスを複製する
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public ITreePlaylistInfo Clone()
        {
            return (ITreePlaylistInfo)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// バインド可能なTreePlaylistInfo
    /// </summary>
    public class BindableTreePlaylistInfo : NonBindableTreePlaylistInfo
    {
        private Visibility beforeSeparatorVisibilityField = Visibility.Hidden;

        private Visibility afterSeparatorVisibilityField = Visibility.Hidden;

        private Brush backgroundColorField = Brushes.Transparent;

        private bool isSelectedField;

        public override bool IsSelected { get => this.isSelectedField; set => this.SetProperty(ref this.isSelectedField, value); }

        public override Brush BackgroundColor { get => this.backgroundColorField; set => this.SetProperty(ref this.backgroundColorField, value); }

        public override Visibility AfterSeparatorVisibility { get => this.afterSeparatorVisibilityField; set => this.SetProperty(ref this.afterSeparatorVisibilityField, value); }

        public override Visibility BeforeSeparatorVisibility { get => this.beforeSeparatorVisibilityField; set => this.SetProperty(ref this.beforeSeparatorVisibilityField, value); }

        /// <summary>
        /// DBの型をBindablePlaylistInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public static new ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist)
        {

            var converted = new BindableTreePlaylistInfo();
            SetData(playlist, converted);
            return converted;
        }

    }

    /// <summary>
    /// リモート設定
    /// </summary>
    public enum RemoteType
    {
        None,
        Mylist,
        UserVideos,
        WatchLater,
        Channel,
        WatchPage,
        Series,
    }
}
