using System;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Download.Comment;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;


namespace Niconicome.Models.Network.Download
{
    public interface IDownloadSettingsHandler
    {
        /// <summary>
        /// 別フォルダーからコピー
        /// </summary>
        ReactiveProperty<bool> IsCopyFromAnotherFolderEnable { get; }

        /// <summary>
        /// コメントDL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingCommentEnable { get; }

        /// <summary>
        /// 過去ログDL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingCommentLogEnable { get; }

        /// <summary>
        /// かんたんコメントDL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingEasyComment { get; }

        /// <summary>
        /// 投コメDL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingOwnerComment { get; }

        /// <summary>
        /// サムネDL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingThumbEnable { get; }

        /// <summary>
        /// 動画DL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingVideoEnable { get; }

        /// <summary>
        /// 動画情報DL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingVideoInfoEnable { get; }

        /// <summary>
        /// 市場情報DL
        /// </summary>
        ReactiveProperty<bool> IsDownloadingIchibaInfoEnable { get; }

        /// <summary>
        /// コメ数制限有効フラグ
        /// </summary>
        ReactiveProperty<bool> IsLimittingCommentCountEnable { get; }

        /// <summary>
        /// 上書きフラグ
        /// </summary>
        ReactiveProperty<bool> IsOverwriteEnable { get; }

        /// <summary>
        /// スキップフラグ
        /// </summary>
        ReactiveProperty<bool> IsSkippingEnable { get; }

        /// <summary>
        /// エンコードスキップフラグ
        /// </summary>
        ReactiveProperty<bool> IsNoEncodeEnable { get; }

        /// <summary>
        /// コメント追記フラグ
        /// </summary>
        ReactiveProperty<bool> IsAppendingCommentEnable { get; }

        /// <summary>
        /// コメ数制限
        /// </summary>
        ReactiveProperty<int> MaxCommentsCount { get; }

        /// <summary>
        /// 解像度設定
        /// </summary>
        ReactiveProperty<VideoInfo::IResolution> Resolution { get; }

        /// <summary>
        /// サムネサイズ設定
        /// </summary>
        ReactiveProperty<VideoInfo::ThumbSize> ThumbnailSize { get; }

        /// <summary>
        /// DL設定を構築
        /// </summary>
        /// <returns></returns>
        DownloadSettings CreateDownloadSettings();
    }

