using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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

        private const int DefaultPlaylistID = -1;

        private readonly Dictionary<int, Dictionary<string, IVideoInfo>> _cache = new();

        #endregion

        #region Method

        public IAttemptResult<IVideoInfo> GetVideo(string niconicoID, int playlistID)
        {
            if (this.TryGetCache(playlistID, niconicoID, out IVideoInfo? cachedVideo))
            {
                return AttemptResult<IVideoInfo>.Succeeded(cachedVideo);
            }

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

            var video = this.ConvertToVideoInfo(sharedData, data.Id, playlistID);

            video.IsAutoUpdateEnabled = false;

            video.IsSelected.Value = data.IsSelected;
            video.IsDownloaded.Value = data.IsDownloaded;
            video.IsEconomy = data.IsEconomy;
            video.AddedAt = data.AddedAt;

            video.IsAutoUpdateEnabled = true;

            if (!this._cache.ContainsKey(playlistID)) this._cache.Add(playlistID, new Dictionary<string, IVideoInfo>());
            if (!this._cache[playlistID].ContainsKey(niconicoID)) this._cache[playlistID].Add(sharedData.NiconicoId, video);

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

        public IAttemptResult Create(string niconicoID, int playlistID)
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

            IAttemptResult result = this._database.Insert(data);

            return result.IsSucceeded ? AttemptResult.Succeeded() : result;

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
            IAttemptResult<Video> targetResult = this._database.GetRecord<Video>(TableNames.Video, ID);
            if (!targetResult.IsSucceeded || targetResult.Data is null)
            {
                return AttemptResult.Fail(targetResult.Message);
            }

            int sharedID = targetResult.Data.SharedVideoID;

            IAttemptResult deleteResult = this._database.Delete(TableNames.Video, ID);
            if (!deleteResult.IsSucceeded) return deleteResult;

            //まだSharedVideoを参照するVideoが残っている場合終了
            if (this._database.Exists<Video>(TableNames.Video, v => v.SharedVideoID == sharedID)) return AttemptResult.Succeeded();

            return this._database.Delete(TableNames.SharedVideo, sharedID);
        }

        public IAttemptResult Clear()
        {
            IAttemptResult videoResult = this._database.Clear(TableNames.Video);
            if (!videoResult.IsSucceeded) return videoResult;

            return this._database.Clear(TableNames.SharedVideo);
        }

        public void Flush()
        {
            this._cache.Clear();
        }

        public bool Exist(string niconicoID, int playlistID)
        {
            if (!this._database.Exists<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == niconicoID)) return false;

            IAttemptResult<SharedVideo> sharedResult = this._database.GetRecord<SharedVideo>(TableNames.SharedVideo, v => v.NiconicoId == niconicoID);
            if (!sharedResult.IsSucceeded || sharedResult.Data is null) return false;

            return this._database.Exists<Video>(TableNames.Video, v => v.SharedVideoID == sharedResult.Data.Id && v.PlaylistID == playlistID);
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
                ChannelName = video.ChannelName,
                ChannelID = video.ChannelID,
                Tags = video.Tags.Select(v => v.ID).ToList()
            };
        }

        private Video ConvertToVideo(IVideoInfo video)
        {
            return new Video()
            {
                Id = video.ID,
                SharedVideoID = video.SharedID,
                PlaylistID = video.PlaylistID,
                IsSelected = video.IsSelected.Value,
                IsDownloaded = video.IsDownloaded.Value,
                IsEconomy = video.IsEconomy,
                VideoFilePath = video.FilePath,
                AddedAt = video.AddedAt,
            };
        }

        private IVideoInfo ConvertToVideoInfo(SharedVideo sharedData, int videoID = DefaultVideoID, int playlistID = DefaultPlaylistID)
        {
            var tags = new List<ITagInfo>();

            foreach (var t in sharedData.Tags)
            {
                IAttemptResult<ITagInfo> tagResult = this._tagStore.GetTag(t);
                if (!tagResult.IsSucceeded || tagResult.Data is null) continue;
                tags.Add(tagResult.Data);
            }


            var video = new VideoInfo(sharedData.NiconicoId, this, tags)
            {
                SharedID = sharedData.Id,
                ID = videoID,
                PlaylistID = playlistID,
            };

            video.IsAutoUpdateEnabled = false;

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
            video.ChannelID = sharedData.ChannelID;
            video.ChannelName = sharedData.ChannelName;

            video.IsAutoUpdateEnabled = true;

            return video;
        }

        /// <summary>
        /// キャッシュされたデータを取得する
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="niconicoID"></param>
        /// <param name="video"></param>
        /// <returns></returns>
        private bool TryGetCache(int playlistID, string niconicoID, [NotNullWhen(true)] out IVideoInfo? video)
        {
            if (!this._cache.ContainsKey(playlistID))
            {
                video = null;
                return false;
            }
            else if (!this._cache[playlistID].ContainsKey(niconicoID))
            {
                video = null;
                return false;
            }
            else
            {
                video = this._cache[playlistID][niconicoID];
                return true;
            }

        }

        #endregion
    }
}
