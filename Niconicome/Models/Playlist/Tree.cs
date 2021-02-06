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
    public interface IPlaylistTreeHandler
    {
        int AddPlaylist(int parentId);
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
        List<ITreeVideoInfo> Videos { get; set; }
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

    public interface ITreeVideoInfo : IClonable<ITreeVideoInfo>
    {
        int Id { get; set; }

        int ViewCount { get; set; }
        string NiconicoId { get; set; }
        string Title { get; set; }
        bool IsDeleted { get; set; }
        bool IsSelected { get; set; }
        string Owner { get; set; }
        string LargeThumbUrl { get; set; }
        string ThumbUrl { get; set; }
        string Message { get; set; }
        string ThumbPath { get; set; }
        string FileName { get; set; }
        string BindableThumbPath { get; }
        string MessageGuid { get; set; }
        IEnumerable<string> Tags { get; set; }
        DateTime UploadedOn { get; set; }
        Uri GetNiconicoPageUri();
        bool CheckDownloaded(string folderPath);
        string GetFilePath(string folderPath);
        static BindableTreeVideoInfo ConvertToTreeVideoInfo(STypes::Video video)
        {
            video.Void();
            return new BindableTreeVideoInfo();
        }
    }

    /// <summary>
    /// ViewModelから触るAPI
    /// </summary>
    public class PlaylistTreeHandler : IPlaylistTreeHandler
    {
        public PlaylistTreeHandler(ITreePlaylistInfoHandler handler, IPlaylistStoreHandler playlistStoreHandler, IVideoStoreHandler videoStoreHandler, State::IErrorMessanger errorMessanger)
        {
            this.Playlists = new ObservableCollection<ITreePlaylistInfo>();
            BindingOperations.EnableCollectionSynchronization(this.Playlists, new object());
            this.handler = handler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.videoStoreHandler = videoStoreHandler;
            this.errorMessanger = errorMessanger;
            this.Refresh();
        }

        private readonly ITreePlaylistInfoHandler handler;

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly IVideoStoreHandler videoStoreHandler;

        private readonly State::IErrorMessanger errorMessanger;

        public ObservableCollection<ITreePlaylistInfo> Playlists { get; private set; }


        /// <summary>
        /// プレイリストを初期化する
        /// </summary>
        private void SetPlaylists()
        {

            //プレイリストを取得する
            var playlists = this.playlistStoreHandler.GetAllPlaylists().Select(p =>
             {
                 var childPlaylists = this.playlistStoreHandler.GetChildPlaylists(p.Id);
                 var converted = BindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p, childPlaylists);
                 if (p.Videos.Count > 0)
                 {
                     var videos = p.Videos.Select(v =>
                     {
                         var video = this.videoStoreHandler.GetVideo(v.Id);
                         if (video is null) return new BindableTreeVideoInfo();
                         return BindableTreeVideoInfo.ConvertToTreeVideoInfo(video);
                     }).Where(v => !v.Title.IsNullOrEmpty()).Distinct(v => v.Id);
                     converted.Videos.AddRange(videos);
                 }
                 return converted;
             });


            this.handler.MergeRange(playlists);
            ITreePlaylistInfo treePlaylistInfo = handler.GetTree();
            this.Playlists.Clear();
            this.Playlists.Add(treePlaylistInfo);
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
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistId"></param>
        public int AddVideo(ITreeVideoInfo video, int playlistId)
        {
            try
            {
                this.handler.GetPlaylist(playlistId)?.Videos?.Add(video);
                return this.playlistStoreHandler.AddVideo(video, playlistId);
            }
            catch (Exception e)
            {
                var logger = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogger>();
                logger.Error("動画の追加に失敗しました。", e);
                this.errorMessanger.FireError($"{video.NiconicoId}の保存に失敗しました。操作は反映されません。");
                return -1;
            }
        }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="playlistId"></param>
        public void RemoveVideo(int videoID, int playlistId)
        {
            try
            {
                this.handler.GetPlaylist(playlistId)?.Videos?.RemoveAll(v => v.Id == videoID);
                this.playlistStoreHandler.RemoveVideo(videoID, playlistId);
            }
            catch (Exception e)
            {
                var logger = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogger>();
                logger.Error("動画の削除に失敗しました。", e);
                this.errorMessanger.FireError($"動画の削除に失敗しました。操作は反映されません。");
            }
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
        /// 動画情報を更新する
        /// </summary>
        /// <param name="video"></param>
        public void UpdateVideo(ITreeVideoInfo video, bool enableRefresh = true)
        {
            this.videoStoreHandler.Update(video);
            if (enableRefresh) this.SetPlaylists();
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
        public List<ITreeVideoInfo> Videos { get; set; } = new();

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
    /// バインド不可能な動画情報
    /// </summary>
    public class NonBindableTreeVideoInfo : BindableBase, ITreeVideoInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 再生回数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// ニコニコ動画におけるID
        /// </summary>
        public string NiconicoId { get; set; } = string.Empty;

        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 選択フラグ
        /// </summary>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// 投稿者名
        /// </summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// 大きなサムネイルのURL
        /// </summary>
        public string LargeThumbUrl { get; set; } = string.Empty;

        /// <summary>
        /// サムネイルのURL
        /// </summary>
        public string ThumbUrl { get; set; } = string.Empty;

        /// <summary>
        /// サムネイルのパス
        /// </summary>
        public string ThumbPath { get; set; } = string.Empty;

        /// <summary>
        /// バインド可能なサムネイルフィルパス
        /// </summary>
        public string BindableThumbPath
        {
            get
            {
                var dir = AppContext.BaseDirectory;
                if (this.ThumbPath is null || this.ThumbPath == string.Empty)
                {
                    var cacheHandler = Utils::DIFactory.Provider.GetRequiredService<Net::ICacheHandler>();
                    string cachePath = cacheHandler.GetCachePath("0", Net.CacheType.Thumbnail);
                    return Path.Combine(dir, cachePath);
                }
                else
                {
                    return Path.Combine(dir, this.ThumbPath);

                }
            }
        }

        /// <summary>
        /// タグ
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// メッセージ
        /// </summary>
        public virtual string Message { get; set; } = string.Empty;

        /// <summary>
        /// メッセージID
        /// </summary>
        public string MessageGuid { get; set; } = Guid.NewGuid().ToString("D");

        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime UploadedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 視聴ページのURIを取得する
        /// </summary>
        /// <returns></returns>
        public Uri GetNiconicoPageUri()
        {
            return new Uri($"https://nico.ms/{this.NiconicoId}");
        }

        public bool CheckDownloaded(string folderPath)
        {
            if (this.FileName.IsNullOrEmpty())
            {
                return false;
            }
            else
            {
                return File.Exists(Path.Combine(folderPath, this.FileName));
            }
        }

        /// <summary>
        /// ファイルパスを取得する
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public string GetFilePath(string folderName)
        {
            if (this.FileName.IsNullOrEmpty())
            {
                return string.Empty;
            }
            else
            {
                return Path.Combine(folderName, this.FileName);
            }
        }

        /// <summary>
        /// DBのデータをTreeVideoInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="dbVideo"></param>
        /// <returns></returns>
        public static NonBindableTreeVideoInfo ConvertToTreeVideoInfo(STypes::Video dbVideo)
        {
            var converted = new NonBindableTreeVideoInfo();
            NonBindableTreeVideoInfo.SetData(converted, dbVideo);
            return converted;
        }

        /// <summary>
        /// オブジェクトをクローンする
        /// </summary>
        /// <returns></returns>
        public ITreeVideoInfo Clone()
        {
            return (ITreeVideoInfo)this.MemberwiseClone();
        }

        /// <summary>
        /// データを設定する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <param name="dbVideo"></param>
        protected static void SetData(ITreeVideoInfo videoInfo, STypes::Video dbVideo)
        {
            videoInfo.Id = dbVideo.Id;
            videoInfo.NiconicoId = dbVideo.NiconicoId;
            videoInfo.Title = dbVideo.Title;
            videoInfo.IsDeleted = dbVideo.IsDeleted;
            videoInfo.Owner = dbVideo.Owner?.Nickname ?? string.Empty;
            videoInfo.UploadedOn = dbVideo.UploadedOn;
            videoInfo.LargeThumbUrl = dbVideo.LargeThumbUrl;
            videoInfo.ThumbUrl = dbVideo.ThumbUrl;
            videoInfo.ThumbPath = dbVideo.ThumbPath;
            videoInfo.FileName = dbVideo.FileName;
            videoInfo.IsSelected = dbVideo.IsSelected;
            videoInfo.Tags = dbVideo.Tags ?? new List<string>();
            videoInfo.ViewCount = dbVideo.ViewCount;
        }
    }

    /// <summary>
    /// バインド可能な動画情報
    /// </summary>
    public class BindableTreeVideoInfo : NonBindableTreeVideoInfo, ITreeVideoInfo
    {
        private bool isSelectedField;


        public override string Message
        {
            get => VideoMessanger.GetMessage(this.MessageGuid); 
            set
            {
                VideoMessanger.Write(this.MessageGuid, value);
                this.OnPropertyChanged();
            }
        }

        public override bool IsSelected { get => this.isSelectedField; set => this.SetProperty(ref this.isSelectedField, value); }

        /// <summary>
        /// DBのデータをTreeVideoInfo型のインスタンスに変換する
        /// </summary>
        /// <param name="dbVideo"></param>
        /// <returns></returns>
        public new static BindableTreeVideoInfo ConvertToTreeVideoInfo(STypes::Video dbVideo)
        {
            var converted = new BindableTreeVideoInfo();
            NonBindableTreeVideoInfo.SetData(converted, dbVideo);
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
