using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList
{
    public class VideoInfoViewModel
    {
        public VideoInfoViewModel(IVideoInfo video)
        {
            this._video = video;
        }

        #region field

        private readonly IVideoInfo _video;

        #endregion

        #region Props

        /// <summary>
        /// ID
        /// </summary>
        public int ID => this._video.ID;

        /// <summary>
        /// 動画ID
        /// </summary>
        public string NiconicoId => this._video.NiconicoId;

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title => this._video.Title;

        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime UploadedOn => this._video.UploadedOn;

        /// <summary>
        /// 閲覧数
        /// </summary>
        public int ViewCount => this._video.ViewCount;

        /// <summary>
        /// コメ数
        /// </summary>
        public int CommentCount => this._video.CommentCount;

        /// <summary>
        /// マイリス数
        /// </summary>
        public int MylistCount => this._video.MylistCount;

        /// <summary>
        /// いいね数
        /// </summary>
        public int LikeCount => this._video.LikeCount;

        /// <summary>
        /// 投稿者ID
        /// </summary>
        public int OwnerID => this._video.OwnerID;

        /// <summary>
        /// 投稿者名
        /// </summary>
        public string OwnerName => this._video.OwnerName;

        /// <summary>
        /// サムネURL(大)
        /// </summary>
        public string LargeThumbUrl => this._video.LargeThumbUrl;

        /// <summary>
        /// サムネURL
        /// </summary>
        public string ThumbUrl => this._video.ThumbUrl;

        /// <summary>
        /// サムネファイルパス
        /// </summary>
        public string ThumbPath => this._video.ThumbPath;

        /// <summary>
        /// 長さ
        /// </summary>
        public int Duration => this._video.Duration;

        /// <summary>
        /// タグ
        /// </summary>
        public IReadOnlyList<TagInfoViewModel> Tags => this._video.Tags.Select(t => new TagInfoViewModel(t)).ToList().AsReadOnly();

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsDeleted => this._video.IsDeleted;

        /// <summary>
        /// 選択フラグ
        /// </summary>
        public bool IsSelected => this._video.IsSelected;

        #endregion
    }
}
