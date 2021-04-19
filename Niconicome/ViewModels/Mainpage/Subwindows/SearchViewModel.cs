using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WS = Niconicome.Workspaces;
using Playlist = Niconicome.Models.Playlist;
using Search = Niconicome.Models.Domain.Niconico.Search;
using NetWork = Niconicome.Models.Network;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Mainpage.Utils;
using Material = MaterialDesignThemes.Wpf;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class SearchViewModel : BindableBase
    {
        public SearchViewModel()
        {

            var keyword = new SearchComboBoxItem("キーワード", Search.SearchType.Keyword);
            var tag = new SearchComboBoxItem("タグ", Search.SearchType.Tag);

            this.SearchSettings = new ObservableCollection<SearchComboBoxItem>()
            {
                keyword,tag
            };

            this.currentSettingField = keyword;

            this.inputField = string.Empty;


            this.SearchCommand = new CommandBase<Window>(arg => !this.isfetching, async arg =>
            {
                if (this.Input == string.Empty || this.CurrentSetting is null) return;

                if (arg is null) return;

                if (arg is Window window)
                {
                    this.messageField.Clear();
                    this.OnPropertyChanged(nameof(this.Message));

                    this.StartFetching();

                    int playlistId = WS::Mainpage.CurrentPlaylist.SelectedPlaylist?.Id ?? -1;
                    var sourceVideos = WS::Mainpage.VideoListContainer.GetVideos().Select(v => v.NiconicoId) ?? new List<string>();
                    var sourceVideosCount = sourceVideos.Count();

                    if (playlistId == -1) return;

                    var result = await WS::Mainpage.RemotePlaylistHandler.TrySearchVideosAsync(new Search::SearchQuery());

                    if (!result.IsSucceeded || result.Data is null)
                    {
                        this.Message = $"検索に失敗しました。(詳細: {result.Message ?? "None"})";
                        this.CompleteFetching();
                        return;
                    }

                    this.Message = $"{result.Data.Count()}件の動画が見つかりました。";

                    var dupe = result.Data.Select(v => v.NiconicoId).Where(v => sourceVideos.Contains(v));
                    this.Message = $"{dupe.Count()}件の動画が既に登録されているのでスキップします。";

                    var videos = await WS::Mainpage.NetworkVideoHandler.GetVideoListInfosAsync(result.Data.Select(v => v.NiconicoId).Where(v => !dupe.Contains(v)));
                    WS::Mainpage.VideoListContainer.AddRange(videos, playlistId);

                    if (sourceVideosCount > videos.Count())
                    {
                        this.Message = $"{sourceVideosCount - videos.Count()}件の動画の登録に失敗しました。";
                    }

                    WS::Mainpage.VideoListContainer.Refresh();

                    this.CompleteFetching();
                }



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

        //検索コマンド
        public CommandBase<Window> SearchCommand { get; init; }

        /// <summary>
        /// 次のページを取得
        /// </summary>
        public CommandBase<object> IncrementPageCommand { get; init; }

        /// <summary>
        /// 前のページを取得
        /// </summary>
        public CommandBase<object> DecrementPageCommand { get; init; }

        /// <summary>
        /// 設定一覧
        /// </summary>
        public ObservableCollection<SearchComboBoxItem> SearchSettings { get; init; }

        private SearchComboBoxItem currentSettingField;

        private string inputField;

        private int pageField = 1;

        /// <summary>
        /// ページ
        /// </summary>
        public int Page { get => this.pageField; set => this.SetProperty(ref this.pageField, value); }

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

        /// <summary>
        /// 入力文字
        /// </summary>
        public string Input { get => this.inputField; set => this.SetProperty(ref this.inputField, value); }

        /// <summary>
        /// 現在の設定
        /// </summary>
        public SearchComboBoxItem CurrentSetting
        {
            get => this.currentSettingField; set => this.SetProperty(ref this.currentSettingField, value);
        }

        private readonly StringBuilder messageField = new();

        /// <summary>
        /// 状態を表すメッセージ
        /// </summary>
        public string Message { get => this.messageField.ToString(); set { this.messageField.AppendLine(value); this.OnPropertyChanged(); } }
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
        }

        public string Query { get; set; } = "東方";

        public string Message { get; set; } = "初期化済み";

        public bool IsTag { get; set; } = true;

        public bool IsKeyWord { get; set; } = false;

        public bool ConfigureDateTimeManualy { get; set; } = true;

        public int Page { get; set; } = 1;

        public ComboboxItem<Search::ISortOption> Sort { get; set; }

        public ComboboxItem<Search::Genre> Genre { get; set; }

        public ComboboxItem<DateTimeOffset> UploadedDT { get; set; }

        public List<ComboboxItem<Search::ISortOption>> SelectableSort { get; init; }

        public List<ComboboxItem<Search::Genre>> SelectableGenre { get; init; }

        public List<ComboboxItem<DateTimeOffset>> SelectableUploadedDT { get; init; }

        public Material::SnackbarMessageQueue MessageQueue { get; init; } = new Material::SnackbarMessageQueue();

        public CommandBase<object> IncrementPageCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> DecrementPageCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });
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
