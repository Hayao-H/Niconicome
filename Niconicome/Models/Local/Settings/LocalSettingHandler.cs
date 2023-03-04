using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.Settings
{


    public interface ILocalSettingHandler
    {
        bool GetBoolSetting(SettingsEnum setting);
        string? GetStringSetting(SettingsEnum setting);
        int GetIntSetting(SettingsEnum setting);
        void SaveSetting<T>(T data, SettingsEnum setting) where T : notnull;
    }

    public class LocalSettingHandler : ILocalSettingHandler
    {
        public LocalSettingHandler(ISettingsContainer settingsConainer)
        {
            this._settingsConainer = settingsConainer;
        }

        private readonly ISettingsContainer _settingsConainer;

        /// <summary>
        /// 文字列の設定を取得する
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public string? GetStringSetting(SettingsEnum setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return null;

            IAttemptResult<ISettingInfo<string>> result = this._settingsConainer.GetSetting(settingname, "");

            if (result.IsSucceeded && result.Data is not null)
            {
                return result.Data.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 真偽値の設定を取得する
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool GetBoolSetting(SettingsEnum setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return false;

            IAttemptResult<ISettingInfo<bool>> result = this._settingsConainer.GetSetting(settingname, false);

            if (result.IsSucceeded && result.Data is not null)
            {
                return result.Data.Value;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 整数値の設定を取得する
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int GetIntSetting(SettingsEnum setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return -1;

            IAttemptResult<ISettingInfo<int>> result = this._settingsConainer.GetSetting(settingname, -1);

            if (result.IsSucceeded && result.Data is not null)
            {
                return result.Data.Value;
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="setting"></param>
        public void SaveSetting<T>(T data, SettingsEnum setting) where T : notnull
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return;

            IAttemptResult<ISettingInfo<T>> result = this._settingsConainer.GetSetting(settingname, data);

            if (result.IsSucceeded && result.Data is not null)
            {
                result.Data.Value = data;
            }
        }

        /// <summary>
        /// 設定名を取得する
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private string? GetSettingName(SettingsEnum settings)
        {
            return settings switch
            {
                SettingsEnum.FileNameFormat => SettingNames.FileNameFormat,
                SettingsEnum.PlayerAPath => SettingNames.PlayerAPath,
                SettingsEnum.PlayerBPath => SettingNames.PlayerBPath,
                SettingsEnum.AppAPath => SettingNames.AppUrlPath,
                SettingsEnum.AppAParam => SettingNames.AppUrlParam,
                SettingsEnum.AppBPath => SettingNames.AppIdPath,
                SettingsEnum.AppBParam => SettingNames.AppIdParam,
                SettingsEnum.FfmpegPath => SettingNames.FFmpegPath,
                SettingsEnum.DefaultFolder => SettingNames.DefaultFolder,
                SettingsEnum.CommentOffset => SettingNames.CommentOffset,
                SettingsEnum.DLVideo => SettingNames.IsDownloadingVideoEnable,
                SettingsEnum.DLComment => SettingNames.IsDownloadingCommentEnable,
                SettingsEnum.DLKako => SettingNames.IsDownloadingKakoroguEnable,
                SettingsEnum.DLEasy => SettingNames.IsDownloadingEasyEnable,
                SettingsEnum.DLThumb => SettingNames.IsDownloadingThumbEnable,
                SettingsEnum.DLOwner => SettingNames.IsDownloadingOwnerEnable,
                SettingsEnum.DLSkip => SettingNames.IsSkipEnable,
                SettingsEnum.DLCopy => SettingNames.IsCopyEnable,
                SettingsEnum.DLOverwrite => SettingNames.IsOverwriteEnable,
                SettingsEnum.SwitchOffset => SettingNames.IsAutoSwitchOffsetEnable,
                SettingsEnum.FFmpegShell => SettingNames.UseShellWhenLaunchingFFmpeg,
                SettingsEnum.AutologinEnable => SettingNames.IsAutologinEnable,
                SettingsEnum.AutologinMode => SettingNames.AutoLoginMode,
                SettingsEnum.MaxParallelDL => SettingNames.MaxParallelDownloadCount,
                SettingsEnum.MaxParallelSegDl => SettingNames.MaxParallelSegmentDownloadCount,
                SettingsEnum.DLAllFromQueue => SettingNames.DownloadAllWhenPushDLButton,
                SettingsEnum.AllowDupeOnStage => SettingNames.AllowDupeOnStage,
                SettingsEnum.ReplaceSBToMB => SettingNames.ReplaceSingleByteToMultiByte,
                SettingsEnum.OverrideVideoFileDTToUploadedDT => SettingNames.OverideVideoFileDTToUploadedDT,
                SettingsEnum.MaxCommentsCount => SettingNames.MaxCommentCount,
                SettingsEnum.LimitCommentsCount => SettingNames.LimitCommentCount,
                SettingsEnum.DLVideoInfo => SettingNames.IsDownloadingVideoInfoEnable,
                SettingsEnum.VideoInfoInJson => SettingNames.IsDownloadingVideoInfoInJsonEnable,
                SettingsEnum.MaxFetchCount => SettingNames.MaxParallelFetchCount,
                SettingsEnum.FetchSleepInterval => SettingNames.FetchSleepInterval,
                SettingsEnum.SkipSSLVerification => SettingNames.SkipSSLVerification,
                SettingsEnum.ExpandAll => SettingNames.ExpandTreeOnStartUp,
                SettingsEnum.InheritExpandedState => SettingNames.SaveTreePrevExpandedState,
                SettingsEnum.EnableResume => SettingNames.IsResumeEnable,
                SettingsEnum.MaxTmpDirCount => SettingNames.MaxTmpSegmentsDirCount,
                SettingsEnum.StoreOnlyNiconicoID => SettingNames.StoreOnlyNiconicoIDOnRegister,
                SettingsEnum.UnsafeCommentHandle => SettingNames.EnableUnsafeCommentHandle,
                SettingsEnum.MWSelectColumnWid => SettingNames.MainWindowSelectColumnWidth,
                SettingsEnum.MWIDColumnWid => SettingNames.MainWindowIDColumnWidth,
                SettingsEnum.MWTitleColumnWid => SettingNames.MainWindowTitleColumnWidth,
                SettingsEnum.MWUploadColumnWid => SettingNames.MainWindowUploadColumnWidth,
                SettingsEnum.MWViewCountColumnWid => SettingNames.MainWindowViewCountColumnWidth,
                SettingsEnum.MWDownloadedFlagColumnWid => SettingNames.MainWindowDownloadedFlagColumnWidth,
                SettingsEnum.MWStateColumnWid => SettingNames.MainWindowStateColumnWidth,
                SettingsEnum.MWThumbColumnWid => SettingNames.MainWindowThumbnailColumnWidth,
                SettingsEnum.ReAllocateCommands => SettingNames.ReAllocateIfVideoisNotSaved,
                SettingsEnum.VListItemdbClick => SettingNames.VideoListItemdbClickAction,
                SettingsEnum.AutoRenameNetPlaylist => SettingNames.AutoRenamingAfterSetNetworkPlaylist,
                SettingsEnum.VideoInfoType => SettingNames.VideoInfoType,
                SettingsEnum.DlWithoutEncode => SettingNames.DownloadVideoWithoutEncodeEnable,
                SettingsEnum.IchibaInfoType => SettingNames.IchibaInfoType,
                SettingsEnum.DlIchiba => SettingNames.IsDownloadingIchibaInfoEnable,
                SettingsEnum.NoRestoreClumnWIdth => SettingNames.IsRestoringColumnWidthDisabled,
                SettingsEnum.FFProfileName => SettingNames.FirefoxProfileName,
                SettingsEnum.HtmlExt => SettingNames.HtmlFileExtension,
                SettingsEnum.JpegExt => SettingNames.JpegFileExtension,
                SettingsEnum.VideoinfoSuffix => SettingNames.VideoInfoSuffix,
                SettingsEnum.IchibaInfoSuffix => SettingNames.IchibaSuffix,
                SettingsEnum.DisableDLFailedHistory => SettingNames.DisableDownloadFailedHistory,
                SettingsEnum.DisableDLSucceededHistory => SettingNames.DisableDownloadSucceededHistory,
                SettingsEnum.SingletonWindows => SettingNames.LimitWindowsToSingleton,
                SettingsEnum.AppTheme => SettingNames.ApplicationTheme,
                SettingsEnum.ConfirmIfDownloading => SettingNames.ConfirmIfDownloading,
                SettingsEnum.MWBookMarkColumnWid => SettingNames.MainWindowBookMarkColumnWidth,
                SettingsEnum.FFmpegFormat => SettingNames.FFmpegFormat,
                SettingsEnum.ThumbSize => SettingNames.ThumbnailSize,
                SettingsEnum.DisableScrollRestore => SettingNames.DisableScrollRestore,
                SettingsEnum.OwnerComSuffix => SettingNames.OwnerCommentSuffix,
                SettingsEnum.ThumbSuffix => SettingNames.ThumbnailSuffix,
                SettingsEnum.IsDevMode => SettingNames.IsDeveloppersMode,
                SettingsEnum.DlTimerEveryDay => SettingNames.IsDlTImerEveryDayEnable,
                SettingsEnum.PostDlAction => SettingNames.PostDownloadAction,
                SettingsEnum.EconomySuffix => SettingNames.EnonomyQualitySuffix,
                SettingsEnum.ExperimentalSafety => SettingNames.IsExperimentalCommentSafetySystemEnable,
                SettingsEnum.SnackbarDuration => SettingNames.SnackbarDuration,
                SettingsEnum.CommentWaitSpan => SettingNames.CommentFetchWaitSpan,
                SettingsEnum.MWEconomyColumnWid => SettingNames.MainWindowEconomyColumnWidth,
                SettingsEnum.DeleteEcoFile => SettingNames.DeleteExistingEconomyFile,
                SettingsEnum.SearchExact => SettingNames.SearchVideosExact,
                SettingsEnum.ShowTasksAsTab => SettingNames.ShowDownloadTasksAsTab,
                SettingsEnum.CommentCountPerBlock => SettingNames.CommentCountPerBlock,
                SettingsEnum.AppendComment => SettingNames.IsAppendingToLocalCommentEnable,
                SettingsEnum.OmitXmlDeclaration => SettingNames.IsOmittingXmlDeclarationIsEnable,
                _ => null
            };
        }
    }

    public enum SettingsEnum
    {
        FileNameFormat,
        PlayerAPath,
        PlayerBPath,
        AppAPath,
        AppBPath,
        AppAParam,
        AppBParam,
        FfmpegPath,
        DefaultFolder,
        CommentOffset,
        DLVideo,
        DLComment,
        DLKako,
        DLEasy,
        DLThumb,
        DLOwner,
        DLSkip,
        DLCopy,
        DLOverwrite,
        SwitchOffset,
        FFmpegShell,
        AutologinEnable,
        AutologinMode,
        MaxParallelDL,
        MaxParallelSegDl,
        DLAllFromQueue,
        AllowDupeOnStage,
        ReplaceSBToMB,
        OverrideVideoFileDTToUploadedDT,
        LimitCommentsCount,
        MaxCommentsCount,
        DLVideoInfo,
        VideoInfoInJson,
        MaxFetchCount,
        FetchSleepInterval,
        SkipSSLVerification,
        ExpandAll,
        InheritExpandedState,
        EnableResume,
        MaxTmpDirCount,
        StoreOnlyNiconicoID,
        UnsafeCommentHandle,
        MWSelectColumnWid,
        MWIDColumnWid,
        MWTitleColumnWid,
        MWUploadColumnWid,
        MWViewCountColumnWid,
        MWDownloadedFlagColumnWid,
        MWStateColumnWid,
        MWThumbColumnWid,
        MWBookMarkColumnWid,
        ReAllocateCommands,
        VListItemdbClick,
        AutoRenameNetPlaylist,
        VideoInfoType,
        DlWithoutEncode,
        IchibaInfoType,
        DlIchiba,
        NoRestoreClumnWIdth,
        FFProfileName,
        HtmlExt,
        JpegExt,
        VideoinfoSuffix,
        IchibaInfoSuffix,
        DisableDLFailedHistory,
        DisableDLSucceededHistory,
        SingletonWindows,
        AppTheme,
        ConfirmIfDownloading,
        FFmpegFormat,
        ThumbSize,
        DisableScrollRestore,
        OwnerComSuffix,
        ThumbSuffix,
        IsDevMode,
        DlTimerEveryDay,
        PostDlAction,
        EconomySuffix,
        ExperimentalSafety,
        SnackbarDuration,
        CommentWaitSpan,
        MWEconomyColumnWid,
        DeleteEcoFile,
        SearchExact,
        ShowTasksAsTab,
        CommentCountPerBlock,
        AppendComment,
        OmitXmlDeclaration,
    }
}
