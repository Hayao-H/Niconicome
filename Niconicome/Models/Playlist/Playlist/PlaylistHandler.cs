using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Local.Settings;
using State = Niconicome.Models.Local.State;
using Utils = Niconicome.Models.Domain.Utils;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface IPlaylistHandler
    {
        int AddPlaylist(int parentId);
        void DeletePlaylist(int playlistID);
        void Update(ITreePlaylistInfo newpaylist);
        IAttemptResult<int> AddVideo(IListVideoInfo video, int playlistID);
        IAttemptResult RemoveVideo(int videoID, int playlistID);
        void Refresh();
        void Refresh(bool expandAll, bool inheritExpandedState);
        void Move(int id, int targetId);
        void SetAsRemotePlaylist(int playlistId, string Id, string name, RemoteType type);
        void SetAsLocalPlaylist(int playlistId);
        IAttemptResult MoveVideoToPrev(int videoIndex, int playlistID);
        IAttemptResult MoveVideoToForward(int videoIndex, int playlistID);
        void SaveAllPlaylists();
        bool IsLastChild(int id);
        bool ContainsVideo(string niconicoId, int playlistId);
        ITreePlaylistInfo? GetPlaylist(int id);
        ITreePlaylistInfo? GetParent(ITreePlaylistInfo child);
        ITreePlaylistInfo? GetRootPlaylist();
        IEnumerable<ITreePlaylistInfo> GetAllPlaylists();
    }

    /// <summary>
    /// ViewModelから触るAPI
    /// </summary>
    public class PlaylistHandler : IPlaylistHandler
    {
        public PlaylistHandler(IPlaylistTreeHandler handler, IPlaylistStoreHandler playlistStoreHandler, State::IErrorMessanger errorMessanger, ILocalSettingHandler settingHandler, ILogger logger)
        {
            //BindingOperations.EnableCollectionSynchronization(this.Playlists, new object());
            this.treeHandler = handler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.errorMessanger = errorMessanger;
            this.settingHandler = settingHandler;
            this.logger = logger;
        }

        #region field
        private readonly IPlaylistTreeHandler treeHandler;

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly State::IErrorMessanger errorMessanger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly ILogger logger;
        #endregion


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
            var playlist = this.GetPlaylist(id);

            //ありえないけどエラー処理
            if (playlist is null) return -1;

            this.treeHandler.MergeRange(new List<ITreePlaylistInfo>(){ playlist });
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
                logger.Error("プレイリストの削除に失敗しました。", e);
                this.errorMessanger.FireError($"プレイリストの削除に失敗しました。操作は反映されません。");
                return;
            }

            this.treeHandler.Remove(playlistID);
        }

        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public IAttemptResult<int> AddVideo(IListVideoInfo video, int playlistID)
        {
            var id = this.playlistStoreHandler.AddVideo(video, playlistID);
            return new AttemptResult<int>() { Data = id, IsSucceeded = true };
        }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public IAttemptResult RemoveVideo(int videoID, int playlistID)
        {
            this.playlistStoreHandler.RemoveVideo(videoID, playlistID);
            return new AttemptResult() { IsSucceeded = true };
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
        }

        /// <summary>
        /// ローカルプレイリストとして設定する
        /// </summary>
        /// <param name="playlistId"></param>
        public void SetAsLocalPlaylist(int playlistId)
        {
            this.playlistStoreHandler.SetAsLocalPlaylist(playlistId);
        }


        /// <summary>
        /// 動画を一つ前に移動させる
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public IAttemptResult MoveVideoToPrev(int videoIndex, int playlistID)
        {
            var result = this.playlistStoreHandler.MoveVideoToPrev(playlistID, videoIndex);
            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error(result.Message!, result.Exception);
                }
                else
                {
                    this.logger.Error($"動画の並び替え操作に失敗しました。({nameof(this.MoveVideoToPrev)})");
                }
            }

            return result;
        }

        /// <summary>
        /// 動画を一つあとに移動させる
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public IAttemptResult MoveVideoToForward(int videoIndex, int playlistID)
        {

            var result = this.playlistStoreHandler.MoveVideoToForward(playlistID, videoIndex);
            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error(result.Message!, result.Exception);
                }
                else
                {
                    this.logger.Error($"動画の並び替え操作に失敗しました。({nameof(this.MoveVideoToPrev)})");
                }
            }

            return result;
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
            return this.treeHandler?.IsLastChild(id) ?? false;
        }

        /// <summary>
        /// プレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ITreePlaylistInfo? GetPlaylist(int id)
        {
            var p = this.playlistStoreHandler.GetPlaylist(id);
            return p is null ? null : BindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p);
        }

        /// <summary>
        /// 親プレイリストを取得する
        /// </summary>
        public ITreePlaylistInfo? GetParent(ITreePlaylistInfo child)
        {
            return this.treeHandler?.GetParent(child.Id);
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
            var playlists = this.treeHandler.GetAllPlaylists();
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
                this.treeHandler.MergeRange(new List<ITreePlaylistInfo>{ newpaylist });
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

            //プレイリスト
            var list = new List<ITreePlaylistInfo>();

            //プレイリストを取得する
            var dbplaylists = this.playlistStoreHandler.GetAllPlaylists();

            for (var i = 0; i < dbplaylists.Count; ++i)
            {
                STypes::Playlist p = dbplaylists[i];

                var ex = false;
                if (expandAll)
                {
                    ex = true;
                }
                else if (inheritExpandedState)
                {
                    ex = p.IsExpanded;
                }
                //var childPlaylists = this.playlistStoreHandler.GetChildPlaylists(p.Id);
                var playlist = BindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p);
                playlist.IsExpanded = ex;
                list.Add(playlist);
            }

            this.treeHandler.Initialize(list);
        }

    }
}
