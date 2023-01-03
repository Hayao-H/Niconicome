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

            foreach (var video in videosResult.Data)
            {
                listner(video.ToString());

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
                videoInfo.ViewCount = video.ViewCount;
                videoInfo.CommentCount = video.CommentCount;
                videoInfo.MylistCount = video.MylistCount;
                videoInfo.LikeCount = video.LikeCount;
                videoInfo.Duration = video.Duration;
                videoInfo.OwnerID = video.OwnerID;
                videoInfo.NiconicoId = video.NiconicoId;
                videoInfo.Title = video.Title;
                videoInfo.LargeThumbUrl = video.LargeThumbUrl;
                videoInfo.ThumbUrl = video.ThumbUrl;
                videoInfo.OwnerName = video.OwnerName;
                videoInfo.IsDeleted = video.IsDeleted;
                videoInfo.UploadedOn = video.UploadedOn;
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

            foreach (var playlist in playlistGetResult.Data)
            {
                if (playlist.IsRoot || playlist.IsDownloadFailedHistory || playlist.IsDownloadSucceededHistory) continue;

                listner(playlist.ToString());

                IAttemptResult<int> playlisrCreationResult = this._playlistStore.Create(playlist.PlaylistName ?? LocalConstant.DefaultPlaylistName);
                if (!playlisrCreationResult.IsSucceeded)
                {
                    failed.Add(new DetailedMigrationResult(playlist.PlaylistName ?? LocalConstant.DefaultPlaylistName, playlisrCreationResult.Message ?? ""));
                    continue;
                }

                IAttemptResult<IPlaylistInfo> playlistResult = this._playlistStore.GetPlaylist(playlisrCreationResult.Data);
                if (!playlistResult.IsSucceeded || playlistResult.Data is null)
                {
                    failed.Add(new DetailedMigrationResult(playlist.PlaylistName ?? LocalConstant.DefaultPlaylistName, playlistResult.Message ?? ""));
                    continue;
                }

                IPlaylistInfo info = playlistResult.Data;
                info.FolderPath = playlist.FolderPath ?? "";
                if (playlist.IsTemporary)
                {
                    info.PlaylistType = PlaylistType.Temporary;
                }
                else if (playlist.IsMylist)
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
                else if (playlist.IsRoot)
                {
                    info.PlaylistType = PlaylistType.Root;
                }
                else if (playlist.IsDownloadSucceededHistory)
                {
                    info.PlaylistType = PlaylistType.DownloadSucceededHistory;
                }
                else if (playlist.IsDownloadFailedHistory)
                {
                    info.PlaylistType = PlaylistType.DownloadFailedHistory;
                }
                else
                {
                    info.PlaylistType = PlaylistType.Local;
                }
                info.RemoteParameter = playlist.RemoteId ?? "";

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

                info.Name.Value = info.Name.Value;

                info.IsAutoUpdateEnabled = true;
            }

            return AttemptResult<PlaylistMigrationResult>.Succeeded(new PlaylistMigrationResult(failed, partlyFailed));
        }

        #endregion

        private record PlaylistMigrationResult(IReadOnlyList<DetailedMigrationResult> FailedPlaylist, IReadOnlyList<DetailedMigrationResult> PartlyFailedPlaylist);
    }
}
