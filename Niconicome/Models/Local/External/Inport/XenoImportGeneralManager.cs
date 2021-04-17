using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Network;
using Niconicome.Models.Playlist;
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

    public interface IXenoImportGeneralManager
    {
        Task<IXenoImportTaskResult> InportFromXeno(IXenoImportSettings settings, Action<string> onMessage, Action<string> onMessageVerbose, CancellationToken token);
    }

    /// <summary>
    /// VMから触るAPI
    /// </summary>
    public class XenoImportGeneralManager : IXenoImportGeneralManager
    {

        public XenoImportGeneralManager(Xeno::IXenoImportManager xenoImportManager, INetworkVideoHandler networkVideoHandler, IPlaylistHandler playlistVideoHandler, IRemotePlaylistHandler remotePlaylistHandler, ILocalSettingHandler settingHandler, IVideoHandler videoHandler)
        {
            this.importManager = xenoImportManager;
            this.networkVideoHandler = networkVideoHandler;
            this.playlistHandler = playlistVideoHandler;
            this.remotePlaylistHandler = remotePlaylistHandler;
            this.videoHandler = videoHandler;
            this.settingHandler = settingHandler;

        }

        private readonly Xeno::IXenoImportManager importManager;

        private readonly INetworkVideoHandler networkVideoHandler;

        private readonly IPlaylistHandler playlistHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IRemotePlaylistHandler remotePlaylistHandler;

        private readonly ILocalSettingHandler settingHandler;

        /// <summary>
        /// Xenoからインポートする
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public async Task<IXenoImportTaskResult> InportFromXeno(IXenoImportSettings settings, Action<string> onMessage, Action<string> onMessageVerbose, CancellationToken token)
        {
            var taskResult = new XenoImportTaskResult();
            var result = this.importManager.TryImportData(settings.DataFilePath, out Xeno.IXenoImportResult? inportResult);

            if (!result || inportResult is null) return taskResult;

            taskResult.FailedPlaylistCount = inportResult.FailedCount;

            onMessage($"{inportResult.SucceededCount}件のプレイリストデータを移行します。");
            taskResult.SucceededPaylistCount += inportResult.SucceededCount;
            taskResult.FailedPlaylistCount += inportResult.FailedCount;

            if (settings.AddDirectly)
            {
                foreach (var child in inportResult.PlaylistInfo.Children)
                {
                    await this.AddPlaylistAsync(child, settings.TargetPlaylistId, taskResult, onMessage, onMessageVerbose, token);
                }
            }
            else
            {
                await this.AddPlaylistAsync(inportResult.PlaylistInfo, settings.TargetPlaylistId, taskResult, onMessage, onMessageVerbose, token);

            }

            return taskResult;
        }

        /// <summary>
        /// 非同期にプレイリスト・動画を保存する
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <param name="parentId"></param>
        /// <param name="result"></param>
        /// <param name="onMessage"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        private async Task AddPlaylistAsync(ITreePlaylistInfo playlistInfo, int parentId, IXenoImportTaskResult result, Action<string> onMessage, Action<string> onMessageVerbose, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                onMessage("処理をキャンセルします。");
                return;
            }

            var id = this.playlistHandler.AddPlaylist(parentId);
            var playlist = this.playlistHandler.GetPlaylist(id);
            if (playlist is null) return;

            onMessage("-".Repeat(50));
            onMessage($"プレイリスト「{playlistInfo.Name}」の追加処理を開始します。");

            playlist.Name = playlistInfo.Name;
            if (playlistInfo.IsRemotePlaylist)
            {
                var registerOnlyID = this.settingHandler.GetBoolSetting(Settings.StoreOnlyNiconicoID);
                if (!registerOnlyID)
                {
                    onMessage("待機中(10s)");
                    await Task.Delay(10 * 1000, token);
                }

                playlist.IsRemotePlaylist = true;
                playlist.RemoteType = RemoteType.Channel;
                playlist.RemoteId = playlistInfo.RemoteId;
                this.playlistHandler.SetAsRemotePlaylist(id, playlistInfo.RemoteId, playlistInfo.RemoteType);

                var videos = new List<IListVideoInfo>();
                var rResult = await this.remotePlaylistHandler.TryGetRemotePlaylistAsync(playlistInfo.RemoteId, videos, RemoteType.Channel, new List<string>(), m => onMessageVerbose(m));

                if (!rResult.IsFailed)
                {
                    foreach (var video in videos)
                    {
                        this.videoHandler.AddVideo(video, id);
                    }
                    result.SucceededVideoCount += rResult.SucceededCount;
                    if (registerOnlyID)
                    {
                        result.SucceededVideoCount += rResult.FailedCount;
                    }
                    else if (!rResult.IsSucceededAll)
                    {
                        onMessage($"{rResult.FailedCount}件の動画の取得に失敗しました。");
                        result.FailedVideoCount += rResult.FailedCount;
                    }
                }
                else
                {
                    onMessage($"チャンネル「{playlistInfo.RemoteId}」の取得に失敗しました。");
                }
            }
            else
            {
                var videos = (await this.networkVideoHandler.GetVideoListInfosAsync(playlistInfo.Videos.Select(v => v.NiconicoId))).ToList();
                if (playlistInfo.Videos.Count > videos.Count)
                {
                    result.FailedVideoCount += playlistInfo.Videos.Count - videos.Count;
                }
                result.SucceededVideoCount += videos.Count;
            }

            this.playlistHandler.Update(playlist);

            onMessage($"プレイリスト「{playlistInfo.Name}」を追加しました。");

            if (token.IsCancellationRequested)
            {
                onMessage("処理をキャンセルします。");
                return;
            }

            foreach (var child in playlistInfo.Children)
            {
                await this.AddPlaylistAsync(child, id, result, onMessage, onMessageVerbose, token);
            }

        }
    }
}
