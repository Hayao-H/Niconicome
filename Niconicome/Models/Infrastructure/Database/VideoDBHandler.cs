using System;
using System.Collections.Generic;
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

        public IAttemptResult<IVideoInfo> GetVideo(int ID)
        {
            IAttemptResult<Video> result = this._database.GetRecord<Video>(TableNames.Video, ID);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IVideoInfo>.Fail(result.Message);
            }

            Video data = result.Data;
            var tags = new List<ITagInfo>();

            foreach (var t in data.Tags)
            {
                IAttemptResult<ITagInfo> tagResult = this._tagStore.GetTag(t);
                if (!tagResult.IsSucceeded || tagResult.Data is null) continue;
                tags.Add(tagResult.Data);
            }


            var video = new VideoInfo(this, tags)
            {
                ID = data.Id,
                NiconicoId = data.NiconicoId,
                Title = data.Title,
                UploadedOn = data.UploadedOn,
                ViewCount = data.ViewCount,
                CommentCount = data.CommentCount,
                MylistCount = data.MylistCount,
                LikeCount = data.LikeCount,
                OwnerID = data.OwnerID,
                OwnerName = data.OwnerName,
                LargeThumbUrl = data.LargeThumbUrl,
                ThumbUrl = data.ThumbUrl,
                ThumbPath = data.ThumbPath,
                Duration = data.Duration,
                IsDeleted = data.IsDeleted,
                IsSelected = data.IsSelected,
            };

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        public IAttemptResult<int> Create(IVideoInfo video)
        {
            Video data = this.Convert(video);
            return this._database.Insert(data);
        }

        public IAttemptResult Update(IVideoInfo video)
        {
            Video data = this.Convert(video);
            return this._database.Update(data);
        }


        public IAttemptResult Delete(int ID)
        {
            return this._database.Delete(TableNames.Video, ID);
        }

        public bool Exist(int ID)
        {
            return this._database.Exists(TableNames.Video, ID);
        }



        #endregion

        #region private

        /// <summary>
        /// ローカル情報をデータベースの型に変換
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private Video Convert(IVideoInfo video)
        {
            return new Video()
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
                IsSelected = video.IsSelected,
                Tags = video.Tags.Select(v => v.ID).ToList()
            };
        }

        #endregion
    }
}
