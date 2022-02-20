using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.SharedUtils;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Playlist
{
    public interface IVideoHandler
    {
        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult<int> AddVideo(IListVideoInfo video);

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult RemoveVideo(IListVideoInfo video);

        /// <summary>
        /// 動画を更新する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult Update(IListVideoInfo video);

        /// <summary>
        /// IDを指定して動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<IListVideoInfo> GetVideo(int id);

        /// <summary>
        /// すべての動画を取得する
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<IListVideoInfo>> GetAllVideos();

        /// <summary>
        /// 動画の存在を確認する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exist(int id);

    }

    public class VideoHandler : IVideoHandler
    {

        public VideoHandler(IVideoStoreHandler storeHandler,IVideoPlaylistConverter converter)
        {
            this._videoStoreHandler = storeHandler;
            this._converter = converter;
        }

        #region field

        private readonly IVideoStoreHandler _videoStoreHandler;

        private readonly IVideoPlaylistConverter _converter;

        #endregion

        public IAttemptResult<int> AddVideo(IListVideoInfo video)
        {
            STypes::Video converted = this._converter.ConvertLocalVideoToStoreVideo(video);

            IAttemptResult<int> result = this._videoStoreHandler.AddVideo(converted);

            return result;
        }

        public IAttemptResult RemoveVideo(IListVideoInfo video)
        {
            IAttemptResult result = this._videoStoreHandler.RemoveVideo(video.Id.Value);

            return result;
        }

        public IAttemptResult Update(IListVideoInfo video)
        {
            STypes::Video converted = this._converter.ConvertLocalVideoToStoreVideo(video);

            IAttemptResult result = this._videoStoreHandler.Update(converted);

            return result;
        }

        public IAttemptResult<IListVideoInfo> GetVideo(int id)
        {

            IAttemptResult<STypes::Video> sResult = this._videoStoreHandler.GetVideo(id);

            if (!sResult.IsSucceeded || sResult.Data is null)
            {
                return AttemptResult<IListVideoInfo>.Fail(sResult.Message);
            }

            IListVideoInfo converted = this._converter.ConvertStoreVideoToLocalVideo(sResult.Data);

            return AttemptResult<IListVideoInfo>.Succeeded(converted);
        }

        public IAttemptResult<IEnumerable<IListVideoInfo>> GetAllVideos()
        {
            IAttemptResult<List<STypes::Video>> sResult = this._videoStoreHandler.GetAllVideos();

            if (!sResult.IsSucceeded || sResult.Data is null)
            {
                return AttemptResult<IEnumerable<IListVideoInfo>>.Fail(sResult.Message);
            }

            IEnumerable<IListVideoInfo> converted = sResult.Data.Select(v => this._converter.ConvertStoreVideoToLocalVideo(v));

            return AttemptResult<IEnumerable<IListVideoInfo>>.Succeeded(converted);

        }

        public bool Exist(int id)
        {
            return this._videoStoreHandler.Exists(id);
        }

    }
}
