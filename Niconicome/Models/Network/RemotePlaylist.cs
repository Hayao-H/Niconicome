using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Domain.Niconico.Search;
using Niconicome.Models.Domain.Niconico.Video.Channel;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Mylist = Niconicome.Models.Domain.Niconico.Mylist;
using Playlist = Niconicome.Models.Playlist;
using Search = Niconicome.Models.Domain.Niconico.Search;
using UVideo = Niconicome.Models.Domain.Niconico.Video;

namespace Niconicome.Models.Network
{

    public interface IRemotePlaylistHandler
    {
        Task<IAttemptResult<string>> TryGetRemotePlaylistAsync(string id, List<Playlist::IListVideoInfo> videos, RemoteType remoteType, IEnumerable<string> registeredVideo, Action<string> onMessage);
        Task<IAttemptResult<IEnumerable<IListVideoInfo>>> TrySearchVideosAsync(ISearchQuery query);
    }

    public class RemotePlaylistHandler : IRemotePlaylistHandler
    {
        public RemotePlaylistHandler(Mylist::IMylistHandler mylistHandler, UVideo::IUserVideoHandler userHandler, Search::ISearch search, Mylist::IWatchLaterHandler watchLaterHandler, IChannelVideoHandler channelVideoHandler, INetworkVideoHandler networkVideoHandler, IDomainModelConverter converter)
        {
            this.mylistHandler = mylistHandler;
            this.userHandler = userHandler;
            this.searchClient = search;
            this.watchLaterHandler = watchLaterHandler;
            this.channelVideoHandler = channelVideoHandler;
            this.networkVideoHandler = networkVideoHandler;
            this.converter = converter;
        }

        #region DIされるコード
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
        /// 動画情報のコンバーター
        /// </summary>
        private readonly IDomainModelConverter converter;
        #endregion

        /// <summary>
        /// 全てのタイプのリモートプレイリストをまとめて取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <param name="remoteType"></param>
        /// <param name="registeredVideo"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> TryGetRemotePlaylistAsync(string id, List<Playlist::IListVideoInfo> videos, RemoteType remoteType, IEnumerable<string> registeredVideo, Action<string> onMessage)
        {
            var result = remoteType switch
            {
                RemoteType.Mylist => await this.TryGetMylistVideosAsync(id, videos),
                RemoteType.UserVideos => await this.TryGetUserVideosAsync(id, videos),
                RemoteType.WatchLater => await this.TryGetWatchLaterAsync(videos),
                RemoteType.Channel => await this.TryGetChannelVideosAsync(id, videos, registeredVideo, onMessage),
                _ => new AttemptResult<string>(),
            };

            return result;
        }

        /// <summary>
        /// マイリストの動画を非同期に取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> TryGetMylistVideosAsync(string id, List<Playlist::IListVideoInfo> videos)
        {
            IAttemptResult<string> result;

            try
            {
                result = await this.mylistHandler.GetVideosAsync(id, videos);

            }
            catch (Exception e)
            {
                return new AttemptResult<string>() { Message = $"不明なエラーが発生しました。(詳細:{e.Message})" };
            }

            return result;
        }

        /// <summary>
        /// ユーザーの投稿動画を非同期で取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> TryGetUserVideosAsync(string id, List<Playlist::IListVideoInfo> videos)
        {
            IAttemptResult<string> result;

            try
            {
                result = await this.userHandler.GetVideosAsync(id, videos);

            }
            catch (Exception e)
            {
                return new AttemptResult<string> { Message = $"不明なエラーが発生しました。(詳細:{e.Message})", Exception = e };
            }

            return result;
        }

        /// <summary>
        /// 動画を非同期で検索する
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<IEnumerable<IListVideoInfo>>> TrySearchVideosAsync(ISearchQuery query)
        {
            Search::ISearchResult searchResult;
            try
            {
                searchResult = await this.searchClient.SearchAsync(query);
            }
            catch (Exception e)
            {
                return new AttemptResult<IEnumerable<IListVideoInfo>> { Message = $"不明なエラーが発生しました。(詳細:{e.Message})", Exception = e };
            }

            if (!searchResult.IsSucceeded)
            {
                return new AttemptResult<IEnumerable<IListVideoInfo>>
                {
                    Message = searchResult.Message,
                };
            }

            return new AttemptResult<IEnumerable<IListVideoInfo>>()
            {
                IsSucceeded = true,
                Data = searchResult.Videos?.Select(v =>
                {
                    var lVIdeo = new NonBindableListVideoInfo();
                    this.converter.ConvertDomainVideoInfoToListVideoInfo(lVIdeo, v);
                    return lVIdeo;
                }),
            };
        }

        /// <summary>
        /// 「あとで見る」を取得する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> TryGetWatchLaterAsync(List<Playlist::IListVideoInfo> videos)
        {
            IAttemptResult<string> result;

            try
            {
                result = await this.watchLaterHandler.GetVideosAsync(videos);

            }
            catch (Exception e)
            {
                return new AttemptResult<string>() { Message = $"不明なエラーが発生しました。(詳細:{e.Message})" };
            }

            return result;
        }

        /// <summary>
        /// チャンネル動画を非同期で取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> TryGetChannelVideosAsync(string id, List<Playlist::IListVideoInfo> videos, IEnumerable<string> registeredVideo, Action<string> onMessage)
        {

            var ids = new List<string>();
            IAttemptResult<string> result;

            try
            {
                result = await this.channelVideoHandler.GetVideosAsync(id, ids, registeredVideo, m =>
                 {
                     onMessage(m);
                 });

            }
            catch (Exception e)
            {
                return new AttemptResult<string>() { Message = $"不明なエラーが発生しました。(詳細:{e.Message})" };
            }

            if (!result.IsSucceeded) return result;

            var dupeCount = ids.Where(id => registeredVideo.Contains(id)).Count();
            if (dupeCount > 0)
            {
                onMessage($"{dupeCount}件の動画が既に登録済みのためスキップされます。");
            }

            ids = ids.Where(id => !registeredVideo.Contains(id)).ToList();
            var retlieved = await this.networkVideoHandler.GetVideoListInfosAsync(ids);

            videos.AddRange(retlieved);

            return new AttemptResult<string>() { IsSucceeded = true, Data = result.Data };
        }



    }

}
