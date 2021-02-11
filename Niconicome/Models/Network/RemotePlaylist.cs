using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVideo = Niconicome.Models.Domain.Niconico.Video;
using Mylist = Niconicome.Models.Domain.Niconico.Mylist;
using Playlist = Niconicome.Models.Playlist;
using Search = Niconicome.Models.Domain.Niconico.Search;
using Channel = Niconicome.Models.Domain.Niconico.Video.Channel;
using Niconicome.Models.Domain.Niconico.Video.Channel;

namespace Niconicome.Models.Network
{

    public interface IRemotePlaylistHandler
    {
        Task<INetworkResult> TryGetMylistVideosAsync(string id, List<Playlist::ITreeVideoInfo> videos);
        Task<INetworkResult> TryGetUserVideosAsync(string id, List<Playlist::ITreeVideoInfo> videos);
        Task<INetworkResult> TryGetWatchLaterAsync(List<Playlist::ITreeVideoInfo> videos);
        Task<INetworkResult> TryGetChannelVideosAsync(string id,List<Playlist::ITreeVideoInfo> videos,Action<string> onMessage);
        Task<Search::ISearchResult> TrySearchVideosAsync(string keyword, Search::SearchType searchType,int page);
        string? ExceptionDetails { get; }
    }

    public class RemotePlaylistHandler : IRemotePlaylistHandler
    {
        public RemotePlaylistHandler(Mylist::IMylistHandler mylistHandler, UVideo::IUserVideoHandler userHandler,Search::ISearch search,Mylist::IWatchLaterHandler watchLaterHandler,Channel::IChannelVideoHandler channelVideoHandler)
        {
            this.mylistHandler = mylistHandler;
            this.userHandler = userHandler;
            this.searchClient = search;
            this.watchLaterHandler = watchLaterHandler;
            this.channelVideoHandler = channelVideoHandler;
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
        private readonly Channel::IChannelVideoHandler channelVideoHandler;

        /// <summary>
        /// 例外の詳細情報
        /// </summary>
        public string? ExceptionDetails { get; private set; }


        /// <summary>
        /// マイリストの動画を非同期に取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<INetworkResult> TryGetMylistVideosAsync(string id, List<Playlist::ITreeVideoInfo> videos)
        {
            var resultInfo = new NetworkResult();
            List<Playlist::ITreeVideoInfo> result;
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
        public async Task<INetworkResult> TryGetUserVideosAsync(string id, List<Playlist::ITreeVideoInfo> videos)
        {
            var resultInfo = new NetworkResult();
            List<Playlist::ITreeVideoInfo> result;

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
        public async Task<Search::ISearchResult> TrySearchVideosAsync(string keyword, Search::SearchType searchType,int page)
        {
            Search::ISearchResult result;
            try
            {
                result = await this.searchClient.SearchAsync(keyword, searchType,page);
            }catch
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
        public async Task<INetworkResult> TryGetWatchLaterAsync(List<Playlist::ITreeVideoInfo> videos)
        {
            var resultInfo = new NetworkResult();
            List<Playlist::ITreeVideoInfo> result;
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
        public async Task<INetworkResult> TryGetChannelVideosAsync(string id, List<Playlist::ITreeVideoInfo> videos, Action<string> onMessage)
        {
            var resultInfo = new NetworkResult();
            IChannelResult result;
            try
            {
                result = await this.channelVideoHandler.GetVideosAsync(id,onMessage);

            }
            catch
            {
                resultInfo.IsFailed = true;
                this.ExceptionDetails = this.channelVideoHandler.CurrentException?.Message;
                return resultInfo;
            }

            resultInfo.FailedCount = result.FailedCounts;
            resultInfo.IsSucceededAll = result.IsSucceededAll;
            resultInfo.SucceededCount = result.RetrievedVideos.Count;

            if (result.RetrievedVideos.Count == 0) return resultInfo;

            videos.AddRange(result.RetrievedVideos);
            return resultInfo;
        }



    }

}
