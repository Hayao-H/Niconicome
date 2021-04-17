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

                    var result = await WS::Mainpage.RemotePlaylistHandler.TrySearchVideosAsync(inputField, this.CurrentSetting.SearchType, this.Page);

                    if (!result.IsSucceeded || result.Videos is null)
                    {
                        this.Message = $"検索に失敗しました。(詳細: {result.Message ?? "None"})";
                        this.CompleteFetching();
                        return;
                    }

                    this.Message = $"{result.Videos.Count()}件の動画が見つかりました。";

                    var dupe = result.Videos.Select(v => v.Id).Where(v => sourceVideos.Contains(v));
                    this.Message = $"{dupe.Count()}件の動画が既に登録されているのでスキップします。";

                    var videos = await WS::Mainpage.NetworkVideoHandler.GetVideoListInfosAsync(result.Videos.Select(v => v.Id).Where(v => !dupe.Contains(v)));
                    WS::Mainpage.VideoListContainer.AddRange(videos, playlistId);

                    if (sourceVideosCount > videos.Count())
                    {
                        this.Message = $"{sourceVideosCount-videos.Count()}件の動画の登録に失敗しました。";
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
