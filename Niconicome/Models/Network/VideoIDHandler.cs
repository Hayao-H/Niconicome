using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Niconico.Search;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Reactive.Bindings;
using Search = Niconicome.Models.Domain.Niconico.Search;

namespace Niconicome.Models.Network
{
    interface IVideoIDHandler
    {
        /// <summary>
        /// 入力した文字列から動画情報を取得する
        /// </summary>
        /// <param name="inputText">ソーステキスト</param>
        /// <param name="filterFUnction">情報を取得するかどうかを選択する関数</param>
        /// <param name="onMessage">メッセージハンドラー</param>
        /// <returns></returns>
        Task<IAttemptResult<IEnumerable<IListVideoInfo>>> GetVideoListInfosAsync(string inputText, Func<string, bool> filterFUnction, Action<string> onMessage);

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        ReactiveProperty<bool> IsProcessing { get; }
    }

    class VideoIDHandler : IVideoIDHandler
    {
        public VideoIDHandler(INetworkVideoHandler networkVideoHandler, INiconicoUtils niconicoUtils, ILocalDirectoryHandler localDirectoryHandler, IRemotePlaylistHandler remotePlaylistHandler, ILogger logger)
        {
            this._networkVideoHandler = networkVideoHandler;
            this._niconicoUtils = niconicoUtils;
            this._localDirectoryHandler = localDirectoryHandler;
            this.remotePlaylistHandler = remotePlaylistHandler;
            this._logger = logger;
        }

        #region field

        private readonly INetworkVideoHandler _networkVideoHandler;

        private readonly INiconicoUtils _niconicoUtils;

        private readonly ILocalDirectoryHandler _localDirectoryHandler;

        private readonly IRemotePlaylistHandler remotePlaylistHandler;

        private readonly ILogger _logger;

        #endregion


        #region Props

        public ReactiveProperty<bool> IsProcessing { get; init; } = new(false);

        #endregion

        #region Method

        public async Task<IAttemptResult<IEnumerable<IListVideoInfo>>> GetVideoListInfosAsync(string inputText, Func<string, bool> filterFUnction, Action<string> onMessage)
        {
            this.StartProcessing();

            inputText = inputText.Trim();
            var videos = new List<IListVideoInfo>();

            onMessage($"入力値：{inputText}");

            if (Path.IsPathRooted(inputText))
            {
                onMessage("ローカルディレクトリーから動画を取得します");
                try
                {
                    videos.AddRange(await this.GetVideoListInfosFromLocalPath(inputText));
                }
                catch (Exception e)
                {
                    this._logger.Error("ローカルディレクトリの探索に失敗しました。", e);
                    this.FinishProcessing();
                    return AttemptResult<IEnumerable<IListVideoInfo>>.Fail("ローカルディレクトリの探索に失敗しました。", e);
                }

            }
            else if (inputText.StartsWith("http"))
            {
                onMessage("ネットワークから動画を取得します。");

                try
                {
                    videos.AddRange(await this.GetVideoListInfoWithNetAsync(inputText, onMessage));
                }
                catch (Exception e)
                {
                    this._logger.Error("ネットワークからの動画取得に失敗しました。", e);
                    this.FinishProcessing();
                    return AttemptResult<IEnumerable<IListVideoInfo>>.Fail("ネットワークからの動画取得に失敗しました。", e);
                }
            }

            if (this._niconicoUtils.IsNiconicoID(inputText))
            {
                onMessage("IDを登録します");
                try
                {
                    videos.Add(await this.GetVideoListInfosFromID(inputText));
                }
                catch (Exception e)
                {
                    this._logger.Error("ネットワークからの動画取得に失敗しました。", e);
                    this.FinishProcessing();
                    return AttemptResult<IEnumerable<IListVideoInfo>>.Fail("ネットワークからの動画取得に失敗しました。", e);
                }
            }
            else
            {
                onMessage("動画を検索します");
                try
                {
                    videos.AddRange(await this.GetVideoListInfosFromSearchResult(inputText, onMessage));
                }
                catch (Exception e)
                {
                    this._logger.Error("動画の検索に失敗しました。", e);
                    this.FinishProcessing();
                    return AttemptResult<IEnumerable<IListVideoInfo>>.Fail("動画の検索に失敗しました。", e);
                }
            }

            this.FinishProcessing();
            return AttemptResult<IEnumerable<IListVideoInfo>>.Succeeded(videos);

        }

        #endregion

        #region private

        /// <summary>
        /// 処理を開始する
        /// </summary>
        private void StartProcessing()
        {
            this.IsProcessing.Value = true;
        }

        /// <summary>
        /// 処理を終了する
        /// </summary>
        private void FinishProcessing()
        {
            this.IsProcessing.Value = false;
        }

        /// <summary>
        /// ローカルフォルダーから動画情報を取得する
        /// </summary>
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosFromLocalPath(string localPath)
        {
            IEnumerable<string> ids = this._localDirectoryHandler.GetVideoIdsFromDirectory(localPath);

            IAttemptResult<IEnumerable<IListVideoInfo>> result = await this._networkVideoHandler.GetVideoListInfosAsync(ids);

            return result.Data!;
        }

        /// <summary>
        /// IDから動画を取得する
        /// </summary>
        private async Task<IListVideoInfo> GetVideoListInfosFromID(string id)
        {
            IAttemptResult<IListVideoInfo> result = await this._networkVideoHandler.GetVideoListInfoAsync(id);

            return result.Data!;
        }

        /// <summary>
        /// ネットワークから動画を取得する（検索＋リモート）
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfoWithNetAsync(string inputText, Action<string> onMessage)
        {

            if (this._niconicoUtils.IsSearchUrl(inputText))
            {
                ISearchQuery query = this._niconicoUtils.GetQueryFromUrl(inputText);

                return await this.GetVideoListInfosFromSearchResult(query, onMessage);
            }
            else
            {
                var type = this._niconicoUtils.GetRemoteType(inputText);
                var id = this._niconicoUtils.GetID(inputText, type);

                if (type == RemoteType.WatchPage)
                {
                    return new List<IListVideoInfo>() { await this.GetVideoListInfosFromID(id) };
                }
                else
                {
                    return await this.GetVideoListInfosFromRemote(type, id, onMessage);
                }
            }
        }

        /// <summary>
        /// リモートプレイリストから取得する
        /// </summary>
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosFromRemote(RemoteType type, string id, Action<string> onMessage)
        {

            var videos = new List<IListVideoInfo>();

            await this.remotePlaylistHandler.TryGetRemotePlaylistAsync(id, videos, type, onMessage);

            return videos;
        }

        /// <summary>
        /// 動画を検索する
        /// </summary>
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
