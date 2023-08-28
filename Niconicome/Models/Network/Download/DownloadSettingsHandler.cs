using System;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico.Download.Comment;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Playlist.V2;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;


namespace Niconicome.Models.Network.Download
{
    public interface IDownloadSettingsHandler
    {
        /// <summary>? 
        /// 別フォルダーからコピー
        /// </summary>? 
        ISettingInfo<bool> IsCopyFromAnotherFolderEnable { get; }

        /// <summary>? 
        /// コメントDL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingCommentEnable { get; }

        /// <summary>? 
        /// 過去ログDL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingCommentLogEnable { get; }

        /// <summary>? 
        /// かんたんコメントDL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingEasyComment { get; }

        /// <summary>? 
        /// 投コメDL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingOwnerComment { get; }

        /// <summary>? 
        /// サムネDL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingThumbEnable { get; }

        /// <summary>? 
        /// 動画DL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingVideoEnable { get; }

        /// <summary>? 
        /// 動画情報DL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingVideoInfoEnable { get; }

        /// <summary>? 
        /// 市場情報DL
        /// </summary>? 
        ISettingInfo<bool> IsDownloadingIchibaInfoEnable { get; }

        /// <summary>? 
        /// コメ数制限有効フラグ
        /// </summary>? 
        ISettingInfo<bool> IsLimittingCommentCountEnable { get; }

        /// <summary>? 
        /// 上書きフラグ
        /// </summary>? 
        ISettingInfo<bool> IsOverwriteEnable { get; }

        /// <summary>? 
        /// スキップフラグ
        /// </summary>? 
        ISettingInfo<bool> IsSkippingEnable { get; }

        /// <summary>? 
        /// エンコードスキップフラグ
        /// </summary>? 
        ISettingInfo<bool> IsNoEncodeEnable { get; }

        /// <summary>? 
        /// コメント追記フラグ
        /// </summary>? 
        ISettingInfo<bool> IsAppendingCommentEnable { get; }

        /// <summary>? 
        /// コメ数制限
        /// </summary>? 
        ISettingInfo<int> MaxCommentsCount { get; }

        /// <summary>? 
        /// 解像度設定
        /// </summary>? 
        IBindableProperty<VideoInfo::IResolution> Resolution { get; }

        /// <summary>? 
        /// サムネサイズ設定
        /// </summary>? 
        ISettingInfo<VideoInfo::ThumbSize> ThumbnailSize { get; }

        /// <summary>
        /// DL設定を構築
        /// </summary>
        /// <returns></returns>
        DownloadSettings CreateDownloadSettings();
    }

    public class DownloadSettingsHandler : BindableBase, IDownloadSettingsHandler
    {
        public DownloadSettingsHandler(ISettingsContainer settingsConainer, IPlaylistVideoContainer playlistVideoContainer)
        {
            this._playlistVideoContainer = playlistVideoContainer;
            this._container = settingsConainer;

            this.IsDownloadingVideoInfoEnable = settingsConainer.GetSetting(SettingNames.IsDownloadingVideoInfoEnable, false).Data!;
            this.IsDownloadingVideoEnable = settingsConainer.GetSetting(SettingNames.IsDownloadingVideoEnable, false).Data!;
            this.IsDownloadingCommentEnable = settingsConainer.GetSetting(SettingNames.IsDownloadingCommentEnable, false).Data!;
            this.IsDownloadingCommentLogEnable = settingsConainer.GetSetting(SettingNames.IsDownloadingKakoroguEnable, false).Data!;
            this.IsDownloadingEasyComment = settingsConainer.GetSetting(SettingNames.IsDownloadingEasyEnable, false).Data!;
            this.IsDownloadingThumbEnable = settingsConainer.GetSetting(SettingNames.IsDownloadingThumbEnable, false).Data!;
            this.IsDownloadingOwnerComment = settingsConainer.GetSetting(SettingNames.IsDownloadingOwnerEnable, false).Data!;
            this.IsOverwriteEnable = settingsConainer.GetSetting(SettingNames.IsOverwriteEnable, false).Data!;
            this.IsSkippingEnable = settingsConainer.GetSetting(SettingNames.IsSkipEnable, false).Data!;
            this.IsCopyFromAnotherFolderEnable = settingsConainer.GetSetting(SettingNames.IsCopyEnable, false).Data!;
            this.IsLimittingCommentCountEnable = settingsConainer.GetSetting(SettingNames.LimitCommentCount, false).Data!;
            this.MaxCommentsCount = settingsConainer.GetSetting(SettingNames.MaxCommentCount, -1).Data!;
            this.IsNoEncodeEnable = settingsConainer.GetSetting(SettingNames.DownloadVideoWithoutEncodeEnable, false).Data!;
            this.IsDownloadingIchibaInfoEnable = settingsConainer.GetSetting(SettingNames.IsDownloadingIchibaInfoEnable, false).Data!;
            this.ThumbnailSize = settingsConainer.GetSetting(SettingNames.ThumbnailSize, VideoInfo::ThumbSize.Large).Data!;
            this.IsAppendingCommentEnable = settingsConainer.GetSetting(SettingNames.IsAppendingToLocalCommentEnable, false).Data!;

            this.Resolution = new BindableProperty<VideoInfo::IResolution>(new VideoInfo::Resolution("1920x1080"));
        }

