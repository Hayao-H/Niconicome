using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            this.SelectableSearchType = new List<string>() { "タグ", "キーワード" };
            this.SelectableGenre = new List<string>() { "指定無し", "ゲーム", "音楽・サウンド", "アニメ", "エンターテインメント", "ダンス", "その他" };
            this.SelectableSortType = new List<string>() { "投稿日時が新しい順", "再生数が多い順", "マイリスト数が多い順", "コメントが新しい順", "コメントが古い順", "再生数が少ない順", "コメント数が多い順", "コメント数が少ない順", "マイリスト数が少ない順", "投稿日時が古い順", "再生時間が長い順", "再生時間が短い順" };

            this.IsRegisterIsProcessing = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.IsAlertShwon = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.AlertMessage = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.InputText = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.Page = new BindableProperty<int>(1).AddTo(this.Bindables);
            this.Videos = new BindableCollection<RemoteVideoViewModel, Remote.VideoInfo>(this._videos, v => new RemoteVideoViewModel(v));
            this.Bindables.Add(this.Videos);
        }

        #region field

        private readonly ObservableCollection<Remote::VideoInfo> _videos = new();

        private string _title = string.Empty;

        private bool _isSearching;

        private readonly List<Timer> _timers = new();

        #endregion

        #region Props

        /// <summary>
        /// 検索タイプ
        /// </summary>
        public List<string> SelectableSearchType { get; init; }

        /// <summary>
        /// ソート
        /// </summary>
        public List<string> SelectableSortType { get; init; }

        /// <summary>
        /// ジャンル
        /// </summary>
        public List<string> SelectableGenre { get; init; }

        /// <summary>
        /// 検索タイプ
        /// </summary>
        public string SelectedSearchType { get; set; } = string.Empty;

        /// <summary>
        /// ソート
        /// </summary>
        public string SelectedSortType { get; set; } = string.Empty;

        /// <summary>
        /// ジャンル
        /// </summary>
        public string SelectedGenre { get; set; } = string.Empty;

        /// <summary>
        /// 入力値
        /// </summary>
        public IBindableProperty<string> InputText { get; set; }

        /// <summary>
        /// ページ
        /// </summary>
        public IBindableProperty<int> Page { get; set; }

        /// <summary>
        /// アラートの表示・非表示
        /// </summary>
        public IBindableProperty<bool> IsAlertShwon { get; init; }

        /// <summary>
        /// アラートメッセージ
        /// </summary>
        public IBindableProperty<string> AlertMessage { get; init; }

        /// <summary>
        /// 登録中
        /// </summary>
        public IBindableProperty<bool> IsRegisterIsProcessing { get; init; }

        /// <summary>
        /// 取得した動画
        /// </summary>
        public BindableCollection<RemoteVideoViewModel, Remote::VideoInfo> Videos;

        /// <summary>
        /// 変更監視
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        #endregion

        #region Method

        public async Task OnSearchButtonClick()
        {
            if (this._isSearching)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.InputText.Value))
            {
                this.ShowAlert(WS.StringHandler.GetContent(SearchVMSC.InputIsEmpty));
                return;
            }

            this._isSearching = true;
            this._videos.Clear();

            Remote::Search.SortOption sort = this.ConvertToSortOption(this.SelectedSortType);
            Remote::Search.Genre genre = this.ConvertToGenre(this.SelectedGenre);
            Remote::Search.SearchType searchType = this.SelectedSearchType == "タグ" ? Remote::Search.SearchType.Tag : Remote::Search.SearchType.Keyword;

            IAttemptResult<Remote::RemotePlaylistInfo> result = await WS.SearchManager.SearchVideosAsync(new Remote.Search.SearchQuery(searchType, genre, sort, this.InputText.Value, Page: this.Page.Value));

            if (!result.IsSucceeded || result.Data is null)
            {
                this.ShowAlert(WS.StringHandler.GetContent(SearchVMSC.FailedToSearch, result.Message));
                this._isSearching = false;
                return;
            }

            this._title = result.Data.PlaylistName;
            this._videos.AddRange(result.Data.Videos);
            this._isSearching = false;
        }

        public async Task OnRegisterButtonClick()
        {
            if (this._videos.Count == 0)
            {
                this.ShowAlert(WS.StringHandler.GetContent(SearchVMSC.ListIsEmpty));
                return;
            }

            this.IsRegisterIsProcessing.Value = true;
            await WS.SearchManager.RegisterVideosAsync(this._title, this.Videos.Where(v => v.IsSelected).Select(v => v.Video).ToList());

            WS.BlazorPageManager.RequestBlazorToNavigate("/videos");
        }

        public void OnBackButtonClick()
        {
            WS.BlazorPageManager.RequestBlazorToNavigate("/videos");
        }

        public void OnPreviousButtonClick()
        {
            if (this.Page.Value == 1)
            {
                return;
            }

            this.Page.Value--;
            _ = this.OnSearchButtonClick();
        }

        public void OnNextButtonClick()
        {
            this.Page.Value++;
            _ = this.OnSearchButtonClick();
        }

        #endregion

        #region private

        private Remote::Search.SortOption ConvertToSortOption(string sortString)
        {
            Remote::Search.Sort sort = sortString switch
            {
                var x when x.StartsWith("再生数") => Remote::Search.Sort.ViewCount,
                var x when x.StartsWith("マイリスト数") => Remote::Search.Sort.MylistCount,
                var x when x.StartsWith("コメント数") => Remote::Search.Sort.MylistCount,
                var x when x.StartsWith("再生時間") => Remote::Search.Sort.Length,
                var x when x.StartsWith("投稿日時") => Remote::Search.Sort.UploadedTime,
                var x when x.StartsWith("コメントが") => Remote::Search.Sort.LastCommentTime,
                _ => Remote::Search.Sort.ViewCount,
            };

            bool isAscending = sortString switch
            {
                var x when x.EndsWith("古い順") || x.EndsWith("少ない順") || x.EndsWith("短い順") => true,
                _ => false
            };

            return new Remote::Search.SortOption(sort, isAscending);
        }

        private Remote::Search.Genre ConvertToGenre(string genreString)
        {
            return genreString switch
            {
                "指定無し" => Remote::Search.Genre.All,
                "ゲーム" => Remote::Search.Genre.Game,
                "音楽・サウンド" => Remote::Search.Genre.MusicSound,
                "アニメ" => Remote::Search.Genre.Anime,
                "エンターテインメント" => Remote::Search.Genre.Entertainment,
                "ダンス" => Remote::Search.Genre.Dance,
                "その他" => Remote::Search.Genre.Other,
                _ => Remote::Search.Genre.All,
            };
        }

        /// <summary>
        /// アラートを表示
        /// </summary>
        /// <param name="message"></param>
        private void ShowAlert(string message)
        {
            this.AlertMessage.Value = message;
            this.IsAlertShwon.Value = true;

            var timer = new Timer(5000);
            timer.AutoReset = false;
            timer.Elapsed += (_, _) =>
            {
                this.IsAlertShwon.Value = false;
                this._timers.Remove(timer);
            };

            this._timers.Add(timer);
            timer.Enabled = true;
        }

        #endregion
    }

    public class RemoteVideoViewModel
    {
        public RemoteVideoViewModel(Remote::VideoInfo video)
        {
            this.NiconicoID = video.NiconicoID;
            this.Title = video.Title;
            this.ThumbUrl = video.ThumbUrl;
            this.UploadedDT = video.UploadedDT.ToString("yyyy/MM/dd HH:mm");

            var m = $"{video.Duration / 60}".PadLeft(2, '0');
            var s = $"{video.Duration % 60}".PadLeft(2, '0');

            this.Duration = $"{m}:{s}";
            this.ViewCount = video.ViewCount.ToString("#,0");
            this.CommentCount = video.CommentCount.ToString("#,0");
            this.MylistCount = video.MylistCount.ToString("#,0");
            this.LikeCount = video.LikeCount.ToString("#,0");

            this.Video = video;
        }

        public string NiconicoID { get; init; }

        public string Title { get; init; }

        public string ThumbUrl { get; init; }

        public string UploadedDT { get; init; }

        public string Duration { get; init; }

        public string ViewCount { get; init; }

        public string CommentCount { get; init; }

        public string MylistCount { get; init; }

        public string LikeCount { get; init; }

        public bool IsSelected { get; set; } = true;

        public Remote::VideoInfo Video { get; init; }
    }
}
