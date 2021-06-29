using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Reactive.Bindings;
using Xeno = Niconicome.Models.Domain.Local.External.Import.Xeno;

namespace Niconicome.Models.Local.External.Import
{
    public interface IXenoImportTaskResult
    {
        int FailedPlaylistCount { get; set; }
        int FailedVideoCount { get; set; }
        bool IsSucceeded { get; set; }
        int SucceededPaylistCount { get; set; }
        int SucceededVideoCount { get; set; }
    }

    public interface IXenoImportSettings
    {
        bool AddDirectly { get; }
        int TargetPlaylistId { get; }
        string DataFilePath { get; }
    }

    public interface IXenoImportGeneralManager
    {
        Task<IXenoImportTaskResult> InportFromXeno(IXenoImportSettings settings);
        void Cancel();
        void AppendMessage(string message);
        ReactiveProperty<string> Message { get; }
        ReactiveProperty<bool> IsProcessing { get; }
    }

    /// <summary>
    /// インポート結果
    /// </summary>
    public class XenoImportTaskResult : IXenoImportTaskResult
    {
        public bool IsSucceeded { get; set; }

        public int SucceededPaylistCount { get; set; }

        public int FailedPlaylistCount { get; set; }

        public int SucceededVideoCount { get; set; }

        public int FailedVideoCount { get; set; }

    }

    /// <summary>
    /// インポート設定
    /// </summary>
    public class XenoImportSettings : IXenoImportSettings
    {
        public int TargetPlaylistId { get; set; }

        public bool AddDirectly { get; set; }

        public string DataFilePath { get; set; } = string.Empty;

    }

    /// <summary>
    /// VMから触るAPI
    /// </summary>
    public class XenoImportGeneralManager : IXenoImportGeneralManager
    {

        public XenoImportGeneralManager(Xeno::IXenoImportManager xenoImportManager, INetworkVideoHandler networkVideoHandler, IRemotePlaylistHandler remotePlaylistHandler, ILocalSettingHandler settingHandler, IVideoHandler videoHandler, IPlaylistStoreHandler playlistStoreHandler, IMessageHandler messageHandler)
        {
            this.importManager = xenoImportManager;
            this.networkVideoHandler = networkVideoHandler;
            this.remotePlaylistHandler = remotePlaylistHandler;
            this.videoHandler = videoHandler;
            this.settingHandler = settingHandler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.messageHandler = messageHandler;

            this.messageSB = new StringBuilder();
            this.IsProcessing = new ReactiveProperty<bool>();
            this.Message = new ReactiveProperty<string>();
            this.MessageVerbose = new ReactiveProperty<string>();

        }

        #region DIされるクラス
        private readonly Xeno::IXenoImportManager importManager;

        private readonly INetworkVideoHandler networkVideoHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IRemotePlaylistHandler remotePlaylistHandler;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly IMessageHandler messageHandler;
        #endregion

        #region field

        private CancellationTokenSource? cts;

        private readonly StringBuilder messageSB;

        private int fetchingIndex;
        #endregion


