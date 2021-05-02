using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store;

namespace Niconicome.Models.Playlist
{
    public interface IVideoHandler
    {
        int AddVideo(IListVideoInfo video, int playlidtId);
        void RemoveVideo(int videoID, int playlistID);
        void Update(IListVideoInfo video);
        bool Exist(int id);
        IEnumerable<IListVideoInfo> GetAllVideos();
        IListVideoInfo GetVideo(int id);

    }

    public class VideoHandler : IVideoHandler
    {

        public VideoHandler(IVideoStoreHandler storeHandler,IPlaylistStoreHandler playlistStoreHandler)
        {
            this.videoStoreHandler = storeHandler;
            this.playlistStoreHandler = playlistStoreHandler;
        }

        /// <summary>
        /// DBにデータを保存
        /// </summary>
        private readonly IVideoStoreHandler videoStoreHandler;

        /// <summary>
        /// DB上のプレイリストにアクセスする
        /// </summary>
        private readonly IPlaylistStoreHandler playlistStoreHandler;

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
            return this.videoStoreHandler.GetAllVideos().Select(v => NonBindableListVideoInfo.ConvertDbDataToVideoListInfo(v));
        }

        /// <summary>
        /// 指定した動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IListVideoInfo GetVideo(int id)
        {
            if (!this.videoStoreHandler.Exists(id)) throw new InvalidOperationException($"動画({id})はデータベースに存在しません。");
            return NonBindableListVideoInfo.ConvertDbDataToVideoListInfo(this.videoStoreHandler.GetVideo(id)!);
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
