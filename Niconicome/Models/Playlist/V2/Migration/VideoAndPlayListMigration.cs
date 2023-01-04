using System;
using System.Collections.Generic;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Playlist.V2.Migration
{
    public interface IVideoAndPlayListMigration
    {
        /// <summary>
        /// 移行が必要かどうか
        /// </summary>
        bool IsMigrationNeeded { get; }

        /// <summary>
        /// DBを移行する
        /// </summary>
        /// <param name="listner">移行中のデータを示す文字列を受けるハンドラ</param>
        /// <returns></returns>
        IAttemptResult<MigrationResult> Migrate(Action<string> listner);
    }

    public class VideoAndPlayListMigration : IVideoAndPlayListMigration
    {
        public VideoAndPlayListMigration(IApplicationStore applicationStore, IPlaylistStoreHandler playlistStoreHandler, IPlaylistStore playlistStore, IVideoStoreHandler videoStoreHandler, IVideoStore videoStore)
        {
            this._applicationStore = applicationStore;
            this._playlistStore = playlistStore;
            this._playlistStoreHandler = playlistStoreHandler;
            this._videoStore = videoStore;
            this._videoStoreHandler = videoStoreHandler;
        }

        #region field

        private readonly IApplicationStore _applicationStore;

        private readonly IPlaylistStoreHandler _playlistStoreHandler;

        private readonly IPlaylistStore _playlistStore;

        private readonly IVideoStoreHandler _videoStoreHandler;

        private readonly IVideoStore _videoStore;

        #endregion

        #region Props

        public bool IsMigrationNeeded
        {
            get
            {
                //return true;
                IAttemptResult<Version> result = this._applicationStore.GetDBVersion();
                if (!result.IsSucceeded || result.Data is null) return false;
                return result.Data < DBVersionConstant.VideosAndPlaylistMigrated;
            }
        }

        #endregion

        #region Method
        public IAttemptResult<MigrationResult> Migrate(Action<string> listner)
        {
            IAttemptResult<IReadOnlyList<DetailedMigrationResult>> videosResult = this.MigrateVideos(listner);
            if (!videosResult.IsSucceeded || videosResult.Data is null) return AttemptResult<MigrationResult>.Fail(videosResult.Message);

            IAttemptResult<PlaylistMigrationResult> playlistResult = this.MigratePlaylists(listner);
            if (!playlistResult.IsSucceeded || playlistResult.Data is null) return AttemptResult<MigrationResult>.Fail(playlistResult.Message);

            this._applicationStore.SetDBVersion(DBVersionConstant.VideosAndPlaylistMigrated);

            return AttemptResult<MigrationResult>.Succeeded(new MigrationResult(videosResult.Data, playlistResult.Data.FailedPlaylist, playlistResult.Data.PartlyFailedPlaylist));
        }


        #endregion

        #region private

        /// <summary>
        /// 動画を移行する
        /// </summary>
        /// <returns></returns>
        private IAttemptResult<IReadOnlyList<DetailedMigrationResult>> MigrateVideos(Action<string> listner)
        {
            IAttemptResult clearResult = this._videoStore.Clear();
            if (!clearResult.IsSucceeded) return AttemptResult<IReadOnlyList<DetailedMigrationResult>>.Fail(clearResult.Message);

            IAttemptResult<List<STypes::Video>> videosResult = this._videoStoreHandler.GetAllVideos();
            if (!videosResult.IsSucceeded || videosResult.Data is null) return AttemptResult<IReadOnlyList<DetailedMigrationResult>>.Fail(videosResult.Message);

            var failed = new List<DetailedMigrationResult>();
            var migrated = new List<string>();

            foreach (var video in videosResult.Data)
            {
                listner(video.ToString());

                if (migrated.Contains(video.NiconicoId))
                {
                    continue;
                }

                IAttemptResult<int> videoCreationResult = this._videoStore.Create(video.NiconicoId);
                if (!videoCreationResult.IsSucceeded)
                {
                    failed.Add(new DetailedMigrationResult(video.NiconicoId, videoCreationResult.Message ?? ""));
                    continue;
                }

                IAttemptResult<IVideoInfo> videoResult = this._videoStore.GetOnlySharedVideoVideo(videoCreationResult.Data);
                if (!videoResult.IsSucceeded || videoResult.Data is null)
                {
                    failed.Add(new DetailedMigrationResult(video.NiconicoId, videoResult.Message ?? ""));
                    continue;
                }

                IVideoInfo videoInfo = videoResult.Data;
                videoInfo.IsAutoUpdateEnabled = false;

                videoInfo.ViewCount = video.ViewCount;
                videoInfo.CommentCount = video.CommentCount;
                videoInfo.MylistCount = video.MylistCount;
                videoInfo.LikeCount = video.LikeCount;
                videoInfo.Duration = video.Duration;
                videoInfo.OwnerID = video.OwnerID;
                videoInfo.LargeThumbUrl = video.LargeThumbUrl;
                videoInfo.ThumbUrl = video.ThumbUrl;
                videoInfo.OwnerName = video.OwnerName;
                videoInfo.IsDeleted = video.IsDeleted;
                videoInfo.UploadedOn = video.UploadedOn;
                videoInfo.Title = video.Title;

                videoInfo.IsAutoUpdateEnabled = true;

                videoInfo.NiconicoId = video.NiconicoId;
                migrated.Add(video.NiconicoId);
            }

            return AttemptResult<IReadOnlyList<DetailedMigrationResult>>.Succeeded(failed.AsReadOnly());
        }

        /// <summary>
        /// プレイリストを移行する
        /// </summary>
        /// <returns></returns>
        private IAttemptResult<PlaylistMigrationResult> MigratePlaylists(Action<string> listner)
        {
            IAttemptResult clearResult = this._playlistStore.Clear();
            if (!clearResult.IsSucceeded) return AttemptResult<PlaylistMigrationResult>.Fail(clearResult.Message);

            IAttemptResult<List<STypes::Playlist>> playlistGetResult = this._playlistStoreHandler.GetAllPlaylists();
            if (!playlistGetResult.IsSucceeded || playlistGetResult.Data is null) return AttemptResult<PlaylistMigrationResult>.Fail(playlistGetResult.Message);

            var failed = new List<DetailedMigrationResult>();
            var partlyFailed = new List<DetailedMigrationResult>();

            IAttemptResult specialResult = this.CreateSpecialPlaylist();
            if (!specialResult.IsSucceeded) return AttemptResult<PlaylistMigrationResult>.Fail(specialResult.Message);

            //[index:旧ID]
            var newPlaylistsDict = new Dictionary<int, IPlaylistInfo>();

            foreach (var playlist in playlistGetResult.Data)
            {

                listner(playlist.ToString());

                IAttemptResult<IPlaylistInfo> playlistResult = this.CreateAndGetPlaylistInfo(playlist);
                if (!playlistResult.IsSucceeded || playlistResult.Data is null)
                {
                    failed.Add(new DetailedMigrationResult(playlist.PlaylistName ?? LocalConstant.DefaultPlaylistName, playlistResult.Message ?? ""));
                    continue;
                }

                IPlaylistInfo info = playlistResult.Data;


                info.FolderPath = playlist.FolderPath ?? "";
                if (playlist.IsMylist)
                {
                    info.PlaylistType = PlaylistType.Mylist;
                }
                else if (playlist.IsSeries)
                {
                    info.PlaylistType = PlaylistType.Series;
                }
                else if (playlist.IsWatchLater)
                {
                    info.PlaylistType = PlaylistType.WatchLater;
                }
                else if (playlist.IsUserVideos)
                {
                    info.PlaylistType = PlaylistType.UserVideos;
                }
                else if (playlist.IsChannel)
                {
                    info.PlaylistType = PlaylistType.Channel;
                }
                else if (!playlist.IsRoot && !playlist.IsTemporary && !playlist.IsDownloadFailedHistory && !playlist.IsDownloadSucceededHistory)
                {
                    info.PlaylistType = PlaylistType.Local;
                }

                info.RemoteParameter = playlist.RemoteId ?? "";

                //動画を追加(一時プレイリストの場合は無視)
                if (!playlist.IsTemporary)
                {

                    info.IsAutoUpdateEnabled = false;

                    var failedVideos = new List<string>();

                    foreach (var video in playlist.Videos)
                    {
                        IAttemptResult<int> videoCreationResult = this._videoStore.Create(video.NiconicoId, info.ID);
                        if (!videoCreationResult.IsSucceeded)
                        {
                            failedVideos.Add(video.ToString());
                            continue;
                        }

                        IAttemptResult<IVideoInfo> videoGetResult = this._videoStore.GetVideo(videoCreationResult.Data, info.ID);
                        if (!videoGetResult.IsSucceeded || videoGetResult.Data is null)
                        {
                            failedVideos.Add(video.ToString());
                            continue;
                        }
                        info.AddVideo(videoGetResult.Data);
                    }

                    if (failedVideos.Count > 0)
                    {
                        partlyFailed.Add(new DetailedMigrationResult(playlist.PlaylistName ?? LocalConstant.DefaultPlaylistName, string.Join(",", failedVideos)));
                    }

                    info.IsAutoUpdateEnabled = true;
                }

                info.Name.Value = info.Name.Value;


                newPlaylistsDict.Add(playlist.Id, info);
            }

            //ツリーを構築
            foreach (var playlist in playlistGetResult.Data)
            {
                if (playlist.ParentPlaylist is null) continue;

                IPlaylistInfo parent = newPlaylistsDict[playlist.ParentPlaylist.Id];
                parent.AddChild(newPlaylistsDict[playlist.Id]);

            }

            return AttemptResult<PlaylistMigrationResult>.Succeeded(new PlaylistMigrationResult(failed, partlyFailed));
        }

        /// <summary>
        /// 特殊なプレイリストを作成する
        /// </summary>
        /// <returns></returns>
        private IAttemptResult CreateSpecialPlaylist()
        {
            IAttemptResult<int> rootCreationResult = this._playlistStore.Create("プレイリスト一覧");
            if (!rootCreationResult.IsSucceeded) return AttemptResult.Fail(rootCreationResult.Message);

            IAttemptResult<IPlaylistInfo> rootGetResult = this._playlistStore.GetPlaylist(rootCreationResult.Data);
            if (!rootGetResult.IsSucceeded || rootGetResult.Data is null) return AttemptResult.Fail(rootGetResult.Message);

            rootGetResult.Data.PlaylistType = PlaylistType.Root;

            IAttemptResult<int> tempCreationResult = this._playlistStore.Create("一時プレイリスト");
            if (!tempCreationResult.IsSucceeded) return AttemptResult.Fail(tempCreationResult.Message);

            IAttemptResult<IPlaylistInfo> tempGetResult = this._playlistStore.GetPlaylist(tempCreationResult.Data);
            if (!tempGetResult.IsSucceeded || tempGetResult.Data is null) return AttemptResult.Fail(tempGetResult.Message);

            tempGetResult.Data.PlaylistType = PlaylistType.Temporary;

            IAttemptResult<int> dlFailedCreationResult = this._playlistStore.Create("ダウンロードに失敗した動画");
            if (!dlFailedCreationResult.IsSucceeded) return AttemptResult.Fail(dlFailedCreationResult.Message);

            IAttemptResult<IPlaylistInfo> dlFailedGetResult = this._playlistStore.GetPlaylist(dlFailedCreationResult.Data);
            if (!dlFailedGetResult.IsSucceeded || dlFailedGetResult.Data is null) return AttemptResult.Fail(dlFailedGetResult.Message);

            dlFailedGetResult.Data.PlaylistType = PlaylistType.DownloadFailedHistory;

            IAttemptResult<int> dlSucceededCreationResult = this._playlistStore.Create("ダウンロードに成功した動画");
            if (!dlSucceededCreationResult.IsSucceeded) return AttemptResult.Fail(dlSucceededCreationResult.Message);

            IAttemptResult<IPlaylistInfo> dlSucceededGetResult = this._playlistStore.GetPlaylist(dlSucceededCreationResult.Data);
            if (!dlSucceededGetResult.IsSucceeded || dlSucceededGetResult.Data is null) return AttemptResult.Fail(dlSucceededGetResult.Message);

            dlSucceededGetResult.Data.PlaylistType = PlaylistType.DownloadSucceededHistory;

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 新しいプレイリストを作成して取得する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        private IAttemptResult<IPlaylistInfo> CreateAndGetPlaylistInfo(STypes::Playlist playlist)
        {
            IPlaylistInfo info;

            if (playlist.IsRoot || playlist.IsTemporary || playlist.IsDownloadFailedHistory || playlist.IsDownloadSucceededHistory)
            {

                PlaylistType type = playlist.IsRoot ? PlaylistType.Root : playlist.IsTemporary ? PlaylistType.Temporary : playlist.IsDownloadFailedHistory ? PlaylistType.DownloadFailedHistory : PlaylistType.DownloadSucceededHistory;

                IAttemptResult<IPlaylistInfo> playlistResult = this._playlistStore.GetPlaylistByType(type);
                if (!playlistResult.IsSucceeded || playlistResult.Data is null) return playlistResult;
                info = playlistResult.Data;

            }
            else
            {
                IAttemptResult<int> playlisrCreationResult = this._playlistStore.Create(playlist.PlaylistName ?? LocalConstant.DefaultPlaylistName);
                if (!playlisrCreationResult.IsSucceeded) return AttemptResult<IPlaylistInfo>.Fail(playlisrCreationResult.Message);

                IAttemptResult<IPlaylistInfo> playlistResult = this._playlistStore.GetPlaylist(playlisrCreationResult.Data);
                if (!playlistResult.IsSucceeded || playlistResult.Data is null) return playlistResult;

                info = playlistResult.Data;
            }

            return AttemptResult<IPlaylistInfo>.Succeeded(info);
        }

        #endregion

        private record PlaylistMigrationResult(IReadOnlyList<DetailedMigrationResult> FailedPlaylist, IReadOnlyList<DetailedMigrationResult> PartlyFailedPlaylist);
    }
}