        ~DownloadSettingsHandler()
        {
            this.Dispose();
        }

        #region field

        private readonly IPlaylistVideoContainer _playlistVideoContainer;

        private readonly ISettingsContainer _container;
        #endregion

        public DownloadSettings CreateDownloadSettings()
        {

            if (this._playlistVideoContainer.CurrentSelectedPlaylist is null) throw new InvalidOperationException("");

            //フラグ系
            bool replaceStricted = this._container.GetSetting(SettingNames.ReplaceSingleByteToMultiByte, false).Data!.Value;
            bool overrideVideoDT = this._container.GetSetting(SettingNames.OverideVideoFileDTToUploadedDT, false).Data!.Value;
            bool resumeEnable = this._container.GetSetting(SettingNames.IsResumeEnable, false).Data!.Value;
            bool unsafeHandle = this._container.GetSetting(SettingNames.EnableUnsafeCommentHandle, false).Data!.Value;
            bool experimentalSafety = true;//this._container.GetSetting(SettingNames.IsExperimentalCommentSafetySystemEnable, false).Data!.Value;
            bool deleteEconomyFile = this._container.GetSetting(SettingNames.DeleteExistingEconomyFile, false).Data!.Value;
            bool omitXmlDec = this._container.GetSetting(SettingNames.IsOmittingXmlDeclarationIsEnable, false).Data!.Value;

            //ファイル系
            string folderPath = this._playlistVideoContainer.CurrentSelectedPlaylist.FolderPath;
            string fileFormat = this._container.GetSetting(SettingNames.FileNameFormat, Format.DefaultFileNameFormat).Data!.Value;
            string thumbExt = this._container.GetSetting(SettingNames.JpegFileExtension, FileFolder.DefaultJpegFileExt).Data!.Value;
            string videoInfoSuffix = this._container.GetSetting(SettingNames.VideoInfoSuffix, Format.DefaultVideoInfoSuffix).Data!.Value;
            string ichibaInfoSuffix = this._container.GetSetting(SettingNames.IchibaSuffix, Format.DefaultIchibaSuffix).Data!.Value;
            string thumbSuffix = this._container.GetSetting(SettingNames.ThumbnailSuffix, Format.DefaultThumbnailSuffix).Data!.Value;
            string ownerComSuffix = this._container.GetSetting(SettingNames.OwnerCommentSuffix, Format.DefaultOwnerCommentSuffix).Data!.Value;
            string economySuffix = this._container.GetSetting(SettingNames.EnonomyQualitySuffix, Format.DefaultEconomyVideoSuffix).Data!.Value;

            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = this._playlistVideoContainer.CurrentSelectedPlaylist.TemporaryFolderPath;
            }

            //動画情報
            VideoInfoTypeSettings videoInfoT = this._container.GetSetting(SettingNames.VideoInfoType, VideoInfoTypeSettings.Text).Data!.Value;
            string videoInfoExt = videoInfoT == VideoInfoTypeSettings.Json ? ".json" : videoInfoT == VideoInfoTypeSettings.Xml ? ".xml" : ".txt";

