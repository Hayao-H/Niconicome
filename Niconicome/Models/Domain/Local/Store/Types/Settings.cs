using System;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Storage;

namespace Niconicome.Models.Domain.Local.Store.Types
{

    /// <summary>
    /// URL設定
    /// </summary>
    public record UrlSetting : IStorable, ISetting
    {
        public static string TableName { get; } = "urlsettings";

        [BsonId]
        public int Id { get; private set; }

        /// <summary>
        /// 設定名
        /// </summary>
        public string? SettingName { get; set; }

        /// <summary>
        /// 設定値
        /// </summary>
        public string? UrlString { get; set; }

        /// <summary>
        /// URLを取得する
        /// </summary>
        /// <returns></returns>
        public Uri GetUrl()
        {
            return this.UrlString != null ? new Uri(this.UrlString) : throw new InvalidOperationException("URLが設定されていないので、オブジェクトを初期化できません。");
        }

    }

    /// <summary>
    /// アプリケーションの設定(真偽値)
    /// </summary>
    public record AppSettingBool : IStorable
    {
        public static string TableName { get; } = "appsettingsbool";

        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// 設定名
        /// </summary>
        public string? SettingName { get; set; }

        /// <summary>
        /// 設定値
        /// </summary>
        public bool Value { get; set; }
    }


    /// <summary>
    /// アプリケーションの設定(文字列)
    /// </summary>
    public record AppSettingString : IStorable
    {
        public static string TableName { get; } = "appsettingsstr";

        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// 設定名
        /// </summary>
        public string? SettingName { get; set; }

        /// <summary>
        /// 設定値
        /// </summary>
        public string? Value { get; set; }
    }

    /// <summary>
    /// アプリケーションの設定(整数値)
    /// </summary>
    public record AppSettingInt : IStorable
    {
        public static string TableName { get; } = "appsettingsint";

        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// 設定名
        /// </summary>
        public string? SettingName { get; set; }

        /// <summary>
        /// 設定値
        /// </summary>
        public int Value { get; set; } = -1;
    }

