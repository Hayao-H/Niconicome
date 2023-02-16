using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Niconicome.Models.Playlist.V2.Migration;

namespace Niconicome.Models.Playlist.V2.Manager
{
    public interface IPlaylistManager
    {
        /// <summary>
        /// ツリーを初期化
        /// </summary>
        void Initialize();

        /// <summary>
        /// プレイリストを削除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult Delete(int ID);

        /// <summary>
        /// プレイリストを作成
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        IAttemptResult Create(int parentID);

        /// <summary>
        /// 特殊なプレイリストを取得する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        IAttemptResult<IPlaylistInfo> GetSpecialPlaylistByType(SpecialPlaylists playlist);
    }

    public class PlaylistManager : IPlaylistManager
    {
        public PlaylistManager(IPlaylistStore playlistStore, IErrorHandler errorHandler, IPlaylistVideoContainer container, IVideoAndPlayListMigration migration)
        {
            this._playlistStore = playlistStore;
            this._errorHandler = errorHandler;
            this._container = container;
            this._migration = migration;
        }

        #region field

        private readonly IPlaylistStore _playlistStore;

        private readonly IErrorHandler _errorHandler;

        private readonly IPlaylistVideoContainer _container;

        private readonly IVideoAndPlayListMigration _migration;

        private Dictionary<int, IPlaylistInfo> _playlists = new();

        #endregion

        #region Method
        public void Initialize()
        {
            //移行が必要な場合は処理を中止
            if (this._migration.IsMigrationNeeded) return;

            //特殊なプレイリストを作成
            IAttemptResult specialResult = this.CreateSpecialPlaylist();
            if (!specialResult.IsSucceeded) return;

            IAttemptResult<IReadOnlyList<IPlaylistInfo>> result = this._playlistStore.GetAllPlaylist();
            if (!result.IsSucceeded || result.Data is null) return;

            //データを準備
            this._playlists = result.Data.ToDictionary(p => p.ID);
            IPlaylistInfo? root = result.Data.FirstOrDefault(p => p.PlaylistType == PlaylistType.Root);

            //ルートプレイリストが見つからない
            if (root is null) return;

            IPlaylistInfo? temp = result.Data.FirstOrDefault(p => p.PlaylistType == PlaylistType.Temporary);
            if (temp is null) return;
            this._container.CurrentSelectedPlaylist = temp;

            root.AddChild(temp, false);

            this.SetChild(root, new List<string>().AsReadOnly());

            this._container.Playlist.Clear();
            this._container.Playlist.Add(root);
        }

        public IAttemptResult Delete(int ID)
        {
            if (!this._playlists.ContainsKey(ID))
            {
                this._errorHandler.HandleError(PlaylistManagerError.PlaylistNotFount, ID);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(PlaylistManagerError.PlaylistNotFount, ID));
            }

            IPlaylistInfo playlist = this._playlists[ID];
            if (playlist.PlaylistType == PlaylistType.Root || playlist.PlaylistType == PlaylistType.Temporary || playlist.PlaylistType == PlaylistType.DownloadFailedHistory || playlist.PlaylistType == PlaylistType.DownloadSucceededHistory)
            {
                this._errorHandler.HandleError(PlaylistManagerError.PlaylistCannotBeDeleted, nameof(playlist.PlaylistType));
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(PlaylistManagerError.PlaylistCannotBeDeleted, nameof(playlist.PlaylistType)));
            }

            //DBから削除
            IAttemptResult deleteResult = this._playlistStore.Delete(ID);
            if (!deleteResult.IsSucceeded) return deleteResult;

            //キャッシュから削除
            this._playlists.Remove(ID);

