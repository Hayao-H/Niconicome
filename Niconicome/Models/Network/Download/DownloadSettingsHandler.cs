using System;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
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
        ReactiveProperty<bool> IsDownloadingIchibaInfoEnable { get; }
        ReactiveProperty<bool> IsLimittingCommentCountEnable { get; }
        ReactiveProperty<bool> IsOverwriteEnable { get; }
        ReactiveProperty<bool> IsSkippingEnable { get; }
        ReactiveProperty<bool> IsNoEncodeEnable { get; }
        ReactiveProperty<int> MaxCommentsCount { get; }
        ReactiveProperty<VideoInfo::IResolution> Resolution { get; }
        ReactiveProperty<VideoInfo::ThumbSize> ThumbnailSize { get; }
        DownloadSettings CreateDownloadSettings();
    }

    class DownloadSettingsHandler : BindableBase, IDownloadSettingsHandler
    {
        public DownloadSettingsHandler(ILocalSettingHandler settingHandler, ICurrent current, IEnumSettingsHandler enumSettingsHandler)
        {
            this.settingHandler = settingHandler;
            this.current = current;
            this.enumSettingsHandler = enumSettingsHandler;

            this.IsDownloadingVideoInfoEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLVideoInfo)).AddTo(this.disposables);
            this.IsLimittingCommentCountEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.LimitCommentsCount)).AddTo(this.disposables);
            this.IsDownloadingVideoEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLVideo)).AddTo(this.disposables);
            this.IsDownloadingCommentEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLComment)).AddTo(this.disposables);
            this.IsDownloadingCommentLogEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLKako)).AddTo(this.disposables);
            this.IsDownloadingEasyComment = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLEasy)).AddTo(this.disposables);
            this.IsDownloadingThumbEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLThumb)).AddTo(this.disposables);
            this.IsDownloadingOwnerComment = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLOwner)).AddTo(this.disposables);
            this.IsOverwriteEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLOverwrite)).AddTo(this.disposables);
            this.IsSkippingEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLSkip)).AddTo(this.disposables);
            this.IsCopyFromAnotherFolderEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DLCopy)).AddTo(this.disposables);
            this.IsLimittingCommentCountEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.LimitCommentsCount)).AddTo(this.disposables);
            this.MaxCommentsCount = new ReactiveProperty<int>(this.settingHandler.GetIntSetting(SettingsEnum.MaxCommentsCount)).AddTo(this.disposables);
            this.IsNoEncodeEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DlWithoutEncode)).AddTo(this.disposables);
            this.IsDownloadingIchibaInfoEnable = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(SettingsEnum.DlIchiba)).AddTo(this.disposables);
            this.ThumbnailSize = new ReactiveProperty<VideoInfo::ThumbSize>(this.enumSettingsHandler.GetSetting<VideoInfo::ThumbSize>());

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
            this.IsNoEncodeEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DlWithoutEncode));
            this.IsDownloadingIchibaInfoEnable.Subscribe(value => this.settingHandler.SaveSetting(value, SettingsEnum.DlIchiba));
            this.ThumbnailSize.Subscribe(value => this.enumSettingsHandler.SaveSetting(value));

            this.Resolution = new ReactiveProperty<VideoInfo::IResolution>(new VideoInfo::Resolution("1920x1080"));
        }

        ~DownloadSettingsHandler()
        {
            this.Dispose();
        }

        #region フィールド
        private readonly ILocalSettingHandler settingHandler;

        private readonly ICurrent current;

        private readonly IEnumSettingsHandler enumSettingsHandler;
        #endregion

        /// <summary>
        /// DL設定を取得する
        /// </summary>
        /// <returns></returns>
        public DownloadSettings CreateDownloadSettings()
        {

            if (this.current.SelectedPlaylist.Value is null) throw new InvalidOperationException("");

            bool replaceStricted = this.settingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB);
            bool overrideVideoDT = this.settingHandler.GetBoolSetting(SettingsEnum.OverrideVideoFileDTToUploadedDT);
            bool resumeEnable = this.settingHandler.GetBoolSetting(SettingsEnum.EnableResume);
            bool unsafeHandle = this.settingHandler.GetBoolSetting(SettingsEnum.UnsafeCommentHandle);
            bool experimentalSafety = this.settingHandler.GetBoolSetting(SettingsEnum.ExperimentalSafety);
            bool deleteEconomyFile = this.settingHandler.GetBoolSetting(SettingsEnum.DeleteEcoFile);
            string folderPath = this.current.PlaylistFolderPath;
            string fileFormat = this.settingHandler.GetStringSetting(SettingsEnum.FileNameFormat) ?? Format.FIleFormat;

            VideoInfoTypeSettings videoInfoT = this.enumSettingsHandler.GetSetting<VideoInfoTypeSettings>();
            string videoInfoExt = videoInfoT == VideoInfoTypeSettings.Json ? ".json" : videoInfoT == VideoInfoTypeSettings.Xml ? ".xml" : ".txt";

            IchibaInfoTypeSettings ichibaInfoT = this.enumSettingsHandler.GetSetting<IchibaInfoTypeSettings>();
            string? htmlExt = this.settingHandler.GetStringSetting(SettingsEnum.HtmlExt);
            string ichibaInfoExt = ichibaInfoT == IchibaInfoTypeSettings.Json ? ".json" : ichibaInfoT == IchibaInfoTypeSettings.Xml ? ".xml" : htmlExt ?? ".html";

            string thumbExt = this.settingHandler.GetStringSetting(SettingsEnum.JpegExt) ?? FileFolder.DefaultJpegFileExt;

            string videoInfoSuffix = this.settingHandler.GetStringSetting(SettingsEnum.VideoinfoSuffix) ?? Format.DefaultVideoInfoSuffix;
            string ichibaInfoSuffix = this.settingHandler.GetStringSetting(SettingsEnum.IchibaInfoSuffix) ?? Format.DefaultIchibaSuffix;
            string thumbSuffix = this.settingHandler.GetStringSetting(SettingsEnum.ThumbSuffix) ?? Format.DefaultThumbnailSuffix;
            string ownerComSuffix = this.settingHandler.GetStringSetting(SettingsEnum.OwnerComSuffix) ?? Format.DefaultOwnerCommentSuffix;

            string commandFormat = this.settingHandler.GetStringSetting(SettingsEnum.FFmpegFormat) ?? Format.DefaultFFmpegFormat;
            if (commandFormat.IsNullOrEmpty())
            {
                commandFormat = Format.DefaultFFmpegFormat;
            }

            string? economySuffix = this.settingHandler.GetStringSetting(SettingsEnum.EconomySuffix);
            if (economySuffix?.IsNullOrEmpty() ?? true)
            {
                economySuffix = null;
            }

            int commentFetchWaitSpan = this.settingHandler.GetIntSetting(SettingsEnum.CommentWaitSpan);
            if (commentFetchWaitSpan < 0)
            {
                commentFetchWaitSpan = LocalConstant.DefaultCommetFetchWaitSpan;
            }

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
                SaveWithoutEncode = this.IsNoEncodeEnable.Value,
                FileNameFormat = fileFormat,
                VideoInfoExt = videoInfoExt,
                IchibaInfoExt = ichibaInfoExt,
                DownloadIchibaInfo = this.IsDownloadingIchibaInfoEnable.Value,
                IchibaInfoType = ichibaInfoT,
                ThumbnailExt = thumbExt,
                VideoInfoSuffix = videoInfoSuffix,
                IchibaInfoSuffix = ichibaInfoSuffix,
                CommandFormat = commandFormat,
                ThumbSize = this.ThumbnailSize.Value,
                ThumbSuffix = thumbSuffix,
                OwnerComSuffix = ownerComSuffix,
                EconomySuffix = economySuffix,
                EnableExperimentalCommentSafetySystem = experimentalSafety,
                CommentFetchWaitSpan = commentFetchWaitSpan,
                DeleteExistingEconomyFile = deleteEconomyFile,
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
        /// エンコードしない
        /// </summary>
        public ReactiveProperty<bool> IsNoEncodeEnable { get; init; }


        /// <summary>
        /// 最大コメ数
        /// </summary>
        public ReactiveProperty<bool> IsLimittingCommentCountEnable { get; init; }

        /// <summary>
        /// 動画情報
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingVideoInfoEnable { get; init; }

        /// <summary>
        /// 市場情報
        /// </summary>
        public ReactiveProperty<bool> IsDownloadingIchibaInfoEnable { get; init; }

        /// <summary>
        /// 最大コメ数
        /// </summary>
        public ReactiveProperty<int> MaxCommentsCount { get; init; }

        /// <summary>
        /// 解像度
        /// </summary>
        public ReactiveProperty<VideoInfo::IResolution> Resolution { get; init; }

        /// <summary>
        /// サムネイルサイズ
        /// </summary>
        public ReactiveProperty<VideoInfo::ThumbSize> ThumbnailSize { get; init; }
    }
}