    public static class SettingNames
    {
        public static string FileNameFormat { get; private set; } = "filenameformat";
        public static string PlayerAPath { get; private set; } = "playerapath";
        public static string PlayerBPath { get; private set; } = "playerbpath";
        public static string AppUrlPath { get; private set; } = "appurlpath";
        public static string AppIdPath { get; private set; } = "appidpath";
        public static string AppUrlParam { get; private set; } = "appurlparam";
        public static string AppIdParam { get; private set; } = "appidparam";
        public static string FFmpegPath { get; private set; } = "ffmpegpath";
        public static string DefaultFolder { get; private set; } = "defaultfolder";
        public static string CommentOffset { get; private set; } = "commentoffset";
        public static string IsDownloadingVideoEnable { get; private set; } = "isdownloadingvideoenable";
        public static string IsDownloadingCommentEnable { get; private set; } = "isdownloadingcommentenable";
        public static string IsDownloadingKakoroguEnable { get; private set; } = "isdownloadingkakoroguenable";
        public static string IsDownloadingEasyEnable { get; private set; } = "isdownloadingeasyenable";
        public static string IsDownloadingThumbEnable { get; private set; } = "isdownloadingthumbenable";
        public static string IsDownloadingOwnerEnable { get; private set; } = "isdownloadingownerenable";
        public static string IsOverwriteEnable { get; private set; } = "isoverwriteenable";
        public static string IsSkipEnable { get; private set; } = "isskipenable";
        public static string IsCopyEnable { get; private set; } = "iscopyenable";
        public static string IsAutoSwitchOffsetEnable { get; set; } = "isautoswitchoffsetenable";
        public static string UseShellWhenLaunchingFFmpeg { get; set; } = "useshellwhenlaunchingffmpeg";
        public static string IsAutologinEnable { get; private set; } = "isautologinenable";
        public static string AutoLoginMode { get; private set; } = "autologinmode";
        public static string MaxParallelDownloadCount { get; private set; } = "maxparalleldownloadcount";
        public static string MaxParallelSegmentDownloadCount { get; private set; } = "maxparallelsegmentdownloadcount";
        public static string DownloadAllWhenPushDLButton { get; private set; } = "downloadallwhenpushdlbutton";
        public static string AllowDupeOnStage { get; private set; } = "allowdupeonstage";
        public static string ReplaceSingleByteToMultiByte { get; private set; } = "replacesinglebytetomultibyte";
        public static string OverideVideoFileDTToUploadedDT { get; private set; } = "overidevideofiledttouploadeddt";
        public static string LimitCommentCount { get; private set; } = "limitcommentcount";
        public static string MaxCommentCount { get; private set; } = "maxcommentcount";
        public static string IsDownloadingVideoInfoEnable { get; private set; } = "isdownloadingvideoinfoenable";
        public static string IsDownloadingIchibaInfoEnable { get; private set; } = "isdownloadingichibainfoenable";
        public static string IsDownloadingVideoInfoInJsonEnable { get; private set; } = "isdownloadingvideoinfoinjsonenable";
        public static string MaxParallelFetchCount { get; private set; } = "maxparallelfetchcount";
        public static string FetchSleepInterval { get; private set; } = "fetchsleepinterval";
        public static string SkipSSLVerification { get; private set; } = "skipsslverification";
        public static string SaveTreePrevExpandedState { get; private set; } = "savetreeprevexpandedstate";
        public static string ExpandTreeOnStartUp { get; private set; } = "expandtreeonstartup";
        public static string IsResumeEnable { get; private set; } = "isresumeenable";
        public static string MaxTmpSegmentsDirCount { get; private set; } = "maxtmpsegmentsdircount";
        public static string StoreOnlyNiconicoIDOnRegister { get; private set; } = "storeonlyniconicoidonregister";
        public static string EnableUnsafeCommentHandle { get; private set; } = "enableunsafecommenthandle";
        public static string MainWindowSelectColumnWidth { get; private set; } = "mainwindowselectcolumnwidth";
        public static string MainWindowIDColumnWidth { get; private set; } = "mainwindowidcolumnwidth";
        public static string MainWindowTitleColumnWidth { get; private set; } = "mainwindowtitlecolumnwidth";
        public static string MainWindowUploadColumnWidth { get; private set; } = "mainwindowuploadcolumnwidth";
        public static string MainWindowViewCountColumnWidth { get; private set; } = "mainwindowviewcountcolumnwidth";
        public static string MainWindowDownloadedFlagColumnWidth { get; private set; } = "mainwindowdownloadedflagcolumnwidth";
        public static string MainWindowStateColumnWidth { get; private set; } = "mainwindowstatecolumnwidth";
        public static string MainWindowThumbnailColumnWidth { get; private set; } = "mainwindowthumbnailcolumnwidth";
        public static string MainWindowBookMarkColumnWidth { get; private set; } = "mainwindowbookmarkcolumnwidth";
        public static string ReAllocateIfVideoisNotSaved { get; private set; } = "reallocateifvideoisnotsaved";
        public static string VideoListItemdbClickAction { get; private set; } = "videolistitemdbclickaction";
        public static string AutoRenamingAfterSetNetworkPlaylist { get; private set; } = "autorenamingaftersetnetworkplaylist";
        public static string VideoInfoType { get; private set; } = "videoInfotype";
        public static string DownloadVideoWithoutEncodeEnable { get; private set; } = "downloadvideowithoutencodeenable";
        public static string IchibaInfoType { get; private set; } = "chibainfotype";
        public static string IsRestoringColumnWidthDisabled { get; private set; } = "isrestoringcolumnwidthdisabled";
        public static string FirefoxProfileName { get; private set; } = "firefoxprofilename";
        public static string HtmlFileExtension { get; private set; } = "htmlfileextension";
        public static string JpegFileExtension { get; private set; } = "jpegfileextension";
        public static string VideoInfoSuffix { get; private set; } = "videoinfosuffix";
        public static string IchibaSuffix { get; private set; } = "ichibainfosuffix";
        public static string DisableDownloadSucceededHistory { get; private set; } = "disabledownloadsucceededhistory";
        public static string DisableDownloadFailedHistory { get; private set; } = "disabledownloadfailedhistory";
        public static string LimitWindowsToSingleton { get; private set; } = "limitwindowstosingleton";
        public static string ApplicationTheme { get; private set; } = "applicationtheme";
        public static string ConfirmIfDownloading { get; private set; } = "confirmifdownloading";
        public static string FFmpegFormat { get; private set; } = "ffmpegformat";
        public static string ThumbnailSize { get; private set; } = "thumbnailsize";
        public static string DisableScrollRestore { get; private set; } = "disablescrollrestore";
        public static string ThumbnailSuffix { get; private set; } = "thumbnailsuffix";
        public static string OwnerCommentSuffix { get; private set; } = "ownercommentsuffix";
        public static string IsDeveloppersMode { get; private set; } = "isdeveloppersmode";
        public static string IsAddonDebuggingEnable { get; private set; } = "isaddondebuggingenable";
        public static string IsDlTImerEveryDayEnable { get; private set; } = "isdltimereverydayenable";
        public static string PostDownloadAction { get; private set; } = "postdownloadaction";
        public static string EnonomyQualitySuffix { get; private set; } = "enonomyqualitysuffix";
        public static string IsExperimentalCommentSafetySystemEnable { get; private set; } = "isexperimentalcommentsafetysystemenable";
        public static string SnackbarDuration { get; private set; } = "snackbarduration";
        public static string CommentFetchWaitSpan { get; private set; } = "commentfetchwaitspan";
        public static string MainWindowEconomyColumnWidth { get; private set; } = "mainwindoweconomycolumnwidth";
        public static string DeleteExistingEconomyFile { get; private set; } = "deleteexistingeconomyfile";
        public static string SearchVideosExact { get; private set; } = "searchvideosexact";
        public static string ShowDownloadTasksAsTab { get; private set; } = "showdownloadtasksastab";
    }
}
