using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Playlist;
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
            this.SelectedVideoInfo = WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex
                .Select(value =>
                {
                    if (value >= 0 && value < WS::Mainpage.VideoListContainer.Videos.Count)
                    {
                        return WS::Mainpage.VideoListContainer.Videos[value].ToString();
                    }
                    else
                    {
                        return "None";
                    }
                }).ToReadOnlyReactiveProperty().AddTo(this.disposables);

            #region コマンドの初期化
            this.SortVideosByID = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.NiconicoID)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage();
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.NiconicoID;
                    this.ShowMessage();
                }).AddTo(this.disposables);

            this.SortVideosByTitle = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.Title)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage();
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.Title;
                    this.ShowMessage();

                }).AddTo(this.disposables);

            this.SortVideosByUploadedDT = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.UploadedDT)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage();
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.UploadedDT;
                    this.ShowMessage();
                }).AddTo(this.disposables);

            this.SortVideosByViewCount = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.ViewCount)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage();
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.ViewCount;
                    this.ShowMessage();
                }).AddTo(this.disposables);

            this.SortVideosByDLFlag = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.DownloadedFlag)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage();
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.DownloadedFlag;
                    this.ShowMessage();
                }).AddTo(this.disposables);

            this.SortVideosByRegister = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == VideoSortType.Register)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage();
                        return;
                    }
                    WS::Mainpage.SortInfoHandler.SortType.Value = VideoSortType.Register;
                    this.ShowMessage();
                }).AddTo(this.disposables);

            this.MoveVideoToPrevCommand = new[]
            {
                WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.Select(value=>value>0),
                WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Select(value=>value is not null),
            }
                .CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    var index = WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.Value;
                    var result = WS::Mainpage.VideoListContainer.MovevideotoPrev(index);
                    if (!result.IsSucceeded)
                    {
                        WS::Mainpage.SnaclbarHandler.Enqueue("動画の並び替えに失敗しました。");
                        WS::Mainpage.Messagehandler.AppendMessage($"動画の並び替えに失敗しました。(詳細:{result.Message})");
                    }
                    else
                    {
                        WS::Mainpage.SnaclbarHandler.Enqueue("動画を1つ前に移動しました。");
                        WS::Mainpage.Messagehandler.AppendMessage($"動画を1つ前に移動しました。({this.SelectedVideoInfo.Value})");
                    }
                }).AddTo(this.disposables);

            this.MoveVideoToForwardCommand = new[]
            {
                WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.Select(value=>value>=0&&value+1< WS::Mainpage.VideoListContainer.Count),
                WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Select(value=>value is not null),
            }
                .CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    var index = WS::Mainpage.CurrentPlaylist.CurrentSelectedIndex.Value;
                    var result = WS::Mainpage.VideoListContainer.MovevideotoForward(index);
                    if (!result.IsSucceeded)
                    {
                        WS::Mainpage.SnaclbarHandler.Enqueue("動画の並び替えに失敗しました。");
                        WS::Mainpage.Messagehandler.AppendMessage($"動画の並び替えに失敗しました。(詳細:{result.Message})");
                    }
                    else
                    {
                        WS::Mainpage.SnaclbarHandler.Enqueue("動画を1つ後ろに移動しました。");
                        WS::Mainpage.Messagehandler.AppendMessage($"動画を1つ後ろに移動しました。({this.SelectedVideoInfo.Value})");
                    }
                }).AddTo(this.disposables);

            this.SwitchDesending = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                }).AddTo(this.disposables);
            #endregion
        }

        #region コマンド
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
        /// 動画を一つ前に移動する
        /// </summary>
        public ReactiveCommand MoveVideoToPrevCommand { get; init; }

        /// <summary>
        /// 動画を一つあとに移動する
        /// </summary>
        public ReactiveCommand MoveVideoToForwardCommand { get; init; }

        /// <summary>
        /// 昇順・降順を切り替える
        /// </summary>
        public ReactiveCommand SwitchDesending { get; init; } = new();
        #endregion

        /// <summary>
        /// 現在のソート方法
        /// </summary>
        public ReactiveProperty<string?> CurrentSortType { get; init; }

        /// <summary>
        /// 昇順・降順
        /// </summary>
        public ReactiveProperty<string?> IsDscending { get; init; }

        /// <summary>
        /// 選択されている動画情報
        /// </summary>
        public ReadOnlyReactiveProperty<string?> SelectedVideoInfo { get; init; }

        /// <summary>
        /// 並び替えメッセージを表示;
        /// </summary>
        /// <param name="sortType"></param>
        private void ShowMessage()
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

        public ReactiveCommand MoveVideoToPrevCommand { get; init; } = new();

        public ReactiveCommand MoveVideoToForwardCommand { get; init; } = new();

        public ReactiveCommand SwitchDesending { get; init; } = new();

        public ReactiveProperty<string?> CurrentSortType { get; init; } = new("再生回数");

        public ReactiveProperty<string?> IsDscending { get; init; } = new("降順");

        public ReactiveProperty<string?> SelectedVideoInfo { get; init; } = new("[sm9]レッツゴー!陰陽師");
    }
}
