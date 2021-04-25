﻿using Niconicome.Models.Domain.Local.Store;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Local
{


    public interface ILocalSettingHandler
    {
        bool GetBoolSetting(Settings setting);
        string? GetStringSetting(Settings setting);
        int GetIntSetting(Settings setting);
        void SaveSetting<T>(T data, Settings setting);
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
        public string? GetStringSetting(Settings setting)
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
        public bool GetBoolSetting(Settings setting)
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
        public int GetIntSetting(Settings setting)
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
        public void SaveSetting<T>(T data, Settings setting)
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
        private string? GetSettingName(Settings settings)
        {
            return settings switch
            {
                Settings.FileNameFormat => STypes::SettingNames.FileNameFormat,
                Settings.PlayerAPath => STypes::SettingNames.PlayerAPath,
                Settings.PlayerBPath => STypes::SettingNames.PlayerBPath,
                Settings.AppUrlPath => STypes::SettingNames.AppUrlPath,
                Settings.AppUrlParam => STypes::SettingNames.AppUrlParam,
                Settings.AppIdPath => STypes::SettingNames.AppIdPath,
                Settings.AppIdParam => STypes::SettingNames.AppIdParam,
                Settings.FfmpegPath => STypes::SettingNames.FFmpegPath,
                Settings.DefaultFolder => STypes::SettingNames.DefaultFolder,
                Settings.CommentOffset => STypes::SettingNames.CommentOffset,
                Settings.DLVideo => STypes::SettingNames.IsDownloadingVideoEnable,
                Settings.DLComment => STypes::SettingNames.IsDownloadingCommentEnable,
                Settings.DLKako => STypes::SettingNames.IsDownloadingKakoroguEnable,
                Settings.DLEasy => STypes::SettingNames.IsDownloadingEasyEnable,
                Settings.DLThumb => STypes::SettingNames.IsDownloadingThumbEnable,
                Settings.DLOwner => STypes::SettingNames.IsDownloadingOwnerEnable,
                Settings.DLSkip => STypes::SettingNames.IsSkipEnable,
                Settings.DLCopy => STypes::SettingNames.IsCopyEnable,
                Settings.DLOverwrite => STypes::SettingNames.IsOverwriteEnable,
                Settings.SwitchOffset => STypes::SettingNames.IsAutoSwitchOffsetEnable,
                Settings.FFmpegShell => STypes::SettingNames.UseShellWhenLaunchingFFmpeg,
                Settings.AutologinEnable => STypes::SettingNames.IsAutologinEnable,
                Settings.AutologinMode => STypes::SettingNames.AutoLoginMode,
                Settings.MaxParallelDL => STypes::SettingNames.MaxParallelDownloadCount,
                Settings.MaxParallelSegDl => STypes::SettingNames.MaxParallelSegmentDownloadCount,
                Settings.DLAllFromQueue => STypes::SettingNames.DownloadAllWhenPushDLButton,
                Settings.AllowDupeOnStage => STypes::SettingNames.AllowDupeOnStage,
                Settings.ReplaceSBToMB => STypes::SettingNames.ReplaceSingleByteToMultiByte,
                Settings.OverrideVideoFileDTToUploadedDT => STypes::SettingNames.OverideVideoFileDTToUploadedDT,
                Settings.MaxCommentsCount => STypes::SettingNames.MaxCommentCount,
                Settings.LimitCommentsCount => STypes::SettingNames.LimitCommentCount,
                Settings.DLVideoInfo => STypes::SettingNames.IsDownloadingVideoInfoEnable,
                Settings.VideoInfoInJson => STypes::SettingNames.IsDownloadingVideoInfoInJsonEnable,
                Settings.MaxFetchCount => STypes::SettingNames.MaxParallelFetchCount,
                Settings.FetchSleepInterval => STypes::SettingNames.FetchSleepInterval,
                Settings.SkipSSLVerification => STypes::SettingNames.SkipSSLVerification,
                Settings.ExpandAll => STypes::SettingNames.ExpandTreeOnStartUp,
                Settings.InheritExpandedState => STypes::SettingNames.SaveTreePrevExpandedState,
                Settings.EnableResume => STypes::SettingNames.IsResumeEnable,
                Settings.MaxTmpDirCount => STypes::SettingNames.MaxTmpSegmentsDirCount,
                Settings.StoreOnlyNiconicoID => STypes::SettingNames.StoreOnlyNiconicoIDOnRegister,
                Settings.UnsafeCommentHandle => STypes::SettingNames.EnableUnsafeCommentHandle,
                Settings.MWSelectColumnWid => STypes::SettingNames.MainWindowSelectColumnWidth,
                Settings.MWIDColumnWid => STypes::SettingNames.MainWindowIDColumnWidth,
                Settings.MWTitleColumnWid => STypes::SettingNames.MainWindowTitleColumnWidth,
                Settings.MWUploadColumnWid => STypes::SettingNames.MainWindowUploadColumnWidth,
                Settings.MWViewCountColumnWid => STypes::SettingNames.MainWindowViewCountColumnWidth,
                Settings.MWDownloadedFlagColumnWid => STypes::SettingNames.MainWindowDownloadedFlagColumnWidth,
                Settings.MWStateColumnWid => STypes::SettingNames.MainWindowStateColumnWidth,
                Settings.MWThumbColumnWid=>STypes::SettingNames.MainWindowThumbnailColumnWidth,
                _ => null
            };
        }
    }

    public enum Settings
    {
        FileNameFormat,
        PlayerAPath,
        PlayerBPath,
        AppUrlPath,
        AppIdPath,
        AppUrlParam,
        AppIdParam,
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
    }
}
