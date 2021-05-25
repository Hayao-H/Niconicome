using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.Types;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    class SortViewModel
    {
        public SortViewModel()
        {
            this.SortVideosByID = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.NiconicoID)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.NiconicoID;
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
    }
}
