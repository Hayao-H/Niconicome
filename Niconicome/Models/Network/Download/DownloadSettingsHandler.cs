using System;
using Niconicome.Extensions.System;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;


namespace Niconicome.Models.Network.Download
{
    interface IDownloadSettingsHandler
    {
        ReactiveProperty<bool> IsCopyFromAnotherFolderEnable { get; }
        ReactiveProperty<bool> IsDownloadingCommentEnable { get; }
        ReactiveProperty<bool> IsDownloadingCommentLogEnable { get; }
        ReactiveProperty<bool> IsDownloadingEasyComment { get; }
        ReactiveProperty<bool> IsDownloadingOwnerComment { get; }
        ReactiveProperty<bool> IsDownloadingThumbEnable { get; }
        ReactiveProperty<bool> IsDownloadingVideoEnable { get; }
        ReactiveProperty<bool> IsDownloadingVideoInfoEnable { get; }
        ReactiveProperty<bool> IsLimittingCommentCountEnable { get; }
        ReactiveProperty<bool> IsOverwriteEnable { get; }
        ReactiveProperty<bool> IsSkippingEnable { get; }
        ReactiveProperty<int> MaxCommentsCount { get; }
        ReactiveProperty<VideoInfo::IResolution> Resolution { get; }

        DownloadSettings CreateDownloadSettings();
    }

    class DownloadSettingsHandler : BindableBase, IDownloadSettingsHandler
    {
        public DownloadSettingsHandler(ILocalSettingHandler settingHandler, ICurrent current)
        {
            this.settingHandler = settingHandler;
            this.current = current;

            this.IsDownloadingVideoInfoEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLVideoInfo));
            this.IsLimittingCommentCountEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.LimitCommentsCount));
            this.IsDownloadingVideoEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLVideo));
            this.IsDownloadingCommentEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLComment));
            this.IsDownloadingCommentLogEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLKako));
            this.IsDownloadingEasyComment = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLEasy));
            this.IsDownloadingThumbEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLThumb));
            this.IsDownloadingOwnerComment = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLOwner));
            this.IsOverwriteEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLOverwrite));
            this.IsSkippingEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLSkip));
            this.IsCopyFromAnotherFolderEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLCopy));
            this.IsLimittingCommentCountEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.LimitCommentsCount));
            this.MaxCommentsCount = new ReactiveProperty<int>(this.settingHandler.GetIntSetting(SettingsEnum.MaxCommentsCount));

            this.IsDownloadingVideoInfoEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLVideoInfo));
            this.IsDownloadingVideoEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLVideo));
            this.IsDownloadingCommentEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLComment));
            this.IsDownloadingCommentLogEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLKako));
            this.IsDownloadingEasyComment.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLEasy));
            this.IsDownloadingThumbEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLThumb));
            this.IsDownloadingOwnerComment.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLOwner));
            this.IsOverwriteEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLOverwrite));
            this.IsSkippingEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLSkip));
            this.IsCopyFromAnotherFolderEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DLCopy));
            this.IsLimittingCommentCountEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.LimitCommentsCount));
            this.MaxCommentsCount.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.MaxCommentsCount));

            this.Resolution = new ReactiveProperty<VideoInfo::IResolution>(new VideoInfo::Resolution("1920x1080"));
        }

        #region フィールド
        private readonly ILocalSettingHandler settingHandler;

        private readonly ICurrent current;
        #endregion

        /// <summary>
        /// DL設定を取得する
        /// </summary>
        /// <returns></returns>
        public DownloadSettings CreateDownloadSettings()
        {

            if (this.current.SelectedPlaylist.Value is null) throw new InvalidOperationException("");

            var replaceStricted = this.settingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB);
            var overrideVideoDT = this.settingHandler.GetBoolSetting(SettingsEnum.OverrideVideoFileDTToUploadedDT);
            var resumeEnable = this.settingHandler.GetBoolSetting(SettingsEnum.EnableResume);
            var unsafeHandle = this.settingHandler.GetBoolSetting(SettingsEnum.UnsafeCommentHandle);
            string folderPath = this.current.SelectedPlaylist.Value.Folderpath.IsNullOrEmpty() ? this.settingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? "downloaded" : this.current.SelectedPlaylist.Value.Folderpath;

            return new DownloadSettings
            {
                Video = this.IsDownloadingVideoEnable.Value,
                Thumbnail = this.IsDownloadingThumbEnable.Value,
                Overwrite = this.IsOverwriteEnable.Value,
                Comment = this.IsDownloadingCommentEnable.Value,
                DownloadLog = this.IsDownloadingCommentLogEnable.Value,
                DownloadEasy = this.IsDownloadingEasyComment.Value,
                DownloadOwner = this.IsDownloadingOwnerComment.Value,
                FromAnotherFolder = this.IsCopyFromAnotherFolderEnable.Value,
                Skip = this.IsSkippingEnable.Value,
                FolderPath = folderPath,
                VerticalResolution = this.Resolution.Value.Vertical,
                PlaylistID = this.current.SelectedPlaylist.Value.Id,
                IsReplaceStrictedEnable = replaceStricted,
                OverrideVideoFileDateToUploadedDT = overrideVideoDT,
                MaxCommentsCount = this.IsLimittingCommentCountEnable.Value ? this.MaxCommentsCount.Value : 0,
                DownloadVideoInfo = this.IsDownloadingVideoInfoEnable.Value,
                ResumeEnable = resumeEnable,
                EnableUnsafeCommentHandle = unsafeHandle,
            };
        }

        /// <summary>
        /// 動画DL
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingVideoEnable { get; init; }

        /// <summary>
        /// コメントDL
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingCommentEnable { get; init; }

        /// <summary>
        /// 過去ログ
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingCommentLogEnable { get; init; }

        /// <summary>
        /// 投コメ
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingOwnerComment { get; init; }

        /// <summary>
        /// かんたんコメント
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingEasyComment { get; init; }

        /// <summary>
        /// サムネ
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingThumbEnable { get; init; }

        /// <summary>
        /// 上書き
        /// </summary>
        public ReactiveProperty<bool> IsOverwriteEnable { get; init; }

        /// <summary>
        /// DL済みをスキップ
        /// </summary>
        public ReactiveProperty<bool> IsSkippingEnable { get; init; }

        /// <summary>
        /// 他フォルダーからコピー
        /// </summary>
        public ReactiveProperty<bool> IsCopyFromAnotherFolderEnable { get; init; }

        /// <summary>
        /// 最大コメ数
        /// </summary>
        public ReactiveProperty<bool> IsLimittingCommentCountEnable { get; init; }

        /// <summary>
        /// 動画情報
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingVideoInfoEnable { get; init; }

        /// <summary>
        /// 最大コメ数
        /// </summary>
        public ReactiveProperty<int> MaxCommentsCount { get; init; }

        /// <summary>
        /// 解像度
        /// </summary>
        public ReactiveProperty<VideoInfo::IResolution> Resolution { get; init; }
    }
}
