using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.Database.Types;

namespace Niconicome.Models.Infrastructure.Database
{
    public class VideoDBHandler : IVideoStore
    {
        public VideoDBHandler(ILiteDBHandler database, ITagStore tagStore)
        {
            this._database = database;
            this._tagStore = tagStore;
        }

        #region field

        private readonly ILiteDBHandler _database;

        private readonly ITagStore _tagStore;

        #endregion

        #region Method

        public IAttemptResult<IVideoInfo> GetVideo(int ID, int playlistID)
        {
            IAttemptResult<SharedVideo> sharedResult = this._database.GetRecord<SharedVideo>(TableNames.SharedVideo, ID);
            if (!sharedResult.IsSucceeded || sharedResult.Data is null)
            {
                return AttemptResult<IVideoInfo>.Fail(sharedResult.Message);
            }

            IAttemptResult<Video> result = this._database.GetRecord<Video>(TableNames.Video, v => v.SharedVideoID == ID && v.PlaylistID == playlistID);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IVideoInfo>.Fail(result.Message);
            }

            SharedVideo sharedData = sharedResult.Data;
            Video data = result.Data;
            var tags = new List<ITagInfo>();

            foreach (var t in sharedData.Tags)
            {
                IAttemptResult<ITagInfo> tagResult = this._tagStore.GetTag(t);
                if (!tagResult.IsSucceeded || tagResult.Data is null) continue;
                tags.Add(tagResult.Data);
            }


            var video = new VideoInfo(this, tags)
            {
                ID = sharedData.Id
            };

            video.IsAutoUpdateEnabled = false;

            video.NiconicoId = sharedData.NiconicoId;
            video.Title = sharedData.Title;
            video.UploadedOn = sharedData.UploadedOn;
            video.ViewCount = sharedData.ViewCount;
            video.CommentCount = sharedData.CommentCount;
            video.MylistCount = sharedData.MylistCount;
            video.LikeCount = sharedData.LikeCount;
            video.OwnerID = sharedData.OwnerID;
            video.OwnerName = sharedData.OwnerName;
            video.LargeThumbUrl = sharedData.LargeThumbUrl;
            video.ThumbUrl = sharedData.ThumbUrl;
            video.ThumbPath = sharedData.ThumbPath;
            video.Duration = sharedData.Duration;
            video.IsDeleted = sharedData.IsDeleted;
            video.IsSelected = data.IsSelected;
            video.IsDownloaded = data.IsDownloaded;
            video.IsEconomy = data.IsEconomy;

            video.IsAutoUpdateEnabled = false;

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        public IAttemptResult<int> Create(IVideoInfo video)
        {
            int id;

            //SharedVideoの方が存在しない場合、そちらも作成
            if (!this._database.Exists<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == video.NiconicoId))
            {
                SharedVideo sharedData = this.ConvertToSharedVideo(video);
                IAttemptResult<int> sharedResult = this._database.Insert(sharedData);
                if (!sharedResult.IsSucceeded) return sharedResult;
                id = sharedResult.Data;
            }
            else
            {
                IAttemptResult<SharedVideo> getResult = this._database.GetRecord<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == v.NiconicoId);
                if (!getResult.IsSucceeded || getResult.Data is null) return AttemptResult<int>.Fail(getResult.Message);
                id = getResult.Data.Id;
            }

            Video data = this.ConvertToVideo(video);
            data.SharedVideoID = id;

            return this._database.Insert(data);

        }

        public IAttemptResult Update(IVideoInfo video)
        {
            SharedVideo shareData = this.ConvertToSharedVideo(video);
            IAttemptResult sharedResult =  this._database.Update(shareData);

            if (!sharedResult.IsSucceeded) return sharedResult;

            Video data = this.ConvertToVideo(video);
            return this._database.Update(data);

        }


        public IAttemptResult Delete(int ID, int playlistID)
        {
            IAttemptResult sharedResult = this._database.DeleteAll<Video>(TableNames.Video, v => v.SharedVideoID == ID && v.PlaylistID == playlistID);
            if (!sharedResult.IsSucceeded) return sharedResult;

            if (this._database.Exists<Video>(TableNames.Video, v => v.SharedVideoID == ID)) return AttemptResult.Succeeded();

            return this._database.Delete(TableNames.SharedVideo, ID);
        }

        public bool Exist(int ID, int playlistID)
        {
            return this._database.Exists<Video>(TableNames.SharedVideo, v => v.SharedVideoID == ID && v.PlaylistID == playlistID);
        }



        #endregion

        #region private

        /// <summary>
        /// ローカル情報をデータベースの型に変換
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private SharedVideo ConvertToSharedVideo(IVideoInfo video)
        {
            return new SharedVideo()
            {
                Id = video.ID,
                NiconicoId = video.NiconicoId,
                Title = video.Title,
                UploadedOn = video.UploadedOn,
                ViewCount = video.ViewCount,
                CommentCount = video.CommentCount,
                MylistCount = video.MylistCount,
                LikeCount = video.LikeCount,
                OwnerID = video.OwnerID,
                OwnerName = video.OwnerName,
                LargeThumbUrl = video.LargeThumbUrl,
                ThumbUrl = video.ThumbUrl,
                ThumbPath = video.ThumbPath,
                Duration = video.Duration,
                IsDeleted = video.IsDeleted,
                Tags = video.Tags.Select(v => v.ID).ToList()
            };
        }

        private Video ConvertToVideo(IVideoInfo video)
        {
            return new Video()
            {
                Id = video.ID,
                PlaylistID = video.PlaylistID,
                IsSelected = video.IsSelected,
                IsDownloaded = video.IsDownloaded,
                IsEconomy = video.IsEconomy,
                VideoFilePath = video.FilePath
            };
        }

        #endregion
    }
}
