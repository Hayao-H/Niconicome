using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Local;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist;
using Niconicome.Views;
using MaterialDesign = MaterialDesignThemes.Wpf;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    class DownloadSettingsViewModel : ConfigurableBase
    {


        public DownloadSettingsViewModel()
        {
            this.IsDownloadingVideoEnable = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLVideo);
            this.IsDownloadingCommentEnable = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLComment);
            this.isDownloadingCommentLogEnableField = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLKako);
            this.IsDownloadingEasyComment = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLEasy);
            this.IsDownloadingThumbEnable = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLThumb);
            this.isDownloadingOwnerCommentField = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLOwner);
            this.isOverwriteEnableField = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLOverwrite);
            this.isSkippingEnablefield = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLSkip);
            this.isCopyFromAnotherFolderEnableFIeld = WS::Mainpage.SettingHandler.GetBoolSetting(Settings.DLCopy);

            WS::Mainpage.Videodownloader.CanDownloadChange += this.OnCanDownloadChange;

            var s1 = new ResolutionSetting("1920x1080");
            var s2 = new ResolutionSetting("1280x720");
            var s3 = new ResolutionSetting("854x480");
            var s4 = new ResolutionSetting("640x360");
            var s5 = new ResolutionSetting("426x240");

            this.Resolutions = new List<ResolutionSetting>() { s1, s2, s3, s4, s5
    };
            this.selectedResolutionField = s1;

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

                   if (!this.IsDownloadingCommentEnable && (this.IsDownloadingCommentLogEnable || this.IsDownloadingEasyComment || this.IsDownloadingOwnerComment))
                   {
                       this.SnackbarMessageQueue.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                       return;
                   }

                   if (!this.IsDownloadingVideoEnable && !this.IsDownloadingCommentEnable && !this.IsDownloadingThumbEnable) return;

                   var videos = WS::Mainpage.CurrentPlaylist.Videos.Where(v => v.IsSelected).Copy();
                   if (!videos.Any()) return;

                   var cts = new CancellationTokenSource();

                   int videoCount = videos.Count();
                   var firstVideo = videos.First();
                   string folderPath = this.playlist!.Folderpath.IsNullOrEmpty() ? WS::Mainpage.SettingHandler.GetStringSetting(Settings.DefaultFolder) ?? "downloaded" : this.playlist.Folderpath;
                   var setting = new DownloadSettings
                   {
                       Video = this.IsDownloadingVideoEnable,
                       Thumbnail = this.IsDownloadingThumbEnable,
                       Overwrite = this.IsOverwriteEnable,
                       Comment = this.IsDownloadingCommentEnable,
                       DownloadLog = this.IsDownloadingCommentLogEnable,
                       DownloadEasy = this.IsDownloadingEasyComment,
                       DownloadOwner = this.IsDownloadingOwnerComment,
                       FromAnotherFolder = this.IsCopyFromAnotherFolderEnable,
                       Skip = this.IsSkippingEnable,
                       FolderPath = folderPath,
                       VerticalResolution = this.SelectedResolution.Resolution.Vertical,
                       PlaylistID = WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist?.Id ?? 0,
                   };


                   WS::Mainpage.Messagehandler.AppendMessage($"動画のダウンロードを開始します。({videoCount}件)");
                   this.SnackbarMessageQueue.Enqueue($"動画のダウンロードを開始します。({videoCount}件)");
                   WS::Mainpage.DownloadTasksHandler.StageVIdeos(videos, setting);
                   WS::Mainpage.DownloadTasksHandler.MoveStagedToQueue(t => t.PlaylistID == this.playlist.Id);

                   await WS::Mainpage.Videodownloader.DownloadVideosFriendly(m => WS::Mainpage.Messagehandler.AppendMessage(m), m => this.SnackbarMessageQueue.Enqueue(m));


               });

            this.StageVideosCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (this.playlist is null)
                {
                    this.SnackbarMessageQueue.Enqueue("プレイリストが選択されていないため、ステージできません");
                    return;
                }

                if (!this.IsDownloadingCommentEnable && (this.IsDownloadingCommentLogEnable || this.IsDownloadingEasyComment || this.IsDownloadingOwnerComment))
                {
                    this.SnackbarMessageQueue.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                    return;
                }

                if (!this.IsDownloadingVideoEnable && !this.IsDownloadingCommentEnable && !this.IsDownloadingThumbEnable) return;

                var videos = WS::Mainpage.CurrentPlaylist.Videos.Where(v => v.IsSelected).Copy();
                if (!videos.Any()) return;

                int videoCount = videos.Count();
                var firstVideo = videos.First();
                string folderPath = this.playlist!.Folderpath.IsNullOrEmpty() ? WS::Mainpage.SettingHandler.GetStringSetting(Settings.DefaultFolder) ?? "downloaded" : this.playlist.Folderpath;

                WS::Mainpage.DownloadTasksHandler.StageVIdeos(videos, new DownloadSettings
                {
                    Video = this.IsDownloadingVideoEnable,
                    Thumbnail = this.IsDownloadingThumbEnable,
                    Overwrite = this.IsOverwriteEnable,
                    Comment = this.IsDownloadingCommentEnable,
                    DownloadLog = this.IsDownloadingCommentLogEnable,
                    DownloadEasy = this.IsDownloadingEasyComment,
                    DownloadOwner = this.IsDownloadingOwnerComment,
                    FromAnotherFolder = this.IsCopyFromAnotherFolderEnable,
                    Skip = this.IsSkippingEnable,
                    FolderPath = folderPath,
                    VerticalResolution = this.SelectedResolution.Resolution.Vertical,
                    PlaylistID = WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist?.Id ?? 0,
                });

                this.SnackbarMessageQueue.Enqueue($"{videos.Count()}件の動画をステージしました。", "管理画面を開く", () => {
                    var windows = new DownloadTasksWindows();
                    windows.Show();
                });
            });

            this.CancelCommand = new CommandBase<object>(_ => this.IsDownloading, _ =>
            {
                WS::Mainpage.Videodownloader.Cancel();
            });

            WS::Mainpage.CurrentPlaylist.SelectedItemChanged += this.OnSelectedChanged;
        }

        ~DownloadSettingsViewModel()
        {
            WS::Mainpage.Videodownloader.CanDownloadChange -= this.OnCanDownloadChange;
        }

        private bool isDownloadingVideoEnableField = true;

        private bool isDownloadingCommentEnableField;

        private bool isDownloadingCommentLogEnableField;

        private bool isDownloadingOwnerCommentField;

        private bool isDownloadingEasyCommentFIeld;

        private bool isDownloadingThumbEnableField;

        private bool isOverwriteEnableField;

        private bool isSkippingEnablefield;

        private bool isCopyFromAnotherFolderEnableFIeld;

        private ResolutionSetting selectedResolutionField;

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
        public bool IsDownloadingVideoEnable { get => this.isDownloadingVideoEnableField; set => this.Savesetting(ref this.isDownloadingVideoEnableField, value, Settings.DLVideo); }

        /// <summary>
        /// コメントダウンロードフラグ
        /// </summary>
        public bool IsDownloadingCommentEnable { get => this.isDownloadingCommentEnableField; set => this.Savesetting(ref this.isDownloadingCommentEnableField, value, Settings.DLComment); }

        /// <summary>
        /// 過去ログダウンロードフラグ
        /// </summary>
        public bool IsDownloadingCommentLogEnable { get => this.isDownloadingCommentLogEnableField; set => this.Savesetting(ref this.isDownloadingCommentLogEnableField, value, Settings.DLKako); }

        /// <summary>
        /// 投稿者コメント
        /// </summary>
        public bool IsDownloadingOwnerComment { get => this.isDownloadingOwnerCommentField; set => this.Savesetting(ref this.isDownloadingOwnerCommentField, value, Settings.DLOwner); }

        /// <summary>
        /// かんたんコメント
        /// </summary>
        public bool IsDownloadingEasyComment { get => this.isDownloadingEasyCommentFIeld; set => this.Savesetting(ref this.isDownloadingEasyCommentFIeld, value, Settings.DLEasy); }

        /// <summary>
        /// サムネイルダウンロードフラグ
        /// </summary>
        public bool IsDownloadingThumbEnable { get => this.isDownloadingThumbEnableField; set => this.Savesetting(ref this.isDownloadingThumbEnableField, value, Settings.DLThumb); }

        /// <summary>
        /// 上書き保存フラグ
        /// </summary>
        public bool IsOverwriteEnable { get => this.isOverwriteEnableField; set => this.Savesetting(ref this.isOverwriteEnableField, value, Settings.DLOverwrite); }

        /// <summary>
        /// ダウンロード済をスキップ
        /// </summary>
        public bool IsSkippingEnable { get => this.isSkippingEnablefield; set => this.Savesetting(ref this.isSkippingEnablefield, value, Settings.DLSkip); }

        /// <summary>
        /// 別フォルダーからコピー
        /// </summary>
        public bool IsCopyFromAnotherFolderEnable { get => this.isCopyFromAnotherFolderEnableFIeld; set => this.Savesetting(ref this.isCopyFromAnotherFolderEnableFIeld, value, Settings.DLCopy); }


        /// <summary>
        /// 選択中の解像度
        /// </summary>
        public ResolutionSetting SelectedResolution { get => this.selectedResolutionField; set => this.SetProperty(ref this.selectedResolutionField, value); }

        /// <summary>
        /// 解像度一覧
        /// </summary>
        public List<ResolutionSetting> Resolutions { get; init; }

        /// <summary>
        /// スナックバー
        /// </summary>
        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }

        private void RaiseCanExecuteChange()
        {
            this.DownloadCommand.RaiseCanExecutechanged();
            this.CancelCommand.RaiseCanExecutechanged();
        }

        private void OnSelectedChanged(object? sender, EventArgs e)
        {
            if (WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist is not null)
            {
                this.playlist = WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist;
                this.RaiseCanExecuteChange();
            }
        }

        private void OnCanDownloadChange(object? sender,EventArgs e)
        {
            this.RaiseCanExecuteChange();
        }
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
