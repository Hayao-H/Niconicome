using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.ClearScript;
using MS.WindowsAPICodePack.Internal;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Niconicome.Models.Playlist.V2.Manager.StringContent;
using Niconicome.Models.Playlist.V2.Migration;
using Windows.Media.Ocr;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;
using Utils = Niconicome.Models.Domain.Utils;

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
        /// ニコニコのIDが含まれたテキストから登録
        /// </summary>
        /// <param name="content"></param>
        /// <param name="playlist"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<VideoRegistrationResult>> RegisterVideoFromTextAsync(string content, IPlaylistInfo playlist, Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        IAttemptResult RegisterVideos(IEnumerable<Remote::VideoInfo> videos, IPlaylistInfo playlist);

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
        public VideoListCRDHandler(IPlaylistVideoContainer container, IErrorHandler errorHandler, IVideoStore videoStore, INetVideosInfomationHandler netVideos, IInputTextParser inputTextParser, ITagStore tagStore, ILocalDirectoryHandler directoryHandler, IStringHandler stringHandler, ISettingsContainer settingsContainer, Utils::INiconicoUtils utils) : base(videoStore, tagStore)
        {
            this._container = container;
            this._errorHandler = errorHandler;
            this._netVideos = netVideos;
            this._inputTextParser = inputTextParser;
            this._directoryHandler = directoryHandler;
            this._stringHandler = stringHandler;
            this._settingsContainer = settingsContainer;
            this._utils = utils;
        }

        #region field

        private readonly IPlaylistVideoContainer _container;

        private readonly IErrorHandler _errorHandler;

        private readonly INetVideosInfomationHandler _netVideos;

        private readonly IInputTextParser _inputTextParser;

        private readonly ILocalDirectoryHandler _directoryHandler;

        private readonly IStringHandler _stringHandler;

        private readonly ISettingsContainer _settingsContainer;

        private readonly Utils::INiconicoUtils _utils;

        #endregion

        #region Method

        public async Task<IAttemptResult<VideoRegistrationResult>> RegisterVideoAsync(string inputText, IPlaylistInfo playlist, Action<string, ErrorLevel> onMessage)
        {
            InputInfomation info = this._inputTextParser.GetInputInfomation(inputText);
            VideoRegistrationResult? rResult = null;

            //ニコニコのID
            if (info.InputType == InputType.NiconicoID)
            {
                IAttemptResult<Remote::VideoInfo> result = await this._netVideos.GetVideoInfoAsync(info.Parameter, onMessage);
                if (!result.IsSucceeded || result.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(result.Message);

                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, result.Data);
                if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(vResult.Message);

                this.AddVideoToPlaylist(vResult.Data, playlist);

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

                    this.AddVideoToPlaylist(vResult.Data, playlist);
                }

                rResult = new VideoRegistrationResult(false, result.Data.Videos.Count, string.Empty, string.Empty);
            }
            else if (info.InputType == InputType.LocalVideo)
            {
                IAttemptResult<IImmutableList<IVideoInfo>> result = await this.GetVIdeosFromLocalDirectory(info.Parameter, playlist.ID, onMessage);
                if (!result.IsSucceeded || result.Data is null)
                {
                    return AttemptResult<VideoRegistrationResult>.Fail(result.Message);
                }

                this.AddVideoToPlaylist(result.Data, playlist);
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

        public IAttemptResult RegisterVideos(IEnumerable<Remote::VideoInfo> videos, IPlaylistInfo playlist)
        {

            foreach (var video in videos)
            {
                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, video);
                if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(vResult.Message);

                this.AddVideoToPlaylist(vResult.Data, playlist);
            }

            return AttemptResult.Succeeded();

        }

        public async Task<IAttemptResult<VideoRegistrationResult>> RegisterVideoFromTextAsync(string content, IPlaylistInfo playlist, Action<string, ErrorLevel> onMessage)
        {
            IEnumerable<string> ids = this._utils.GetNiconicoIdsFromText(content).Where(x => !string.IsNullOrEmpty(x));

            onMessage(this._stringHandler.GetContent(VideoListManagerString.VideosFoundFromText, ids.Count()), ErrorLevel.Log);

            IAttemptResult<ISettingInfo<bool>> settingResult = this._settingsContainer.GetSetting(SettingNames.StoreOnlyNiconicoIDOnRegister, false);
            if (!settingResult.IsSucceeded || settingResult.Data is null)
            {
                return AttemptResult<VideoRegistrationResult>.Fail(settingResult.Message);
            }

            var videos = new List<IVideoInfo>();


            if (settingResult.Data.Value)
            {
                foreach (var video in ids)
                {
                    if (!this._videoStore.Exist(video, playlist.ID))
                    {
                        IAttemptResult cResult = this._videoStore.Create(video, playlist.ID);
                        if (!cResult.IsSucceeded)
                        {
                            return AttemptResult<VideoRegistrationResult>.Fail(cResult.Message);
                        }
                    }

                    IAttemptResult<IVideoInfo> vResult = this._videoStore.GetVideo(video, playlist.ID);
                    if (!vResult.IsSucceeded || vResult.Data is null)
                    {
                        return AttemptResult<VideoRegistrationResult>.Fail(vResult.Message);
                    }

                    videos.Add(vResult.Data);
                }

                this.AddVideoToPlaylist(videos, playlist);

                return AttemptResult<VideoRegistrationResult>.Succeeded(new VideoRegistrationResult(false, videos.Count, string.Empty, string.Empty));
            }

            IAttemptResult<Remote::RemotePlaylistInfo> result = await this._netVideos.GetVideoInfoAsync(ids, onMessage);
            if (!result.IsSucceeded || result.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(result.Message);

            foreach (var video in result.Data.Videos)
            {
                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlist.ID, video);
                if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult<VideoRegistrationResult>.Fail(vResult.Message);

                videos.Add(vResult.Data);
            }

            this.AddVideoToPlaylist(videos, playlist);

            return AttemptResult<VideoRegistrationResult>.Succeeded(new VideoRegistrationResult(false, videos.Count, string.Empty, string.Empty));
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

        #region private

        private async Task<IAttemptResult<IImmutableList<IVideoInfo>>> GetVIdeosFromLocalDirectory(string directoryPath, int playlistID, Action<string, ErrorLevel> onMessage)
        {

            IAttemptResult<IImmutableList<string>> idResult = this._directoryHandler.GetVideoIdsFromDirectory(directoryPath);
            if (!idResult.IsSucceeded || idResult.Data is null)
            {
                return AttemptResult<IImmutableList<IVideoInfo>>.Fail(idResult.Message);
            }

            onMessage(this._stringHandler.GetContent(VideoListManagerString.VideosFoundInLocalDirectory, idResult.Data.Count), ErrorLevel.Log);

            IAttemptResult<ISettingInfo<bool>> settingResult = this._settingsContainer.GetSetting(SettingNames.StoreOnlyNiconicoIDOnRegister, false);
            if (!settingResult.IsSucceeded || settingResult.Data is null)
            {
                return AttemptResult<IImmutableList<IVideoInfo>>.Fail(settingResult.Message);
            }

            var videos = new List<IVideoInfo>();


            if (settingResult.Data.Value)
            {
                foreach (var video in idResult.Data)
                {
                    if (!this._videoStore.Exist(video, playlistID))
                    {
                        IAttemptResult cResult = this._videoStore.Create(video, playlistID);
                        if (!cResult.IsSucceeded)
                        {
                            return AttemptResult<IImmutableList<IVideoInfo>>.Fail(cResult.Message);
                        }
                    }

                    IAttemptResult<IVideoInfo> vResult = this._videoStore.GetVideo(video, playlistID);
                    if (!vResult.IsSucceeded || vResult.Data is null)
                    {
                        return AttemptResult<IImmutableList<IVideoInfo>>.Fail(vResult.Message);
                    }

                    videos.Add(vResult.Data);
                }

                return AttemptResult<IImmutableList<IVideoInfo>>.Succeeded(videos.ToImmutableList());
            }

            IAttemptResult<Remote::RemotePlaylistInfo> result = await this._netVideos.GetVideoInfoAsync(idResult.Data, onMessage);
            if (!result.IsSucceeded || result.Data is null) return AttemptResult<IImmutableList<IVideoInfo>>.Fail(result.Message);

            foreach (var video in result.Data.Videos)
            {
                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(playlistID, video);
                if (!vResult.IsSucceeded || vResult.Data is null) return AttemptResult<IImmutableList<IVideoInfo>>.Fail(vResult.Message);

                videos.Add(vResult.Data);
            }


            return AttemptResult<IImmutableList<IVideoInfo>>.Succeeded(videos.ToImmutableList());
        }

        /// <summary>
        /// 複数の動画を登録
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="playlist"></param>
        private void AddVideoToPlaylist(IEnumerable<IVideoInfo> videos, IPlaylistInfo playlist)
        {
            foreach (var video in videos)
            {
                this.AddVideoToPlaylist(video, playlist);
            }
        }


        /// <summary>
        /// 動画を登録
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlist"></param>
        private void AddVideoToPlaylist(IVideoInfo video, IPlaylistInfo playlist)
        {
            if (playlist.Videos.Any(v => v.NiconicoId == video.NiconicoId))
            {
                return;
            }

            playlist.AddVideo(video);
        }

        #endregion
    }
}
