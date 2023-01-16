using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
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

        private const int DefaultVideoID = -1;

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

            var video = this.ConvertToVideoInfo(sharedData, data.Id);

            video.IsAutoUpdateEnabled = false;

            video.IsSelected.Value = data.IsSelected;
            video.IsDownloaded = data.IsDownloaded;
            video.IsEconomy = data.IsEconomy;

            video.IsAutoUpdateEnabled = true;

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        public IAttemptResult<IVideoInfo> GetVideo(string niconicoID, int playlistID)
        {
            IAttemptResult<SharedVideo> sharedResult = this._database.GetRecord<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == niconicoID);
            if (!sharedResult.IsSucceeded || sharedResult.Data is null)
            {
                return AttemptResult<IVideoInfo>.Fail(sharedResult.Message);
            }
            SharedVideo sharedData = sharedResult.Data;

            IAttemptResult<Video> result = this._database.GetRecord<Video>(TableNames.Video, v => v.SharedVideoID == sharedData.Id && v.PlaylistID == playlistID);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IVideoInfo>.Fail(result.Message);
            }

            Video data = result.Data;

            var video = this.ConvertToVideoInfo(sharedData, data.Id);

            video.IsAutoUpdateEnabled = false;

            video.IsSelected.Value = data.IsSelected;
            video.IsDownloaded = data.IsDownloaded;
            video.IsEconomy = data.IsEconomy;

            video.IsAutoUpdateEnabled = true;

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        public IAttemptResult<IVideoInfo> GetOnlySharedVideoVideo(int ID)
        {
            IAttemptResult<SharedVideo> sharedResult = this._database.GetRecord<SharedVideo>(TableNames.SharedVideo, ID);
            if (!sharedResult.IsSucceeded || sharedResult.Data is null)
            {
                return AttemptResult<IVideoInfo>.Fail(sharedResult.Message);
            }

            SharedVideo sharedData = sharedResult.Data;

            var video = this.ConvertToVideoInfo(sharedData);

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        public IAttemptResult<int> Create(string niconicoID)
        {
            SharedVideo data = new SharedVideo() { NiconicoId = niconicoID };
            IAttemptResult<int> result = this._database.Insert(data);
            return result;
        }

        public IAttemptResult<int> Create(string niconicoID, int playlistID)
        {

            int id;
            //SharedVideoの方が存在しない場合、そちらも作成
            if (!this._database.Exists<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == niconicoID))
            {
                IAttemptResult<int> sharedResult = this.Create(niconicoID);
                if (!sharedResult.IsSucceeded) return sharedResult;
                id = sharedResult.Data;
            }
            else
            {
                IAttemptResult<SharedVideo> getResult = this._database.GetRecord<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == niconicoID);
                if (!getResult.IsSucceeded || getResult.Data is null) return AttemptResult<int>.Fail(getResult.Message);
                id = getResult.Data.Id;
            }


            var data = new Video() { SharedVideoID = id, PlaylistID = playlistID };

            IAttemptResult<int> result = this._database.Insert(data);
            return result.IsSucceeded ? AttemptResult<int>.Succeeded(id) : result;

        }

        public IAttemptResult Update(IVideoInfo video)
        {
            SharedVideo shareData = this.ConvertToSharedVideo(video);
            IAttemptResult sharedResult = this._database.Update(shareData);

            if (!sharedResult.IsSucceeded || video.ID == DefaultVideoID) return sharedResult;

            Video data = this.ConvertToVideo(video);
            return this._database.Update(data);

        }


        public IAttemptResult Delete(int ID, int playlistID)
        {
            IAttemptResult sharedResult = this._database.DeleteAll<Video>(TableNames.Video, v => v.SharedVideoID == ID && v.PlaylistID == playlistID);
            if (!sharedResult.IsSucceeded) return sharedResult;

            //まだSharedVideoを参照するVideoが残っている場合終了
            if (this._database.Exists<Video>(TableNames.Video, v => v.SharedVideoID == ID)) return AttemptResult.Succeeded();

            return this._database.Delete(TableNames.SharedVideo, ID);
        }

        public IAttemptResult Clear()
        {
            IAttemptResult videoResult = this._database.Clear(TableNames.Video);
            if (!videoResult.IsSucceeded) return videoResult;

            return this._database.Clear(TableNames.SharedVideo);
        }

        public bool Exist(int ID, int playlistID)
        {
            return this._database.Exists<Video>(TableNames.Video, v => v.SharedVideoID == ID && v.PlaylistID == playlistID);
        }

        public bool Test(string niconicoID)
        {
            return this._database.Exists<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == niconicoID);

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
                Id = video.SharedID,
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
                IsSelected = video.IsSelected.Value,
                IsDownloaded = video.IsDownloaded,
                IsEconomy = video.IsEconomy,
                VideoFilePath = video.FilePath
            };
        }

        private IVideoInfo ConvertToVideoInfo(SharedVideo sharedData, int videoID = DefaultVideoID)
        {
            var tags = new List<ITagInfo>();

            foreach (var t in sharedData.Tags)
            {
                IAttemptResult<ITagInfo> tagResult = this._tagStore.GetTag(t);
                if (!tagResult.IsSucceeded || tagResult.Data is null) continue;
                tags.Add(tagResult.Data);
            }


            var video = new VideoInfo(this, tags)
            {
                SharedID = sharedData.Id,
                ID = videoID
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
            video.Duration = sharedData.Duration;
            video.IsDeleted = sharedData.IsDeleted;

            video.IsAutoUpdateEnabled = true;

            return video;
        }

        #endregion
    }
}
