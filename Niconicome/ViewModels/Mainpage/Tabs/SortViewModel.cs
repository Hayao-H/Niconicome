using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Mainpage.Tabs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    class SortViewModel : TabViewModelBase
    {
        public SortViewModel() : base("並び替え", "")
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
            this.SortVideos = WS::Mainpage.CurrentPlaylist.SelectedPlaylist
                .Select(value => value is not null)
                .ToReactiveCommand<VideoSortType>()
                .WithSubscribe((type) =>
                {
                    if (WS::Mainpage.SortInfoHandler.SortType.Value == type)
                    {
                        WS::Mainpage.SortInfoHandler.IsDescending.Value = !WS::Mainpage.SortInfoHandler.IsDescending.Value;
                        this.ShowMessage();
                        return;
                    }

                    WS::Mainpage.SortInfoHandler.SortType.Value =type;
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
                    var result = WS::Mainpage.VideoListContainer.MovevideotoPrev(index, commit: !WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);
                    if (!result.IsSucceeded)
                    {
                        WS::Mainpage.SnackbarHandler.Enqueue("動画の並び替えに失敗しました。");
                        WS::Mainpage.Messagehandler.AppendMessage($"動画の並び替えに失敗しました。(詳細:{result.Message})");
                    }
                    else
                    {
                        WS::Mainpage.SnackbarHandler.Enqueue("動画を1つ前に移動しました。");
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
                    var result = WS::Mainpage.VideoListContainer.MovevideotoForward(index, commit: !WS::Mainpage.CurrentPlaylist.IsTemporaryPlaylist.Value);
                    if (!result.IsSucceeded)
                    {
                        WS::Mainpage.SnackbarHandler.Enqueue("動画の並び替えに失敗しました。");
                        WS::Mainpage.Messagehandler.AppendMessage($"動画の並び替えに失敗しました。(詳細:{result.Message})");
                    }
                    else
                    {
                        WS::Mainpage.SnackbarHandler.Enqueue("動画を1つ後ろに移動しました。");
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
        /// /動画をソート
        /// </summary>
        public ReactiveCommand<VideoSortType> SortVideos { get; init; } 


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
            WS::Mainpage.SnackbarHandler.Enqueue($"動画を{sortTypeStr}の順に{orderStr}で並び替えました。");
        }
    }

    [Obsolete("デザイナー専用！", true)]
    class SortViewModelD
    {
        public ReactiveCommand<VideoSortType> SortVideos { get; init; } = new();

        public ReactiveCommand MoveVideoToPrevCommand { get; init; } = new();

        public ReactiveCommand MoveVideoToForwardCommand { get; init; } = new();

        public ReactiveCommand SwitchDesending { get; init; } = new();

        public ReactiveProperty<string?> CurrentSortType { get; init; } = new("再生回数");

        public ReactiveProperty<string?> IsDscending { get; init; } = new("降順");

        public ReactiveProperty<string?> SelectedVideoInfo { get; init; } = new("[sm9]レッツゴー!陰陽師");
    }
}
