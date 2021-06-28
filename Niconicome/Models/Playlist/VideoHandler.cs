using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Playlist.VideoList;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Playlist
{
    public interface IVideoHandler
    {
        void Update(IListVideoInfo video);
        bool Exist(int id);
        IEnumerable<IListVideoInfo> GetAllVideos();
        IListVideoInfo GetVideo(int id);

    }

    public class VideoHandler : IVideoHandler
    {

        public VideoHandler(IVideoStoreHandler storeHandler, IPlaylistStoreHandler playlistStoreHandler, IVideoInfoContainer videoInfoContainer)
        {
            this.videoStoreHandler = storeHandler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.videoInfoContainer = videoInfoContainer;
        }

        #region DI

        private readonly IVideoInfoContainer videoInfoContainer;

        private readonly IVideoStoreHandler videoStoreHandler;

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        #endregion

        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlidtId"></param>
        /// <returns></returns>
        public int AddVideo(IListVideoInfo video, int playlidtId)
        {
            int id = this.playlistStoreHandler.AddVideo(video, playlidtId);
            return id;
        }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="playlistID"></param>
        public void RemoveVideo(int videoID, int playlistID)
        {
            this.playlistStoreHandler.RemoveVideo(videoID, playlistID);
        }

        /// <summary>
        /// 動画情報を更新する
        /// </summary>
        /// <param name="video"></param>
        public void Update(IListVideoInfo video)
        {
            this.videoStoreHandler.Update(video);
        }


        /// <summary>
        /// 全ての動画を取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IListVideoInfo> GetAllVideos()
        {
            return this.videoStoreHandler.GetAllVideos().Select(v =>
            {

                IListVideoInfo video = this.videoInfoContainer.GetVideo(v.NiconicoId);
                video.SetDataBaseData(v);
                return video;
            });
        }

        /// <summary>
        /// 指定した動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IListVideoInfo GetVideo(int id)
        {
            if (!this.videoStoreHandler.Exists(id)) throw new InvalidOperationException($"動画({id})はデータベースに存在しません。");

            STypes::Video dbVideo = this.videoStoreHandler.GetVideo(id)!;

            IListVideoInfo video = this.videoInfoContainer.GetVideo(dbVideo.NiconicoId);
            video.SetDataBaseData(dbVideo);
            return video;
        }

        /// <summary>
        /// 動画の存在をチェックする
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exist(int id)
        {
            return this.videoStoreHandler.Exists(id);
        }



    }
}
