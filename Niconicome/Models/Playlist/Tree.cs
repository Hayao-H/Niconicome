using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.ViewModels;
using Local = Niconicome.Models.Local;
using Net = Niconicome.Models.Domain.Network;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Utils = Niconicome.Models.Domain.Utils;
using State = Niconicome.Models.Local.State;

namespace Niconicome.Models.Playlist
{
    public interface IPlaylistVideoHandler
    {
        int AddPlaylist(int parentId);
        void AddVideo(IVideoListInfo video, int playlistId);
        void RemoveVideo(int videoId, int playlistId);
        void UpdateVideo(IVideoListInfo video, int playlistId);
        void DeletePlaylist(int playlistID);
        void Update(ITreePlaylistInfo newpaylist);
        void Refresh();
        void Move(int id, int targetId);
        void SetAsRemotePlaylist(int playlistId, string Id, RemoteType type);
        void SetAsLocalPlaylist(int playlistId);
        bool IsLastChild(int id);
        bool ContainsVideo(string niconicoId, int playlistId);
        ITreePlaylistInfo? GetPlaylist(int id);
        ITreePlaylistInfo? GetParent(ITreePlaylistInfo child);
        ITreePlaylistInfo? GetRootPlaylist();
        IEnumerable<ITreePlaylistInfo> GetAllPlaylists();
        ObservableCollection<ITreePlaylistInfo> Playlists { get; }
    }

    public interface ITreePlaylistInfoHandler
    {
        ITreePlaylistInfo? GetPlaylist(int id);
        void Remove(int id);
        void Add(ITreePlaylistInfo playlist);
        void AddRange(IEnumerable<ITreePlaylistInfo> playlists);
        void Merge(ITreePlaylistInfo playlist);
        void MergeRange(IEnumerable<ITreePlaylistInfo> playlists);
        bool Contains(int id);
        bool ContainsVideo(string niconicoId, int playlistId);
        bool IsLastChild(ITreePlaylistInfo child);
        bool IsLastChild(int id);
        ITreePlaylistInfo? GetParent(int id);
        ITreePlaylistInfo GetRoot();
        ITreePlaylistInfo GetTree();
    }

