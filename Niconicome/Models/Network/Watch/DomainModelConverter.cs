using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store.V2;
using Remote = Niconicome.Models.Domain.Niconico.Remote;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Watch
{
    public interface IDomainModelConverter
    {
        /// <summary>
        /// ドメインの動画情報を変換する
        /// </summary>
        /// <param name="info"></param>
        /// <param name="domainVideoInfo"></param>
        void ConvertDomainVideoInfoToListVideoInfo(IListVideoInfo info, IDomainVideoInfo domainVideoInfo);

        /// <summary>
        /// ドメインの動画情報を変換する
        /// </summary>
        /// <param name="info"></param>
        /// <param name="domainVideoInfo"></param>
        void ConvertDomainVideoInfoToVideoInfo(IVideoInfo info, IDomainVideoInfo domainVideoInfo);

        /// <summary>
        /// リモートプレイリストの動画情報を変換する
        /// </summary>
        /// <param name="remoteVideoInfo"></param>
        /// <param name="listVideoInfo"></param>
        void ConvertRemoteVideoInfoToListVideoInfo(Remote::VideoInfo remoteVideoInfo, IListVideoInfo listVideoInfo);
    }

    public class DomainModelConverter : IDomainModelConverter
    {
        public DomainModelConverter(ITagStore tagStore)
        {
            this._tagStore = tagStore;
        }

        #region field

        private readonly ITagStore _tagStore;

        #endregion

        #region Method

        public void ConvertDomainVideoInfoToListVideoInfo(IListVideoInfo info, IDomainVideoInfo domainVideoInfo)
        {

            //タイトル
            info.Title.Value = domainVideoInfo.Title;

            //ID
            info.NiconicoId.Value = domainVideoInfo.Id;

            //タグ
            info.Tags = domainVideoInfo.Tags;

            //再生回数
            info.ViewCount.Value = domainVideoInfo.ViewCount;
            info.CommentCount.Value = domainVideoInfo.CommentCount;
            info.MylistCount.Value = domainVideoInfo.MylistCount;
            info.LikeCount.Value = domainVideoInfo.LikeCount;

            //チャンネル情報
            info.ChannelID.Value = domainVideoInfo.ChannelID;
            info.ChannelName.Value = domainVideoInfo.ChannelName;

            //投稿者情報
            info.OwnerID.Value = domainVideoInfo.OwnerID;
            info.OwnerName.Value = domainVideoInfo.Owner;

            //再生時間
            info.Duration.Value = domainVideoInfo.Duration;

            //投稿日時
            if (domainVideoInfo.DmcInfo is not null)
            {
                info.UploadedOn.Value = domainVideoInfo.DmcInfo.UploadedOn;
            }

            //サムネイル
            if (domainVideoInfo.DmcInfo is not null && domainVideoInfo.DmcInfo.ThumbInfo is not null)
            {
                info.ThumbUrl.Value = domainVideoInfo.DmcInfo.ThumbInfo.GetSpecifiedThumbnail(ThumbSize.Large);
            }

        }

        public void ConvertDomainVideoInfoToVideoInfo(IVideoInfo info, IDomainVideoInfo domainVideoInfo)
        {
            info.IsAutoUpdateEnabled = false;

            //タイトル
            info.Title = domainVideoInfo.Title;

            //タグ
            foreach (var tag in domainVideoInfo.Tags)
            {
                if (!this._tagStore.Exist(tag))
                {
                    this._tagStore.Create(tag);
                }

                IAttemptResult<ITagInfo> result = this._tagStore.GetTag(tag);
                if (!result.IsSucceeded || result.Data is null)
                {
                    continue;
                }

                info.AddTag(result.Data);
            }

            //再生回数
            info.ViewCount = domainVideoInfo.ViewCount;
            info.CommentCount = domainVideoInfo.CommentCount;
            info.MylistCount = domainVideoInfo.MylistCount;
            info.LikeCount = domainVideoInfo.LikeCount;

            //チャンネル情報
            info.ChannelID = domainVideoInfo.ChannelID;
            info.ChannelName = domainVideoInfo.ChannelName;

            //投稿者情報
            info.OwnerID = domainVideoInfo.OwnerID.ToString();
            info.OwnerName = domainVideoInfo.Owner;

            //投稿日時
            if (domainVideoInfo.DmcInfo is not null)
            {
                info.UploadedOn = domainVideoInfo.DmcInfo.UploadedOn;
            }

            //サムネイル
            if (domainVideoInfo.DmcInfo is not null && domainVideoInfo.DmcInfo.ThumbInfo is not null)
            {
                info.ThumbUrl = domainVideoInfo.DmcInfo.ThumbInfo.GetSpecifiedThumbnail(ThumbSize.Large);
            }

            info.IsAutoUpdateEnabled = true;

            //再生時間
            info.Duration = domainVideoInfo.Duration;
        }


        public void ConvertRemoteVideoInfoToListVideoInfo(Remote::VideoInfo remoteVideoInfo, IListVideoInfo listVideoInfo)
        {
            listVideoInfo.NiconicoId.Value = remoteVideoInfo.ID;
            listVideoInfo.Title.Value = remoteVideoInfo.Title;
            listVideoInfo.OwnerName.Value = remoteVideoInfo.OwnerName;
            listVideoInfo.OwnerID.Value = remoteVideoInfo.OwnerID;
            listVideoInfo.UploadedOn.Value = remoteVideoInfo.UploadedDT;
            listVideoInfo.ViewCount.Value = (int)remoteVideoInfo.ViewCount;
            listVideoInfo.CommentCount.Value = (int)remoteVideoInfo.CommentCount;
            listVideoInfo.MylistCount.Value = (int)remoteVideoInfo.MylistCount;
            listVideoInfo.ThumbUrl.Value = remoteVideoInfo.ThumbUrl;
        }

        #endregion
    }
}