            //市場情報
            IchibaInfoTypeSettings ichibaInfoT = this._container.GetSetting(SettingNames.IchibaInfoType, IchibaInfoTypeSettings.Html).Data!.Value;
            string htmlExt = this._container.GetSetting(SettingNames.HtmlFileExtension, FileFolder.DefaultHtmlFileExt).Data!.Value;
            string ichibaInfoExt = ichibaInfoT == IchibaInfoTypeSettings.Json ? ".json" : ichibaInfoT == IchibaInfoTypeSettings.Xml ? ".xml" : htmlExt;

            //FFmpeg系
            string commandFormat = this._container.GetSetting(SettingNames.FFmpegFormat, Format.DefaultFFmpegFormat).Data!.Value;
            if (commandFormat.IsNullOrEmpty())
            {
                commandFormat = Format.DefaultFFmpegFormat;
            }

            //コメントDL系
            int commentCountPerBlock = this._container.GetSetting(SettingNames.CommentCountPerBlock, NetConstant.DefaultCommentCountPerBlock).Data!.Value;

            //時関・並列係
            int commentFetchWaitSpan = this._container.GetSetting(SettingNames.CommentFetchWaitSpan, LocalConstant.DefaultCommetFetchWaitSpan).Data!.Value;
            if (commentFetchWaitSpan < 0)
            {
                commentFetchWaitSpan = LocalConstant.DefaultCommetFetchWaitSpan;
            }

            int maxParallelSegmentDownloadCount = this._container.GetSetting(SettingNames.MaxParallelSegmentDownloadCount, NetConstant.DefaultMaxParallelSegmentDownloadCount).Data!.Value;
            if (maxParallelSegmentDownloadCount <= 0)
            {
                maxParallelSegmentDownloadCount = NetConstant.DefaultMaxParallelDownloadCount;
            }

            //履歴系
            bool saveSucceeded = !this._container.GetSetting(SettingNames.DisableDownloadSucceededHistory, false).Data!.Value; ;
            bool saveFailed = !this._container.GetSetting(SettingNames.DisableDownloadFailedHistory, false).Data!.Value; ;

            //外部ダウンローダー系
            bool useExternalSoftware = this._container.GetOnlyValue(SettingNames.UseExternalSoftware, false).Data;
            string externalSoftwarePath = this._container.GetOnlyValue(SettingNames.ExternalDLSoftwarePath, string.Empty).Data!;
            string externalSoftwareParam = this._container.GetOnlyValue(SettingNames.ExternalDLSoftwareParam, string.Empty).Data!;

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
                PlaylistID = this._playlistVideoContainer.CurrentSelectedPlaylist.ID,
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
                VideoInfoType = videoInfoT,
                SaveFailedHistory = saveFailed,
                SaveSucceededHistory = saveSucceeded,
                CommentCountPerBlock = commentCountPerBlock,
                UseExternalDownloader = useExternalSoftware,
                ExternalDownloaderParam = externalSoftwareParam,
                ExternalDownloaderPath = externalSoftwarePath,
            };
        }

        public ISettingInfo<bool> IsDownloadingVideoEnable { get; init; }

        public ISettingInfo<bool> IsDownloadingCommentEnable { get; init; }

        public ISettingInfo<bool> IsDownloadingCommentLogEnable { get; init; }

        public ISettingInfo<bool> IsDownloadingOwnerComment { get; init; }

        public ISettingInfo<bool> IsDownloadingEasyComment { get; init; }

        public ISettingInfo<bool> IsDownloadingThumbEnable { get; init; }

        public ISettingInfo<bool> IsOverwriteEnable { get; init; }

        public ISettingInfo<bool> IsSkippingEnable { get; init; }

        public ISettingInfo<bool> IsAppendingCommentEnable { get; init; }

        public ISettingInfo<bool> IsCopyFromAnotherFolderEnable { get; init; }

        public ISettingInfo<bool> IsNoEncodeEnable { get; init; }

        public ISettingInfo<bool> IsLimittingCommentCountEnable { get; init; }

        public ISettingInfo<bool> IsDownloadingVideoInfoEnable { get; init; }

        public ISettingInfo<bool> IsDownloadingIchibaInfoEnable { get; init; }

        public ISettingInfo<int> MaxCommentsCount { get; init; }

        public IBindableProperty<VideoInfo::IResolution> Resolution { get; init; }

        public ISettingInfo<VideoInfo::ThumbSize> ThumbnailSize { get; init; }
    }
}
