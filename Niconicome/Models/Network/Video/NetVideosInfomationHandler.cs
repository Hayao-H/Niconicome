using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video.Error;
using Niconicome.Models.Network.Video.StringContent;
using Niconicome.Models.Utils.ParallelTaskV2;
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
        Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(InputInfomation inputInfomation, Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// リモートプレイリストを取得する
        /// </summary>
        /// <param name="remoteType"></param>
        /// <param name="remoteParameter"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(RemoteType remoteType, string remoteParameter, Action<string, ErrorLevel> onMessage);


        /// <summary>
        /// 動画IDから動画を取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<Remote::VideoInfo>> GetVideoInfoAsync(string niconicoID, Action<string, ErrorLevel> onMessage);

        /// <summary>
        /// 複数の動画IDから動画情報を取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetVideoInfoAsync(IEnumerable<string> niconicoID, Action<string, ErrorLevel> onMessage, CancellationToken ct = default);

    }

    public class NetVideosInfomationHandler : INetVideosInfomationHandler
    {
        public NetVideosInfomationHandler(Remote::Mylist.IMylistHandler mylistHandler, Remote::Mylist.IWatchLaterHandler watchLaterHandler, Remote::Series.ISeriesHandler seriesHandler, Remote::Channel.IChannelVideoHandler channelVideoHandler, Remote::UserVideo.IUserVideoHandler userVideoHandler, Remote::Search.ISearch search, Watch::IWatchPageInfomationHandler watch, IErrorHandler errorHandler, IStringHandler stringHandler, ISettingsContainer settingsContainer)
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
            this._settingsContainer = settingsContainer;
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

        private readonly ISettingsContainer _settingsContainer;

        #endregion

        #region Method

        public async Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(InputInfomation inputInfomation, Action<string, ErrorLevel> onMessage)
        {
            if (!inputInfomation.IsRemote)
            {
                this._errorHandler.HandleError(NetVideosInfomationHandlerError.NotRemotePlaylist, inputInfomation.InputType.ToString());
                return AttemptResult<Remote::RemotePlaylistInfo>.Fail(this._errorHandler.GetMessageForResult(NetVideosInfomationHandlerError.NotRemotePlaylist, inputInfomation.InputType.ToString()));
            }

            return await this.GetRemotePlaylistAsync(inputInfomation.RemoteType, inputInfomation.Parameter, onMessage);
        }

        public async Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetRemotePlaylistAsync(RemoteType remoteType, string remoteParameter, Action<string, ErrorLevel> onMessage)
        {

            onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingRemotePlaylistHasStarted, remoteType, remoteParameter), ErrorLevel.Log);


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

            if (result.IsSucceeded)
            {
                onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingRemotePlaylistHasCompleted, remoteType, result.Data?.Videos.Count ?? 0, remoteParameter), ErrorLevel.Log);
            }
            else
            {
                onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingRemotePlaylistHasFailed, remoteType, remoteParameter), ErrorLevel.Error);
            }

            return result;
        }

        public async Task<IAttemptResult<Remote::VideoInfo>> GetVideoInfoAsync(string niconicoID, Action<string, ErrorLevel> onMessage)
        {
            onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingVideoHasStarted, niconicoID), ErrorLevel.Log);

            IAttemptResult<Info::IDomainVideoInfo> result = await this._watch.GetVideoInfoAsync(niconicoID);
            if (!result.IsSucceeded || result.Data is null)
            {
                onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingOfVideoHasFailed, niconicoID), ErrorLevel.Error);
                onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingOfVideoHasFailedDetail, result.Message ?? ""), ErrorLevel.Error);
                return AttemptResult<Remote::VideoInfo>.Fail(result.Message);
            }

            onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.RetrievingVideoHasCompleted, niconicoID, result.Data.Title), ErrorLevel.Log);

            return AttemptResult<Remote::VideoInfo>.Succeeded(new Remote.VideoInfo()
            {
                NiconicoID = niconicoID,
                Title = result.Data.Title,
                ThumbUrl = result.Data.DmcInfo.ThumbInfo.GetSpecifiedThumbnail(Info::ThumbSize.Large),
                OwnerName = result.Data.Owner,
                OwnerID = result.Data.OwnerID.ToString(),
                ViewCount = result.Data.ViewCount,
                CommentCount = result.Data.CommentCount,
                LikeCount = result.Data.LikeCount,
                MylistCount = result.Data.MylistCount,
                ChannelID = result.Data.ChannelID,
                Description = result.Data.Description,
                ChannelName = result.Data.ChannelName,
                AddedAt = DateTime.Now,
                Duration = result.Data.Duration,
                Tags = result.Data.Tags.Select(t => new Remote::Tag() { IsNicodicExist = t.IsNicodicExist, Name = t.Name }).ToList().AsReadOnly(),
            }); 
        }

        public async Task<IAttemptResult<Remote::RemotePlaylistInfo>> GetVideoInfoAsync(IEnumerable<string> source, Action<string, ErrorLevel> onMessage, CancellationToken ct = default)
        {
            if (source.Count() == 0)
            {
                return AttemptResult<Remote::RemotePlaylistInfo>.Fail(this._errorHandler.GetMessageForResult(NetVideosInfomationHandlerError.SourceIsEmpty));
            }

            IAttemptResult<ISettingInfo<int>> pResult = this._settingsContainer.GetSetting(SettingNames.MaxParallelFetchCount, NetConstant.DefaultMaxParallelFetchCount);
            IAttemptResult<ISettingInfo<int>> sleepResult = this._settingsContainer.GetSetting(SettingNames.FetchSleepInterval, NetConstant.DefaultFetchWaitInterval);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult<Remote::RemotePlaylistInfo>.Fail(pResult.Message);
            }

            if (!sleepResult.IsSucceeded || sleepResult.Data is null)
            {
                return AttemptResult<Remote::RemotePlaylistInfo>.Fail(sleepResult.Message);
            }

            var failed = new List<string>();
            var succeeded = new List<Remote::VideoInfo>();

            var handler = new ParallelTasksHandler<string>(pResult.Data.Value, sleepResult.Data.Value, 15);


            foreach (var id in source)
            {
                handler.AddTaskToQueue(new ParallelTask<string>(id, async (_, _) =>
                {
                    if (ct.IsCancellationRequested) return;

                    IAttemptResult<Remote::VideoInfo> result = await this.GetVideoInfoAsync(id, onMessage);

                    if (!result.IsSucceeded || result.Data is null)
                    {
                        failed.Add(id);
                        return;
                    }

                    succeeded.Add(result.Data);
                }, _ => onMessage(this._stringHandler.GetContent(NetVideosInfomationHandlerStringContent.FetchSleeping), ErrorLevel.Log)));
            }

            await handler.ProcessTasksAsync(ct: ct);

            return AttemptResult<Remote::RemotePlaylistInfo>.Succeeded(new Remote.RemotePlaylistInfo() { Videos = succeeded, FailedVideos = failed.ToImmutableList() });
        }



        #endregion
    }
}
