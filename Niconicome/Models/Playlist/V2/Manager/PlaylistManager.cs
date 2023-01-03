using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2.Manager.Error;

namespace Niconicome.Models.Playlist.V2.Manager
{
    public interface IPlaylistManager
    {
        /// <summary>
        /// ツリーを初期化
        /// </summary>
        public void Initialize();

        /// <summary>
        /// プレイリストを削除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public IAttemptResult Delete(int ID);

        /// <summary>
        /// プレイリストを作成
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public IAttemptResult Create(int parentID);
    }

    public class PlaylistManager : IPlaylistManager
    {
        public PlaylistManager(IPlaylistStore playlistStore, IErrorHandler errorHandler, IPlaylistVideoContainer container)
        {
            this._playlistStore = playlistStore;
            this._errorHandler = errorHandler;
            this._container = container;
        }

        #region field

        private readonly IPlaylistStore _playlistStore;

        private readonly IErrorHandler _errorHandler;

        private readonly IPlaylistVideoContainer _container;

        private Dictionary<int, IPlaylistInfo> _playlists = new();

        #endregion

        #region Method
        public void Initialize()
        {

            IAttemptResult<IReadOnlyList<IPlaylistInfo>> result = this._playlistStore.GetAllPlaylist();
            if (!result.IsSucceeded || result.Data is null) return;

            //データを準備
            this._playlists = result.Data.ToDictionary(p => p.ID);
            IPlaylistInfo? root = result.Data.FirstOrDefault(p => p.PlaylistType == PlaylistType.Root);

            //ルートプレイリストが見つからない
            if (root is null) return;

            this.SetChild(root);

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


        #endregion

        #region private

        private void SetChild(IPlaylistInfo current)
        {
            foreach (var childID in current.ChildrenID)
            {
                //子プレイリストを取得
                IPlaylistInfo child = this._playlists[childID];
                current.AddChild(child);

                //削除処理用に親のIDを追加
                child.ParentID = current.ID;

                //再帰的にツリーを構築
                this.SetChild(child);
            }
        }

        #endregion
    }
}