            if (!this._playlists.ContainsKey(playlist.ParentID))
            {
                this._errorHandler.HandleError(PlaylistManagerError.PlaylistNotFount, ID);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(PlaylistManagerError.PlaylistNotFount, ID));
            }

            IPlaylistInfo parent = this._playlists[playlist.ParentID];

            return parent.RemoveChild(playlist);
        }

        public IAttemptResult Create(int parentID)
        {
            IAttemptResult<int> cResult = this._playlistStore.Create(LocalConstant.DefaultPlaylistName);
            if (!cResult.IsSucceeded)
            {
                return AttemptResult.Fail(cResult.Message);
            }

            IAttemptResult<IPlaylistInfo> pResult = this._playlistStore.GetPlaylist(cResult.Data);
            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail(pResult.Message);
            }

            //キャッシュに追加
            IPlaylistInfo playlist = pResult.Data;
            playlist.ParentID = parentID;
            this._playlists.Add(playlist.ID, playlist);

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IPlaylistInfo> GetSpecialPlaylistByType(SpecialPlaylists playlist)
        {
            PlaylistType type = playlist switch
            {
                SpecialPlaylists.PlaybackHistory => PlaylistType.PlaybackHistory,
                SpecialPlaylists.DownloadSucceededHistory => PlaylistType.DownloadSucceededHistory,
                _ => PlaylistType.DownloadFailedHistory,
            };

            return this._playlistStore.GetPlaylistByType(type);
        }

        #endregion

        #region private

        private void SetChild(IPlaylistInfo current, IReadOnlyList<string> parents)
        {
            foreach (var childID in current.ChildrenID)
            {
                //子プレイリストを取得
                IPlaylistInfo child = this._playlists[childID];
                current.AddChild(child);

                //削除処理用に親のIDを追加
                child.ParentID = current.ID;

                //親プレイリストの名前を設定
                var list = new List<string>(parents)
                {
                    child.Name.Value
                };
                child.SetParentNamesList(list);

                //再帰的にツリーを構築
                this.SetChild(child, list.AsReadOnly());
            }
        }

        /// <summary>
        /// 特殊なプレイリストを作成する
        /// </summary>
        /// <returns></returns>
        private IAttemptResult CreateSpecialPlaylist()
        {
            if (!this._playlistStore.Exist(PlaylistType.Root))
            {
                IAttemptResult<int> rootCreationResult = this._playlistStore.Create("プレイリスト一覧");
                if (!rootCreationResult.IsSucceeded) return AttemptResult.Fail(rootCreationResult.Message);

                IAttemptResult<IPlaylistInfo> rootGetResult = this._playlistStore.GetPlaylist(rootCreationResult.Data);
                if (!rootGetResult.IsSucceeded || rootGetResult.Data is null) return AttemptResult.Fail(rootGetResult.Message);

                rootGetResult.Data.PlaylistType = PlaylistType.Root;
            }

            if (!this._playlistStore.Exist(PlaylistType.Temporary))
            {
                IAttemptResult<int> tempCreationResult = this._playlistStore.Create("一時プレイリスト");
                if (!tempCreationResult.IsSucceeded) return AttemptResult.Fail(tempCreationResult.Message);

                IAttemptResult<IPlaylistInfo> tempGetResult = this._playlistStore.GetPlaylist(tempCreationResult.Data);
                if (!tempGetResult.IsSucceeded || tempGetResult.Data is null) return AttemptResult.Fail(tempGetResult.Message);

                tempGetResult.Data.PlaylistType = PlaylistType.Temporary;
            }

            if (!this._playlistStore.Exist(PlaylistType.DownloadFailedHistory))
            {
                IAttemptResult<int> dlFailedCreationResult = this._playlistStore.Create("ダウンロードに失敗した動画");
                if (!dlFailedCreationResult.IsSucceeded) return AttemptResult.Fail(dlFailedCreationResult.Message);

                IAttemptResult<IPlaylistInfo> dlFailedGetResult = this._playlistStore.GetPlaylist(dlFailedCreationResult.Data);
                if (!dlFailedGetResult.IsSucceeded || dlFailedGetResult.Data is null) return AttemptResult.Fail(dlFailedGetResult.Message);

                dlFailedGetResult.Data.PlaylistType = PlaylistType.DownloadFailedHistory;
            }

            if (!this._playlistStore.Exist(PlaylistType.DownloadSucceededHistory))
            {
                IAttemptResult<int> dlSucceededCreationResult = this._playlistStore.Create("ダウンロードに成功した動画");
                if (!dlSucceededCreationResult.IsSucceeded) return AttemptResult.Fail(dlSucceededCreationResult.Message);

                IAttemptResult<IPlaylistInfo> dlSucceededGetResult = this._playlistStore.GetPlaylist(dlSucceededCreationResult.Data);
                if (!dlSucceededGetResult.IsSucceeded || dlSucceededGetResult.Data is null) return AttemptResult.Fail(dlSucceededGetResult.Message);

                dlSucceededGetResult.Data.PlaylistType = PlaylistType.DownloadSucceededHistory;
            }

            return AttemptResult.Succeeded();
        }

        #endregion
    }

    public enum SpecialPlaylists
    {
        DownloadSucceededHistory,
        DownloadFailedHistory,
        PlaybackHistory,
    }
}