    public class DownloadSettingsHandler : BindableBase, IDownloadSettingsHandler
    {
        public DownloadSettingsHandler(ILocalSettingHandler settingHandler, ICurrent current, IEnumSettingsHandler enumSettingsHandler, ILocalSettingsContainer container)
        {
            this.settingHandler = settingHandler;
            this.current = current;
            this.enumSettingsHandler = enumSettingsHandler;
            this._container = container;

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
            this.IsAppendingCommentEnable = this._container.GetReactiveBoolSetting(SettingsEnum.AppendComment);

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

        #region field
        private readonly ILocalSettingHandler settingHandler;

        private readonly ICurrent current;

        private readonly IEnumSettingsHandler enumSettingsHandler;

        private readonly ILocalSettingsContainer _container;
        #endregion

        public DownloadSettings CreateDownloadSettings()
        {

            if (this.current.SelectedPlaylist.Value is null) throw new InvalidOperationException("");

            //フラグ系
            bool replaceStricted = this.settingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB);
            bool overrideVideoDT = this.settingHandler.GetBoolSetting(SettingsEnum.OverrideVideoFileDTToUploadedDT);
            bool resumeEnable = this.settingHandler.GetBoolSetting(SettingsEnum.EnableResume);
            bool unsafeHandle = this.settingHandler.GetBoolSetting(SettingsEnum.UnsafeCommentHandle);
            bool experimentalSafety = this.settingHandler.GetBoolSetting(SettingsEnum.ExperimentalSafety);
            bool deleteEconomyFile = this.settingHandler.GetBoolSetting(SettingsEnum.DeleteEcoFile);
            bool omitXmlDec = this.settingHandler.GetBoolSetting(SettingsEnum.OmitXmlDeclaration);

            //ファイル系
            string folderPath = this.current.PlaylistFolderPath;
            string fileFormat = this.settingHandler.GetStringSetting(SettingsEnum.FileNameFormat) ?? Format.DefaultFileNameFormat;
            string thumbExt = this.settingHandler.GetStringSetting(SettingsEnum.JpegExt) ?? FileFolder.DefaultJpegFileExt;
            string videoInfoSuffix = this.settingHandler.GetStringSetting(SettingsEnum.VideoinfoSuffix) ?? Format.DefaultVideoInfoSuffix;
            string ichibaInfoSuffix = this.settingHandler.GetStringSetting(SettingsEnum.IchibaInfoSuffix) ?? Format.DefaultIchibaSuffix;
            string thumbSuffix = this.settingHandler.GetStringSetting(SettingsEnum.ThumbSuffix) ?? Format.DefaultThumbnailSuffix;
            string ownerComSuffix = this.settingHandler.GetStringSetting(SettingsEnum.OwnerComSuffix) ?? Format.DefaultOwnerCommentSuffix;
            string economySuffix = this.settingHandler.GetStringSetting(SettingsEnum.EconomySuffix) ?? "";

            //動画情報
            VideoInfoTypeSettings videoInfoT = this.enumSettingsHandler.GetSetting<VideoInfoTypeSettings>();
            string videoInfoExt = videoInfoT == VideoInfoTypeSettings.Json ? ".json" : videoInfoT == VideoInfoTypeSettings.Xml ? ".xml" : ".txt";

            //市場情報
            IchibaInfoTypeSettings ichibaInfoT = this.enumSettingsHandler.GetSetting<IchibaInfoTypeSettings>();
            string? htmlExt = this.settingHandler.GetStringSetting(SettingsEnum.HtmlExt);
            string ichibaInfoExt = ichibaInfoT == IchibaInfoTypeSettings.Json ? ".json" : ichibaInfoT == IchibaInfoTypeSettings.Xml ? ".xml" : htmlExt ?? ".html";

            //FFmpeg系
            string commandFormat = this.settingHandler.GetStringSetting(SettingsEnum.FFmpegFormat) ?? Format.DefaultFFmpegFormat;
            if (commandFormat.IsNullOrEmpty())
            {
                commandFormat = Format.DefaultFFmpegFormat;
            }

            //コメントDL系
            int commentCountPerBlock = this.settingHandler.GetIntSetting(SettingsEnum.CommentCountPerBlock);

            //時関・並列係
            int commentFetchWaitSpan = this.settingHandler.GetIntSetting(SettingsEnum.CommentWaitSpan);
            if (commentFetchWaitSpan < 0)
            {
                commentFetchWaitSpan = LocalConstant.DefaultCommetFetchWaitSpan;
            }

            int maxParallelSegmentDownloadCount = this.settingHandler.GetIntSetting(SettingsEnum.MaxParallelSegDl);
            if (maxParallelSegmentDownloadCount <= 0)
            {
                maxParallelSegmentDownloadCount = NetConstant.DefaultMaxParallelDownloadCount;
            }

            int commentOffset = this.settingHandler.GetIntSetting(SettingsEnum.CommentOffset);
            if (commentOffset <= 0)
            {
                commentOffset = CommentCollection.NumberToThrough;
            }

            //履歴系
            bool saveSucceeded = !this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLSucceededHistory);
            bool saveFailed = !this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLFailedHistory);


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
                AppendingToLocalComment = this.IsAppendingCommentEnable.Value,
                OmittingXmlDeclaration = omitXmlDec,
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
                MaxParallelSegmentDLCount = maxParallelSegmentDownloadCount,
                CommentOffset = commentOffset,
                VideoInfoType = videoInfoT,
                SaveFailedHistory = saveFailed,
                SaveSucceededHistory = saveSucceeded,
                CommentCountPerBlock = commentCountPerBlock,
            };
        }

        public ReactiveProperty<bool> IsDownloadingVideoEnable { get; init; }

        public ReactiveProperty<bool> IsDownloadingCommentEnable { get; init; }

        public ReactiveProperty<bool> IsDownloadingCommentLogEnable { get; init; }

        public ReactiveProperty<bool> IsDownloadingOwnerComment { get; init; }

        public ReactiveProperty<bool> IsDownloadingEasyComment { get; init; }

        public ReactiveProperty<bool> IsDownloadingThumbEnable { get; init; }

        public ReactiveProperty<bool> IsOverwriteEnable { get; init; }

        public ReactiveProperty<bool> IsSkippingEnable { get; init; }

        public ReactiveProperty<bool> IsAppendingCommentEnable { get; init; }

        public ReactiveProperty<bool> IsCopyFromAnotherFolderEnable { get; init; }

        public ReactiveProperty<bool> IsNoEncodeEnable { get; init; }

        public ReactiveProperty<bool> IsLimittingCommentCountEnable { get; init; }

        public ReactiveProperty<bool> IsDownloadingVideoInfoEnable { get; init; }

        public ReactiveProperty<bool> IsDownloadingIchibaInfoEnable { get; init; }

        public ReactiveProperty<int> MaxCommentsCount { get; init; }

        public ReactiveProperty<VideoInfo::IResolution> Resolution { get; init; }

        public ReactiveProperty<VideoInfo::ThumbSize> ThumbnailSize { get; init; }
    }
}
