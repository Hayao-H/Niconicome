using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Threading.Tasks;
using MS.WindowsAPICodePack.Internal;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Niconicome.Models.Playlist.V2.Manager.StringContent;
using Niconicome.Models.Playlist.V2.Migration;
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
        /// <returns></returns>
        Task LoadVideosAsync(bool quick = false, bool refresh = true);

        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult> RegisterVideoAsync(string inputText, Action<string> onMessage);

        /// <summary>
        /// リモートプレイリストと同期
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> SyncWithRemotePlaylistAsync(Action<string> onMessage);

        /// <summary>
        /// 現在のプレイリストから動画を取得
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        IAttemptResult<IVideoInfo> GetVideoFromCurrentPlaylist(string niconicoID);
    }

    public class VideoListManager : IVideoListManager
    {
        public VideoListManager(IPlaylistVideoContainer container, ILocalVideoLoader loader, IErrorHandler errorHandler, IVideoAndPlayListMigration migration, IVideoStore videoStore, INetVideosInfomationHandler netVideos, IInputTextParser inputTextParser, IStringHandler stringHandler)
        {
            this._container = container;
            this._loader = loader;
            this._errorHandler = errorHandler;
            this._migration = migration;
            this._videoStore = videoStore;
            this._netVideos = netVideos;
            this._inputTextParser = inputTextParser;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly IPlaylistVideoContainer _container;

        private readonly ILocalVideoLoader _loader;

        private readonly IErrorHandler _errorHandler;

        private readonly IVideoAndPlayListMigration _migration;

        private readonly IVideoStore _videoStore;

        private readonly INetVideosInfomationHandler _netVideos;

        private readonly IInputTextParser _inputTextParser;

        private readonly IStringHandler _stringHandler;

        #endregion

        #region Method

        public async Task LoadVideosAsync(bool quick = false, bool refresh = true)
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

            await this._loader.SetPathAsync(refresh ? this._container.CurrentSelectedPlaylist.Videos : this._container.Videos, quick);

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

        public async Task<IAttemptResult> RegisterVideoAsync(string inputText, Action<string> onMessage)
        {
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }

            IPlaylistInfo playlist = this._container.CurrentSelectedPlaylist;
            InputInfomation info = this._inputTextParser.GetInputInfomation(inputText);
            var videos = new List<IVideoInfo>();

            //ニコニコのID
            if (info.InputType == InputType.NiconicoID)
            {
                IAttemptResult<Remote::VideoInfo> result = await this._netVideos.GetVideoInfoAsync(info.Parameter, onMessage);
                if (!result.IsSucceeded || result.Data is null) return AttemptResult.Fail(result.Message);

                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, result.Data);
                if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult.Fail(vResult.Message);

                if (playlist.PlaylistType != PlaylistType.Temporary)
                {
                    playlist.AddVideo(vResult.Data);
                }
                else
                {
                    videos.Add(vResult.Data);
                }

            }
            //リモートプレイリスト
            else if (info.IsRemote)
            {
                IAttemptResult<Remote::RemotePlaylistInfo> result = await this._netVideos.GetRemotePlaylistAsync(info, onMessage);
                if (!result.IsSucceeded || result.Data is null) return AttemptResult.Fail(result.Message);

                foreach (var video in result.Data.Videos)
                {
                    IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, video);
                    if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult.Fail(vResult.Message);

                    if (playlist.PlaylistType != PlaylistType.Temporary)
                    {
                        playlist.AddVideo(vResult.Data);
                    }
                    else
                    {
                        videos.Add(vResult.Data);
                    }
                }
            }

            if (this._container.CurrentSelectedPlaylist?.ID == playlist.ID)
            {
                //一時プレイリストの場合は特殊処理
                if (playlist.PlaylistType == PlaylistType.Temporary) this._container.Videos.AddRange(videos);

                await this.LoadVideosAsync(true, playlist.PlaylistType != PlaylistType.Temporary);
            }

            return AttemptResult.Succeeded();

        }

        public async Task<IAttemptResult> SyncWithRemotePlaylistAsync(Action<string> onMessage)
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

            onMessage(this._stringHandler.GetContent(VideoListManagerString.SyncWithRemotePlaylistHasCompleted, addedVideos, removedVideos, videos.Count - addedVideos));
            this._errorHandler.HandleError(VideoListManagerError.SyncWithRemotePlaylistHasCompleted, remoteType, addedVideos, removedVideos, videos.Count - addedVideos);

            return AttemptResult.Succeeded(this._stringHandler.GetContent(VideoListManagerString.SyncWithRemotePlaylistHasCompleted, addedVideos, removedVideos, videos.Count - addedVideos));
        }

        public IAttemptResult<IVideoInfo> GetVideoFromCurrentPlaylist(string niconicoID)
        {
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult<IVideoInfo>.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }

            IVideoInfo? video = this._container.Videos.FirstOrDefault(v => v.NiconicoId == niconicoID);

            if (video is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.VideoDoesNotExistInCurrentPlaylist, this._container.CurrentSelectedPlaylist.ID, niconicoID);
                return AttemptResult<IVideoInfo>.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.VideoDoesNotExistInCurrentPlaylist, this._container.CurrentSelectedPlaylist.ID, niconicoID));
            }
            else
            {
                return AttemptResult<IVideoInfo>.Succeeded(video);
            }
        }


        #endregion

        #region private

        /// <summary>
        /// 動画情報をローカル形式に変換
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private IAttemptResult<IVideoInfo> ConvertToVideoInfo(int playlistID, Remote::VideoInfo source)
        {

            if (!this._videoStore.Exist(source.NiconicoID, playlistID))
            {
                IAttemptResult cResult = this._videoStore.Create(source.NiconicoID, playlistID);
                if (!cResult.IsSucceeded) return AttemptResult<IVideoInfo>.Fail(cResult.Message);
            }

            IAttemptResult<IVideoInfo> getResult = this._videoStore.GetVideo(source.NiconicoID, playlistID);
            if (!getResult.IsSucceeded || getResult.Data is null) return AttemptResult<IVideoInfo>.Fail(getResult.Message);

            IVideoInfo video = getResult.Data;
            video.IsAutoUpdateEnabled = false;

            video.OwnerName = source.OwnerName;
            video.OwnerID = source.OwnerID;
            video.ThumbUrl = source.ThumbUrl;
            video.UploadedOn = source.UploadedDT;
            video.ViewCount = source.ViewCount;
            video.CommentCount = source.CommentCount;
            video.MylistCount = source.MylistCount;
            video.LikeCount = source.LikeCount;
            video.AddedAt = source.AddedAt;

            video.IsAutoUpdateEnabled = true;

            video.Title = source.Title;

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        /// <summary>
        /// リモートプレイリストの形式を取得
        /// </summary>
        /// <param name="playlistType"></param>
        /// <returns></returns>
        private RemoteType ConvertToRemoteType(PlaylistType playlistType)
        {
            return playlistType switch
            {
                PlaylistType.WatchLater => RemoteType.WatchLater,
                PlaylistType.Mylist => RemoteType.Mylist,
                PlaylistType.Series => RemoteType.Series,
                PlaylistType.UserVideos => RemoteType.UserVideos,
                PlaylistType.Channel => RemoteType.Channel,
                _ => RemoteType.None,
            };
        }

        #endregion
    }
}
