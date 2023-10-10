using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Playlist.V2.Manager;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList
{
    public class VideoInfoViewModel : IDisposable
    {
        public VideoInfoViewModel(IVideoInfo video)
        {
            var bindable = new Bindables();

            this._video = video;
            this._thumbConverter = path =>
            {
                byte[] thumbData = File.ReadAllBytes(path);
                return Convert.ToBase64String(thumbData);
            };

            if (video.ThumbPath.Value.IsNullOrEmpty())
            {
                this.Base64ThumbData = new BindableProperty<string>(string.Empty).AddTo(bindable);
            }
            else
            {
                this.Base64ThumbData = new BindableProperty<string>(this._thumbConverter(video.ThumbPath.Value)).AddTo(bindable);
            }

            this._thumbHandler = p =>
            {
                if (this._video.ThumbPath.Value.IsNullOrEmpty()) return;
                this.Base64ThumbData.Value = this._thumbConverter(this._video.ThumbPath.Value);
            };

            video.ThumbPath.Subscribe(this._thumbHandler);

            this.IsSelected = video.IsSelected.AddTo(bindable);
            this.IsDownloaded = video.IsDownloaded.AddTo(bindable);
            this.Message = video.Message.AddTo(bindable);

            this.Bindable = bindable;

        }

        ~VideoInfoViewModel()
        {
            this.Dispose();
        }

        #region field

        private readonly IVideoInfo _video;

        private readonly Func<string, string> _thumbConverter;

        private readonly Action<string> _thumbHandler;

        #endregion

        #region Props

        /// <summary>
        /// IVIdeoInfoオブジェクト
        /// </summary>
        public IVideoInfo VideoInfo => this._video;

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindable { get; init; }


        /// <summary>
        /// ID
        /// </summary>
        public int ID => this._video.SharedID;

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
        public string UploadedOn => this._video.UploadedOn.ToString("yyyy/M/dd HH:mm");

        /// <summary>
        /// メッセージ
        /// </summary>
        public IBindableProperty<string> Message { get; init; }

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
        public string OwnerID => this._video.OwnerID;

        /// <summary>
        /// 投稿者名
        /// </summary>
        public string OwnerName => this._video.OwnerName.IsNullOrEmpty() ? "不明" : this._video.OwnerName;

        /// <summary>
        /// サムネURL(大)
        /// </summary>
        public string LargeThumbUrl => this._video.LargeThumbUrl;

        /// <summary>
        /// サムネURL
        /// </summary>
        public string ThumbUrl => this._video.ThumbUrl;

        /// <summary>
        /// サムネファイルデータ
        /// </summary>
        public IBindableProperty<string> Base64ThumbData { get; init; }

        /// <summary>
        /// 長さ
        /// </summary>
        public string Duration
        {
            get
            {
                string minute = Math.Floor((double)(this._video.Duration / 60)).ToString().PadLeft(2, '0');
                string second = (this._video.Duration % 60).ToString().PadLeft(2, '0');
                return $"{minute}:{second}";
            }
        }


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
        public IBindableProperty<bool> IsSelected { get; init; }

        /// <summary>
        /// DLフラグ
        /// </summary>
        public IBindableProperty<bool> IsDownloaded { get; init; }

        #endregion

        #region Method

        public void OnClick(MouseEventArgs e)
        {
            if (e.Button == 1)
            {
                WS.PlaylistEventManager.OnVideoClick(this._video,EventType.MiddleClick);
            }
        }

        public void OnDBClick(MouseEventArgs _)
        {
            WS.PlaylistEventManager.OnVideoClick(this._video, EventType.DoubleClick);
        }

        #endregion

        public void Dispose()
        {
            this._video.ThumbPath.UnSubscribe(this._thumbHandler);
            this.Bindable.Dispose();
        }

        public override string ToString()
        {
            return $"[{this.NiconicoId}]{this.Title}";
        }
    }
}
