using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Playlist;
using Network = Niconicome.Models.Network;

namespace Niconicome.Models.Network
{
    public interface IVideoThumnailUtility
    {
        Task GetAndSetThumbFilePathAsync(IListVideoInfo video, bool fource = false);
        Task SetThumbPathAsync(IListVideoInfo video, bool overwrite = false);
        Task<string> GetThumbPathAsync(string niconicoId, string url, bool overwrite = false);
        string GetThumbFilePath(string niconicoId);
        bool IsValidThumbnail(IListVideoInfo video);
        bool HasThumbnailCache(IListVideoInfo video);
    }

    public class VideoThumnailUtility:IVideoThumnailUtility
    {
        public VideoThumnailUtility(ICacheHandler cacheHandler,IWatch watchInfohandler)
        {
            this.cacheHandler = cacheHandler;
            this.watchInfohandler = watchInfohandler;
        }

        //キャッシュハンドラ
        private readonly ICacheHandler cacheHandler;

        /// <summary>
        /// 動画情報取得
        /// </summary>
        private readonly IWatch watchInfohandler;


        /// <summary>
        /// 非同期でサムネイルのパスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetThumbPathAsync(string niconicoId, string url, bool overwrite = false)
        {
            string path = await this.cacheHandler.GetOrCreateCacheAsync(niconicoId, CacheType.Thumbnail, url, overwrite);
            return path;
        }

        /// <summary>
        /// サムネイルのパスを設定する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public async Task SetThumbPathAsync(IListVideoInfo video, bool overwrite = false)
        {
            if (video.ThumbUrl == string.Empty || video.NiconicoId == string.Empty) return;

            string path = await this.GetThumbPathAsync(video.NiconicoId, video.ThumbUrl, overwrite);
            video.ThumbPath = path;

        }

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
        public async Task GetAndSetThumbFilePathAsync(IListVideoInfo video, bool fource = false)
        {
            if (this.IsValidThumbnail(video) && !fource) return;


            if (fource || !this.IsValidThumbnailUrl(video))
            {
                var info = new Network::VIdeoInfo();
                var result = await this.watchInfohandler.TryGetVideoInfoAsync(video.NiconicoId, info);
                if (result.IsSucceeded)
                {
                    var newVideo = info.ConvertToTreeVideoInfo();
                    video.ThumbUrl = newVideo.ThumbUrl;
                    video.LargeThumbUrl = newVideo.LargeThumbUrl;
                }
                else
                {
                    return;
                }
            }

            await this.SetThumbPathAsync(video, true);
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

    }
}
