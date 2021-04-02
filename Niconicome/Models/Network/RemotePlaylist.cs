using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Domain.Niconico.Video.Channel;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Mylist = Niconicome.Models.Domain.Niconico.Mylist;
using Playlist = Niconicome.Models.Playlist;
using Search = Niconicome.Models.Domain.Niconico.Search;
using UVideo = Niconicome.Models.Domain.Niconico.Video;

namespace Niconicome.Models.Network
{

    public interface IRemotePlaylistHandler
    {
        Task<INetworkResult> TryGetRemotePlaylistAsync(string id, List<Playlist::IListVideoInfo> videos, RemoteType remoteType, IEnumerable<string> registeredVideo, Action<string> onMessage);
        Task<Search::ISearchResult> TrySearchVideosAsync(string keyword, Search::SearchType searchType, int page);
        string? ExceptionDetails { get; }
    }

    public class RemotePlaylistHandler : IRemotePlaylistHandler
    {
        public RemotePlaylistHandler(Mylist::IMylistHandler mylistHandler, UVideo::IUserVideoHandler userHandler, Search::ISearch search, Mylist::IWatchLaterHandler watchLaterHandler, IChannelVideoHandler channelVideoHandler, IMessageHandler messageHandler, INetworkVideoHandler networkVideoHandler)
        {
            this.mylistHandler = mylistHandler;
            this.userHandler = userHandler;
            this.searchClient = search;
            this.watchLaterHandler = watchLaterHandler;
            this.channelVideoHandler = channelVideoHandler;
            this.networkVideoHandler = networkVideoHandler;
        }

        /// <summary>
        /// マイリストのハンドラ
        /// </summary>
        private readonly Mylist::IMylistHandler mylistHandler;

        /// <summary>
        /// ユーザー投稿動画のハンドラ
        /// </summary>
        private readonly UVideo::IUserVideoHandler userHandler;

        /// <summary>
        /// 検索
        /// </summary>
        private readonly Search::ISearch searchClient;

        /// <summary>
        /// 「後で見る」のハンドラ
        /// </summary>
        private readonly Mylist::IWatchLaterHandler watchLaterHandler;

        /// <summary>
        /// チャンネル動画のハンドラ
        /// </summary>
        private readonly IChannelVideoHandler channelVideoHandler;

        /// <summary>
        /// ネットワークハンドラ
        /// </summary>
        private readonly INetworkVideoHandler networkVideoHandler;

        /// <summary>
        /// 例外の詳細情報
        /// </summary>
        public string? ExceptionDetails { get; private set; }

        /// <summary>
        /// 全てのタイプのリモートプレイリストをまとめて取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <param name="remoteType"></param>
        /// <param name="registeredVideo"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public async Task<INetworkResult> TryGetRemotePlaylistAsync(string id, List<Playlist::IListVideoInfo> videos, RemoteType remoteType, IEnumerable<string> registeredVideo, Action<string> onMessage)
        {
            var result = remoteType switch
            {
                RemoteType.Mylist => await this.TryGetMylistVideosAsync(id, videos),
                RemoteType.UserVideos => await this.TryGetUserVideosAsync(id, videos),
                RemoteType.WatchLater => await this.TryGetWatchLaterAsync(videos),
                RemoteType.Channel => await this.TryGetChannelVideosAsync(id, videos, registeredVideo, onMessage),
                _ => new NetworkResult()
            };

            return result;
        }

        /// <summary>
        /// マイリストの動画を非同期に取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<INetworkResult> TryGetMylistVideosAsync(string id, List<Playlist::IListVideoInfo> videos)
        {
            var resultInfo = new NetworkResult();
            List<Playlist::IListVideoInfo> result;
            try
            {
                result = await this.mylistHandler.GetVideosAsync(id);

            }
            catch
            {
                resultInfo.IsFailed = true;
                this.ExceptionDetails = this.mylistHandler.CurrentException?.Message;
                return resultInfo;
            }

            if (result.Count == 0) return resultInfo;

            videos.AddRange(result);
            resultInfo.IsSucceededAll = true;
            resultInfo.SucceededCount = videos.Count;
            return resultInfo;
        }

