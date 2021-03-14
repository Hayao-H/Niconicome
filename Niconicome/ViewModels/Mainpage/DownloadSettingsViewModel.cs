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

            var s1 = new ResolutionSetting("1920x1080");
            var s2 = new ResolutionSetting("1280x720");
            var s3 = new ResolutionSetting("854x480");
            var s4 = new ResolutionSetting("640x360");
            var s5 = new ResolutionSetting("426x240");

            this.Resolutions = new List<ResolutionSetting>() { s1, s2, s3, s4, s5
    };
            this.selectedResolutionField = s1;

            this.SnackbarMessageQueue = WS::Mainpage.SnaclbarHandler.Queue;

            this.DownloadCommand = new CommandBase<object>(_ => this.playlist is not null && !this.isDownloadingField, async _ =>
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
                   INetworkResult? result = null;

                   try
                   {
                       result = await WS::Mainpage.Videodownloader.DownloadVideos(videos, setting, cts.Token);

                   }
                   catch (Exception e)
                   {
                       WS::Mainpage.Messagehandler.AppendMessage($"ダウンロード中にエラーが発生しました。(詳細: {e.Message})");
                       this.SnackbarMessageQueue.Enqueue($"ダウンロード中にエラーが発生しました。");
                   }

                   if (result?.SucceededCount > 1)
                   {
                       if (result?.FirstVideo is not null)
                       {
                           WS::Mainpage.Messagehandler.AppendMessage($"{result.FirstVideo.NiconicoId}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");
                           this.SnackbarMessageQueue.Enqueue($"{result.FirstVideo.NiconicoId}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");

                           if (!result.IsSucceededAll)
                           {
                               WS::Mainpage.Messagehandler.AppendMessage($"{result.FailedCount}件の動画のダウンロードに失敗しました。");
                           }
                       }
                   }
                   else if (result?.SucceededCount == 1)
                   {
                       if (result?.FirstVideo is not null)
                       {
                           WS::Mainpage.Messagehandler.AppendMessage($"{result.FirstVideo.NiconicoId}をダウンロードしました。");
                           this.SnackbarMessageQueue.Enqueue($"{result.FirstVideo.NiconicoId}をダウンロードしました。");
                       }
                   }
                   else
                   {
                       WS::Mainpage.Messagehandler.AppendMessage($"ダウンロード出来ませんでした。");
                       this.SnackbarMessageQueue.Enqueue($"ダウンロード出来ませんでした。");
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
