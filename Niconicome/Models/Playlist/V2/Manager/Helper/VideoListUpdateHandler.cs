using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Video;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Niconicome.Models.Playlist.V2.Manager.StringContent;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.Reactive;
using ParallelTask = Niconicome.Models.Utils.ParallelTaskV2;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;

namespace Niconicome.Models.Playlist.V2.Manager.Helper
{
    public interface IVideoListUpdateHandler
    {
        /// <summary>
        /// リモートプレイリストと同期
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> SyncWithRemotePlaylistAsync(Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// 動画情報を更新
        /// </summary>
        /// <param name="source"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult> UpdateVideosAsync(ReadOnlyCollection<IVideoInfo> source, Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// 動画情報の更新をキャンセル
        /// </summary>
        void CancelUpdate();

        /// <summary>
        /// 更新フラグ
        /// </summary>
        IBindableProperty<bool> IsUpdating { get; }
    }

    internal class VideoListUpdateHandler : VideoListManagerHelperBase, IVideoListUpdateHandler
    {
        public VideoListUpdateHandler(IPlaylistVideoContainer container, IErrorHandler errorHandler, INetVideosInfomationHandler netVideos, IStringHandler stringHandler, IVideoStore videoStore, ITagStore tagStore, ISettingsContainer settingsContainer) : base(videoStore, tagStore)
        {
            this._container = container;
            this._errorHandler = errorHandler;
            this._netVideos = netVideos;
            this._stringHandler = stringHandler;
            this._settingsContainer = settingsContainer;
        }

        #region field

        private readonly IPlaylistVideoContainer _container;

        private readonly IErrorHandler _errorHandler;

        private readonly INetVideosInfomationHandler _netVideos;

        private readonly IStringHandler _stringHandler;

        private readonly ISettingsContainer _settingsContainer;

        private CancellationTokenSource? cts;

        #endregion

        #region Props

        public IBindableProperty<bool> IsUpdating { get; init; } = new BindableProperty<bool>(false);


        #endregion

        #region Method

        public async Task<IAttemptResult> SyncWithRemotePlaylistAsync(Action<string, ErrorLevel> onMessage)
        {
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }


            IPlaylistInfo playlist = this._container.CurrentSelectedPlaylist;
            var videos = new List<IVideoInfo>();

            RemoteType remoteType = this.ConvertToRemoteType(playlist.PlaylistType);
            if (remoteType == RemoteType.None)
            {
                this._errorHandler.HandleError(VideoListManagerError.NotARemotePlaylist, playlist.PlaylistType);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.NotARemotePlaylist, playlist.PlaylistType));
            }


            IAttemptResult<Remote::RemotePlaylistInfo> result = await this._netVideos.GetRemotePlaylistAsync(remoteType, playlist.RemoteParameter, onMessage);
            if (!result.IsSucceeded || result.Data is null) return AttemptResult.Fail(result.Message);

            foreach (var video in result.Data.Videos)
            {
                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, video);
                if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult.Fail(vResult.Message);

                videos.Add(vResult.Data);
            }

            var removedVideos = 0;
            var addedVideos = 0;

            //リモートに存在しない動画は削除
            foreach (var currentVideo in playlist.Videos.ToList())
            {
                if (!videos.Any(v => v.NiconicoId == currentVideo.NiconicoId))
                {
                    playlist.RemoveVideo(currentVideo);
                    removedVideos++;
                }
            }

            //ローカルに存在しない動画は追加
            foreach (var video in videos)
            {
                if (!playlist.Videos.Any(v => v.NiconicoId == video.NiconicoId))
                {
                    playlist.AddVideo(video);
                    addedVideos++;
                }
            }

            onMessage(this._stringHandler.GetContent(VideoListManagerString.SyncWithRemotePlaylistHasCompleted, addedVideos, removedVideos, videos.Count - addedVideos), ErrorLevel.Log);
            this._errorHandler.HandleError(VideoListManagerError.SyncWithRemotePlaylistHasCompleted, remoteType, addedVideos, removedVideos, videos.Count - addedVideos);

            return AttemptResult.Succeeded(this._stringHandler.GetContent(VideoListManagerString.SyncWithRemotePlaylistHasCompleted, addedVideos, removedVideos, videos.Count - addedVideos));
        }

        public async Task<IAttemptResult> UpdateVideosAsync(ReadOnlyCollection<IVideoInfo> source, Action<string, ErrorLevel> onMessage)
        {

            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }

            if (this.IsUpdating.Value)
            {
                this._errorHandler.HandleError(VideoListManagerError.CurrentlyUpdating);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.CurrentlyUpdating));
            }

            IPlaylistInfo playlist = this._container.CurrentSelectedPlaylist;

            this.cts = new CancellationTokenSource();

            this.IsUpdating.Value = true;
            onMessage(this._stringHandler.GetContent(VideoListManagerString.UpdateOfVideoHasStarted), ErrorLevel.Log);


            IAttemptResult<Remote::RemotePlaylistInfo> result = await this._netVideos.GetVideoInfoAsync(source.Select(v => v.NiconicoId), onMessage, cts.Token);

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<Remote::RemotePlaylistInfo>.Fail(result.Message);
            }

            foreach (var video in result.Data.Videos)
            {
                IAttemptResult<IVideoInfo> cResult = this.ConvertToVideoInfo(playlist.ID, video);
                if (!cResult.IsSucceeded || cResult.Data is null)
                {
                    continue;
                }

                cResult.Data.IsSelected.Value = false;
            }

            this.IsUpdating.Value = false;

            return AttemptResult.Succeeded();
        }

        public void CancelUpdate()
        {
            this.IsUpdating.Value = false;
            this.cts?.Cancel();
        }

        #endregion

        private class VideoUpdateTask : ParallelTask::ParallelTask<IVideoInfo>
        {
            public VideoUpdateTask(IVideoInfo taskItem, Func<IVideoInfo, object, Task> taskFunction, Action<int> onWait) : base(taskItem, taskFunction, onWait) { }
        }
    }
}