        /// <summary>
        /// ユーザーの投稿動画を非同期で取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<INetworkResult> TryGetUserVideosAsync(string id, List<Playlist::IListVideoInfo> videos)
        {
            var resultInfo = new NetworkResult();
            List<Playlist::IListVideoInfo> result;

            try
            {
                result = await this.userHandler.GetVideosAsync(id);

            }
            catch
            {
                resultInfo.IsFailed = true;
                this.ExceptionDetails = this.userHandler.CurrentException?.Message;
                return resultInfo;
            }
            if (result.Count == 0) return resultInfo;

            videos.AddRange(result);
            resultInfo.IsSucceededAll = true;
            resultInfo.SucceededCount = videos.Count;
            return resultInfo;
        }

        /// <summary>
        /// 動画を非同期で検索する
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public async Task<Search::ISearchResult> TrySearchVideosAsync(string keyword, Search::SearchType searchType, int page)
        {
            Search::ISearchResult result;
            try
            {
                result = await this.searchClient.SearchAsync(keyword, searchType, page);
            }
            catch
            {
                return new Search::SearchResult { Message = "不明なエラーが発生しました。" };
            }

            return result;
        }

        /// <summary>
        /// 「あとで見る」を取得する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<INetworkResult> TryGetWatchLaterAsync(List<Playlist::IListVideoInfo> videos)
        {
            var resultInfo = new NetworkResult();
            List<Playlist::IListVideoInfo> result;
            try
            {
                result = await this.watchLaterHandler.GetVideosAsync();

            }
            catch
            {
                resultInfo.IsFailed = true;
                this.ExceptionDetails = this.mylistHandler.CurrentException?.Message;
                return resultInfo;
            }

            if (result.Count == 0) return resultInfo;

            videos.AddRange(result);
            resultInfo.IsSucceededAll = true;
            resultInfo.SucceededCount = videos.Count;
            return resultInfo;
        }

        /// <summary>
        /// チャンネル動画を非同期で取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<INetworkResult> TryGetChannelVideosAsync(string id, List<Playlist::IListVideoInfo> videos, IEnumerable<string> registeredVideo, Action<string> onMessage)
        {
            var resultInfo = new NetworkResult();
            IEnumerable<string> ids;
            try
            {
                ids = await this.channelVideoHandler.GetVideosAsync(id, registeredVideo, m =>
                {
                    onMessage(m);
                });

            }
            catch
            {
                resultInfo.IsFailed = true;
                this.ExceptionDetails = this.channelVideoHandler.CurrentException?.Message;
                return resultInfo;
            }

            var dupeCount = ids.Where(id => registeredVideo.Contains(id)).Count();
            if (dupeCount > 0)
            {
                onMessage($"{dupeCount}件の動画が既に登録済みのためスキップされます。");
            }

            ids = ids.Where(id => !registeredVideo.Contains(id));
            var retlieved = await this.networkVideoHandler.GetVideoListInfosAsync(ids, (v, _, _, lockObj) =>
            {
                lock (lockObj)
                {
                    resultInfo.SucceededCount++;
                }
                onMessage($"{v.NiconicoId}の取得に成功しました。");
            },
            (result, v) =>
            {
                onMessage($"{v.NiconicoId}の取得に失敗しました。(詳細:{result.Message})");
            },
            (v, _) =>
            {
                onMessage($"{v.NiconicoId}の取得を開始します。");
            },
            _ =>
            {
                onMessage("-".Repeat(40));
                onMessage("待機中...(15s)");
                onMessage("-".Repeat(40));
            }
            );

            videos.AddRange(retlieved);

            resultInfo.FailedCount = ids.Count() - resultInfo.SucceededCount;

            return resultInfo;
        }



    }

}
