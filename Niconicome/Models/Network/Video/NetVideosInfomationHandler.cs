using System;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video.Error;
using Niconicome.Models.Network.Video.StringContent;
using Info = Niconicome.Models.Domain.Niconico.Video.Infomations;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;
using Watch = Niconicome.Models.Domain.Niconico.Watch.V2;

namespace Niconicome.Models.Network.Video
{
    public interface INetVideosInfomationHandler
    {
        /// <summary>
        /// リモートプレイリストを取得する
        /// </summary>
        /// <param name="inputInfomation"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(InputInfomation inputInfomation, Action<string> onMessage);

        /// <summary>
        /// リモートプレイリストを取得する
        /// </summary>
        /// <param name="remoteType"></param>
        /// <param name="remoteParameter"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(RemoteType remoteType, string remoteParameter, Action<string> onMessage);


        /// <summary>
        /// 動画IDから動画を取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<Remote::VideoInfo>> GetVideoInfoAsync(string niconicoID, Action<string> onMessage);
    }

    public class NetVideosInfomationHandler : INetVideosInfomationHandler
    {
        public NetVideosInfomationHandler(Remote::Mylist.IMylistHandler mylistHandler, Remote::Mylist.IWatchLaterHandler watchLaterHandler, Remote::Series.ISeriesHandler seriesHandler, Remote::Channel.IChannelVideoHandler channelVideoHandler, Remote::UserVideo.IUserVideoHandler userVideoHandler, Remote::Search.ISearch search, Watch::IWatchPageInfomationHandler watch, IErrorHandler errorHandler, IStringHandler stringHandler)
        {
            this._mylistHandler = mylistHandler;
            this._watchLaterHandler = watchLaterHandler;
            this._seriesHandler = seriesHandler;
            this._channelVideoHandler = channelVideoHandler;
            this._userVideoHandler = userVideoHandler;
            this._search = search;
            this._watch = watch;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly Remote::Mylist.IMylistHandler _mylistHandler;

        private readonly Remote::Mylist.IWatchLaterHandler _watchLaterHandler;

        private readonly Remote::Series.ISeriesHandler _seriesHandler;

        private readonly Remote::Channel.IChannelVideoHandler _channelVideoHandler;

        private readonly Remote::UserVideo.IUserVideoHandler _userVideoHandler;

        private readonly Remote::Search.ISearch _search;

        private readonly Watch::IWatchPageInfomationHandler _watch;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(InputInfomation inputInfomation, Action<string> onMessage)
        {
            if (!inputInfomation.IsRemote)
            {
                this._errorHandler.HandleError(NetVideosInfomationHandlerError.NotRemotePlaylist, inputInfomation.InputType.ToString());
                return AttemptResult<Remote::RemotePlaylistInfo>.Fail(this._errorHandler.GetMessageForResult(NetVideosInfomationHandlerError.NotRemotePlaylist, inputInfomation.InputType.ToString()));
            }

            return await this.GetRemotePlaylistAsync(inputInfomation.RemoteType, inputInfomation.Parameter, onMessage);
        }

        public async Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(RemoteType remoteType, string remoteParameter, Action<string> onMessage)
        {

            onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingRemotePlaylistHasStarted, remoteType, remoteParameter));


            IAttemptResult<Remote::RemotePlaylistInfo> result = remoteType switch
            {
                RemoteType.WatchLater => await this._watchLaterHandler.GetVideosAsync(),
                RemoteType.Mylist => await this._mylistHandler.GetVideosAsync(remoteParameter),
                RemoteType.WatchPage => await this._watchLaterHandler.GetVideosAsync(),
                RemoteType.Series => await this._seriesHandler.GetSeries(remoteParameter),
                RemoteType.Channel => await this._channelVideoHandler.GetVideosAsync(remoteParameter, onMessage),
                RemoteType.UserVideos => await this._userVideoHandler.GetVideosAsync(remoteParameter),
                _ => await this._search.SearchAsync(new Remote::Search.SearchQuery(Remote::Search.SearchType.Keyword, Remote::Search.Genre.All, new Remote::Search.SortOption(Remote::Search.Sort.ViewCount, false), remoteParameter))
            };

            onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingRemotePlaylistHasCompleted, remoteType, result.Data?.Videos.Count ?? 0, remoteParameter));

            return result;
        }

        public async Task<IAttemptResult<Remote::VideoInfo>> GetVideoInfoAsync(string niconicoID, Action<string> onMessage)
        {
            onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingVideoHasStarted, niconicoID));

            IAttemptResult<Info::IDomainVideoInfo> result = await this._watch.GetVideoInfoAsync(niconicoID);
            if (!result.IsSucceeded || result.Data is null) return AttemptResult<Remote::VideoInfo>.Fail(result.Message);

            onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingVideoHasCompleted, niconicoID, result.Data.Title));

            return AttemptResult<Remote::VideoInfo>.Succeeded(new Remote.VideoInfo()
            {
                NiconicoID = niconicoID,
                Title = result.Data.Title,
                ThumbUrl = result.Data.DmcInfo.ThumbInfo.GetSpecifiedThumbnail(Info::ThumbSize.Large),
                OwnerName = result.Data.Owner,
                OwnerID = result.Data.OwnerID.ToString(),
                ViewCount = result.Data.ViewCount,
                CommentCount = result.Data.CommentCount,
                MylistCount = result.Data.MylistCount,
            });
        }


        #endregion
    }
}