        /// <summary>
        /// 詳細メッセージ
        /// </summary>
        public ReactiveProperty<string> MessageVerbose { get; init; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public ReactiveProperty<string> Message { get; init; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        public ReactiveProperty<bool> IsProcessing { get; init; }

        /// <summary>
        /// Xenoからインポートする
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public async Task<IXenoImportTaskResult> InportFromXeno(IXenoImportSettings settings)
        {
            var taskResult = new XenoImportTaskResult();

            if (this.IsProcessing.Value)
            {
                return taskResult;
            }

            this.IsProcessing.Value = true;
            this.cts = new CancellationTokenSource();
            this.ClearMessage();
            this.fetchingIndex = 0;

            var result = this.importManager.TryImportData(settings.DataFilePath, out Xeno.IXenoImportResult? inportResult);

            if (!result || inportResult is null)
            {
                this.IsProcessing.Value = false;
                return taskResult;
            }

            taskResult.FailedPlaylistCount = inportResult.FailedCount;

            this.OnMessage($"{inportResult.SucceededCount}件のプレイリストデータを移行します。");
            taskResult.SucceededPaylistCount += inportResult.SucceededCount;
            taskResult.FailedPlaylistCount += inportResult.FailedCount;

            await Task.Run(async () =>
             {
                 if (settings.AddDirectly)
                 {
                     foreach (var child in inportResult.PlaylistInfo.Children)
                     {
                         await this.AddPlaylistAsync(child, settings.TargetPlaylistId, taskResult, this.cts.Token);
                     }
                 }
                 else
                 {
                     await this.AddPlaylistAsync(inportResult.PlaylistInfo, settings.TargetPlaylistId, taskResult, this.cts.Token);
                 }
             });

            this.IsProcessing.Value = false;
            return taskResult;
        }

        /// <summary>
        /// キャンセル処理
        /// </summary>
        public void Cancel()
        {
            this.cts?.Cancel();
            this.IsProcessing.Value = false;
            this.OnMessage("キャンセルされました。");
        }

        /// <summary>
        /// メッセージを追記
        /// </summary>
        /// <param name="message"></param>
        public void AppendMessage(string message)
        {
            this.OnMessage(message);
        }


        #region private
        /// <summary>
        /// 非同期にプレイリスト・動画を保存する
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <param name="parentId"></param>
        /// <param name="result"></param>
        /// <param name="onMessage"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        private async Task AddPlaylistAsync(ITreePlaylistInfo playlistInfo, int parentId, IXenoImportTaskResult result, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var id = this.playlistStoreHandler.AddPlaylist(parentId, playlistInfo.Name.Value);
            var playlist = this.playlistStoreHandler.GetPlaylist(id);
            var sleepInterval = this.settingHandler.GetIntSetting(SettingsEnum.FetchSleepInterval);

            if (playlist is null) return;

            this.OnMessage("-".Repeat(50));
            this.OnMessage($"プレイリスト「{playlistInfo.Name}」の追加処理を開始します。");

            if (playlistInfo.IsRemotePlaylist)
            {
                var registerOnlyID = this.settingHandler.GetBoolSetting(SettingsEnum.StoreOnlyNiconicoID);
                if ((this.fetchingIndex + 1) % sleepInterval == 0)
                {
                    this.OnMessage("待機中(10s)");
                    await Task.Delay(10 * 1000, token);
                }

                this.playlistStoreHandler.SetAsRemotePlaylist(id, playlistInfo.RemoteId, playlistInfo.RemoteType);

                var videos = new List<IListVideoInfo>();
                var rResult = await this.remotePlaylistHandler.TryGetRemotePlaylistAsync(playlistInfo.RemoteId, videos, RemoteType.Channel, new List<string>(), m => this.OnMessageVerbose(m));

                if (rResult.IsSucceeded)
                {
                    foreach (var video in videos)
                    {
                        this.playlistStoreHandler.AddVideo(video, id);
                    }
                    result.SucceededVideoCount += videos.Count;
                }
                else
                {
                    this.OnMessage($"チャンネル「{playlistInfo.RemoteId}」の取得に失敗しました。");
                }

                this.fetchingIndex++;
            }
            else
            {
                var videos = (await this.networkVideoHandler.GetVideoListInfosAsync(playlistInfo.Videos.Select(v => v.NiconicoId.Value))).ToList();
                foreach (var video in videos)
                {
                    this.playlistStoreHandler.AddVideo(video, id);
                }

                if (playlistInfo.Videos.Count > videos.Count)
                {
                    result.FailedVideoCount += playlistInfo.Videos.Count - videos.Count;
                }
                result.SucceededVideoCount += videos.Count;
            }

            this.OnMessage($"プレイリスト「{playlistInfo.Name}」を追加しました。");

            if (token.IsCancellationRequested)
            {
                this.OnMessage("処理をキャンセルします。");
                return;
            }


            foreach (var child in playlistInfo.Children)
            {
                await this.AddPlaylistAsync(child, id, result, token);
            }

        }

        /// <summary>
        /// 簡易メッセージ
        /// </summary>
        /// <param name="message"></param>
        private void OnMessage(string message)
        {
            this.messageSB.AppendLine(message);
            this.Message.Value = this.messageSB.ToString();
        }

        /// <summary>
        /// メッセージをクリア
        /// </summary>
        private void ClearMessage()
        {
            this.messageSB.Clear();
            this.Message.Value = this.messageSB.ToString();
        }

        /// <summary>
        /// メッセージ
        /// </summary>
        /// <param name="message"></param>
        private void OnMessageVerbose(string message)
        {

            this.messageHandler.AppendMessage(message);
        }
        #endregion
    }
}
