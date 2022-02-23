using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Local.Store
{
    public interface IVideoStoreHandler
    {
        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult<int> AddVideo(STypes::Video video);

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult RemoveVideo(int videoID);

        /// <summary>
        /// 動画を更新する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult Update(STypes::Video video);

        /// <summary>
        /// すべての動画を取得する
        /// </summary>
        /// <returns></returns>
        IAttemptResult<List<STypes::Video>> GetAllVideos();

        /// <summary>
        /// IDを指定して動画を取得する
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        IAttemptResult<STypes::Video> GetVideo(int Id);

        /// <summary>
        /// ニコニコのIDを指定して動画を取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        IAttemptResult<STypes::Video> GetVideo(string niconicoId);

        /// <summary>
        /// 動画の存在を確認する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(int id);

        /// <summary>
        /// 動画の存在を確認する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        bool Exists(string niconicoId);
    }

    public class VideoStoreHandler : IVideoStoreHandler
    {

        public VideoStoreHandler(IDataBase dataBase, ILogger logger)
        {
            this._databaseInstance = dataBase;
            this._logger = logger;
        }

        #region field

        private readonly IDataBase _databaseInstance;

        private readonly ILogger _logger;

        #endregion

        public IAttemptResult<int> AddVideo(STypes::Video video)
        {

            //既にデータベースに存在する場合は再利用
            if (this.Exists(video.NiconicoId))
            {
                IAttemptResult<STypes::Video> vResult = this.GetVideo(video.NiconicoId);

                if (!vResult.IsSucceeded || vResult.Data is null)
                {
                    return AttemptResult<int>.Fail(vResult.Message);
                }


                //動画情報が存在する場合は更新
                if (string.IsNullOrEmpty(video.Title))
                {
                    return AttemptResult<int>.Succeeded(vResult.Data.Id);
                }

                video.Id = vResult.Data.Id;

                IAttemptResult uResult = this.UpdateInternal(video);

                if (uResult.IsSucceeded)
                {
                    this._logger.Log($"動画を上書きモードで追加しました。(niconicoID:{video.NiconicoId})");
                }

                return uResult.IsSucceeded switch
                {
                    true => AttemptResult<int>.Succeeded(vResult.Data.Id),
                    _ => AttemptResult<int>.Fail(uResult.Message)
                };
            }
            else
            {

                IAttemptResult<int> result = this._databaseInstance.Store(video, STypes::Video.TableName);

                if (result.IsSucceeded)
                {
                    this._logger.Log($"動画を追加しました。(niconicoID:{video.NiconicoId})");
                }

                return result;
            }

        }

        public IAttemptResult RemoveVideo(int videoID)
        {

            if (!this.Exists(videoID))
            {
                return AttemptResult.Fail("指定された動画が存在しません。");
            }

            IAttemptResult result = this._databaseInstance.DeleteAll<STypes::Video>(STypes::Video.TableName, video => video.Id == videoID);

            if (result.IsSucceeded)
            {
                this._logger.Log($"動画({videoID})を削除しました。");
            }

            return result;


        }

        public IAttemptResult Update(STypes::Video video)
        {

            if (!this.Exists(video.Id))
            {
                return AttemptResult.Fail("指定された動画が存在しません。");
            }

            return this.UpdateInternal(video);
        }

        public IAttemptResult<List<STypes::Video>> GetAllVideos()
        {

            IAttemptResult<List<STypes::Video>> result = this._databaseInstance.GetAllRecords<STypes::Video>(STypes::Video.TableName);

            if (!result.IsSucceeded || result.Data is null)
            {

                return AttemptResult<List<STypes::Video>>.Fail(result.Message);
            }

            return AttemptResult<List<STypes::Video>>.Succeeded(result.Data);
        }

        public IAttemptResult<STypes::Video> GetVideo(int id)
        {
            if (!this.Exists(id))
            {
                return AttemptResult<STypes::Video>.Fail("指定された動画が存在しません。");
            }

            IAttemptResult<STypes::Video> result = this._databaseInstance.GetRecord<STypes::Video>(STypes::Video.TableName, id);

            return result;
        }

        public IAttemptResult<STypes::Video> GetVideo(string niconicoId)
        {

            if (!this.Exists(niconicoId))
            {
                return AttemptResult<STypes::Video>.Fail("指定された動画が存在しません。");
            }

            IAttemptResult<STypes::Video> result = this._databaseInstance.GetRecord<STypes::Video>(STypes::Video.TableName, v => v.NiconicoId == niconicoId);

            return result;
        }

        public bool Exists(int id)
        {
            return this._databaseInstance.Exists<STypes::Video>(STypes::Video.TableName, id);
        }

        public bool Exists(string niconicoId)
        {
            return this._databaseInstance.Exists<STypes::Video>(STypes::Video.TableName, v => v.NiconicoId == niconicoId);
        }


        #region private

        private IAttemptResult UpdateInternal(STypes::Video video)
        {
            return this._databaseInstance.Update(video, STypes::Video.TableName);
        }

        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="dbVideo"></param>
        /// <param name="videoData"></param>
        private void SetData(STypes::Video dbVideo, IListVideoInfo videoData)
        {
            dbVideo.NiconicoId = videoData.NiconicoId.Value;
            dbVideo.Title = videoData.Title.Value;
            dbVideo.UploadedOn = videoData.UploadedOn.Value;
            dbVideo.ThumbUrl = videoData.ThumbUrl.Value;
            dbVideo.LargeThumbUrl = videoData.LargeThumbUrl.Value;
            dbVideo.ThumbPath = videoData.ThumbPath.Value;
            dbVideo.IsSelected = videoData.IsSelected.Value;
            dbVideo.FileName = videoData.FileName.Value;
            dbVideo.Tags = videoData.Tags.ToList();
            dbVideo.ViewCount = videoData.ViewCount.Value;
            dbVideo.CommentCount = videoData.CommentCount.Value;
            dbVideo.MylistCount = videoData.MylistCount.Value;
            dbVideo.LikeCount = videoData.LikeCount.Value;
            dbVideo.Duration = videoData.Duration.Value;
            dbVideo.OwnerID = videoData.OwnerID.Value;
            dbVideo.OwnerName = videoData.OwnerName.Value;
        }

        #endregion


    }
}
