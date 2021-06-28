using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Search;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Windows.Graphics.Printing.PrintTicket;
using Remote = Niconicome.Models.Domain.Niconico.Remote;
using UVideo = Niconicome.Models.Domain.Niconico.Video;

namespace Niconicome.Models.Network
{

    public interface IRemotePlaylistHandler
    {
        Task<IAttemptResult<string>> TryGetRemotePlaylistAsync(string id, List<IListVideoInfo> videos, RemoteType remoteType, IEnumerable<string> registeredVideo, Action<string> onMessage);
        Task<IAttemptResult<IEnumerable<IListVideoInfo>>> TrySearchVideosAsync(ISearchQuery query);
    }

    public class RemotePlaylistHandler : IRemotePlaylistHandler
    {
        public RemotePlaylistHandler(Remote::Mylist.IMylistHandler mylistHandler, UVideo::IUserVideoHandler userHandler, Remote::Search.ISearch search, Remote::Mylist.IWatchLaterHandler watchLaterHandler, Remote::Channel.IChannelVideoHandler channelVideoHandler, INetworkVideoHandler networkVideoHandler, IDomainModelConverter converter, Remote::Series.ISeriesHandler seriesHandler,ILogger logger,IVideoInfoContainer videoInfoContainer)
        {
            this.mylistHandler = mylistHandler;
            this.userHandler = userHandler;
            this.searchClient = search;
            this.watchLaterHandler = watchLaterHandler;
            this.channelVideoHandler = channelVideoHandler;
            this.networkVideoHandler = networkVideoHandler;
            this.converter = converter;
            this.seriesHandler = seriesHandler;
            this.logger = logger;
            this.videoInfoContainer = videoInfoContainer;
        }

        #region DIされるコード

        private readonly Remote::Mylist.IMylistHandler mylistHandler;

        private readonly UVideo::IUserVideoHandler userHandler;

        private readonly Remote::Search.ISearch searchClient;

        private readonly Remote::Mylist.IWatchLaterHandler watchLaterHandler;

        private readonly Remote::Channel.IChannelVideoHandler channelVideoHandler;

        private readonly INetworkVideoHandler networkVideoHandler;

        private readonly IDomainModelConverter converter;

        private readonly Remote::Series.ISeriesHandler seriesHandler;

        private readonly ILogger logger;

        private readonly IVideoInfoContainer videoInfoContainer;
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
        public async Task<IAttemptResult<string>> TryGetRemotePlaylistAsync(string id, List<IListVideoInfo> videos, RemoteType remoteType, IEnumerable<string> registeredVideo, Action<string> onMessage)
        {
            var result = remoteType switch
            {
                RemoteType.Mylist => await this.TryGetMylistVideosAsync(id, videos),
                RemoteType.UserVideos => await this.TryGetUserVideosAsync(id, videos),
                RemoteType.WatchLater => await this.TryGetWatchLaterAsync(videos),
                RemoteType.Channel => await this.TryGetChannelVideosAsync(id, videos, registeredVideo, onMessage),
                RemoteType.Series => await this.TryGetSeriesAsync(id, videos),
                _ => new AttemptResult<string>(),
            };

            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error(result.Message ?? "リモートプレイリストの取得に失敗しました。", result.Exception);
                } else
                {
                    this.logger.Error(result.Message ?? "リモートプレイリストの取得に失敗しました。");
                }
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
            Remote::Search.ISearchResult searchResult;
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
                    IListVideoInfo lVIdeo = this.videoInfoContainer.GetVideo(v.Id);
                    this.converter.ConvertDomainVideoInfoToListVideoInfo(lVIdeo, v);
                    return lVIdeo;
                }),
            };
        }

        #region private

        /// <summary>
        /// マイリストの動画を非同期に取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> TryGetMylistVideosAsync(string id, List<IListVideoInfo> videos)
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
        private async Task<IAttemptResult<string>> TryGetUserVideosAsync(string id, List<IListVideoInfo> videos)
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
        /// 「あとで見る」を取得する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> TryGetWatchLaterAsync(List<IListVideoInfo> videos)
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
        private async Task<IAttemptResult<string>> TryGetChannelVideosAsync(string id, List<IListVideoInfo> videos, IEnumerable<string> registeredVideo, Action<string> onMessage)
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

        /// <summary>
        /// シリーズを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> TryGetSeriesAsync(string id, List<IListVideoInfo> videos)
        {
            IAttemptResult<Remote::RemotePlaylistInfo> remoteResult;

            try
            {
                remoteResult = await this.seriesHandler.GetSeries(id);
            }
            catch (Exception e)
            {
                return new AttemptResult<string>() { Message = $"シリーズの取得に失敗しました。(詳細:{e.Message})" };
            }

            if (!remoteResult.IsSucceeded || remoteResult.Data is null)
            {
                return new AttemptResult<string>() { Message = remoteResult.Message, Exception = remoteResult.Exception };
            }

            IEnumerable<IListVideoInfo> convertedVideos = remoteResult.Data.Videos.Select(v =>
            {
                var video = this.videoInfoContainer.GetVideo(v.ID);
                this.converter.ConvertRemoteVideoInfoToListVideoInfo(v, video);
                return video;
            });

            videos.AddRange(convertedVideos);

            return new AttemptResult<string>() { Data = remoteResult.Data.PlaylistName, IsSucceeded = true };
        }

        #endregion

    }

}
