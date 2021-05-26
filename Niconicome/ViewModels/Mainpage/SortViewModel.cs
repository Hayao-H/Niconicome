using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.Types;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    class SortViewModel : BindableBase
    {
        public SortViewModel()
        {
            this.CurrentSortType = WS::Mainpage.SortInfoHandler.SortTypeStr.ToReactiveProperty().AddTo(this.disposables);
            this.IsDscending = WS::Mainpage.SortInfoHandler.IsDescendingStr.ToReactiveProperty().AddTo(this.disposables);

            #region コマンドの初期化
            this.SortVideosByID = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.NiconicoID)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage(VideoSortType.NiconicoID);
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.NiconicoID;
                    this.ShowMessage(VideoSortType.NiconicoID);
                });

            this.SortVideosByTitle = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.Title)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.Title;

                });

            this.SortVideosByUploadedDT = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.UploadedDT)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.UploadedDT;
                });

            this.SortVideosByViewCount = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.ViewCount)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.ViewCount;
                });

            this.SortVideosByDLFlag = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.DownloadedFlag)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.DownloadedFlag;
                });

            this.SortVideosByRegister = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.Register)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.Register;
                });
            #endregion
        }

        /// <summary>
        /// IDでソート
        /// </summary>
        public ReactiveCommand SortVideosByID { get; init; }

        /// <summary>
        /// タイトルでソート
        /// </summary>
        public ReactiveCommand SortVideosByTitle { get; init; }

        /// <summary>
        /// 投稿日時でソート
        /// </summary>
        public ReactiveCommand SortVideosByUploadedDT { get; init; }

        /// <summary>
        /// 再生回数でソート
        /// </summary>
        public ReactiveCommand SortVideosByViewCount { get; init; }

        /// <summary>
        /// DL済みでソート
        /// </summary>
        public ReactiveCommand SortVideosByDLFlag { get; init; }

        /// <summary>
        /// 登録順でソート
        /// </summary>
        public ReactiveCommand SortVideosByRegister { get; init; }

        /// <summary>
        /// 現在のソート方法
        /// </summary>
        public ReactiveProperty<string?> CurrentSortType { get; init; }

        /// <summary>
        /// 昇順・降順
        /// </summary>
        public ReactiveProperty<string?> IsDscending { get; init; } = new("降順");

        /// <summary>
        /// 並び替えメッセージを表示;
        /// </summary>
        /// <param name="sortType"></param>
        private void ShowMessage(VideoSortType sortType)
        {
            var sortTypeStr = this.CurrentSortType.Value;
            var orderStr = this.IsDscending.Value;

            WS::Mainpage.Messagehandler.AppendMessage($"動画を{sortTypeStr}の順に{orderStr}で並び替えました。");
            WS::Mainpage.SnaclbarHandler.Enqueue($"動画を{sortTypeStr}の順に{orderStr}で並び替えました。");
        }


    }

    [Obsolete("デザイナー専用！", true)]
    class SortViewModelD
    {
        public ReactiveCommand SortVideosByID { get; init; } = new();

        public ReactiveCommand SortVideosByTitle { get; init; } = new();

        public ReactiveCommand SortVideosByUploadedDT { get; init; } = new();

        public ReactiveCommand SortVideosByViewCount { get; init; } = new();

        public ReactiveCommand SortVideosByDLFlag { get; init; } = new();

        public ReactiveCommand SortVideosByRegister { get; init; } = new();

        public ReactiveProperty<string?> CurrentSortType { get; init; } = new("再生回数");

        public ReactiveProperty<string?> IsDscending { get; init; } = new("降順");
    }
}
