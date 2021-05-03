using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.Views;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using MaterialDesign = MaterialDesignThemes.Wpf;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    class DownloadSettingsViewModel : ConfigurableBase
    {


        public DownloadSettingsViewModel()
        {
            this.IsDownloadingVideoInfoEnable = WS::Mainpage.DownloadSettingsHandler.IsDownloadingVideoInfoEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsLimittingCommentCountEnable = WS::Mainpage.DownloadSettingsHandler.IsLimittingCommentCountEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsDownloadingVideoEnable = WS::Mainpage.DownloadSettingsHandler.IsDownloadingVideoEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsDownloadingCommentEnable = WS::Mainpage.DownloadSettingsHandler.IsDownloadingCommentEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsDownloadingCommentLogEnable = WS::Mainpage.DownloadSettingsHandler.IsDownloadingCommentLogEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsDownloadingEasyComment = WS::Mainpage.DownloadSettingsHandler.IsDownloadingEasyComment.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsDownloadingThumbEnable = WS::Mainpage.DownloadSettingsHandler.IsDownloadingThumbEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsDownloadingOwnerComment = WS::Mainpage.DownloadSettingsHandler.IsDownloadingOwnerComment.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsOverwriteEnable = WS::Mainpage.DownloadSettingsHandler.IsOverwriteEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsSkippingEnable = WS::Mainpage.DownloadSettingsHandler.IsSkippingEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsCopyFromAnotherFolderEnable = WS::Mainpage.DownloadSettingsHandler.IsCopyFromAnotherFolderEnable.ToReactivePropertyAsSynchronized(x => x.Value);
            this.MaxCommentsCount = WS::Mainpage.DownloadSettingsHandler.MaxCommentsCount.ToReactivePropertyAsSynchronized(x => x.Value);

            WS::Mainpage.Videodownloader.CanDownloadChange += this.OnCanDownloadChange;

            var s1 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("1920x1080"), "1080px");
            var s2 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("1280x720"), "720px");
            var s3 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("854x480"), "480px");
            var s4 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("640x360"), "360px");
            var s5 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("426x240"), "240px");

            this.Resolutions = new List<ComboboxItem<VideoInfo::IResolution>>() { s1, s2, s3, s4, s5 };
            this.SelectedResolution = WS::Mainpage.DownloadSettingsHandler.Resolution
                .ToReactivePropertyAsSynchronized(
                x => x.Value,
                x => x.Vertical switch
                {
                    1080 => s1,
                    720 => s2,
                    480 => s3,
                    360 => s4,
                    240 => s5,
                    _ => s1
                }, x => x.Value);

            this.SnackbarMessageQueue = WS::Mainpage.SnaclbarHandler.Queue;

            this.DownloadCommand = new CommandBase<object>(_ => this.playlist is not null && !this.IsDownloading, async _ =>
               {
                   if (this.playlist is null)
                   {
                       this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、ダウンロードできません");
                       return;
                   }

                   if (!WS::Mainpage.Session.IsLogin)
                   {
                       this.SnackbarMessageQueue.Enqueue("動画をダウンロードするにはログインが必要です。");
                       return;
                   }

                   if (!this.IsDownloadingCommentEnable.Value && (this.IsDownloadingCommentLogEnable.Value || this.IsDownloadingEasyComment.Value || this.IsDownloadingOwnerComment.Value))
                   {
                       this.SnackbarMessageQueue.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                       return;
                   }

                   if (!this.IsDownloadingVideoEnable.Value && !this.IsDownloadingCommentEnable.Value && !this.IsDownloadingThumbEnable.Value && !this.IsDownloadingVideoInfoEnable.Value) return;

                   var videos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value).Copy();
                   if (!videos.Any()) return;

                   var cts = new CancellationTokenSource();

                   int videoCount = videos.Count();
                   var firstVideo = videos.First();
                   var allowDupe = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.AllowDupeOnStage);
                   var setting = WS::Mainpage.DownloadSettingsHandler.CreateDownloadSettings();
                   var dlFromQueue = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.DLAllFromQueue);


                   WS::Mainpage.DownloadTasksHandler.StageVIdeos(videos, setting, allowDupe);

                   if (dlFromQueue)
                   {
                       WS::Mainpage.DownloadTasksHandler.MoveStagedToQueue();
                   }
                   else
                   {
                       WS::Mainpage.DownloadTasksHandler.MoveStagedToQueue(t => t.PlaylistID == this.playlist.Id);
                   }

                   await WS::Mainpage.Videodownloader.DownloadVideosFriendly(m => WS::Mainpage.Messagehandler.AppendMessage(m), m => this.SnackbarMessageQueue.Enqueue(m));


               });

            this.StageVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (this.playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、ステージできません");
                    return;
                }

                if (!this.IsDownloadingCommentEnable.Value && (this.IsDownloadingCommentLogEnable.Value || this.IsDownloadingEasyComment.Value || this.IsDownloadingOwnerComment.Value))
                {
                    this.SnackbarMessageQueue.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                    return;
                }

                if (!this.IsDownloadingVideoEnable.Value && !this.IsDownloadingCommentEnable.Value && !this.IsDownloadingThumbEnable.Value && !this.IsDownloadingVideoInfoEnable.Value) return;

                var videos = WS::Mainpage.VideoListContainer.Videos.Where(v => v.IsSelected.Value).Copy();
                if (!videos.Any()) return;

                int videoCount = videos.Count();
                var firstVideo = videos.First();
                var allowDupe = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.AllowDupeOnStage);

                WS::Mainpage.DownloadTasksHandler.StageVIdeos(videos, WS::Mainpage.DownloadSettingsHandler.CreateDownloadSettings(), allowDupe);

                this.SnackbarMessageQueue.Enqueue($"{videos.Count()}件の動画をステージしました。", "管理画面を開く", () =>
                {
                    var windows = new DownloadTasksWindows();
                    windows.Show();
                });
            });

            this.CancelCommand = new CommandBase<object>(_ => this.IsDownloading, _ =>
            {
                WS::Mainpage.Videodownloader.Cancel();
            });

            WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Subscribe(_ => this.OnSelectedChanged());
        }

        ~DownloadSettingsViewModel()
        {
            WS::Mainpage.Videodownloader.CanDownloadChange -= this.OnCanDownloadChange;
        }

        private ITreePlaylistInfo? playlist;

        /// <summary>
        /// ダウンロードフラグ
        /// </summary>
        public bool IsDownloading { get => !WS::Mainpage.Videodownloader.CanDownload; }

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        public CommandBase<object> DownloadCommand { get; init; }

        /// <summary>
        /// ダウンロードをキャンセルする
        /// </summary>
        public CommandBase<object> CancelCommand { get; init; }

        /// <summary>
        /// ステージする
        /// </summary>
        public CommandBase<object> StageVideosCommand { get; init; }

        /// <summary>
        /// 動画ダウンロードフラグ
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingVideoEnable { get; init; }

        /// <summary>
        /// コメントダウンロードフラグ
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingCommentEnable { get; init; }

        /// <summary>
        /// 過去ログダウンロードフラグ
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingCommentLogEnable { get; init; }

        /// <summary>
        /// 投稿者コメント
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingOwnerComment { get; init; }

        /// <summary>
        /// かんたんコメント
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingEasyComment { get; init; }

        /// <summary>
        /// サムネイルダウンロードフラグ
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingThumbEnable { get; init; }

        /// <summary>
        /// 動画情報
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingVideoInfoEnable { get; init; }


        /// <summary>
        /// 上書き保存フラグ
        /// </summary>
        public ReactiveProperty<bool> IsOverwriteEnable { get; init; }

        /// <summary>
        /// ダウンロード済をスキップ
        /// </summary>
        public ReactiveProperty<bool> IsSkippingEnable { get; init; }

        /// <summary>
        /// 別フォルダーからコピー
        /// </summary>
        public ReactiveProperty<bool> IsCopyFromAnotherFolderEnable { get; init; }

        /// <summary>
        /// コメント取得数を制限する
        /// </summary>
        public ReactiveProperty<bool> IsLimittingCommentCountEnable { get; init; }

        /// <summary>
        /// コメントの最大取得数
        /// </summary>
        public ReactiveProperty<int> MaxCommentsCount { get; init; }

        /// <summary>
        /// 選択中の解像度
        /// </summary>
        public ReactiveProperty<ComboboxItem<VideoInfo::IResolution>> SelectedResolution { get; init; }

        /// <summary>
        /// 解像度一覧
        /// </summary>
        public List<ComboboxItem<VideoInfo::IResolution>> Resolutions { get; init; }

        /// <summary>
        /// スナックバー
        /// </summary>
        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }

        private void RaiseCanExecuteChange()
        {
            this.DownloadCommand.RaiseCanExecutechanged();
            this.CancelCommand.RaiseCanExecutechanged();
        }

        private void OnSelectedChanged()
        {
            if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value is not null)
            {
                this.playlist = WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value;
                this.RaiseCanExecuteChange();
            }
        }

        private void OnCanDownloadChange(object? sender, EventArgs e)
        {
            this.RaiseCanExecuteChange();
        }
    }

    class DownloadSettingsViewModelD
    {
        public DownloadSettingsViewModelD()
        {
            var s1 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("1920x1080"), "1080px");
            var s2 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("1280x720"), "720px");
            var s3 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("854x480"), "480px");
            var s4 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("640x360"), "360px");
            var s5 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("426x240"), "240px");

            this.Resolutions = new List<ComboboxItem<VideoInfo::IResolution>>() { s1, s2, s3, s4, s5 };

            this.SelectedResolution = new ReactiveProperty<ComboboxItem<VideoInfo::IResolution>>(s1);
        }

        public bool IsDownloading { get => false; }

        public CommandBase<object> DownloadCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> CancelCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> StageVideosCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public ReactiveProperty<bool> IsDownloadingVideoEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsDownloadingCommentEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsDownloadingCommentLogEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsDownloadingOwnerComment { get; set; } = new(true);

        public ReactiveProperty<bool> IsDownloadingEasyComment { get; set; } = new(true);

        public ReactiveProperty<bool> IsDownloadingThumbEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsOverwriteEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsSkippingEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsCopyFromAnotherFolderEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsLimittingCommentCountEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsDownloadingVideoInfoEnable { get; set; } = new(true);

        public ReactiveProperty<int> MaxCommentsCount { get; set; } = new(2000);

        public ReactiveProperty<ComboboxItem<VideoInfo::IResolution>> SelectedResolution { get; set; }

        public List<ComboboxItem<VideoInfo::IResolution>> Resolutions { get; init; }

        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; } = new MaterialDesign::SnackbarMessageQueue();
    }

    class ResolutionSetting
    {
        public ResolutionSetting(string resolution)
        {
            this.Resolution = new VideoInfo::Resolution(resolution);
            this.DisplayValue = this.Resolution.Vertical.ToString() + "px";
        }

        public VideoInfo::IResolution Resolution { get; init; }

        public string DisplayValue { get; init; }
    }
}
