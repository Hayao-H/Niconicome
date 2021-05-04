using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Niconico.Search;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;
using Reactive.Bindings;
using Search = Niconicome.Models.Domain.Niconico.Search;

namespace Niconicome.Models.Network
{
    interface IVideoIDHandler
    {
        Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose);
        Task<INetworkResult> TryGetVideoListInfosAsync(List<IListVideoInfo> videos, string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose);
        ReactiveProperty<bool> IsProcessing { get; }
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
        /// 処理中フラグ
        /// </summary>
        public ReactiveProperty<bool> IsProcessing { get; init; } = new(false);

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="registeredVideos"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose)
        {
            this.IsProcessing.Value = true;

            inputText = inputText.Trim();
            var videos = new List<IListVideoInfo>();

            if (Path.IsPathRooted(inputText))
            {
                onMessage("ローカルディレクトリーから動画を取得します");
                var retlieved = await this.GetVideoListInfosFromLocalPath(inputText);
                videos.AddRange(retlieved);
                this.IsProcessing.Value = false;
                return videos;
            }
            else if (inputText.StartsWith("http"))
            {
                if (this.niconicoUtils.IsSearchUrl(inputText))
                {
                    ISearchQuery query;

                    try
                    {
                        query = this.niconicoUtils.GetQueryFromUrl(inputText);
                    }
                    catch (Exception e)
                    {
                        onMessage("検索URLの解析に失敗しました");
                        onMessageVerbose($"検索URLの解析に失敗しました(詳細:{e.Message})");
                        this.IsProcessing.Value = false;
                        return videos;
                    }

                    var retlieved = await this.GetVideoListInfosFromSearchResult(query, onMessageVerbose);
                    videos.AddRange(retlieved);
                    this.IsProcessing.Value = false;
                    return videos;
                }
                else
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
                        this.IsProcessing.Value = false;
                        return videos;
                    }
                }

            }

            if (this.niconicoUtils.IsNiconicoID(inputText))
            {
                onMessage("IDを登録します");
                var retlievedId = await this.GetVideoListInfosFromID(inputText);
                videos.AddRange(retlievedId);
                this.IsProcessing.Value = false;
                return videos;
            }
            else
            {
                onMessage("動画を検索します");
                var retlievedId = await this.GetVideoListInfosFromSearchResult(inputText, onMessageVerbose);
                videos.AddRange(retlievedId);
                this.IsProcessing.Value = false;
                return videos;
            }

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
        public async Task<INetworkResult> TryGetVideoListInfosAsync(List<IListVideoInfo> videos, string inputText, IEnumerable<string> registeredVideos, Action<string> onMessage, Action<string> onMessageVerbose)
        {
            try
            {
                var retlieved = await this.GetVideoListInfosAsync(inputText, registeredVideos, onMessage, onMessageVerbose);
                videos.AddRange(retlieved);
            }
            catch
            {
                var result = new NetworkResult()
                {
                    IsFailed = true
                };

                return result;
            }

            return new NetworkResult();


        }

        #region 内部処理
        /// <summary>
        /// ローカルフォルダーから動画情報を取得する
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="onFailed"></param>
        /// <param name="onStarted"></param>
        /// <param name="onWaiting"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosFromLocalPath(string localPath)
        {
            var ids = this.localDirectoryHandler.GetVideoIdsFromDirectory(localPath);

            var videos = await this.networkVideoHandler.GetVideoListInfosAsync(ids);

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
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosFromID(string id)
        {
            var videos = await this.networkVideoHandler.GetVideoListInfosAsync(new List<string>() { id });

            return videos;
        }

        /// <summary>
        /// リモートプレイリストから取得する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="registeredVideos"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosFromRemote(IEnumerable<string> registeredVideos, RemoteType type, string id, Action<string> onMessage)
        {

            var videos = new List<IListVideoInfo>();

            await this.remotePlaylistHandler.TryGetRemotePlaylistAsync(id, videos, type, registeredVideos, onMessage);

            return videos;
        }

        /// <summary>
        /// 動画を検索する
        /// </summary>
        /// <param name="registeredVideos"></param>
        /// <param name="keyword"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="onFailed"></param>
        /// <param name="onStarted"></param>
        /// <param name="onWaiting"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosFromSearchResult(string keyword, Action<string> onMessage)
        {
            var query = new Search::SearchQuery()
            {
                Query = keyword,
                SearchType = SearchType.Keyword,
                Page = 1,
            };

            return await this.GetVideoListInfosFromSearchResult(query, onMessage);

        }

        /// <summary>
        /// 動画を検索する
        /// </summary>
        /// <param name="registeredVideos"></param>
        /// <param name="keyword"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="onFailed"></param>
        /// <param name="onStarted"></param>
        /// <param name="onWaiting"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosFromSearchResult(ISearchQuery query, Action<string> onMessage)
        {
            var searchResult = await this.remotePlaylistHandler.TrySearchVideosAsync(query);

            if (!searchResult.IsSucceeded || searchResult.Data is null)
            {
                onMessage($"検索に失敗しました(詳細: {searchResult.Message ?? "none"})");
                return new List<IListVideoInfo>();
            }

            return searchResult.Data;
        }
        #endregion
    }
}
