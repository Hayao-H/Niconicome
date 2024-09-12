using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video;
using Niconicome.Models.Playlist.V2.Migration;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;

namespace Niconicome.Models.Playlist.V2.Manager.Helper
{
    public class VideoListManagerHelperBase
    {
        public VideoListManagerHelperBase(IVideoStore videoStore, ITagStore tagStore)
        {
            this._videoStore = videoStore;
            this._tagStore = tagStore;
        }

        #region field

        protected readonly IVideoStore _videoStore;

        protected readonly ITagStore _tagStore;

        #endregion

        #region private


        /// <summary>
        /// 動画情報をローカル形式に変換
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected IAttemptResult<IVideoInfo> ConvertToVideoInfo(int playlistID, Remote::VideoInfo source)
        {

            if (!this._videoStore.Exist(source.NiconicoID, playlistID))
            {
                IAttemptResult cResult = this._videoStore.Create(source.NiconicoID, playlistID);
                if (!cResult.IsSucceeded) return AttemptResult<IVideoInfo>.Fail(cResult.Message);
            }

            IAttemptResult<IVideoInfo> getResult = this._videoStore.GetVideo(source.NiconicoID, playlistID);
            if (!getResult.IsSucceeded || getResult.Data is null) return AttemptResult<IVideoInfo>.Fail(getResult.Message);

            IVideoInfo video = getResult.Data;
            video.IsAutoUpdateEnabled = false;

            video.OwnerName = source.OwnerName;
            video.OwnerID = source.OwnerID;
            video.ThumbUrl = source.ThumbUrl;
            video.UploadedOn = source.UploadedDT;
            video.ViewCount = source.ViewCount;
            video.CommentCount = source.CommentCount;
            video.MylistCount = source.MylistCount;
            video.LikeCount = source.LikeCount;
            video.AddedAt = source.AddedAt;
            video.ChannelName = source.ChannelName;
            video.ChannelID = source.ChannelID;
            video.Duration = source.Duration;

            if (!string.IsNullOrEmpty(source.Description))
            {
                video.Description = source.Description;
            }

            if (source.Tags.Count > 0)
            {
                video.ClearTags();
            }

            foreach (var tag in source.Tags)
            {
                if (!this._tagStore.Exist(tag.Name))
                {
                    IAttemptResult cResult = this._tagStore.Create(tag.Name);
                    if (!cResult.IsSucceeded)
                    {
                        continue;
                    }
                }

                IAttemptResult<ITagInfo> tagResult = this._tagStore.GetTag(tag.Name);
                if (!tagResult.IsSucceeded || tagResult.Data is null)
                {
                    continue;
                }

                tagResult.Data.IsNicodicExist = tag.IsNicodicExist;

                if (!video.Tags.Any(t => t.Name == tag.Name))
                {
                    video.AddTag(tagResult.Data);
                }
            }

            video.IsAutoUpdateEnabled = true;

            video.Title = source.Title;

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        /// <summary>
        /// リモートプレイリストの形式を取得
        /// </summary>
        /// <param name="playlistType"></param>
        /// <returns></returns>
        protected RemoteType ConvertToRemoteType(PlaylistType playlistType)
        {
            return playlistType switch
            {
                PlaylistType.WatchLater => RemoteType.WatchLater,
                PlaylistType.Mylist => RemoteType.Mylist,
                PlaylistType.Series => RemoteType.Series,
                PlaylistType.UserVideos => RemoteType.UserVideos,
                PlaylistType.Channel => RemoteType.Channel,
                _ => RemoteType.None,
            };
        }

        #endregion
    }
}
