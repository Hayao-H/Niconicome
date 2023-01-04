using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;

namespace Niconicome.Models.Domain.Playlist
{
    public interface IVideoInfo : IUpdatable
    {
        /// <summary>
        /// ID
        /// </summary>
        int SharedID { get; }

        /// <summary>
        /// ID
        /// </summary>
        int ID { get; }

        /// <summary>
        /// プレイリストのID
        /// </summary>
        int PlaylistID { get; }

        /// <summary>
        /// 動画ID
        /// </summary>
        string NiconicoId { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        DateTime UploadedOn { get; set; }

        /// <summary>
        /// 閲覧数
        /// </summary>
        int ViewCount { get; set; }

        /// <summary>
        /// コメ数
        /// </summary>
        int CommentCount { get; set; }

        /// <summary>
        /// マイリス数
        /// </summary>
        int MylistCount { get; set; }

        /// <summary>
        /// いいね数
        /// </summary>
        int LikeCount { get; set; }

        /// <summary>
        /// 投稿者ID
        /// </summary>
        int OwnerID { get; set; }

        /// <summary>
        /// 投稿者名
        /// </summary>
        string OwnerName { get; set; }

        /// <summary>
        /// サムネURL(大)
        /// </summary>
        string LargeThumbUrl { get; set; }

        /// <summary>
        /// サムネURL
        /// </summary>
        string ThumbUrl { get; set; }

        /// <summary>
        /// サムネファイルパス
        /// </summary>
        string ThumbPath { get; set; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// 長さ
        /// </summary>
        int Duration { get; set; }

        /// <summary>
        /// タグ
        /// </summary>
        IReadOnlyList<ITagInfo> Tags { get; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// 選択フラグ
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// DL済みフラグ
        /// </summary>
        bool IsDownloaded { get; set; }

        /// <summary>
        /// エコノミーフラグ
        /// </summary>
        bool IsEconomy { get; set; }

        /// <summary>
        /// タグを追加
        /// </summary>
        /// <param name="tag"></param>
        void AddTag(ITagInfo tag);

        /// <summary>
        /// タグをクリア
        /// </summary>
        /// <param name="tag"></param>
        void ClearTags();
    }

    internal class VideoInfo : UpdatableInfoBase<IVideoStore, IVideoInfo>, IVideoInfo
    {
        public VideoInfo(IStoreUpdater<IVideoInfo> updater, List<ITagInfo> tags) : base(updater)
        {
            this._tags = tags;
        }

        #region field

        private string _niconicoId = string.Empty;

        private string _title = string.Empty;

        private DateTime _uploadedOn;

        private int _viewCount;

        private int _commentCount;

        private int _mylistCount;

        private int _likeCount;

        private int _ownerID;

        private string _ownerName = string.Empty;

        private string _largeThumbUrl = string.Empty;

        private string _thumbUrl = string.Empty;

        private string _filePath = string.Empty;

        private int _duration;

        private readonly List<ITagInfo> _tags = new();

        private bool _isDeleted;

        private bool _isSelected;

        private bool _isEconomy;

        private bool _isDownloaded;

        #endregion

        #region Props

        public int SharedID { get; init; }

        public int ID { get; init; }

        public int PlaylistID { get; init; }

        public string NiconicoId
        {
            get => this._niconicoId;
            set
            {
                this._niconicoId = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public string Title
        {
            get => this._title;
            set
            {
                this._title = value;
                if (this.IsAutoUpdateEnabled)
                {
                    this.Update(this);
                }
            }
        }

        public DateTime UploadedOn
        {
            get => this._uploadedOn;
            set
            {
                this._uploadedOn = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public int ViewCount
        {
            get => this._viewCount;
            set
            {
                this._viewCount = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public int CommentCount
        {
            get => this._commentCount;
            set
            {
                this._commentCount = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public int MylistCount
        {
            get => this._mylistCount;
            set
            {
                this._mylistCount = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public int LikeCount
        {
            get => this._likeCount;
            set
            {
                this._likeCount = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public int OwnerID
        {
            get => this._ownerID;
            set
            {
                this._ownerID = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public string OwnerName
        {
            get => this._ownerName;
            set
            {
                this._ownerName = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public string LargeThumbUrl

        {
            get => this._largeThumbUrl;
            set
            {
                this._largeThumbUrl = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public string ThumbUrl
        {
            get => this._thumbUrl;
            set
            {
                this._thumbUrl = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public string ThumbPath { get; set; } = string.Empty;

        public string FilePath
        {
            get => this._filePath;
            set
            {
                this._filePath = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }


        public int Duration
        {
            get => this._duration;
            set
            {
                this._duration = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public IReadOnlyList<ITagInfo> Tags => this._tags.AsReadOnly();

        public bool IsDeleted
        {
            get => this._isDeleted;
            set
            {
                this._isDeleted = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public bool IsSelected
        {
            get => this._isSelected;
            set
            {
                this._isSelected = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public bool IsDownloaded
        {
            get => this._isDownloaded;
            set
            {
                this._isDownloaded = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        public bool IsEconomy
        {
            get => this._isEconomy;
            set
            {
                this._isEconomy = value;
                if (this.IsAutoUpdateEnabled) this.Update(this);
            }
        }

        #endregion

        #region Method

        public void AddTag(ITagInfo tag)
        {
            this._tags.Add(tag);
            if (this.IsAutoUpdateEnabled) this.Update(this);
        }

        public void ClearTags()
        {
            this._tags.Clear();
            if (this.IsAutoUpdateEnabled) this.Update(this);
        }

        #endregion

    }
}
