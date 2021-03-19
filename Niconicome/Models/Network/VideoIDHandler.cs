using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network
{
    interface IVideoIDHandler
    {
        Task<IEnumerable<IVideoListInfo>> GetVideoListInfosAsync(string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose);
        Task<INetworkResult> TryGetVideoListInfosAsync(List<IVideoListInfo> videos, string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose);
    }

    class VideoIDHandler : IVideoIDHandler
    {
        public VideoIDHandler(INetworkVideoHandler networkVideoHandler, INiconicoUtils niconicoUtils, ILocalDirectoryHandler localDirectoryHandler, IRemotePlaylistHandler remotePlaylistHandler)
        {
            this.networkVideoHandler = networkVideoHandler;
            this.niconicoUtils = niconicoUtils;
            this.localDirectoryHandler = localDirectoryHandler;
            this.remotePlaylistHandler = remotePlaylistHandler;
        }

        private readonly INetworkVideoHandler networkVideoHandler;

        private readonly INiconicoUtils niconicoUtils;

        private readonly ILocalDirectoryHandler localDirectoryHandler;

        private readonly IRemotePlaylistHandler remotePlaylistHandler;

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="registeredVideos"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IVideoListInfo>> GetVideoListInfosAsync(string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose)
        {
            inputText = inputText.Trim();
            var videos = new List<IVideoListInfo>();

            if (Path.IsPathRooted(inputText))
            {
                onMessage("ローカルディレクトリーから動画を取得します");
                var retlieved = await this.GetVideoListInfosFromLocalPath(inputText, (_, _, _) => { }, (_, _) => { }, (_, _) => { }, (_) => { });
                videos.AddRange(retlieved);
                return videos;
            }
            else if (inputText.StartsWith("http"))
            {
                var type = this.niconicoUtils.GetRemoteType(inputText);
                var id = this.niconicoUtils.GetID(inputText, type);

                if (type == RemoteType.WatchPage)
                {
                    inputText = id;
                }
                else
                {
                    onMessage("ネットワークから動画を取得します");
                    var retlieved = await this.GetVideoListInfosFromRemote(registeredVideos, type, id, (m) => onMessageVerbose(m));
                    videos.AddRange(retlieved);
                    return videos;
                }
            }

            onMessage("IDを登録します");
            var retlievedId = await this.GetVideoListInfosFromID(inputText, (_, _, _) => { }, (_, _) => { }, (_, _) => { }, (_) => { });
            videos.AddRange(retlievedId);
            return videos;
        }

        /// <summary>
        /// 安全に動画情報を取得する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="inputText"></param>
        /// <param name="registeredVideos"></param>
        /// <param name="onMessage"></param>
        /// <param name="onMessageVerbose"></param>
        /// <returns></returns>
        public async Task<INetworkResult> TryGetVideoListInfosAsync(List<IVideoListInfo> videos, string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose)
        {
            try
            {
                var retlieved = await this.GetVideoListInfosAsync(inputText, registeredVideos, onMessage, onMessageVerbose);
                videos.AddRange(retlieved);
            } catch(Exception e)
            {
                var result = new NetworkResult()
                {
                    IsFailed = true
                };

                return result;
            }

             return new NetworkResult();


        }

        /// <summary>
        /// ローカルフォルダーから動画情報を取得する
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="onFailed"></param>
        /// <param name="onStarted"></param>
        /// <param name="onWaiting"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IVideoListInfo>> GetVideoListInfosFromLocalPath(string localPath, Action<IVideoListInfo, IVideoListInfo, int> onSucceeded, Action<IResult, IVideoListInfo> onFailed, Action<IVideoListInfo, int> onStarted, Action<IVideoListInfo> onWaiting)
        {
            var ids = this.localDirectoryHandler.GetVideoIdsFromDirectory(localPath);

            var videos = await this.networkVideoHandler.GetVideoListInfosAsync(ids, onSucceeded, onFailed, onStarted, onWaiting);

            return videos;
        }

        /// <summary>
        /// IDから動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="onFailed"></param>
        /// <param name="onStarted"></param>
        /// <param name="onWaiting"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IVideoListInfo>> GetVideoListInfosFromID(string id, Action<IVideoListInfo, IVideoListInfo, int> onSucceeded, Action<IResult, IVideoListInfo> onFailed, Action<IVideoListInfo, int> onStarted, Action<IVideoListInfo> onWaiting)
        {
            var videos = await this.networkVideoHandler.GetVideoListInfosAsync(new List<string>() { id }, onSucceeded, onFailed, onStarted, onWaiting);

            return videos;
        }

        /// <summary>
        /// リモートプレイリストから取得する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="registeredVideos"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IVideoListInfo>> GetVideoListInfosFromRemote(IEnumerable<string> registeredVideos, RemoteType type, string id, Action<string> onMessage)
        {

            var videos = new List<IVideoListInfo>();

            await this.remotePlaylistHandler.TryGetRemotePlaylistAsync(id, videos, type, registeredVideos, onMessage);

            return videos;
        }
    }
}
