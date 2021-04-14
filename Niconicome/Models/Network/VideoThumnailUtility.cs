using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network
{
    public interface IVideoThumnailUtility
    {
        void GetThumbAsync(IListVideoInfo video, bool overwrite = false);
        string GetThumbFilePath(string niconicoId);
        bool IsValidThumbnail(IListVideoInfo video);
        bool HasThumbnailCache(IListVideoInfo video);
    }

    public class VideoThumnailUtility : IVideoThumnailUtility
    {
        public VideoThumnailUtility(ICacheHandler cacheHandler)
        {
            this.cacheHandler = cacheHandler;
            this.thumbConfigs = new Queue<ThumbConfig>();
            this.lockObj = new object();
        }

        //キャッシュハンドラ
        private readonly ICacheHandler cacheHandler;

        /// <summary>
        /// キャッシュのキュー
        /// </summary>
        private readonly Queue<ThumbConfig> thumbConfigs;

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
        /// サムネイルの形式が正しいかどうかを確認する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public bool IsValidThumbnail(IListVideoInfo video)
        {

            string deletedVideoPath = this.GetThumbFilePath("0");

            return (!video.ThumbPath.IsNullOrEmpty() && video.ThumbPath != deletedVideoPath && this.IsValidThumbnailUrl(video));
        }

        /// <summary>
        /// サムネイルURLの形式が正しいかどうかを確認する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private bool IsValidThumbnailUrl(IListVideoInfo video)
        {
            string deletedVideoUrl = VIdeoInfo.DeletedVideoThumb;
            return !(video.ThumbUrl.IsNullOrEmpty() || video.ThumbUrl == deletedVideoUrl);
        }

        /// <summary>
        /// サムネイルのパスをサーバーから取得して設定する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="fource"></param>
        /// <returns></returns>
        public void GetThumbAsync(IListVideoInfo video, bool overwrite = false)
        {
            if (this.IsValidThumbnail(video)) return;

            lock (this.lockObj)
            {
                if (!this.thumbConfigs.Any(c => c.NiconicoID == video.NiconicoId))
                {
                    this.thumbConfigs.Enqueue(new ThumbConfig() { NiconicoID = video.NiconicoId, Url = video.ThumbPath, Overwrite = overwrite });
                }
            }

            if (!this.isFetching)
            {
                _ = this.GetThumbAsync();
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

            if (this.thumbConfigs.Count > 0)
            {
                await Task.Delay(1000 * 3);
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
            return this.cacheHandler.HasCache(video.NiconicoId, CacheType.Thumbnail);
        }

        private class ThumbConfig
        {
            public bool Overwrite { get; set; }

            public string Url { get; set; } = string.Empty;

            public string NiconicoID { get; set; } = string.Empty;
        }

    }
}
