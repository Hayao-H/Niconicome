using Niconicome.Models.Domain.Local.Store;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Local.Settings
{


    public interface ILocalSettingHandler
    {
        bool GetBoolSetting(SettingsEnum setting);
        string? GetStringSetting(SettingsEnum setting);
        int GetIntSetting(SettingsEnum setting);
        void SaveSetting<T>(T data, SettingsEnum setting);
    }

    public class LocalSettingHandler : ILocalSettingHandler
    {
        public LocalSettingHandler(ISettingHandler settingHandler)
        {
            this.settingHandler = settingHandler;
        }

        private readonly ISettingHandler settingHandler;

        /// <summary>
        /// 文字列の設定を取得する
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public string? GetStringSetting(SettingsEnum setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return null;

            if (this.settingHandler.Exists(settingname, SettingType.stringSetting))
            {
                return this.settingHandler.GetStringSetting(settingname).Data;
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

            if (this.settingHandler.Exists(settingname, SettingType.boolSetting))
            {
                return this.settingHandler.GetBoolSetting(settingname).Data;
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

            if (this.settingHandler.Exists(settingname, SettingType.intSetting))
            {
                return this.settingHandler.GetIntSetting(settingname).Data;
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
        public void SaveSetting<T>(T data, SettingsEnum setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return;

            if (data is bool boolData)
            {
                this.settingHandler.SaveBoolSetting(settingname, boolData);
            }
            else if (data is string stringData)
            {
                this.settingHandler.SaveStringSetting(settingname, stringData);
            }
            else if (data is int intData)
            {
                this.settingHandler.SaveIntSetting(settingname, intData);
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
                SettingsEnum.FileNameFormat => STypes::SettingNames.FileNameFormat,
                SettingsEnum.PlayerAPath => STypes::SettingNames.PlayerAPath,
                SettingsEnum.PlayerBPath => STypes::SettingNames.PlayerBPath,
                SettingsEnum.AppAPath => STypes::SettingNames.AppUrlPath,
                SettingsEnum.AppAParam => STypes::SettingNames.AppUrlParam,
                SettingsEnum.AppBPath => STypes::SettingNames.AppIdPath,
                SettingsEnum.AppBParam => STypes::SettingNames.AppIdParam,
                SettingsEnum.FfmpegPath => STypes::SettingNames.FFmpegPath,
                SettingsEnum.DefaultFolder => STypes::SettingNames.DefaultFolder,
                SettingsEnum.CommentOffset => STypes::SettingNames.CommentOffset,
                SettingsEnum.DLVideo => STypes::SettingNames.IsDownloadingVideoEnable,
                SettingsEnum.DLComment => STypes::SettingNames.IsDownloadingCommentEnable,
                SettingsEnum.DLKako => STypes::SettingNames.IsDownloadingKakoroguEnable,
                SettingsEnum.DLEasy => STypes::SettingNames.IsDownloadingEasyEnable,
                SettingsEnum.DLThumb => STypes::SettingNames.IsDownloadingThumbEnable,
                SettingsEnum.DLOwner => STypes::SettingNames.IsDownloadingOwnerEnable,
                SettingsEnum.DLSkip => STypes::SettingNames.IsSkipEnable,
                SettingsEnum.DLCopy => STypes::SettingNames.IsCopyEnable,
                SettingsEnum.DLOverwrite => STypes::SettingNames.IsOverwriteEnable,
                SettingsEnum.SwitchOffset => STypes::SettingNames.IsAutoSwitchOffsetEnable,
                SettingsEnum.FFmpegShell => STypes::SettingNames.UseShellWhenLaunchingFFmpeg,
                SettingsEnum.AutologinEnable => STypes::SettingNames.IsAutologinEnable,
                SettingsEnum.AutologinMode => STypes::SettingNames.AutoLoginMode,
                SettingsEnum.MaxParallelDL => STypes::SettingNames.MaxParallelDownloadCount,
                SettingsEnum.MaxParallelSegDl => STypes::SettingNames.MaxParallelSegmentDownloadCount,
                SettingsEnum.DLAllFromQueue => STypes::SettingNames.DownloadAllWhenPushDLButton,
                SettingsEnum.AllowDupeOnStage => STypes::SettingNames.AllowDupeOnStage,
                SettingsEnum.ReplaceSBToMB => STypes::SettingNames.ReplaceSingleByteToMultiByte,
                SettingsEnum.OverrideVideoFileDTToUploadedDT => STypes::SettingNames.OverideVideoFileDTToUploadedDT,
                SettingsEnum.MaxCommentsCount => STypes::SettingNames.MaxCommentCount,
                SettingsEnum.LimitCommentsCount => STypes::SettingNames.LimitCommentCount,
                SettingsEnum.DLVideoInfo => STypes::SettingNames.IsDownloadingVideoInfoEnable,
                SettingsEnum.VideoInfoInJson => STypes::SettingNames.IsDownloadingVideoInfoInJsonEnable,
                SettingsEnum.MaxFetchCount => STypes::SettingNames.MaxParallelFetchCount,
                SettingsEnum.FetchSleepInterval => STypes::SettingNames.FetchSleepInterval,
                SettingsEnum.SkipSSLVerification => STypes::SettingNames.SkipSSLVerification,
                SettingsEnum.ExpandAll => STypes::SettingNames.ExpandTreeOnStartUp,
                SettingsEnum.InheritExpandedState => STypes::SettingNames.SaveTreePrevExpandedState,
                SettingsEnum.EnableResume => STypes::SettingNames.IsResumeEnable,
                SettingsEnum.MaxTmpDirCount => STypes::SettingNames.MaxTmpSegmentsDirCount,
                SettingsEnum.StoreOnlyNiconicoID => STypes::SettingNames.StoreOnlyNiconicoIDOnRegister,
                SettingsEnum.UnsafeCommentHandle => STypes::SettingNames.EnableUnsafeCommentHandle,
                SettingsEnum.MWSelectColumnWid => STypes::SettingNames.MainWindowSelectColumnWidth,
                SettingsEnum.MWIDColumnWid => STypes::SettingNames.MainWindowIDColumnWidth,
                SettingsEnum.MWTitleColumnWid => STypes::SettingNames.MainWindowTitleColumnWidth,
                SettingsEnum.MWUploadColumnWid => STypes::SettingNames.MainWindowUploadColumnWidth,
                SettingsEnum.MWViewCountColumnWid => STypes::SettingNames.MainWindowViewCountColumnWidth,
                SettingsEnum.MWDownloadedFlagColumnWid => STypes::SettingNames.MainWindowDownloadedFlagColumnWidth,
                SettingsEnum.MWStateColumnWid => STypes::SettingNames.MainWindowStateColumnWidth,
                SettingsEnum.MWThumbColumnWid => STypes::SettingNames.MainWindowThumbnailColumnWidth,
                SettingsEnum.ReAllocateCommands => STypes::SettingNames.ReAllocateIfVideoisNotSaved,
                SettingsEnum.VListItemdbClick => STypes::SettingNames.VideoListItemdbClickAction,
                SettingsEnum.AutoRenameNetPlaylist => STypes::SettingNames.AutoRenamingAfterSetNetworkPlaylist,
                SettingsEnum.VideoInfoType => STypes::SettingNames.VideoInfoType,
                SettingsEnum.DlWithoutEncode => STypes::SettingNames.DownloadVideoWithoutEncodeEnable,
                SettingsEnum.IchibaInfoType => STypes::SettingNames.IchibaInfoType,
                SettingsEnum.DlIchiba => STypes::SettingNames.IsDownloadingIchibaInfoEnable,
                SettingsEnum.NoRestoreClumnWIdth => STypes::SettingNames.IsRestoringColumnWidthDisabled,
                SettingsEnum.FFProfileName => STypes::SettingNames.FirefoxProfileName,
                SettingsEnum.HtmlExt => STypes::SettingNames.HtmlFileExtension,
                SettingsEnum.JpegExt => STypes::SettingNames.JpegFileExtension,
                SettingsEnum.VideoinfoSuffix => STypes::SettingNames.VideoInfoSuffix,
                SettingsEnum.IchibaInfoSuffix => STypes::SettingNames.IchibaSuffix,
                SettingsEnum.DisableDLFailedHistory => STypes::SettingNames.DisableDownloadFailedHistory,
                SettingsEnum.DisableDLSucceededHistory => STypes::SettingNames.DisableDownloadSucceededHistory,
                SettingsEnum.SingletonWindows => STypes::SettingNames.LimitWindowsToSingleton,
                SettingsEnum.AppTheme => STypes::SettingNames.ApplicationTheme,
                SettingsEnum.ConfirmIfDownloading => STypes::SettingNames.ConfirmIfDownloading,
                SettingsEnum.MWBookMarkColumnWid => STypes::SettingNames.MainWindowBookMarkColumnWidth,
                SettingsEnum.FFmpegFormat => STypes::SettingNames.FFmpegFormat,
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
    }
}
