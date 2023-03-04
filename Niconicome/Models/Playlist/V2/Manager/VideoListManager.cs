using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Threading.Tasks;
using MS.WindowsAPICodePack.Internal;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.OS;
using Niconicome.Models.Network.Video;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Niconicome.Models.Playlist.V2.Manager.Helper;
using Niconicome.Models.Playlist.V2.Manager.StringContent;
using Niconicome.Models.Playlist.V2.Migration;
using Niconicome.Models.Utils.Reactive;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;

namespace Niconicome.Models.Playlist.V2.Manager
{
    public interface IVideoListManager
    {
        /// <summary>
        /// 現在のプレイリスト内の動画を読み込む
        /// </summary>
        /// <param name="quick"></param>
        /// <param name="refresh"></param>
        /// <param name="setPath"></param>
        /// <returns></returns>
        Task LoadVideosAsync(bool quick = false, bool refresh = true, bool setPath = true);

        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<VideoRegistrationResult>> RegisterVideoAsync(string inputText, Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// クリップボードから動画を登録
        /// </summary>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<VideoRegistrationResult>> RegisterVideosFromClipbordAsync(Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// リモートプレイリストと同期
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> SyncWithRemotePlaylistAsync(Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// 現在のプレイリストから動画を取得
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        IAttemptResult<IVideoInfo> GetVideoFromCurrentPlaylist(string niconicoID);

        /// <summary>
        /// 現在の動画から選択された動画を取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IReadOnlyList<IVideoInfo>> GetSelectedVideoFromCurrentPlaylist();

        /// <summary>
        /// 現在のプレイリストから動画を削除
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        IAttemptResult RemoveVideosFromCurrentPlaylist(IReadOnlyList<IVideoInfo> videos);

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

    public class VideoListManager : IVideoListManager
    {
        public VideoListManager(IPlaylistVideoContainer container, ILocalVideoLoader loader, IErrorHandler errorHandler, IVideoAndPlayListMigration migration, IVideoListUpdateHandler updateHandler, IVideoListCRDHandler cRDHandler,IClipbordManager clipbordManager)
        {
            this._container = container;
            this._loader = loader;
            this._errorHandler = errorHandler;
            this._migration = migration;
            this._updateHandler = updateHandler;
            this._CRDHandler = cRDHandler;
            this._clipbordManager = clipbordManager;
        }

        #region field

        private readonly IPlaylistVideoContainer _container;

        private readonly ILocalVideoLoader _loader;

        private readonly IErrorHandler _errorHandler;

        private readonly IVideoAndPlayListMigration _migration;

        private readonly IVideoListUpdateHandler _updateHandler;

        private readonly IVideoListCRDHandler _CRDHandler;

        private readonly IClipbordManager _clipbordManager;

        #endregion

        #region Props

        public IBindableProperty<bool> IsUpdating => this._updateHandler.IsUpdating;

        #endregion

        #region Method

        public async Task LoadVideosAsync(bool quick = false, bool refresh = true, bool setPath = true)
        {
            //移行が必要な場合は処理を中止
            if (this._migration.IsMigrationNeeded) return;

            //プレイリストが選択されていない場合はエラー
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return;
            }

            int playlistID = this._container.CurrentSelectedPlaylist.ID;

            if (setPath)
            {
                await this._loader.SetPathAsync(refresh ? this._container.CurrentSelectedPlaylist.Videos : this._container.Videos, quick);
            }

            if (playlistID != (this._container.CurrentSelectedPlaylist?.ID ?? -1))
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistChanged);
                return;
            }

            if (refresh)
            {
                this._container.Videos.Clear();
                this._container.Videos.AddRange(this._container.CurrentSelectedPlaylist!.Videos);
            }

        }

        public async Task<IAttemptResult<VideoRegistrationResult>> RegisterVideoAsync(string inputText, Action<string, ErrorLevel> onMessage)
        {
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult<VideoRegistrationResult>.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }

            IPlaylistInfo playlist = this._container.CurrentSelectedPlaylist;

            IAttemptResult<VideoRegistrationResult> result = await this._CRDHandler.RegisterVideoAsync(inputText, playlist, onMessage);

            await this.LoadVideosAsync(true, playlist.PlaylistType != PlaylistType.Temporary);

            return result;
        }

        public async Task<IAttemptResult<VideoRegistrationResult>> RegisterVideosFromClipbordAsync(Action<string, ErrorLevel> onMessage)
        {
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult<VideoRegistrationResult>.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }

            IPlaylistInfo playlist = this._container.CurrentSelectedPlaylist;

            IAttemptResult<string> text = this._clipbordManager.GetClipboardContent();
            if (!text.IsSucceeded||text.Data is null)
            {
                return AttemptResult<VideoRegistrationResult>.Fail(text.Message);
            }

            IAttemptResult<VideoRegistrationResult> result = await this._CRDHandler.RegisterVideoFromTextAsync(text.Data, playlist, onMessage);

            await this.LoadVideosAsync(true, playlist.PlaylistType != PlaylistType.Temporary);

            return result;
        }


        public async Task<IAttemptResult> SyncWithRemotePlaylistAsync(Action<string, ErrorLevel> onMessage)
        {
            return await this._updateHandler.SyncWithRemotePlaylistAsync(onMessage);
        }

        public IAttemptResult<IVideoInfo> GetVideoFromCurrentPlaylist(string niconicoID)
        {
            return this._CRDHandler.GetVideoFromCurrentPlaylist(niconicoID);
        }

        public IAttemptResult<IReadOnlyList<IVideoInfo>> GetSelectedVideoFromCurrentPlaylist()
        {

            return this._CRDHandler.GetSelectedVideoFromCurrentPlaylist();

        }

        public IAttemptResult RemoveVideosFromCurrentPlaylist(IReadOnlyList<IVideoInfo> videos)
        {
            return this._CRDHandler.RemoveVideosFromCurrentPlaylist(videos);

        }

        public async Task<IAttemptResult> UpdateVideosAsync(ReadOnlyCollection<IVideoInfo> source, Action<string, ErrorLevel> onMessage)
        {
            return await this._updateHandler.UpdateVideosAsync(source, onMessage);
        }

        public void CancelUpdate()
        {
            this._updateHandler.CancelUpdate();
        }

        #endregion

    }

    public record VideoRegistrationResult(bool IsChannelVideo, int VideosCount, string ChannelName, string ChannelID);

}
