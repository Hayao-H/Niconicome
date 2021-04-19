using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Mainpage.Utils;
using Material = MaterialDesignThemes.Wpf;
using Search = Niconicome.Models.Domain.Niconico.Search;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class SearchViewModel : BindableBase
    {
        public SearchViewModel()
        {

            #region 初期化
            //ジャンル
            var all = new ComboboxItem<Search::Genre>(Search::Genre.All, "指定しない");
            var game = new ComboboxItem<Search::Genre>(Search::Genre.Game, "ゲーム");
            var music = new ComboboxItem<Search::Genre>(Search::Genre.MusicSound, "音楽・サウンド");
            var entertainment = new ComboboxItem<Search::Genre>(Search::Genre.Entertainment, "エンターテイメント");
            var genre = new ComboboxItem<Search::Genre>(Search::Genre.Other, "その他");

            this.genreField = all;
            this.SelectableGenre = new List<ComboboxItem<Search.Genre>>() { all, game, music, entertainment, genre };

            //日付
            var dtNone = new ComboboxItem<DateTimeOffset>(default, "指定しない");
            var oneHour = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromHours(1), "1時間以内");
            var oneDay = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromHours(24), "24時間以内");
            var oneWeek = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromDays(7), "1週刊以内");
            var oneMonth = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromDays(30), "1カ月以内");
            this.uploadedDTField = dtNone;
            this.SelectableUploadedDT = new List<ComboboxItem<DateTimeOffset>>() { dtNone, oneHour, oneDay, oneWeek, oneMonth };

            //ソート
            var sortByViewCount = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.ViewCount }, "再生数が多い順");
            var sortByMylistCount = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.MylistCount }, "マイリスト数が多い順");
            var sortByCommentCount = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.CommentCount }, "コメント数が多い順");
            var sortByLength = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.Length }, "再生時間が長い順");
            var sortByUploadedTime = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.UploadedTime }, "投稿日時が新しい順");
            var sortByLastCommentTime = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.LastCommentTime }, "コメントが新しい順");

            var sortByViewCountA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.ViewCount, IsAscending = true }, "再生数が少ない順");
            var sortByMylistCountA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.MylistCount, IsAscending = true }, "マイリスト数が少ない順");
            var sortByCommentCountA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.CommentCount, IsAscending = true }, "コメント数が少ない順");
            var sortByLengthA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.Length, IsAscending = true }, "再生時間が短い順");
            var sortByUploadedTimeA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.UploadedTime, IsAscending = true }, "投稿日時が古い順");
            var sortByLastCommentTimeA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.LastCommentTime, IsAscending = true }, "コメントが古い順");
            this.sortField = sortByViewCount;
            this.SelectableSort = new List<ComboboxItem<Search.ISortOption>>() { sortByViewCount, sortByMylistCount, sortByCommentCount, sortByLength, sortByUploadedTime, sortByLastCommentTime, sortByViewCountA, sortByMylistCountA, sortByCommentCountA, sortByLengthA, sortByUploadedTimeA, sortByLastCommentTimeA };

            this.SearchResult = new ObservableCollection<IListVideoInfo>();

            this.queryField = string.Empty;
            this.messageField = new StringBuilder();
            this.Message = "初期化済み";

            this.IsTag = true;
            this.Page = 1;
            #endregion


            this.SearchCommand = new CommandBase<object>(arg => !this.isfetching, async arg =>
            {
                if (this.Query == string.Empty)
                {
                    this.MessageQueue.Enqueue("キーワードが空です！");
                    return;
                }

                this.messageField.Clear();
                this.OnPropertyChanged(nameof(this.Message));

                this.StartFetching();

                int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist?.Id ?? -1;
                var sourceVideos = WS::Mainpage.VideoListContainer.GetVideos().Select(v => v.NiconicoId) ?? new List<string>();
                var sourceVideosCount = sourceVideos.Count();

                if (playlistId == -1)
                {
                    this.Message = "プレイリストが選択されていません。";
                    this.MessageQueue.Enqueue("プレイリストが選択されていません");
                    return;
                }

                this.SearchResult.Clear();
                this.Message = "検索を開始します。";
                this.MessageQueue.Enqueue("検索を開始します");

                var query = new Search::SearchQuery()
                {
                    Query = this.Query,
                    SearchType = this.IsTag ? Search::SearchType.Tag : Search::SearchType.Keyword,
                    Genre = this.Genre.Value,
                    SortOption = this.Sort.Value,
                    UploadedDateTimeStart = this.ConfigureDateTimeManualy ? this.UploadedDT.Value == default ? null : this.UploadedDT.Value : this.UploadedDT.Value == default ? null : this.UploadedDT.Value,
                    UploadedDateTimeEnd = this.ConfigureDateTimeManualy ? null : null,
                    Page = this.Page,
                };

                var result = await WS::Mainpage.RemotePlaylistHandler.TrySearchVideosAsync(query);

                if (!result.IsSucceeded || result.Data is null)
                {
                    this.Message = $"検索に失敗しました。(詳細: {result.Message ?? "None"})";
                    this.MessageQueue.Enqueue("検索に失敗しました");
                    this.CompleteFetching();
                    return;
                }

                this.Message = $"{result.Data.Count()}件の動画が見つかりました。";
                this.MessageQueue.Enqueue($"{result.Data.Count()}件の動画が見つかりました");

                this.SearchResult.Addrange(result.Data);

                this.CompleteFetching();

            });

            this.IncrementPageCommand = new CommandBase<object>(_ => true, _ =>
            {
                ++this.Page;
                this.RaiseCanExecuteChanged();
            });

            this.DecrementPageCommand = new CommandBase<object>(_ => this.Page > 1, _ =>
            {
                if (this.Page == 1) return;
                --this.Page;
                this.RaiseCanExecuteChanged();
            });

        }

        /// <summary>
        /// 同期中フラグ
        /// </summary>
        private bool isfetching;

        private void RaiseCanExecuteChanged()
        {
            this.IncrementPageCommand.RaiseCanExecutechanged();
            this.DecrementPageCommand.RaiseCanExecutechanged();
        }

        private void StartFetching()
        {

            this.isfetching = true;
            this.SearchCommand.RaiseCanExecutechanged();
        }

        private void CompleteFetching()
        {
            this.isfetching = false;
            this.SearchCommand.RaiseCanExecutechanged();
        }

        #region フィールド
        private string queryField;

        private readonly StringBuilder messageField;

        private bool isKeyWordField;

        private bool isTagField;

        private bool configureDateTimeManualyField;

        private bool isAllSelectedField;

        private int pageField;

        private ComboboxItem<Search::ISortOption> sortField;

        private ComboboxItem<Search::Genre> genreField;

        private ComboboxItem<DateTimeOffset> uploadedDTField;
        #endregion

        /// <summary>
        /// 検索文字列
        /// </summary>
        public string Query { get => this.queryField; set => this.SetProperty(ref this.queryField, value); }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message
        {
            get => this.messageField.ToString(); set
            {
                this.messageField.AppendLine(value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// タグ検索フラグ
        /// </summary>
        public bool IsTag { get => this.isTagField; set => this.SetProperty(ref this.isTagField, value); }

        /// <summary>
        /// キーワード検索フラグ
        /// </summary>
        public bool IsKeyWord { get => this.isKeyWordField; set => this.SetProperty(ref this.isKeyWordField, value); }

        /// <summary>
        /// 日付指定フラグ
        /// </summary>
        public bool ConfigureDateTimeManualy { get => this.configureDateTimeManualyField; set => this.SetProperty(ref this.configureDateTimeManualyField, value); }

        /// <summary>
        /// 全選択フラグ
        /// </summary>
        public bool IsAllSelected { get => this.isAllSelectedField; set => this.SetProperty(ref this.isAllSelectedField, value); }

        /// <summary>
        /// ページ
        /// </summary>
        public int Page
        {
            get => this.pageField; set
            {
                if (value < 1) return;
                this.SetProperty(ref this.pageField, value);
            }
        }

        /// <summary>
        /// ソート設定
        /// </summary>
        public ComboboxItem<Search::ISortOption> Sort { get => this.sortField; set => this.SetProperty(ref this.sortField, value); }

        /// <summary>
        /// ジャンル設定
        /// </summary>
        public ComboboxItem<Search::Genre> Genre { get => this.genreField; set => this.SetProperty(ref this.genreField, value); }

        /// <summary>
        /// 投稿日時設定
        /// </summary>
        public ComboboxItem<DateTimeOffset> UploadedDT { get => this.uploadedDTField; set => this.SetProperty(ref this.uploadedDTField, value); }

        /// <summary>
        /// ソート設定一覧
        /// </summary>
        public List<ComboboxItem<Search::ISortOption>> SelectableSort { get; init; }

        /// <summary>
        /// ジャンル設定一覧
        /// </summary>
        public List<ComboboxItem<Search::Genre>> SelectableGenre { get; init; }

        /// <summary>
        /// 投稿日時設定一覧
        /// </summary>
        public List<ComboboxItem<DateTimeOffset>> SelectableUploadedDT { get; init; }

        /// <summary>
        /// 検索結果
        /// </summary>
        public ObservableCollection<IListVideoInfo> SearchResult { get; init; }

        /// <summary>
        /// メッセージキュー
        /// </summary>
        public Material::SnackbarMessageQueue MessageQueue { get; init; } = new Material::SnackbarMessageQueue();

        /// <summary>
        /// 次のページを取得
        /// </summary>
        public CommandBase<object> IncrementPageCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        /// <summary>
        /// 前のページを取得
        /// </summary>
        public CommandBase<object> DecrementPageCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        /// <summary>
        /// 検索コマンド
        /// </summary>
        public CommandBase<object> SearchCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

    }

    public class SearchViewModelD
    {
        public SearchViewModelD()
        {
            //ジャンル
            var all = new ComboboxItem<Search::Genre>(Search::Genre.All, "指定しない");
            var game = new ComboboxItem<Search::Genre>(Search::Genre.Game, "ゲーム");
            var music = new ComboboxItem<Search::Genre>(Search::Genre.MusicSound, "音楽・サウンド");
            var entertainment = new ComboboxItem<Search::Genre>(Search::Genre.Entertainment, "エンターテイメント");
            var genre = new ComboboxItem<Search::Genre>(Search::Genre.Other, "その他");

            this.Genre = all;
            this.SelectableGenre = new List<ComboboxItem<Search.Genre>>() { all, game, music, entertainment, genre };

            //日付
            var dtNone = new ComboboxItem<DateTimeOffset>(default, "指定しない");
            var oneHour = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromHours(1), "1時間以内");
            var oneDay = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromHours(24), "24時間以内");
            var oneWeek = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromDays(7), "1週刊以内");
            var oneMonth = new ComboboxItem<DateTimeOffset>(DateTime.Now - TimeSpan.FromDays(30), "1カ月以内");
            this.UploadedDT = dtNone;
            this.SelectableUploadedDT = new List<ComboboxItem<DateTimeOffset>>() { dtNone, oneHour, oneDay, oneWeek, oneMonth };

            //ソート
            var sortByViewCount = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.ViewCount }, "再生数が多い順");
            var sortByMylistCount = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.MylistCount }, "マイリスト数が多い順");
            var sortByCommentCount = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.CommentCount }, "コメント数が多い順");
            var sortByLength = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.Length }, "再生時間が長い順");
            var sortByUploadedTime = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.UploadedTime }, "投稿日時が新しい順");
            var sortByLastCommentTime = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.LastCommentTime }, "コメントが新しい順");

            var sortByViewCountA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.ViewCount, IsAscending = true }, "再生数が少ない順");
            var sortByMylistCountA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.MylistCount, IsAscending = true }, "マイリスト数が少ない順");
            var sortByCommentCountA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.CommentCount, IsAscending = true }, "コメント数が少ない順");
            var sortByLengthA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.Length, IsAscending = true }, "再生時間が短い順");
            var sortByUploadedTimeA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.UploadedTime, IsAscending = true }, "投稿日時が古い順");
            var sortByLastCommentTimeA = new ComboboxItem<Search::ISortOption>(new Search::SortOption() { Sort = Search::Sort.LastCommentTime, IsAscending = true }, "コメントが古い順");
            this.Sort = sortByViewCount;
            this.SelectableSort = new List<ComboboxItem<Search.ISortOption>>() { sortByViewCount, sortByMylistCount, sortByCommentCount, sortByLength, sortByUploadedTime, sortByLastCommentTime, sortByViewCountA, sortByMylistCountA, sortByCommentCountA, sortByLengthA, sortByUploadedTimeA, sortByLastCommentTimeA };

            this.SearchResult = new ObservableCollection<IListVideoInfo>();
            var v1 = new BindableListVIdeoInfo() { NiconicoId = "sm9", Title = "レッツゴー!陰陽師", UploadedOn = DateTime.Now, ViewCount = 1000000 };
            this.SearchResult.Add(v1);
        }

        public string Query { get; set; } = "東方";

        public string Message { get; set; } = "初期化済み";

        public bool IsTag { get; set; } = true;

        public bool IsKeyWord { get; set; } = false;

        public bool ConfigureDateTimeManualy { get; set; } = true;

        public bool IsAllSelected { get; set; } = true;

        public int Page { get; set; } = 1;

        public ComboboxItem<Search::ISortOption> Sort { get; set; }

        public ComboboxItem<Search::Genre> Genre { get; set; }

        public ComboboxItem<DateTimeOffset> UploadedDT { get; set; }

        public List<ComboboxItem<Search::ISortOption>> SelectableSort { get; init; }

        public List<ComboboxItem<Search::Genre>> SelectableGenre { get; init; }

        public List<ComboboxItem<DateTimeOffset>> SelectableUploadedDT { get; init; }

        public ObservableCollection<IListVideoInfo> SearchResult { get; init; }

        public Material::SnackbarMessageQueue MessageQueue { get; init; } = new Material::SnackbarMessageQueue();

        public CommandBase<object> IncrementPageCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> DecrementPageCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> SearchCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

    }

    class SearchComboBoxItem
    {
        public SearchComboBoxItem(string name, Search::SearchType searchType)
        {
            this.Name = name;
            this.SearchType = searchType;
        }

        /// <summary>
        /// 設定名
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 列挙値
        /// </summary>
        public Search::SearchType SearchType { get; init; }
    }
}
