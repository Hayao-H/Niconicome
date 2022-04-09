using System;
using System.Linq;
using System.Reactive.Linq;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface ISortInfoHandler
    {
        /// <summary>
        /// ソート方法
        /// </summary>
        ReactiveProperty<VideoSortType> SortType { get; }

        /// <summary>
        /// ソート方法（文字列）
        /// </summary>
        ReactiveProperty<string?> SortTypeStr { get; }

        /// <summary>
        /// 降順
        /// </summary>
        ReactiveProperty<bool> IsDescending { get; }

        /// <summary>
        /// 降順（文字列）
        /// </summary>
        ReactiveProperty<string?> IsDescendingStr { get; }

        /// <summary>
        /// ID列
        /// </summary>
        ReactiveProperty<string> IdColumnTitle { get; }

        /// <summary>
        /// タイトル列
        /// </summary>
        ReactiveProperty<string> TitleColumnTitle { get; }

        /// <summary>
        /// 投稿日時列
        /// </summary>
        ReactiveProperty<string> UploadColumnTitle { get; }

        /// <summary>
        /// 再生回数列
        /// </summary>
        ReactiveProperty<string> ViewCountColumnTitle { get; }

        /// <summary>
        /// DL済み列
        /// </summary>
        ReactiveProperty<string> DlFlagColumnTitle { get; }

        /// <summary>
        /// 状態列
        /// </summary>
        ReactiveProperty<string> StateColumnTitle { get; }

        /// <summary>
        /// エコノミー列
        /// </summary>
        ReactiveProperty<string> EconomyColumnTitle { get; }

    }

    /// <summary>
    /// ソート情報をVMに伝える
    /// この情報を参照できるのはVMのみで、例えばReflesherなどは自身に渡されたプレイリストの情報を参照する
    /// </summary>
    class SortInfoHandler : BindableBase, ISortInfoHandler
    {
        public SortInfoHandler(IPlaylistHandler playlistHandler, ICurrent current, IVideoListContainer container, ILogger logger)
        {
            this.playlistHandler = playlistHandler;
            this.current = current;
            this.container = container;
            this.logger = logger;

            this.SortType = new ReactiveProperty<VideoSortType>(current.SelectedPlaylist.Value?.VideoSortType ?? VideoSortType.Register);
            this.IsDescending = new ReactiveProperty<bool>(current.SelectedPlaylist.Value?.IsVideoDescending ?? false);
            this.IdColumnTitle = new ReactiveProperty<string>(DefaultIdColumnTitle);
            this.TitleColumnTitle = new ReactiveProperty<string>(DefaultTitleColumnTitle);
            this.UploadColumnTitle = new ReactiveProperty<string>(DefaultUploadColumnTitle);
            this.DlFlagColumnTitle = new ReactiveProperty<string>(DefaultDlFlagColumnTitle);
            this.ViewCountColumnTitle = new ReactiveProperty<string>(DefaultViewCountColumnTitle);
            this.StateColumnTitle = new ReactiveProperty<string>(DefaultStateColumnTitle);
            this.EconomyColumnTitle = new ReactiveProperty<string>(DefaultEconomyColumnTitle);
            this.SortTypeStr = this.SortType.Select(value => value switch
            {
                VideoSortType.NiconicoID => DefaultIdColumnTitle,
                VideoSortType.Title => DefaultTitleColumnTitle,
                VideoSortType.UploadedDT => DefaultUploadColumnTitle,
                VideoSortType.ViewCount => DefaultViewCountColumnTitle,
                VideoSortType.Custom => "カスタム",
                VideoSortType.State => DefaultStateColumnTitle,
                VideoSortType.Economy => DefaultEconomyColumnTitle,
                _ => "登録順",
            }).ToReactiveProperty().AddTo(this.disposables);
            this.IsDescendingStr = this.IsDescending.Select(value => value ? "降順" : "昇順").ToReactiveProperty().AddTo(this.disposables);


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
                bool isZero = this.container.Count <= 0;

                playlist.VideoSortType = value;
                this.playlistHandler.Update(playlist);

                var result = this.container.Sort(value, this.IsDescending.Value, this.current.SelectedPlaylist.Value?.CustomSortSequence);

                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"動画の並び替えに失敗しました。(type:{value}, Message:{result.Message})", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"動画の並び替えに失敗しました。(type:{value}, Message:{result.Message})");
                    }
                }
                if (!isZero && this.container.Count <= 0)
                {
                    this.logger.Error($"動画リストの動画数が0です。(type:{value})");
                }
            }).AddTo(this.disposables);

            this.IsDescending.Subscribe(value =>
            {
                if (noSort) return;
                this.SetTitle();

                var playlist = this.current.SelectedPlaylist.Value;
                if (playlist is null) return;
                bool isZero = this.container.Count <= 0;

                playlist.IsVideoDescending = value;
                this.playlistHandler.Update(playlist);

                var result = this.container.Sort(this.SortType.Value, value, this.current.SelectedPlaylist.Value?.CustomSortSequence);

                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"動画の並び替えに失敗しました。(isDescending:{value}, Message:{result.Message})", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"動画の並び替えに失敗しました。(isDescending:{value}, Message:{result.Message})");
                    }
                }
                if (!isZero && this.container.Count <= 0)
                {
                    this.logger.Error($"動画リストの動画数が0です。(type:{value})");
                }
            }).AddTo(this.disposables);
        }

        #region DIされるインスタンス

        private readonly IPlaylistHandler playlistHandler;

        private readonly ICurrent current;

        private readonly IVideoListContainer container;

        private readonly ILogger logger;

        #endregion

        #region field
        public const string DefaultIdColumnTitle = "ID";

        public const string DefaultTitleColumnTitle = "タイトル";

        public const string DefaultUploadColumnTitle = "投稿日";

        public const string DefaultViewCountColumnTitle = "再生回数";

        public const string DefaultDlFlagColumnTitle = "DL済み";

        public const string DefaultStateColumnTitle = "状態";

        public const string DefaultEconomyColumnTitle = "エコノミー";

        private bool noSort;
        #endregion

        #region Props

        public ReactiveProperty<VideoSortType> SortType { get; init; }

        public ReactiveProperty<bool> IsDescending { get; init; }

        public ReactiveProperty<string?> SortTypeStr { get; init; }

        public ReactiveProperty<string?> IsDescendingStr { get; init; }

        public ReactiveProperty<string> IdColumnTitle { get; init; }

        public ReactiveProperty<string> TitleColumnTitle { get; init; }

        public ReactiveProperty<string> UploadColumnTitle { get; init; }

        public ReactiveProperty<string> ViewCountColumnTitle { get; init; }

        public ReactiveProperty<string> DlFlagColumnTitle { get; init; }

        public ReactiveProperty<string> StateColumnTitle { get; init; }

        public ReactiveProperty<string> EconomyColumnTitle { get; init; }

        #endregion


        #region private

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
            this.StateColumnTitle.Value = sort == VideoSortType.State ? DefaultStateColumnTitle + dMark : DefaultStateColumnTitle;
            this.EconomyColumnTitle.Value = sort == VideoSortType.Economy ? DefaultEconomyColumnTitle + dMark : DefaultEconomyColumnTitle;
        }

        #endregion


    }
}