    public interface ITreePlaylistInfo : IClonable<ITreePlaylistInfo>
    {
        int Id { get; set; }
        string Name { get; set; }
        string RemoteId { get; set; }
        string Folderpath { get; set; }
        List<int> ChildrensIds { get; }
        List<ITreePlaylistInfo> Children { get; }
        List<IVideoListInfo> Videos { get; set; }
        int ParentId { get; set; }
        bool IsExpanded { get; set; }
        bool IsRoot { get; set; }
        bool IsConcrete { get; }
        Brush BackgroundColor { get; set; }
        Visibility BeforeSeparatorVisibility { get; set; }
        Visibility AfterSeparatorVisibility { get; set; }
        bool HasChildPlaylist { get; }
        bool HasVideo(string niconicoId);
        bool IsRemotePlaylist { get; set; }
        RemoteType RemoteType { get; set; }
        int GetLayer(ITreePlaylistInfoHandler handler);
        static ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist)
        {
            playlist.Void();
            return new NonBindableTreePlaylistInfo() { Id = playlist.Id };
        }
        static ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist, IEnumerable<STypes::Playlist> childPlaylists)
        {
            playlist.Void();
            childPlaylists.Void();
            return new NonBindableTreePlaylistInfo();
        }

    }


    /// <summary>
    /// ViewModelから触るAPI
    /// これはあくまでメモリ上への動画データしか変更できないため、
    /// DBのデータを変更したい場合は別途VideoHandlerクラスのインスタンスを利用する必要がある
    /// </summary>
    public class PlaylistVideoHandler : IPlaylistVideoHandler
    {
        public PlaylistVideoHandler(ITreePlaylistInfoHandler handler, IPlaylistStoreHandler playlistStoreHandler, State::IErrorMessanger errorMessanger)
        {
            this.Playlists = new ObservableCollection<ITreePlaylistInfo>();
            BindingOperations.EnableCollectionSynchronization(this.Playlists, new object());
            this.handler = handler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.errorMessanger = errorMessanger;
            this.Refresh();
        }

        private readonly ITreePlaylistInfoHandler handler;

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly State::IErrorMessanger errorMessanger;

        public ObservableCollection<ITreePlaylistInfo> Playlists { get; private set; }

        /// <summary>
        /// 動画をメモリ上のプレイリストに追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistId"></param>
       　public void AddVideo(IVideoListInfo video, int playlistId)
        {
            var playlist = this.handler.GetPlaylist(playlistId);
            playlist?.Videos.AddUnique(video, list => !list.Any(v => v.Id == video.Id));
        }

        /// <summary>
        /// 動画をメモリ上のプレイリストから削除する
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="playlistId"></param>
        public void RemoveVideo(int videoId, int playlistId)
        {
            var playlist = this.handler.GetPlaylist(playlistId);
            playlist?.Videos.RemoveAll(v => v.Id == videoId);
        }

        /// <summary>
        /// 動画情報を更新する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistId"></param>
        public void UpdateVideo(IVideoListInfo video, int playlistId)
        {
            this.RemoveVideo(video.Id, playlistId);
            this.AddVideo(video, playlistId);
        }


        /// <summary>
        /// プレイリストを追加
        /// </summary>
        /// <param name="parentId"></param>
        public int AddPlaylist(int parentId)
        {
            int id;
            try
            {

                id = this.playlistStoreHandler.AddPlaylist(parentId, "新しいプレイリスト");
            }
            catch (Exception e)
            {
                var logger = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogger>();
                logger.Error("プレイリストの追加に失敗しました。", e);
                this.errorMessanger.FireError($"プレイリストの追加に失敗しました。操作は反映されません。");
                return -1;
            }
            this.SetPlaylists();
            return id;
        }

        /// <summary>
        /// プレイリストを削除する
        /// </summary>
        /// <param name="playlistID"></param>
        public void DeletePlaylist(int playlistID)
        {
            try
            {

                this.playlistStoreHandler.DeletePlaylist(playlistID);
            }
            catch (Exception e)
            {
                var logger = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogger>();
                logger.Error("プレイリストの削除に失敗しました。", e);
                this.errorMessanger.FireError($"プレイリストの削除に失敗しました。操作は反映されません。");
                return;
            }
            this.SetPlaylists();
        }

        /// <summary>
        /// プレイリストを移動する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        public void Move(int id, int targetId)
        {
            this.playlistStoreHandler.Move(id, targetId);
            this.SetPlaylists();
        }

        /// <summary>
        /// リモートプレイリストとして設定する
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="Id"></param>
        /// <param name="type"></param>
        public void SetAsRemotePlaylist(int playlistId, string Id, RemoteType type)
        {
            this.playlistStoreHandler.SetAsRemotePlaylist(playlistId, Id, type);
            this.SetPlaylists();
        }

        /// <summary>
        /// ローカルプレイリストとして設定する
        /// </summary>
        /// <param name="playlistId"></param>
        public void SetAsLocalPlaylist(int playlistId)
        {
            this.playlistStoreHandler.SetAsLocalPlaylist(playlistId);
            this.SetPlaylists();
        }

        /// <summary>
        /// プレイリストを含んでいるかどうか
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsVideo(string niconicoId, int playlistId)
        {
            return this.handler.ContainsVideo(niconicoId, playlistId);
        }

        /// <summary>
        /// 最後の子プレイリストであることを確認する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsLastChild(int id)
        {
            return this.handler?.IsLastChild(id) ?? false;
        }

        /// <summary>
        /// プレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ITreePlaylistInfo? GetPlaylist(int id)
        {
            return this.handler.GetPlaylist(id);
        }

        /// <summary>
        /// 親プレイリストを取得する
        /// </summary>
        public ITreePlaylistInfo? GetParent(ITreePlaylistInfo child)
        {
            return this.handler?.GetParent(child.Id);
        }

        /// <summary>
        /// データを更新する
        /// </summary>
        public void Refresh()
        {
            this.playlistStoreHandler.Refresh();
            this.SetPlaylists();
        }

        /// <summary>
        /// プレイリストを更新する
        /// </summary>
        /// <param name="newpaylist"></param>
        public void Update(ITreePlaylistInfo newpaylist)
        {
            if (this.playlistStoreHandler.Exists(newpaylist.Id))
            {
                this.playlistStoreHandler.Update(newpaylist);
                this.SetPlaylists();
            }
        }

        /// <summary>
        /// 全てのプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITreePlaylistInfo> GetAllPlaylists()
        {
            return this.playlistStoreHandler.GetAllPlaylists().Select(p => BindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p));
        }

        /// <summary>
        /// ルートプレイリストを取得
        /// </summary>
        /// <returns></returns>
        public ITreePlaylistInfo? GetRootPlaylist()
        {
            return BindableTreePlaylistInfo.ConvertToTreePlaylistInfo(this.playlistStoreHandler.GetRootPlaylist());
        }

        /// <summary>
        /// プレイリストを初期化する
        /// </summary>
        private void SetPlaylists()
        {

            //プレイリストを取得する
            var playlists = this.playlistStoreHandler.GetAllPlaylists().Select(p =>
            {
                var childPlaylists = this.playlistStoreHandler.GetChildPlaylists(p.Id);
                return BindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p, childPlaylists);
            });


            this.handler.MergeRange(playlists);
            ITreePlaylistInfo treePlaylistInfo = handler.GetTree();
            this.Playlists.Clear();
            this.Playlists.Add(treePlaylistInfo);
        }

    }

    /// <summary>
    /// TreePlaylistInfoをよしなにしてくれる
    /// </summary>
    public class TreePlaylistInfoHandler : ITreePlaylistInfoHandler
    {
        private List<ITreePlaylistInfo> TreePlaylistInfoes { get; set; } = new();

        /// <summary>
        /// ツリー構築済フラグ
        /// </summary>
        private bool IsTreeInitialized;

        /// <summary>
        /// 指定したIDのプレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ITreePlaylistInfo? GetPlaylist(int id)
        {
            return this.TreePlaylistInfoes.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 指定したIDのプレイリストを削除する
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            this.TreePlaylistInfoes.RemoveAll(pl => pl.Id == id);
        }

        /// <summary>
        /// 指定されたIDのプレイリストを含むかどうかを返す
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id)
        {
            return this.TreePlaylistInfoes.Any(p => p.Id == id);
        }

        /// <summary>
        /// 指定されたIDの動画を含むかどうかを返す
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        public bool ContainsVideo(string niconicoId, int playlistId)
        {
            var playlist = this.GetPlaylist(playlistId);
            if (playlist is null) return false;
            return playlist.Videos.Any(v => v.NiconicoId == niconicoId);
        }

        /// <summary>
        /// 最後の子プレイリストであるかどうかを判断する
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool IsLastChild(ITreePlaylistInfo child)
        {
            return this.IsLastChild(child.Id);
        }

        /// <summary>
        /// 最後の子プレイリストであるかどうかを判断する
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool IsLastChild(int id)
        {
            var parent = this.GetParent(id);
            //nullチェック
            if (parent == null) return false;

            if (parent.ChildrensIds.Count == 0) return false;

            return parent.ChildrensIds.Last() == id;
        }

        /// <summary>
        /// プレイリストを追加する
        /// </summary>
        /// <param name="playlist"></param>
        public void Add(ITreePlaylistInfo playlist)
        {

            //重複を削除
            if (this.Contains(playlist.Id))
            {
                this.Remove(playlist.Id);
            }

            this.TreePlaylistInfoes.Add(playlist);

            //ツリーは未初期化
            this.IsTreeInitialized = false;
        }

        /// <summary>
        /// プレイリストを一括で追加する
        /// </summary>
        /// <param name="playlists"></param>
        public void AddRange(IEnumerable<ITreePlaylistInfo> playlists)
        {
            foreach (var playlist in playlists)
            {
                this.Add(playlist);
            }
        }

        /// <summary>
        /// 開閉状態等を保持して追加
        /// </summary>
        /// <param name="playlist"></param>
        public void Merge(ITreePlaylistInfo after)
        {

            //重複があった場合はプロパティーを引き継いでから削除する
            if (this.Contains(after.Id))
            {
                ITreePlaylistInfo? before = this.GetPlaylist(after.Id);
                if (before != null)
                {
                    after.BeforeSeparatorVisibility = before.BeforeSeparatorVisibility;
                    after.AfterSeparatorVisibility = before.AfterSeparatorVisibility;
                    after.IsExpanded = before.IsExpanded;
                    this.MergeVideo(before, after);
                    this.Remove(before.Id);
                }
            }

            this.TreePlaylistInfoes.Add(after);

            //ツリーは未初期化
            this.IsTreeInitialized = false;
        }

        /// <summary>
        /// メッセージを保持して動画を追加
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        private void MergeVideo(ITreePlaylistInfo before, ITreePlaylistInfo after)
        {
            foreach (var afterVideo in after.Videos)
            {
                if (before.Videos.Any(v => v.Id == afterVideo.Id))
                {
                    var beforeVideo = before.Videos.First(v => v.Id == afterVideo.Id);
                    afterVideo.Message = beforeVideo.Message;
                }
            }
        }

        /// <summary>
        /// プレイリストを一括でマージする
        /// </summary>
        /// <param name="playlists"></param>
        public void MergeRange(IEnumerable<ITreePlaylistInfo> playlists)
        {
            foreach (var playlist in playlists)
            {
                this.Merge(playlist);
            }
        }

        /// <summary>
        /// 親プレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ITreePlaylistInfo? GetParent(int id)
        {
            ITreePlaylistInfo? self = this.GetPlaylist(id);
            if (self == null || self.ParentId == default)
            {
                return null;
            }

            int parentId = self.ParentId;
            return this.GetPlaylist(parentId);
        }

        /// <summary>
        /// ルートプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public ITreePlaylistInfo GetRoot()
        {
            return this.TreePlaylistInfoes.First(p => p.IsRoot);
        }

        /// <summary>
        /// ツリーを取得する
        /// </summary>
        /// <returns></returns>
        public ITreePlaylistInfo GetTree()
        {
            var root = this.TreePlaylistInfoes.FirstOrDefault(pl => pl.IsRoot);
            //ルートプレイリストがnullの場合はエラーを返す
            if (root == null) throw new InvalidOperationException("ルートプレイリストが存在しません。");

            //初期化済みでない場合は完全なツリーを構築する
            if (this.IsTreeInitialized)
            {
                return root;
            }
            else
            {
                this.IsTreeInitialized = true;
                return this.ConstructPlaylistInfo(root.Id);
            }

        }

        /// <summary>
        /// 完全なプレイリストツリーを構築する
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private ITreePlaylistInfo ConstructPlaylistInfo(int Id)
        {
            var playlist = this.TreePlaylistInfoes.FirstOrDefault(pl => pl.Id == Id);

            //ルートプレイリストがnullの場合はエラーを返す
            if (playlist == null) throw new InvalidOperationException($"指定されたプレイリスト(id:{Id})が存在しません。");

            //子プレイリストを構築する
            if (playlist.ChildrensIds.Count > 0)
            {
                foreach (var id in playlist.ChildrensIds)
                {
                    playlist.Children.Add(this.ConstructPlaylistInfo(id));
                }

                playlist.Children.Sort((a, b) => a.Id - b.Id);
            }

            return playlist;
        }
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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// リモートID
        /// </summary>
        public string RemoteId { get; set; } = string.Empty;

        /// <summary>
        /// フォルダー名
        /// </summary>
        public string Folderpath { get; set; } = string.Empty;


        /// <summary>
        /// 子プレイリストのID一覧
        /// </summary>
        public List<int> ChildrensIds { get; set; } = new();

        /// <summary>
        /// 子プレイリスト一覧
        /// 通常は空のリスト
        /// </summary>
        public List<ITreePlaylistInfo> Children { get; private set; } = new();

        /// <summary>
        /// 動画情報のリスト
        /// </summary>
        public List<IVideoListInfo> Videos { get; set; } = new();

        /// <summary>
        /// 親プレイリストのID
        /// </summary>
        public int ParentId { get; set; } = -1;

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
        /// 子プレイリストが存在する場合
        /// </summary>
        public bool HasChildPlaylist
        {
            get
            {
                return this.ChildrensIds.Count > 0;
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

            return this.Videos.Any(v => v.NiconicoId == niconicoId);
        }

        /// <summary>
        /// レイヤーを取得する
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public int GetLayer(ITreePlaylistInfoHandler handler)
        {

            int layer = 1;

            if (this.ParentId == -1)
            {
                return layer;
            }

            int parentId = this.Id;
            while ((parentId = (handler.GetParent(parentId)?.Id ?? -1)) != -1)
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
        public static ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist, IEnumerable<STypes::Playlist> childPlaylists)
        {

            var converted = new NonBindableTreePlaylistInfo()
            {
                ChildrensIds = childPlaylists.Select(p => p.Id).ToList(),
            };
            NonBindableTreePlaylistInfo.SetData(playlist, converted);
            return converted;
        }

        /// <summary>
        /// DBの型をNonBindableTreePlaylistInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public static ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist)
        {

            var converted = new NonBindableTreePlaylistInfo();
            NonBindableTreePlaylistInfo.SetData(playlist, converted);
            return converted;
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
            playlistInfo.Name = dbPlaylist.PlaylistName ?? string.Empty;
            playlistInfo.IsRemotePlaylist = dbPlaylist.IsRemotePlaylist;
            playlistInfo.RemoteType = dbPlaylist.IsMylist ? RemoteType.Mylist : dbPlaylist.IsUserVideos ? RemoteType.UserVideos : dbPlaylist.IsWatchLater ? RemoteType.WatchLater : dbPlaylist.IsChannel ? RemoteType.Channel : RemoteType.None;
            playlistInfo.RemoteId = dbPlaylist.RemoteId ?? string.Empty;
            playlistInfo.Folderpath = dbPlaylist.FolderPath ?? string.Empty;
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
        /// DBの型をBindableTreePlaylistInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public static new ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist, IEnumerable<STypes::Playlist> childPlaylists)
        {

            var converted = new BindableTreePlaylistInfo()
            {
                ChildrensIds = childPlaylists.Select(p => p.Id).ToList(),
            };
            NonBindableTreePlaylistInfo.SetData(playlist, converted);
            return converted;
        }

        /// <summary>
        /// DBの型をBindablePlaylistInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public static new ITreePlaylistInfo ConvertToTreePlaylistInfo(STypes::Playlist playlist)
        {

            var converted = new BindableTreePlaylistInfo();
            NonBindableTreePlaylistInfo.SetData(playlist, converted);
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
        Channel
    }
}
