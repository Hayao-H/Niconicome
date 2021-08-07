using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Remote;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Watch
{
    public interface IDomainModelConverter
    {
        void ConvertDomainVideoInfoToListVideoInfo(IListVideoInfo info, IDomainVideoInfo domainVideoInfo);

        void ConvertRemoteVideoInfoToListVideoInfo(VideoInfo remoteVideoInfo, IListVideoInfo listVideoInfo);
    }

    public class DomainModelConverter : IDomainModelConverter
    {
        /// <summary>
        /// Domainの動画情報を変換する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
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

        /// <summary>
        /// リモートプレイリストの動画情報を変換する
        /// </summary>
        /// <param name="remoteVideoInfo"></param>
        /// <param name="listVideoInfo"></param>
        public void ConvertRemoteVideoInfoToListVideoInfo(VideoInfo remoteVideoInfo, IListVideoInfo listVideoInfo)
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

    }
}
