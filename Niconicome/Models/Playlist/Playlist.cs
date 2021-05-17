using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels;
using State = Niconicome.Models.Local.State;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Playlist
{
    public interface IPlaylistHandler
    {
        int AddPlaylist(int parentId);
        void DeletePlaylist(int playlistID);
        void Update(ITreePlaylistInfo newpaylist);
        void Refresh();
        void Refresh(bool expandAll, bool inheritExpandedState);
        void Move(int id, int targetId);
        void SetAsRemotePlaylist(int playlistId, string Id, string name, RemoteType type);
        void SetAsLocalPlaylist(int playlistId);
        void SaveAllPlaylists();
        bool IsLastChild(int id);
        bool ContainsVideo(string niconicoId, int playlistId);
        ITreePlaylistInfo? GetPlaylist(int id);
        ITreePlaylistInfo? GetParent(ITreePlaylistInfo child);
        ITreePlaylistInfo? GetRootPlaylist();
        IEnumerable<ITreePlaylistInfo> GetAllPlaylists();
        ObservableCollection<ITreePlaylistInfo> Playlists { get; }
    }

    public interface IPlaylistTreeConstructor
    {
        ITreePlaylistInfo? GetPlaylist(int id);
        void Remove(int id);
        void Add(ITreePlaylistInfo playlist);
        void AddRange(IEnumerable<ITreePlaylistInfo> playlists);
        void Merge(ITreePlaylistInfo playlist);
        void MergeRange(IEnumerable<ITreePlaylistInfo> playlists);
        void Clear();
        bool Contains(int id);
        bool IsLastChild(ITreePlaylistInfo child);
        bool IsLastChild(int id);
        ITreePlaylistInfo? GetParent(int id);
        ITreePlaylistInfo GetRoot();
        ITreePlaylistInfo GetTree();
        IEnumerable<ITreePlaylistInfo> GetAllPlaylists();
    }

    public interface ITreePlaylistInfo : IClonable<ITreePlaylistInfo>
    {
        int Id { get; set; }
        string Name { get; set; }
        string RemoteId { get; set; }
        string Folderpath { get; set; }
        List<int> ChildrensIds { get; }
        List<ITreePlaylistInfo> Children { get; }
        List<IListVideoInfo> Videos { get; set; }
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
        int GetLayer(IPlaylistTreeConstructor handler);

    }


    /// <summary>
    /// ViewModelから触るAPI
    /// </summary>
    public class PlaylistHandler : IPlaylistHandler
    {
        public PlaylistHandler(IPlaylistTreeConstructor handler, IPlaylistStoreHandler playlistStoreHandler, State::IErrorMessanger errorMessanger, ILocalSettingHandler settingHandler)
        {
            this.Playlists = new ObservableCollection<ITreePlaylistInfo>();
            BindingOperations.EnableCollectionSynchronization(this.Playlists, new object());
            this.handler = handler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.errorMessanger = errorMessanger;
            this.settingHandler = settingHandler;
        }

        #region field
        private readonly IPlaylistTreeConstructor handler;

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly State::IErrorMessanger errorMessanger;

        private readonly ILocalSettingHandler settingHandler;
        #endregion

        public ObservableCollection<ITreePlaylistInfo> Playlists { get; private set; }

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
        public void SetAsRemotePlaylist(int playlistId, string Id, string name, RemoteType type)
        {
            this.playlistStoreHandler.SetAsRemotePlaylist(playlistId, Id, type);

            if (this.settingHandler.GetBoolSetting(SettingsEnum.AutoRenameNetPlaylist))
            {
                var playlistName = type switch
                {
                    RemoteType.Mylist => name,
                    RemoteType.UserVideos => $"{name}さんの投稿動画",
                    RemoteType.WatchLater => "あとで見る",
                    RemoteType.Channel => name,
                    _ => name,
                };

                if (!name.IsNullOrEmpty())
                {
                    var playlist = this.GetPlaylist(playlistId);

                    if (playlist is not null)
                    {
                        playlist.Name = playlistName;
                        this.Update(playlist);
                    }
                }
            }

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
            return this.playlistStoreHandler.ContainsVideo(niconicoId, playlistId);
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
        /// 展開状況を引き継いで更新する
        /// </summary>
        /// <param name="expandAll"></param>
        /// <param name="inheritExpandedState"></param>
        public void Refresh(bool expandAll, bool inheritExpandedState)
        {
            this.playlistStoreHandler.Refresh();
            this.SetPlaylists(expandAll, inheritExpandedState);
        }


        /// <summary>
        /// すべてのプレイリストを保存する
        /// </summary>
        public void SaveAllPlaylists()
        {
            this.Refresh();
            var playlists = this.handler.GetAllPlaylists();
            foreach (var p in playlists)
            {
                if (!this.playlistStoreHandler.Exists(p.Id)) continue;
                this.playlistStoreHandler.Update(p);
            }
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
        private void SetPlaylists(bool expandAll = false, bool inheritExpandedState = false)
        {

            //プレイリストを取得する
            var playlists = this.playlistStoreHandler.GetAllPlaylists().Select(p =>
            {
                var ex = false;
                if (expandAll)
                {
                    ex = true;
                }
                else if (inheritExpandedState)
                {
                    ex = p.IsExpanded;
                }
                var childPlaylists = this.playlistStoreHandler.GetChildPlaylists(p.Id);
                var playlist = BindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p, childPlaylists);
                playlist.IsExpanded = ex;
                return playlist;
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
    public class PlaylistTreeConstructor : IPlaylistTreeConstructor
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
        /// プレイリストをクリアする
        /// </summary>
        public void Clear()
        {
            this.TreePlaylistInfoes.Clear();
            this.IsTreeInitialized = false;
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
                    this.Remove(before.Id);
                }
            }

            this.TreePlaylistInfoes.Add(after);

            //ツリーは未初期化
            this.IsTreeInitialized = false;
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
        /// すべてのプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITreePlaylistInfo> GetAllPlaylists()
        {
            return this.TreePlaylistInfoes;
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
        public List<IListVideoInfo> Videos { get; set; } = new();

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

            return this.Videos.Any(v => v.NiconicoId.Value == niconicoId);
        }

        /// <summary>
        /// レイヤーを取得する
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public int GetLayer(IPlaylistTreeConstructor handler)
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
            playlistInfo.IsExpanded = dbPlaylist.IsExpanded;
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
        Channel,
        WatchPage
    }
}
