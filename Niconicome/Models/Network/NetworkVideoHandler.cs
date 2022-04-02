using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Niconicome.Models.Utils;
using DWatch = Niconicome.Models.Domain.Niconico.Watch;
using State = Niconicome.Models.Local.State;

namespace Niconicome.Models.Network
{
    public interface INetworkVideoHandler
    {
        /// <summary>
        /// IDから動画を取得する
        /// </summary>
        /// <param name="ids">ID</param>
        /// <param name="ct">CT</param>
        /// <returns>取得した動画</returns>
        Task<IAttemptResult<IListVideoInfo>> GetVideoListInfoAsync(string id, CancellationToken? ct = null);

        /// <summary>
        /// 複数のIDから動画を取得する
        /// </summary>
        /// <param name="ids">ID一覧</param>
        /// <param name="ct">CT</param>
        /// <param name="onSucceeded">成功時に実行する関数</param>
        /// <param name="ct"></param>
        /// <returns>取得した動画の一覧</returns>
        Task<IAttemptResult<IEnumerable<IListVideoInfo>>> GetVideoListInfosAsync(IEnumerable<string> ids, Action<string>? onSucceeded = null, CancellationToken? ct = null);

    }

    /// <summary>
    /// 動画情報の操作で、ネットワークに関する機能を提供する
    /// </summary>
    class NetworkVideoHandler : INetworkVideoHandler
    {

        #region field

        private readonly IWatch _wacthPagehandler;

        private readonly State::IMessageHandler _messageHandler;

        private readonly ILocalSettingHandler _settingHandler;

        private readonly IVideoInfoContainer _videoInfoContainer;

        #endregion

        public NetworkVideoHandler(IWatch watchPageHandler, State::IMessageHandler messageHandler, ILocalSettingHandler settingHandler, IVideoInfoContainer videoInfoContainer)
        {
            this._wacthPagehandler = watchPageHandler;
            this._messageHandler = messageHandler;
            this._settingHandler = settingHandler;
            this._videoInfoContainer = videoInfoContainer;
        }


        #region Methods

        public async Task<IAttemptResult<IEnumerable<IListVideoInfo>>> GetVideoListInfosAsync(IEnumerable<string> ids, Action<string>? onSucceeded = null, CancellationToken? ct = null)
        {

            var videos = new List<IListVideoInfo>();

            int videosCount = ids.Count();
            int maxParallelCount = this._settingHandler.GetIntSetting(SettingsEnum.MaxFetchCount);
            int waitInterval = this._settingHandler.GetIntSetting(SettingsEnum.FetchSleepInterval);

            if (maxParallelCount < 1) maxParallelCount = NetConstant.DefaultMaxParallelFetchCount;
            if (waitInterval < 1) waitInterval = NetConstant.DefaultFetchWaitInterval;

            var handler = new ParallelTasksHandler<NetworkVideoParallelTask>(maxParallelCount, waitInterval, 15);

            foreach (var id in ids)
            {

                var task = new NetworkVideoParallelTask(async (_, lockObj) =>
                {
                    IAttemptResult<IListVideoInfo> result = await this.GetVideoListInfoAsync(id, ct);
                    if (result.IsSucceeded && result.Data is not null)
                    {
                        videos.Add(result.Data);
                        onSucceeded?.Invoke(id);
                    }

                }, index => this._messageHandler.AppendMessage("待機中...(15s)"));

                handler.AddTaskToQueue(task);

            }

            await handler.ProcessTasksAsync(() => { }, () => this._messageHandler.AppendMessage("動画情報の取得処理が中断されました。"), ct ?? CancellationToken.None);


            return AttemptResult<IEnumerable<IListVideoInfo>>.Succeeded(videos);
        }

        public async Task<IAttemptResult<IListVideoInfo>> GetVideoListInfoAsync(string id, CancellationToken? ct = null)
        {

            IListVideoInfo video = this._videoInfoContainer.GetVideo(id);

            bool registerOnlyID = this._settingHandler.GetBoolSetting(SettingsEnum.StoreOnlyNiconicoID);
            if (registerOnlyID)
            {
                return AttemptResult<IListVideoInfo>.Succeeded(video);
            }



            this._messageHandler.AppendMessage($"{id}の取得を開始します。");

            IAttemptResult<IListVideoInfo> result = await this._wacthPagehandler.TryGetVideoInfoAsync(id);

            if (!result.IsSucceeded || result.Data is null)
            {
                this._messageHandler.AppendMessage($"{id}の取得に失敗しました。(詳細:{result.Message})");
                return AttemptResult<IListVideoInfo>.Fail($"{id}の取得に失敗しました。(詳細:{result.Message})");
            }
            else
            {
                this._messageHandler.AppendMessage($"{id}の取得に成功しました。");
                video.SetNewData(result.Data);
            }

            return AttemptResult<IListVideoInfo>.Succeeded(video);

        }

        #endregion
    }


    public class NetworkVideoParallelTask : IParallelTask<NetworkVideoParallelTask>
    {
        public NetworkVideoParallelTask(Func<NetworkVideoParallelTask, object, Task> taskFunction, Action<int> onwait)
        {
            this.TaskFunction = taskFunction;
            this.OnWait = onwait;
        }

        public int Index { get; set; }

        public Func<NetworkVideoParallelTask, object, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }
    }

}
