using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.ViewModels.Mainpage.Tabs;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.ViewModels.Setting.Utils;
using Niconicome.Views;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using MaterialDesign = MaterialDesignThemes.Wpf;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;
using WS = Niconicome.Workspaces;
namespace Niconicome.ViewModels.Mainpage
{
    class DownloadSettingsViewModel : TabViewModelBase, IDisposable
    {
        public DownloadSettingsViewModel(IEventAggregator ea, IDialogService dialogService, IRegionManager regionManager) : base("ダウンロード", "")
        {

            this._dialogService = dialogService;
            this._regionManager = regionManager;

            this.IsDownloadingVideoInfoEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingVideoInfoEnable);
            this.IsLimittingCommentCountEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsLimittingCommentCountEnable);
            this.IsDownloadingVideoEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingVideoEnable);
            this.IsDownloadingCommentEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingCommentEnable);
            this.IsDownloadingCommentLogEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingCommentLogEnable);
            this.IsDownloadingEasyComment = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingEasyComment);
            this.IsDownloadingThumbEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingThumbEnable);
            this.IsDownloadingOwnerComment = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingOwnerComment);
            this.IsOverwriteEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsOverwriteEnable);
            this.IsSkippingEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsSkippingEnable);
            this.IsCopyFromAnotherFolderEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsCopyFromAnotherFolderEnable);
            this.MaxCommentsCount = new SettingInfoViewModel<int>(WS::Mainpage.DownloadSettingsHandler.MaxCommentsCount);
            this.IsNotEncodeEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsNoEncodeEnable);
            this.IsDownloadingIchibaInfoEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsDownloadingIchibaInfoEnable);
            this.IsCommentAppendingEnable = new SettingInfoViewModel<bool>(WS::Mainpage.DownloadSettingsHandler.IsAppendingCommentEnable);

            var s1 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("1920x1080"), "1080px");
            var s2 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("1280x720"), "720px");
            var s3 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("854x480"), "480px");
            var s4 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("640x360"), "360px");
            var s5 = new ComboboxItem<VideoInfo::IResolution>(new VideoInfo::Resolution("426x240"), "240px");

            this.Resolutions = new List<ComboboxItem<VideoInfo::IResolution>>() { s1, s2, s3, s4, s5 };

            this.disposables = new CompositeDisposable();

            this.SelectedResolution = new BindableProperty<ComboboxItem<VideoInfo::IResolution>>(s1);

            this._handler = x =>
            {
                if (this.SelectedResolution.Value.Value.Vertical == x.Vertical) return;
                this.SelectedResolution.Value = x.Vertical switch
                {
                    1080 => s1,
                    720 => s2,
                    480 => s3,
                    360 => s4,
                    240 => s5,
                    _ => s1
                };
            };
            WS::Mainpage.DownloadSettingsHandler.Resolution.Subscribe(this._handler);

            this.SelectedResolution.Subscribe(x =>
            {
                if (WS::Mainpage.DownloadSettingsHandler.Resolution.Value.Vertical == this.SelectedResolution.Value.Value.Vertical) return;
                WS::Mainpage.DownloadSettingsHandler.Resolution.Value = x.Value;
            });

            var e1 = new ComboboxItem<EconomySettins>(EconomySettins.AlwaysDownload, "常にしない");
            var e2 = new ComboboxItem<EconomySettins>(EconomySettins.SkipWhenPremiumExists, "高画質が存在する場合");
            var e3 = new ComboboxItem<EconomySettins>(EconomySettins.AlwaysSkip, "常にスキップ");

            this.SelectableEconomySettings = new List<ComboboxItem<EconomySettins>>() { e1, e2, e3 };
            var alwaysSkip = WS::Mainpage.SettingsConainer.GetOnlyValue(SettingNames.AlwaysSkipEconomyDownload, false).Data;
            var skipWhenPremiumExists = WS::Mainpage.SettingsConainer.GetOnlyValue(SettingNames.SkipEconomyDownloadIfPremiumExists, false).Data;

            if (alwaysSkip)
            {
                this.EconomySetting = new BindableProperty<ComboboxItem<EconomySettins>>(e3);
            }
            else if (skipWhenPremiumExists)
            {
                this.EconomySetting = new BindableProperty<ComboboxItem<EconomySettins>>(e2);
            }
            else
            {
                this.EconomySetting = new BindableProperty<ComboboxItem<EconomySettins>>(e1);
            }

            this.EconomySetting.Subscribe(x =>
            {
                IAttemptResult<ISettingInfo<bool>> alwaysSkip = WS::Mainpage.SettingsConainer.GetSetting(SettingNames.AlwaysSkipEconomyDownload, false);
                IAttemptResult<ISettingInfo<bool>> skipWhenPremiumExists = WS::Mainpage.SettingsConainer.GetSetting(SettingNames.SkipEconomyDownloadIfPremiumExists, false);

                if (!alwaysSkip.IsSucceeded || alwaysSkip.Data is null || !skipWhenPremiumExists.IsSucceeded || skipWhenPremiumExists.Data is null) return;

                if (x.Value == EconomySettins.AlwaysSkip)
                {
                    alwaysSkip.Data.Value = true;
                    skipWhenPremiumExists.Data.Value = false;
                }
                else if (x.Value == EconomySettins.SkipWhenPremiumExists)
                {
                    alwaysSkip.Data.Value = false;
                    skipWhenPremiumExists.Data.Value = true;
                }
                else if (x.Value == EconomySettins.AlwaysDownload)
                {
                    alwaysSkip.Data.Value = false;
                    skipWhenPremiumExists.Data.Value = false;
                }
            });

            #region サムネ

            var t1 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Large, "大");
            var t2 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Middle, "中");
            var t3 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Normal, "普通");
            var t4 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Player, "プレイヤー");

            this.ThumbSizes = new List<ComboboxItem<VideoInfo.ThumbSize>>() { t1, t2, t3, t4 };
            this.SelectedThumbSize = new ConvertableSettingInfoViewModel<VideoInfo.ThumbSize, ComboboxItem<VideoInfo.ThumbSize>>(WS::Mainpage.DownloadSettingsHandler.ThumbnailSize, x => x switch
            {
                VideoInfo::ThumbSize.Large => t1,
                VideoInfo::ThumbSize.Middle => t2,
                VideoInfo::ThumbSize.Normal => t3,
                _ => t4
            }, x => x.Value);

            #endregion

            this.IsDownloading = new BindableProperty<bool>(false);
            WS::Mainpage.DownloadManager.IsProcessing.Subscribe(x => this.IsDownloading.Value = x);

            this.DownloadCommand = new BindableCommand(async () => await this.DownloadVideo(null), WS::Mainpage.DownloadManager.IsProcessing
                .Select(x => !x)).AddTo(this.disposables);

            this.StageVideosCommand = new BindableCommand(() =>
            {
                if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
                {
                    WS::Mainpage.SnackbarHandler.Enqueue("プレイリストが選択されていないため、ステージできません");
                    return;
                }

                if (!this.IsDownloadingCommentEnable.Value && (this.IsDownloadingCommentLogEnable.Value || this.IsDownloadingEasyComment.Value || this.IsDownloadingOwnerComment.Value))
                {
                    WS::Mainpage.SnackbarHandler.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                    return;
                }

                if (!this.IsDownloadingVideoEnable.Value && !this.IsDownloadingCommentEnable.Value && !this.IsDownloadingThumbEnable.Value && !this.IsDownloadingVideoInfoEnable.Value && !this.IsDownloadingIchibaInfoEnable.Value) return;

                foreach (var video in WS.Mainpage.PlaylistVideoContainer.Videos.Where(v => v.IsSelected.Value))
                {
                    WS::Mainpage.DownloadManager.StageVIdeo(video);
                }

                WS::Mainpage.SnackbarHandler.Enqueue($"選択された動画をステージしました。", "管理画面を開く", () =>
                {
                    WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/downloadtask/stage");
                    WS::Mainpage.WindowTabHelper.OpenDownloadTaskWindow(this._regionManager);
                });
            })
            .AddTo(this.disposables);

            this.CancelCommand = new BindableCommand(() =>
            {
                WS::Mainpage.DownloadManager.CancelDownload();

            }, WS::Mainpage.DownloadManager.IsProcessing).AddTo(this.disposables);

            //イベントを購読
            ea.GetEvent<PubSubEvent<MVVMEvent<VideoInfoViewModel>>>().Subscribe(this.OnDoubleClick);
        }

        ~DownloadSettingsViewModel()
        {
            WS::Mainpage.DownloadSettingsHandler.Resolution.UnSubscribe(this._handler);
            this.Dispose();
        }

        #region field

        private readonly IDialogService _dialogService;

        private readonly IRegionManager _regionManager;

        private Action<VideoInfo::IResolution> _handler;

        #endregion


        /// <summary>
        /// ダウンロードフラグ
        /// </summary>
        public BindableProperty<bool> IsDownloading { get; init; }

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        public BindableCommand DownloadCommand { get; init; }

        /// <summary>
        /// ダウンロードをキャンセルする
        /// </summary>
        public BindableCommand CancelCommand { get; init; }

        /// <summary>
        /// ステージする
        /// </summary>
        public BindableCommand StageVideosCommand { get; init; }

        /// <summary>
        /// 動画ダウンロードフラグ
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingVideoEnable { get; init; }

        /// <summary>
        /// コメントダウンロードフラグ
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingCommentEnable { get; init; }

        /// <summary>
        /// 過去ログダウンロードフラグ
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingCommentLogEnable { get; init; }

        /// <summary>
        /// 投稿者コメント
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingOwnerComment { get; init; }

        /// <summary>
        /// かんたんコメント
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingEasyComment { get; init; }

        /// <summary>
        /// サムネイルダウンロードフラグ
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingThumbEnable { get; init; }

        /// <summary>
        /// 動画情報
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingVideoInfoEnable { get; init; }

        /// <summary>
        /// 市場情報
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadingIchibaInfoEnable { get; init; }


        /// <summary>
        /// 上書き保存フラグ
        /// </summary>
        public SettingInfoViewModel<bool> IsOverwriteEnable { get; init; }

        /// <summary>
        /// ダウンロード済をスキップ
        /// </summary>
        public SettingInfoViewModel<bool> IsSkippingEnable { get; init; }

        /// <summary>
        /// 別フォルダーからコピー
        /// </summary>
        public SettingInfoViewModel<bool> IsCopyFromAnotherFolderEnable { get; init; }

        /// <summary>
        /// コメント取得数を制限する
        /// </summary>
        public SettingInfoViewModel<bool> IsLimittingCommentCountEnable { get; init; }

        /// <summary>
        /// エンコードしない
        /// </summary>
        public SettingInfoViewModel<bool> IsNotEncodeEnable { get; init; }

        /// <summary>
        /// コメント追記設定
        /// </summary>
        public SettingInfoViewModel<bool> IsCommentAppendingEnable { get; init; }

        /// <summary>
        /// コメントの最大取得数
        /// </summary>
        public SettingInfoViewModel<int> MaxCommentsCount { get; init; }

        /// <summary>
        /// エコノミー設定
        /// </summary>
        public IBindableProperty<ComboboxItem<EconomySettins>> EconomySetting { get; init; }

        /// <summary>
        /// 選択中の解像度
        /// </summary>
        public IBindableProperty<ComboboxItem<VideoInfo::IResolution>> SelectedResolution { get; init; }

        /// <summary>
        /// サムネイルサイズ
        /// </summary>
        public ConvertableSettingInfoViewModel<VideoInfo::ThumbSize, ComboboxItem<VideoInfo::ThumbSize>> SelectedThumbSize { get; init; }

        /// <summary>
        /// 解像度一覧
        /// </summary>
        public List<ComboboxItem<VideoInfo::IResolution>> Resolutions { get; init; }

        /// <summary>
        /// サムネイルサイズ一覧
        /// </summary>
        public List<ComboboxItem<VideoInfo::ThumbSize>> ThumbSizes { get; init; }

        /// <summary>
        /// 選択可能なエコノミー設定
        /// </summary>
        public List<ComboboxItem<EconomySettins>> SelectableEconomySettings { get; init; }

        #region private

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private async Task DownloadVideo(VideoInfoViewModel? vm)
        {
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                WS::Mainpage.SnackbarHandler.Enqueue("プレイリストが選択されていないため、ダウンロードできません");
                return;
            }

            if (!WS::Mainpage.Session.IsLogin.Value)
            {
                WS::Mainpage.SnackbarHandler.Enqueue("動画をダウンロードするにはログインが必要です。");
                return;
            }

            if (!this.IsDownloadingCommentEnable.Value && (this.IsDownloadingCommentLogEnable.Value || this.IsDownloadingEasyComment.Value || this.IsDownloadingOwnerComment.Value))
            {
                WS::Mainpage.SnackbarHandler.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                return;
            }

            if (!this.IsDownloadingVideoEnable.Value && !this.IsDownloadingCommentEnable.Value && !this.IsDownloadingThumbEnable.Value && !this.IsDownloadingVideoInfoEnable.Value && !this.IsDownloadingIchibaInfoEnable.Value) return;

            foreach (var video in WS.Mainpage.PlaylistVideoContainer.Videos.Where(v => v.IsSelected.Value))
            {
                WS::Mainpage.DownloadManager.StageVIdeo(video);
            }

            await WS::Mainpage.DownloadManager.StartDownloadAsync(m => WS::Mainpage.SnackbarHandler.Enqueue(m), m => WS::Mainpage.Messagehandler.AppendMessage(m));
            WS::Mainpage.PostDownloadTasksManager.HandleAction();
        }

        /// <summary>
        /// ダブルクリックでDL
        /// </summary>
        /// <param name="e"></param>
        private void OnDoubleClick(MVVMEvent<VideoInfoViewModel> e)
        {
            if (!e.CheckTarget(this.GetType())) return;
            if (e.EventType != EventType.Download) return;
            _ = this.DownloadVideo(e.Data);
        }
        #endregion
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

            this.SelectedResolution = new BindableProperty<ComboboxItem<VideoInfo::IResolution>>(s1);

            var t1 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Large, "大");
            var t2 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Middle, "中");
            var t3 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Normal, "普通");
            var t4 = new ComboboxItem<VideoInfo::ThumbSize>(VideoInfo::ThumbSize.Player, "プレイヤー");

            this.ThumbSizes = new List<ComboboxItem<VideoInfo.ThumbSize>>() { t1, t2, t3, t4 };
            this.SelectedThumbSize = new ConvertableSettingInfoViewModelD<ComboboxItem<VideoInfo.ThumbSize>>(t1);

            var e1 = new ComboboxItem<EconomySettins>(EconomySettins.AlwaysDownload, "常にしない");
            var e2 = new ComboboxItem<EconomySettins>(EconomySettins.SkipWhenPremiumExists, "高画質が存在する場合");
            var e3 = new ComboboxItem<EconomySettins>(EconomySettins.AlwaysSkip, "常にスキップ");

            this.SelectableEconomySettings = new List<ComboboxItem<EconomySettins>>() { e1, e2, e3 };
            this.EconomySetting = new BindableProperty<ComboboxItem<EconomySettins>>(e1);

        }

        public BindableProperty<bool> IsDownloading { get; init; } = new(false);

        public ReactiveCommand DownloadCommand { get; init; } = new();

        public ReactiveCommand CancelCommand { get; init; } = new();

        public ReactiveCommand StageVideosCommand { get; init; } = new();

        public SettingInfoViewModelD<bool> IsDownloadingVideoEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadingCommentEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadingCommentLogEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadingOwnerComment { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadingEasyComment { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadingThumbEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsOverwriteEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsSkippingEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsCopyFromAnotherFolderEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsLimittingCommentCountEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadingVideoInfoEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadingIchibaInfoEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsNotEncodeEnable { get; init; } = new(false);

        public SettingInfoViewModelD<bool> IsCommentAppendingEnable { get; init; } = new(true);

        public SettingInfoViewModelD<int> MaxCommentsCount { get; set; } = new(2000);

        public IBindableProperty<ComboboxItem<EconomySettins>> EconomySetting { get; init; }

        public IBindableProperty<ComboboxItem<VideoInfo::IResolution>> SelectedResolution { get; set; }

        public List<ComboboxItem<VideoInfo::IResolution>> Resolutions { get; init; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<VideoInfo::ThumbSize>> SelectedThumbSize { get; init; }

        public List<ComboboxItem<VideoInfo::ThumbSize>> ThumbSizes { get; init; }


        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; } = new MaterialDesign::SnackbarMessageQueue();

        public List<ComboboxItem<EconomySettins>> SelectableEconomySettings { get; init; }

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

    public enum EconomySettins
    {
        AlwaysDownload,
        SkipWhenPremiumExists,
        AlwaysSkip
    }
}
