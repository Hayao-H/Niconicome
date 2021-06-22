using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Unity;

namespace Niconicome.Models.Network
{
    public interface IVideoThumnailUtility
    {
        void GetThumbAsync(IListVideoInfo video, Action onDone, bool overwrite = false);
        void GetFundamentalThumbsIfNotExist();
        string GetThumbFilePath(string niconicoId);
        bool IsValidThumbnailUrl(IListVideoInfo video);
        bool IsValidThumbnailPath(IListVideoInfo video);
        bool HasThumbnailCache(IListVideoInfo video);
    }

    public class VideoThumnailUtility : IVideoThumnailUtility
    {
        public VideoThumnailUtility(ICacheHandler cacheHandler)
        {
            this.cacheHandler = cacheHandler;
            this.thumbConfigs = new Queue<ThumbConfig>();
            this.niconicoIDsAndActions = new Dictionary<string, Action>();
            this.lockObj = new object();
        }

        //キャッシュハンドラ
        private readonly ICacheHandler cacheHandler;

        /// <summary>
        /// キャッシュのキュー
        /// </summary>
        private readonly Queue<ThumbConfig> thumbConfigs;

        /// <summary>
        /// IDのリスト
        /// </summary>
        private readonly Dictionary<string, Action> niconicoIDsAndActions;

        /// <summary>
        /// 非同期ロックオブジェクト
        /// </summary>
        private readonly object lockObj;

        private bool isFetching;


        /// <summary>
        /// サムネイルの相対パスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public string GetThumbFilePath(string niconicoId)
        {
            return this.cacheHandler.GetCachePath(niconicoId, CacheType.Thumbnail);
        }

        /// <summary>
        /// サムネイルのパスが正しいかどうかを確認する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public bool IsValidThumbnailPath(IListVideoInfo video)
        {

            string deletedVideoPath = this.GetThumbFilePath("0");

            return (!video.ThumbPath.Value.IsNullOrEmpty() && video.ThumbPath.Value != deletedVideoPath);
        }

        /// <summary>
        /// サムネイルURLの形式が正しいかどうかを確認する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public bool IsValidThumbnailUrl(IListVideoInfo video)
        {
            string deletedVideoUrl = Net.NiconicoDeletedVideothumb;
            return !(video.ThumbUrl.Value.IsNullOrEmpty() || video.ThumbUrl.Value == deletedVideoUrl);
        }

        /// <summary>
        /// サムネイルのパスをサーバーから取得して設定する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="fource"></param>
        /// <returns></returns>
        public void GetThumbAsync(IListVideoInfo video, Action onDone, bool overwrite = false)
        {
            if (!this.IsValidThumbnailUrl(video)) return;

            lock (this.lockObj)
            {
                if (this.niconicoIDsAndActions.ContainsKey(video.NiconicoId.Value))
                {
                    this.niconicoIDsAndActions[video.NiconicoId.Value] += onDone;
                }

                if (!this.niconicoIDsAndActions.Any(p => p.Key == video.NiconicoId.Value))
                {
                    this.thumbConfigs.Enqueue(new ThumbConfig() { NiconicoID = video.NiconicoId.Value, Url = video.ThumbUrl.Value, Overwrite = overwrite });
                    this.niconicoIDsAndActions.Add(video.NiconicoId.Value, onDone);
                }
            }

            if (!this.isFetching)
            {
                Task.Run(() => this.GetThumbAsync());
            }
        }


        /// <summary>
        /// 非同期でサムネイルのパスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GetThumbAsync()
        {

            if (this.isFetching) return;
            if (this.thumbConfigs.Count < 1) return;

            this.isFetching = true;
            ThumbConfig config;

            lock (this.lockObj)
            {
                config = this.thumbConfigs.Dequeue();
            }

            await this.cacheHandler.GetOrCreateCacheAsync(config.NiconicoID, CacheType.Thumbnail, config.Url, config.Overwrite);

            lock (this.lockObj)
            {
                this.niconicoIDsAndActions[config.NiconicoID]();
                this.niconicoIDsAndActions.Remove(config.NiconicoID);
            }

            if (this.thumbConfigs.Count > 0)
            {
                await Task.Delay(1000 * 1);
                this.isFetching = false;
                _ = this.GetThumbAsync();
            }
            else
            {
                this.isFetching = false;
            }
        }

        /// <summary>
        /// サムネイルキャッシュの存在を確認する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public bool HasThumbnailCache(IListVideoInfo video)
        {
            return this.cacheHandler.HasCache(video.NiconicoId.Value, CacheType.Thumbnail);
        }

        /// <summary>
        /// 基本的な画像を取得する
        /// </summary>
        public void GetFundamentalThumbsIfNotExist()
        {
            if (this.cacheHandler.HasCache("0", CacheType.Thumbnail)) return;

            lock (this.lockObj)
            {
                if (!this.niconicoIDsAndActions.Any(p => p.Key == "0"))
                {
                    this.thumbConfigs.Enqueue(new ThumbConfig() { NiconicoID = "0", Url = Net.NiconicoDeletedVideothumb, Overwrite = false });
                    this.niconicoIDsAndActions.Add("0", () => { });
                }
            }

            if (!this.isFetching)
            {
                Task.Run(() => this.GetThumbAsync());
            }
        }


        private class ThumbConfig
        {
            public bool Overwrite { get; set; }

            public string Url { get; set; } = string.Empty;

            public string NiconicoID { get; set; } = string.Empty;
        }

    }
}
