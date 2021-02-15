using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
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
        Task<IXenoImportTaskResult> InportFromXeno(IXenoImportSettings settings, Action<string> onMessage,CancellationToken token);
    }

    /// <summary>
    /// VMから触るAPI
    /// </summary>
    public class XenoImportGeneralManager : IXenoImportGeneralManager
    {

        public XenoImportGeneralManager(Xeno::IXenoImportManager xenoImportManager, INetworkVideoHandler networkVideoHandler, IPlaylistVideoHandler playlistVideoHandler, IRemotePlaylistHandler remotePlaylistHandler)
        {
            this.importManager = xenoImportManager;
            this.networkVideoHandler = networkVideoHandler;
            this.playlistVideoHandler = playlistVideoHandler;
            this.remotePlaylistHandler = remotePlaylistHandler;

        }

        private readonly Xeno::IXenoImportManager importManager;

        private readonly INetworkVideoHandler networkVideoHandler;

        private readonly IPlaylistVideoHandler playlistVideoHandler;

        private readonly IRemotePlaylistHandler remotePlaylistHandler;

        /// <summary>
        /// Xenoからインポートする
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public async Task<IXenoImportTaskResult> InportFromXeno(IXenoImportSettings settings, Action<string> onMessage,CancellationToken token)
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
                    await this.AddPlaylistAsync(child, settings.TargetPlaylistId, taskResult, onMessage,token);
                }
            }
            else
            {
                await this.AddPlaylistAsync(inportResult.PlaylistInfo, settings.TargetPlaylistId, taskResult, onMessage,token);

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
        private async Task AddPlaylistAsync(ITreePlaylistInfo playlistInfo, int parentId, IXenoImportTaskResult result, Action<string> onMessage, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                onMessage("処理をキャンセルします。");
                return;
            }

            var id = this.playlistVideoHandler.AddPlaylist(parentId);
            var playlist = this.playlistVideoHandler.GetPlaylist(id);
            if (playlist is null) return;

            onMessage("-".Repeat(50));
            onMessage($"プレイリスト「{playlistInfo.Name}」の追加処理を開始します。");

            playlist.Name = playlistInfo.Name;
            if (playlistInfo.IsRemotePlaylist)
            {
                onMessage("待機中(10s)");
                await Task.Delay(10 * 1000,token);

                playlist.IsRemotePlaylist = true;
                playlist.RemoteType = RemoteType.Channel;
                playlist.RemoteId = playlistInfo.RemoteId;
                this.playlistVideoHandler.SetAsRemotePlaylist(id, playlistInfo.RemoteId, playlistInfo.RemoteType);

                var videos = new List<ITreeVideoInfo>();
                var rResult = await this.remotePlaylistHandler.TryGetChannelVideosAsync(playlistInfo.RemoteId, videos, m => onMessage(m));

                if (!rResult.IsFailed)
                {
                    await this.networkVideoHandler.AddVideosAsync(videos, id);
                    result.SucceededVideoCount += rResult.SucceededCount;
                    if (!rResult.IsSucceededAll)
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
                await this.networkVideoHandler.AddVideosAsync(playlistInfo.Videos.Select(v => v.NiconicoId), id, r =>
                {
                    result.FailedVideoCount++;
                    onMessage(r.Message);
                }, v =>
                {
                    result.SucceededVideoCount++;
                    onMessage($"{v.NiconicoId}の登録に成功しました。");
                });
            }

            this.playlistVideoHandler.Update(playlist);

            onMessage($"プレイリスト「{playlistInfo.Name}」を追加しました。");

            if (token.IsCancellationRequested)
            {
                onMessage("処理をキャンセルします。");
                return;
            }

            foreach (var child in playlistInfo.Children)
            {
                await this.AddPlaylistAsync(child, id, result, onMessage, token);
            }

        }
    }
}
