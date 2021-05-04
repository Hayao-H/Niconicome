using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Watch
{
    public interface IDomainModelConverter
    {
        void ConvertDomainVideoInfoToListVideoInfo(IListVideoInfo info, IDomainVideoInfo domainVideoInfo);
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
                if (!domainVideoInfo.DmcInfo.ThumbInfo.Large.IsNullOrEmpty())
                {
                    info.ThumbUrl.Value = domainVideoInfo.DmcInfo.ThumbInfo.Large;
                }

                if (!domainVideoInfo.DmcInfo.ThumbInfo.Normal.IsNullOrEmpty())
                {
                    info.ThumbUrl.Value = domainVideoInfo.DmcInfo.ThumbInfo.Normal;
                }
            }

        }
    }
}
