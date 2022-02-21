using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist.SharedUtils;
using State = Niconicome.Models.Local.State;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface IPlaylistHandler
    {
        /// <summary>
        /// 指定したプレイリストに子プレイリストを作成する
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        IAttemptResult<int> AddPlaylist(int parentId);

        /// <summary>
        /// ルート直下にプレイリストを作成する
        /// </summary>
        /// <returns></returns>
        IAttemptResult<int> AddPlaylistToRoot();

        /// <summary>
        /// プレイリストを削除する
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult DeletePlaylist(int playlistID);

        /// <summary>
        /// プレイリストを更新する
        /// </summary>
        /// <param name="newpaylist"></param>
        IAttemptResult Update(ITreePlaylistInfo newpaylist);

        /// <summary>
        /// 指定したプレイリストを取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<ITreePlaylistInfo> GetPlaylist(int id);

        /// <summary>
        /// 特殊プレイリストを取得
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        IAttemptResult<ITreePlaylistInfo> GetSpecialPlaylist(SpecialPlaylistTypes types);

        /// <summary>
        /// ルートプレイリストを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<ITreePlaylistInfo> GetRootPlaylist();

        /// <summary>
        /// すべてのプレイリストを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<ITreePlaylistInfo>> GetAllPlaylists();

        /// <summary>
        /// 動画をプレイリストに追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult WireVideoToPlaylist(int videoID, int playlistID);

        /// <summary>
        /// 動画をプレイリストから削除する
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult UnWireVideoToPlaylist(int videoID, int playlistID);

        /// <summary>
        /// 指定した動画をブックマーク
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult BookMark(int videoID, int playlistID);

        /// <summary>
        /// 指定した動画を前に移動
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult MoveVideoToPrev(int videoIndex, int playlistID);

        /// <summary>
        /// 指定した動画を後ろに移動
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult MoveVideoToForward(int videoIndex, int playlistID);

        /// <summary>
        /// すべてのプレイリストを更新する
        /// </summary>
        IAttemptResult Refresh();

        /// <summary>
        /// 状態を指定してすべてのプレイリストを更新する
        /// </summary>
        /// <param name="expandAll"></param>
        /// <param name="inheritExpandedState"></param>
        IAttemptResult Refresh(bool expandAll, bool inheritExpandedState);

        /// <summary>
        /// プレイリストを移動する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        IAttemptResult Move(int id, int targetId);

        /// <summary>
        /// すべてのプレイリストの状態を保存
        /// </summary>
        IAttemptResult SaveAllPlaylists();

        /// <summary>
        /// リモートプレイリストとして設定
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IAttemptResult SetAsRemotePlaylist(int playlistId, string remoteID, string name, RemoteType type);

        /// <summary>
        /// ローカルプレイリストとして設定
        /// </summary>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        IAttemptResult SetAsLocalPlaylist(int playlistId);
    }

    public class PlaylistHandler : IPlaylistHandler
    {
        public PlaylistHandler(IPlaylistTreeHandler handler, IPlaylistStoreHandler playlistStoreHandler, ILocalSettingHandler settingHandler, ILogger logger, IVideoPlaylistConverter converter,IVideoStoreHandler videoStoreHandler)
        {
            this._treeHandler = handler;
            this._playlistStoreHandler = playlistStoreHandler;
            this._settingHandler = settingHandler;
            this._logger = logger;
            this._converter = converter;
            this._videoStoreHandler = videoStoreHandler;
        }

        #region field
        private readonly IPlaylistTreeHandler _treeHandler;

        private readonly IPlaylistStoreHandler _playlistStoreHandler;

        private readonly IVideoStoreHandler _videoStoreHandler;

        private readonly ILocalSettingHandler _settingHandler;

        private readonly ILogger _logger;

        private readonly IVideoPlaylistConverter _converter;


        #endregion

        #region CRUD

        public IAttemptResult<int> AddPlaylist(int parentId)
        {
            IAttemptResult<int> result = this._playlistStoreHandler.AddPlaylist(parentId, "新しいプレイリスト");

            if (!result.IsSucceeded)
            {
                return AttemptResult<int>.Fail(result.Message);
            }

            IAttemptResult<ITreePlaylistInfo> pResult = this.GetPlaylist(result.Data);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult<int>.Fail(pResult.Message);
            }


            this._treeHandler.MergeRange(new List<ITreePlaylistInfo>() { pResult.Data });

            return AttemptResult<int>.Succeeded(pResult.Data.Id);
        }

        public IAttemptResult<int> AddPlaylistToRoot()
        {
            IAttemptResult<ITreePlaylistInfo> rResult = this.GetRootPlaylist();

            if (!rResult.IsSucceeded || rResult.Data is null)
            {
                return AttemptResult<int>.Fail(rResult.Message);
            }

            IAttemptResult<int> result = this.AddPlaylist(rResult.Data.Id);

            if (!result.IsSucceeded)
            {
                return AttemptResult<int>.Fail(result.Message);
            }

            return AttemptResult<int>.Succeeded(result.Data);

        }

        public IAttemptResult DeletePlaylist(int playlistID)
        {
            IAttemptResult dResult = this._playlistStoreHandler.DeletePlaylist(playlistID);
            if (!dResult.IsSucceeded)
            {
                return dResult;
            }

            this._treeHandler.Remove(playlistID);

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<ITreePlaylistInfo> GetPlaylist(int id)
        {
            IAttemptResult<STypes::Playlist> result = this._playlistStoreHandler.GetPlaylist(id);

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<ITreePlaylistInfo>.Fail(result.Message);
            }

            ITreePlaylistInfo converted = this._converter.ConvertStorePlaylistToLocalPlaylist(result.Data);

            return AttemptResult<ITreePlaylistInfo>.Succeeded(converted);
        }

        public IAttemptResult<ITreePlaylistInfo> GetRootPlaylist()
        {
            IAttemptResult<STypes::Playlist> result = this._playlistStoreHandler.GetRootPlaylist();

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<ITreePlaylistInfo>.Fail(result.Message);
            }

            ITreePlaylistInfo converted = this._converter.ConvertStorePlaylistToLocalPlaylist(result.Data);

            return AttemptResult<ITreePlaylistInfo>.Succeeded(converted);
        }

        public IAttemptResult<IEnumerable<ITreePlaylistInfo>> GetAllPlaylists()
        {
            IAttemptResult<List<STypes::Playlist>> result = this._playlistStoreHandler.GetAllPlaylists();

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<ITreePlaylistInfo>>.Fail(result.Message);
            }

            return AttemptResult<IEnumerable<ITreePlaylistInfo>>.Succeeded(result.Data.Select(p => this._converter.ConvertStorePlaylistToLocalPlaylist(p)));
        }

        public IAttemptResult<ITreePlaylistInfo> GetSpecialPlaylist(SpecialPlaylistTypes types)
        {
            IAttemptResult<STypes::Playlist> result = types switch
            {
                SpecialPlaylistTypes.DLFailedHistory => this._playlistStoreHandler.GetPlaylist(p => p.IsDownloadFailedHistory),
                _ => this._playlistStoreHandler.GetPlaylist(p => p.IsDownloadSucceededHistory),
            };

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<ITreePlaylistInfo>.Fail(result.Message);
            }

            ITreePlaylistInfo converted = this._converter.ConvertStorePlaylistToLocalPlaylist(result.Data);

            return AttemptResult<ITreePlaylistInfo>.Succeeded(converted);
        }

        #endregion

        #region 動画系

        public IAttemptResult WireVideoToPlaylist(int videoID, int playlistID)
        {

            IAttemptResult<STypes::Video> vResult = this._videoStoreHandler.GetVideo(videoID);

            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return AttemptResult.Fail("追加する動画はDBに保存されていません。");
            }

            STypes::Video video = vResult.Data;
            video.PlaylistIds.AddUnique(playlistID);
            this._videoStoreHandler.Update(video);

            IAttemptResult result = this._playlistStoreHandler.WireVideo(video, playlistID);

            return result;
        }

        public IAttemptResult UnWireVideoToPlaylist(int videoID, int playlistID)
        {
            IAttemptResult result = this._playlistStoreHandler.UnWireVideo(videoID, playlistID);
            return result;
        }

        public IAttemptResult BookMark(int videoID, int playlistID)
        {
            IAttemptResult<ITreePlaylistInfo> result = this.GetPlaylist(playlistID);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }

            ITreePlaylistInfo playlist = result.Data;
            playlist.BookMarkedVideoID = videoID;

            return this.Update(playlist);

        }

        public IAttemptResult MoveVideoToPrev(int videoIndex, int playlistID)
        {
            if (videoIndex == 0)
            {
                return AttemptResult.Fail("指定された動画は先頭に存在するため前に移動できません。");
            }

            IAttemptResult<ITreePlaylistInfo> result = this.GetPlaylist(playlistID);

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }

            ITreePlaylistInfo playlist = result.Data;

            try
            {
                playlist.Videos.InsertIntoPrev(videoIndex);
            }
            catch (Exception e)
            {
                this._logger.Error($"動画の並び替え操作に失敗しました。({nameof(this.MoveVideoToPrev)})", e);
                return AttemptResult.Fail($"動画の並び替え操作に失敗しました。(詳細:{e.Message})", e);
            }

            return this.Update(playlist);
        }

        public IAttemptResult MoveVideoToForward(int videoIndex, int playlistID)
        {

            IAttemptResult<ITreePlaylistInfo> result = this.GetPlaylist(playlistID);

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }

            ITreePlaylistInfo playlist = result.Data;

            if (videoIndex == playlist.Videos.Count - 1)
            {
                return AttemptResult.Fail("指定された動画は最後に存在するため前に移動できません。");
            }

            try
            {
                playlist.Videos.InsertIntoForward(videoIndex);
            }
            catch (Exception e)
            {
                this._logger.Error($"動画の並び替え操作に失敗しました。({nameof(this.MoveVideoToForward)})", e);
                return AttemptResult.Fail($"動画の並び替え操作に失敗しました。(詳細:{e.Message})", e);
            }

            return this.Update(playlist);
        }

        #endregion

        #region プレイリスト系

        public IAttemptResult Update(ITreePlaylistInfo newpaylist)
        {
            if (!this._playlistStoreHandler.Exists(newpaylist.Id))
            {
                return AttemptResult.Fail("指定されたプレイリストが存在しません。");
            }

            STypes::Playlist converted = this._converter.ConvertLocalPlaylistToStorePlaylist(newpaylist);

            IAttemptResult result = this._playlistStoreHandler.Update(converted);

            if (!result.IsSucceeded)
            {
                return AttemptResult.Fail();
            }

            this._treeHandler.MergeRange(new List<ITreePlaylistInfo> { newpaylist });

            return AttemptResult.Succeeded();
        }

        public IAttemptResult Refresh()
        {
            return this.SetPlaylists();
        }

        public IAttemptResult Refresh(bool expandAll, bool inheritExpandedState)
        {
            return this.SetPlaylists(expandAll, inheritExpandedState);
        }

        public IAttemptResult Move(int id, int targetId)
        {
            IAttemptResult mResult = this._playlistStoreHandler.Move(id, targetId);

            if (!mResult.IsSucceeded)
            {
                return mResult;
            }

            return this.SetPlaylists();
        }

        public IAttemptResult SaveAllPlaylists()
        {
            var playlists = this._treeHandler.GetAllPlaylists();

            foreach (var p in playlists)
            {
                if (!this._playlistStoreHandler.Exists(p.Id)) continue;
                IAttemptResult result = this.Update(p);

                if (!result.IsSucceeded)
                {
                    return AttemptResult.Fail();
                }
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult SetAsRemotePlaylist(int playlistId, string remoteID, string name, RemoteType type)
        {
            IAttemptResult result = this._playlistStoreHandler.SetAsRemotePlaylist(playlistId, remoteID, type);

            if (!result.IsSucceeded)
            {
                return result;
            }

            bool autoRename = this._settingHandler.GetBoolSetting(SettingsEnum.AutoRenameNetPlaylist);

            if (!autoRename)
            {
                return AttemptResult.Succeeded();
            }

            string playlistName = type switch
            {
                RemoteType.Mylist => name,
                RemoteType.UserVideos => $"{name}さんの投稿動画",
                RemoteType.WatchLater => "あとで見る",
                RemoteType.Channel => name,
                RemoteType.Series => name,
                _ => name,
            };

            if (string.IsNullOrEmpty(name))
            {
                return AttemptResult.Succeeded();
            }

            IAttemptResult<ITreePlaylistInfo> pResult = this.GetPlaylist(playlistId);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail(pResult.Message);
            }

            ITreePlaylistInfo playlist = pResult.Data;

            playlist.Name.Value = playlistName;
            IAttemptResult uResult = this.Update(playlist);

            return uResult;
        }

        public IAttemptResult SetAsLocalPlaylist(int playlistId)
        {
            return this._playlistStoreHandler.SetAsLocalPlaylist(playlistId);
        }
        #endregion

        private IAttemptResult SetPlaylists(bool expandAll = false, bool inheritExpandedState = false)
        {

            //プレイリスト
            var list = new List<ITreePlaylistInfo>();

            //プレイリストを取得する
            IAttemptResult<List<STypes::Playlist>> pResult = this._playlistStoreHandler.GetAllPlaylists();

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail();
            }

            for (var i = 0; i < pResult.Data.Count; ++i)
            {
                STypes::Playlist p = pResult.Data[i];

                ITreePlaylistInfo playlist = this._converter.ConvertStorePlaylistToLocalPlaylist(p);

                var ex = playlist.IsExpanded;
                if (expandAll)
                {
                    ex = true;
                }
                else if (inheritExpandedState)
                {
                    ex = p.IsExpanded;
                }

                playlist.IsExpanded = ex;
                list.Add(playlist);
            }

            this._treeHandler.Initialize(list);

            return AttemptResult.Succeeded();
        }
    }

    public enum SpecialPlaylistTypes
    {
        DLSucceedeeHistory,
        DLFailedHistory,
    }
}
