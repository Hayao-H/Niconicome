using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Extensions.System;
using Niconicome.Models.Local;
using Niconicome.Models.Network;
using Niconicome.Models.Playlist;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;
using WS = Niconicome.Workspaces;
using MaterialDesign = MaterialDesignThemes.Wpf;
using Niconicome.Extensions.System.List;

namespace Niconicome.ViewModels.Mainpage
{
    class DownloadSettingsViewModel : BindableBase
    {


        public DownloadSettingsViewModel()
        {
            this.IsDownloadingVideoEnable = true;
            this.IsDownloadingCommentEnable = true;
            this.IsDownloadingEasyComment = true;
            this.IsDownloadingThumbEnable = true;

            var s1 = new ResolutionSetting("1920x1080");
            var s2 = new ResolutionSetting("1280x720");
            var s3 = new ResolutionSetting("854x480");
            var s4 = new ResolutionSetting("640x360");
            var s5 = new ResolutionSetting("426x240");

            this.Resolutions = new List<ResolutionSetting>() { s1, s2, s3, s4, s5 };
            this.selectedResolutionField = s1;

            this.SnackbarMessageQueue = WS::Mainpage.SnackbarMessageQueue;

            this.DownloadCommand = new CommandBase<object>(_ => this.playlist is not null && !this.isDownloadingField, async _ =>
               {
                   if (this.playlist is null) return;

                   if (!WS::Mainpage.Session.IsLogin)
                   {
                       this.SnackbarMessageQueue.Enqueue("動画をダウンロードするにはログインが必要です。");
                       return;
                   }
                   if (!this.IsDownloadingVideoEnable && !this.IsDownloadingCommentEnable && !this.IsDownloadingCommentLogEnable && !this.IsDownloadingThumbEnable) return;

                   var videos = WS::Mainpage.CurrentPlaylist.Videos.Where(v => v.IsSelected).Copy();
                   if (!videos.Any()) return;

                   var cts = new CancellationTokenSource();
                   this.downloacts = cts;
                   this.StartDownload();

                   int videoCount = videos.Count();
                   var firstVideo = videos.First();
                   string folderPath = this.playlist!.Folderpath.IsNullOrEmpty() ? WS::Mainpage.SettingHandler.GetStringSetting(Settings.DefaultFolder) ?? "downloaded" : this.playlist.Folderpath;
                   var setting = new DownloadSettings()
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
                       FolderPath = folderPath
                   };


                   WS::Mainpage.Messagehandler.AppendMessage($"動画のダウンロードを開始します。({videoCount}件)");
                   this.SnackbarMessageQueue.Enqueue($"動画のダウンロードを開始します。({videoCount}件)");

                   try
                   {
                       await WS::Mainpage.Videodownloader.DownloadVideos(videos, setting, cts.Token);

                   }
                   catch (Exception e)
                   {
                       WS::Mainpage.Messagehandler.AppendMessage($"ダウンロード中にエラーが発生しました。(詳細: {e.Message})");
                       this.SnackbarMessageQueue.Enqueue($"ダウンロード中にエラーが発生しました。");
                   }

                   if (videoCount > 1)
                   {
                       WS::Mainpage.Messagehandler.AppendMessage($"{firstVideo.NiconicoId}ほか{videoCount-1}件の動画をダウンロードしました。");
                       this.SnackbarMessageQueue.Enqueue($"{firstVideo.NiconicoId}ほか{videoCount-1}件の動画をダウンロードしました。");
                   } else
                   {
                       WS::Mainpage.Messagehandler.AppendMessage($"{firstVideo.NiconicoId}をダウンロードしました。");
                       this.SnackbarMessageQueue.Enqueue($"{firstVideo.NiconicoId}をダウンロードしました。");
                   }

                   this.CompleteDownload();

               });

            this.CancelCommand = new CommandBase<object>(_ => this.isDownloadingField, _ =>
            {
                this.downloacts?.Cancel();
                this.downloacts = null;
                this.CompleteDownload();
            });

            WS::Mainpage.CurrentPlaylist.SelectedItemChanged += this.OnSelectedChanged;
        }

        private bool isDownloadingVideoEnableField = true;

        private bool isDownloadingCommentEnableField;

        private bool isDownloadingCommentLogEnableField;

        private bool isDownloadingOwnerCommentField;

        private bool isDownloadingEasyCommentFIeld;

        private bool isDownloadingThumbEnableField;

        private bool isOverwriteEnableField;

        private bool isDownloadingField;

        private bool isSkippingEnablefield;

        private bool isCopyFromAnotherFolderEnableFIeld;

        private ResolutionSetting selectedResolutionField;

        private ITreePlaylistInfo? playlist;

        private CancellationTokenSource? downloacts;

        /// <summary>
        /// ダウンロードフラグ
        /// </summary>
        public bool IsDownloading { get => this.isDownloadingField; set => this.SetProperty(ref this.isDownloadingField, value); }

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        public CommandBase<object> DownloadCommand { get; init; }

        /// <summary>
        /// ダウンロードをキャンセルする
        /// </summary>
        public CommandBase<object> CancelCommand { get; init; }

        /// <summary>
        /// 動画ダウンロードフラグ
        /// </summary>
        public bool IsDownloadingVideoEnable { get => this.isDownloadingVideoEnableField; set => this.SetProperty(ref this.isDownloadingVideoEnableField, value); }

        /// <summary>
        /// コメントダウンロードフラグ
        /// </summary>
        public bool IsDownloadingCommentEnable { get => this.isDownloadingCommentEnableField; set => this.SetProperty(ref this.isDownloadingCommentEnableField, value); }

        /// <summary>
        /// 過去ログダウンロードフラグ
        /// </summary>
        public bool IsDownloadingCommentLogEnable { get => this.isDownloadingCommentLogEnableField; set => this.SetProperty(ref this.isDownloadingCommentLogEnableField, value); }

        /// <summary>
        /// 投稿者コメント
        /// </summary>
        public bool IsDownloadingOwnerComment { get => this.isDownloadingOwnerCommentField; set => this.SetProperty(ref this.isDownloadingOwnerCommentField, value); }

        /// <summary>
        /// かんたんコメント
        /// </summary>
        public bool IsDownloadingEasyComment { get => this.isDownloadingEasyCommentFIeld; set => this.SetProperty(ref this.isDownloadingEasyCommentFIeld, value); }

        /// <summary>
        /// サムネイルダウンロードフラグ
        /// </summary>
        public bool IsDownloadingThumbEnable { get => this.isDownloadingThumbEnableField; set => this.SetProperty(ref this.isDownloadingThumbEnableField, value); }

        /// <summary>
        /// 上書き保存フラグ
        /// </summary>
        public bool IsOverwriteEnable { get => this.isOverwriteEnableField; set => this.SetProperty(ref this.isOverwriteEnableField, value); }

        /// <summary>
        /// ダウンロード済をスキップ
        /// </summary>
        public bool IsSkippingEnable { get => this.isSkippingEnablefield; set => this.SetProperty(ref this.isSkippingEnablefield, value); }

        /// <summary>
        /// 別フォルダーからコピー
        /// </summary>
        public bool IsCopyFromAnotherFolderEnable { get => this.isCopyFromAnotherFolderEnableFIeld; set => this.SetProperty(ref this.isCopyFromAnotherFolderEnableFIeld, value); }


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

        private void StartDownload()
        {
            this.IsDownloading = true;
            this.RaiseCanExecuteChange();
        }

        private void CompleteDownload()
        {
            this.IsDownloading = false;
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
