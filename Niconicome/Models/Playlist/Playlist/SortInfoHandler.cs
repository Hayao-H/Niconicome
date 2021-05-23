using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface ISortInfoHandler
    {
        ReactiveProperty<VideoSortType> SortType { get; }
        ReactiveProperty<bool> IsDescending { get; }
        ReactiveProperty<string> IdColumnTitle { get; }
        ReactiveProperty<string> TitleColumnTitle { get; }
        ReactiveProperty<string> UploadColumnTitle { get; }
        ReactiveProperty<string> ViewCountColumnTitle { get; }
        ReactiveProperty<string> DlFlagColumnTitle { get; }
    }

    /// <summary>
    /// ソート情報をVMに伝える
    /// この情報を参照できるのはVMのみで、例えばReflesherなどは自身に渡されたプレイリストの情報を参照する
    /// </summary>
    class SortInfoHandler : BindableBase, ISortInfoHandler
    {
        public SortInfoHandler(IPlaylistHandler playlistHandler, ICurrent current, IVideoListContainer container)
        {
            this.playlistHandler = playlistHandler;
            this.current = current;
            this.container = container;

            this.SortType = new ReactiveProperty<VideoSortType>(current.SelectedPlaylist.Value?.VideoSortType ?? VideoSortType.Register);
            this.IsDescending = new ReactiveProperty<bool>(current.SelectedPlaylist.Value?.IsVideoDescending ?? false);
            this.IdColumnTitle = new ReactiveProperty<string>(DefaultIdColumnTitle);
            this.TitleColumnTitle = new ReactiveProperty<string>(DefaultTitleColumnTitle);
            this.UploadColumnTitle = new ReactiveProperty<string>(DefaultUploadColumnTitle);
            this.DlFlagColumnTitle = new ReactiveProperty<string>(DefaultDlFlagColumnTitle);
            this.ViewCountColumnTitle = new ReactiveProperty<string>(DefaultViewCountColumnTitle);

            //プレイリストが変更されたら並び替え情報を変更する
            this.current.SelectedPlaylist.Subscribe(value =>
            {
                if (value is null) return;
                noSort = true;
                this.SortType.Value = value.VideoSortType;
                this.IsDescending.Value = value.IsVideoDescending;
                noSort = false;
                this.SetTitle();
            }).AddTo(this.disposables);

            //並び替え情報が変更されたら永続化する
            this.SortType.Subscribe(value =>
            {
                if (noSort) return;
                this.SetTitle();

                var playlist = this.current.SelectedPlaylist.Value;
                if (playlist is null) return;

                playlist.VideoSortType = value;
                this.container.Sort(value, this.IsDescending.Value, this.current.SelectedPlaylist.Value?.CustomSortSequence);
            }).AddTo(this.disposables);

            this.IsDescending.Subscribe(value =>
            {
                if (noSort) return;
                this.SetTitle();

                var playlist = this.current.SelectedPlaylist.Value;
                if (playlist is null) return;

                playlist.IsVideoDescending = value;
                this.playlistHandler.Update(playlist);
                this.container.Sort(this.SortType.Value, value, this.current.SelectedPlaylist.Value?.CustomSortSequence);
            }).AddTo(this.disposables);
        }

        #region DIされるインスタンス

        private readonly IPlaylistHandler playlistHandler;

        private readonly ICurrent current;

        private readonly IVideoListContainer container;

        #endregion

        #region field
        public const string DefaultIdColumnTitle = "ID";

        public const string DefaultTitleColumnTitle = "タイトル";

        public const string DefaultUploadColumnTitle = "投稿日";

        public const string DefaultViewCountColumnTitle = "再生回数";

        public const string DefaultDlFlagColumnTitle = "DL済み";

        private bool noSort;
        #endregion

        /// <summary>
        /// 並び替え情報
        /// </summary>
        public ReactiveProperty<VideoSortType> SortType { get; init; }

        /// <summary>
        /// 降順
        /// </summary>
        public ReactiveProperty<bool> IsDescending { get; init; }


        /// <summary>
        /// ID
        /// </summary>
        public ReactiveProperty<string> IdColumnTitle { get; init; }

        /// <summary>
        /// タイトル
        /// </summary>
        public ReactiveProperty<string> TitleColumnTitle { get; init; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        public ReactiveProperty<string> UploadColumnTitle { get; init; }

        /// <summary>
        /// 視聴回数
        /// </summary>
        public ReactiveProperty<string> ViewCountColumnTitle { get; init; }

        /// <summary>
        /// ダウンロードフラグ
        /// </summary>
        public ReactiveProperty<string> DlFlagColumnTitle { get; init; }

        /// <summary>
        /// カラムタイトルを設定する
        /// </summary>
        private void SetTitle()
        {
            var sort = this.SortType.Value;
            var dMark = this.IsDescending.Value ? "▼" : "▲";
            this.IdColumnTitle.Value = sort == VideoSortType.NiconicoID ? DefaultIdColumnTitle + dMark : DefaultIdColumnTitle;
            this.TitleColumnTitle.Value = sort == VideoSortType.Title ? DefaultTitleColumnTitle + dMark : DefaultTitleColumnTitle;
            this.UploadColumnTitle.Value = sort == VideoSortType.UploadedDT ? DefaultUploadColumnTitle + dMark : DefaultUploadColumnTitle;
            this.ViewCountColumnTitle.Value = sort == VideoSortType.ViewCount ? DefaultViewCountColumnTitle + dMark : DefaultViewCountColumnTitle;
            this.DlFlagColumnTitle.Value = sort == VideoSortType.DownloadedFlag ? DefaultDlFlagColumnTitle + dMark : DefaultDlFlagColumnTitle;
        }


    }
}
