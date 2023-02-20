using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Niconicome.Models.Playlist.V2.Migration;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;

namespace Niconicome.Models.Playlist.V2.Manager.Helper
{
    public interface IVideoListCRDHandler
    {
        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="playlist"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<VideoRegistrationResult>> RegisterVideoAsync(string inputText, IPlaylistInfo playlist, Action<string, ErrorLevel> onMessage);

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
    }

    public class VideoListCRDHandler : VideoListManagerHelperBase, IVideoListCRDHandler
    {
        public VideoListCRDHandler(IPlaylistVideoContainer container, IErrorHandler errorHandler, IVideoStore videoStore, INetVideosInfomationHandler netVideos, IInputTextParser inputTextParser, ITagStore tagStore) : base(videoStore, tagStore)
        {
            this._container = container;
            this._errorHandler = errorHandler;
            this._netVideos = netVideos;
            this._inputTextParser = inputTextParser;
        }

        #region field

        private readonly IPlaylistVideoContainer _container;

        private readonly IErrorHandler _errorHandler;

        private readonly INetVideosInfomationHandler _netVideos;

        private readonly IInputTextParser _inputTextParser;

        #endregion

        #region Method



        public async Task<IAttemptResult<VideoRegistrationResult>> RegisterVideoAsync(string inputText, IPlaylistInfo playlist, Action<string, ErrorLevel> onMessage)
        {
            InputInfomation info = this._inputTextParser.GetInputInfomation(inputText);
            var videos = new List<IVideoInfo>();
            VideoRegistrationResult? rResult = null;

            //ニコニコのID
            if (info.InputType == InputType.NiconicoID)
            {
                IAttemptResult<Remote::VideoInfo> result = await this._netVideos.GetVideoInfoAsync(info.Parameter, onMessage);
                if (!result.IsSucceeded || result.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(result.Message);

                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, result.Data);
                if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(vResult.Message);

                if (playlist.PlaylistType != PlaylistType.Temporary)
                {
                    playlist.AddVideo(vResult.Data);
                }
                else
                {
                    videos.Add(vResult.Data);
                }


                if (vResult.Data.ChannelName.IsNullOrEmpty())
                {
                    rResult = new VideoRegistrationResult(false, 1, string.Empty, string.Empty);
                }
                else
                {
                    rResult = new VideoRegistrationResult(true, 1, vResult.Data.ChannelName, vResult.Data.ChannelID);
                }
            }
            //リモートプレイリスト
            else if (info.IsRemote)
            {
                IAttemptResult<Remote::RemotePlaylistInfo> result = await this._netVideos.GetRemotePlaylistAsync(info, onMessage);
                if (!result.IsSucceeded || result.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(result.Message);

                foreach (var video in result.Data.Videos)
                {
                    IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, video);
                    if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(vResult.Message);

                    if (playlist.PlaylistType != PlaylistType.Temporary)
                    {
                        playlist.AddVideo(vResult.Data);
                    }
                    else
                    {
                        videos.Add(vResult.Data);
                    }
                }

                rResult = new VideoRegistrationResult(false, result.Data.Videos.Count, string.Empty, string.Empty);
            }

            if (this._container.CurrentSelectedPlaylist?.ID == playlist.ID)
            {
                //一時プレイリストの場合は特殊処理
                if (playlist.PlaylistType == PlaylistType.Temporary) this._container.Videos.AddRange(videos);
            }

            if (rResult is null)
            {
                return AttemptResult<VideoRegistrationResult>.Succeeded(new VideoRegistrationResult(false, 0, string.Empty, string.Empty));
            }
            else
            {
                return AttemptResult<VideoRegistrationResult>.Succeeded(rResult);
            }


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

        public IAttemptResult<IReadOnlyList<IVideoInfo>> GetSelectedVideoFromCurrentPlaylist()
        {

            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult<IReadOnlyList<IVideoInfo>>.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }

            var videos = this._container.Videos.Where(v => v.IsSelected.Value).ToList().AsReadOnly();

            return AttemptResult<IReadOnlyList<IVideoInfo>>.Succeeded(videos);

        }


        public IAttemptResult RemoveVideosFromCurrentPlaylist(IReadOnlyList<IVideoInfo> videos)
        {
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoListManagerError.PlaylistIsNotSelected));
            }

            IPlaylistInfo playlist = this._container.CurrentSelectedPlaylist;

            foreach (var video in videos)
            {
                IAttemptResult deleteResult = this._videoStore.Delete(video.ID, playlist.ID);
                if (!deleteResult.IsSucceeded)
                {
                    return deleteResult;
                }

                IAttemptResult playlistResult = playlist.RemoveVideo(video);
                if (!playlistResult.IsSucceeded)
                {
                    return playlistResult;
                }
            }

            return AttemptResult.Succeeded();

        }

        #endregion
    }
}
