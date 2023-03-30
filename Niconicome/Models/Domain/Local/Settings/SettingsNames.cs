using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Niconicome.Models.Domain.Local.Settings
{
    public static class SettingNames
    {
        //Download.File
        public static string FileNameFormat { get; private set; } = "Download.File.FileNameFormat";

        public static string FFmpegPath { get; private set; } = "Download.File.FFmpegPath";

        public static string FFprobePath { get; private set; } = "Download.File.FFprobePath";

        public static string DefaultFolder { get; private set; } = "Download.File.Defaultfolder";

        public static string UseShellWhenLaunchingFFmpeg { get; set; } = "Download.File.UseShellWhenLaunchingffmpeg";

        public static string ReplaceSingleByteToMultiByte { get; private set; } = "Download.File.ReplaceSingleByteToMultiByte";

        public static string OverideVideoFileDTToUploadedDT { get; private set; } = "Download.File.OverideVideoFileDateTimeToUploadedDateTime";

        public static string HtmlFileExtension { get; private set; } = "Download.File.HtmlFileExtension";

        public static string JpegFileExtension { get; private set; } = "Download.File.JpegFileExtension";

        public static string VideoInfoSuffix { get; private set; } = "Download.File.VideoInfoSuffix";

        public static string IchibaSuffix { get; private set; } = "Download.File.IchibaSuffix";

        public static string EnonomyQualitySuffix { get; private set; } = "Download.File.EnonomyQualitySuffix";

        public static string DeleteExistingEconomyFile { get; private set; } = "Download.File.DeleteExistingEconomyFile";

        //Download.General
        public static string MaxParallelDownloadCount { get; private set; } = "Download.General.MaxParallelDownloadCount";

        public static string DownloadAllWhenPushDLButton { get; private set; } = "Download.General.DownloadAllVideosWhenDLButtonPushed";

        public static string AllowDupeOnStage { get; private set; } = "Download.General.AllowDupeOnStage";

        public static string IsDlTImerEveryDayEnable { get; private set; } = "Download.General.IsDlTImerEveryDayEnable";

        public static string PostDownloadAction { get; private set; } = "Download.General.PostDownloadAction";

        //Download.Video
        public static string IsResumeEnable { get; private set; } = "Download.Video.IsResumeEnable";

        public static string MaxTmpSegmentsDirCount { get; private set; } = "Download.Video.MaxTmporarySegmentsDirectoryCount";

        public static string MaxParallelSegmentDownloadCount { get; private set; } = "Download.Video.MaxParallelSegmentDownloadCount";

        public static string DownloadVideoWithoutEncodeEnable { get; private set; } = "Download.Video.IsDownloadingVideoWithoutEncodingEnabled";

        public static string FFmpegFormat { get; private set; } = "Download.Video.FFmpegParameterFormat";

        //Download.Comment
        public static string CommentOffset { get; private set; } = "Download.Comment.Commentoffset";

        public static string IsAutoSwitchOffsetEnable { get; set; } = "Download.Comment.IsAutoSwitchOffsetEnabled";

        public static string EnableUnsafeCommentHandle { get; private set; } = "Download.Comment.EnableUnsafeCommentHandle";

        public static string LimitCommentCount { get; private set; } = "Download.Comment.IsLimitingCommentCountEnabled";

        public static string MaxCommentCount { get; private set; } = "Download.Comment.MaxCommentCount";

        public static string OwnerCommentSuffix { get; private set; } = "Download.Comment.OwnerCommentSuffix";

        public static string CommentFetchWaitSpan { get; private set; } = "Download.Comment.CommentFetchWaitSpan";

        public static string CommentCountPerBlock { get; private set; } = "Download.Comment.CommentCountPerBlock";

        public static string IsAppendingToLocalCommentEnable { get; private set; } = "Download.Comment.IsAppendingToLocalCommentEnable";

        public static string IsOmittingXmlDeclarationIsEnable { get; private set; } = "Download.Comment.IsOmittingXmlDeclarationIsEnable";

        public static string IsExperimentalCommentSafetySystemEnable { get; private set; } = "Downlaod.Comment.IsExperimentalCommentSafetySystemEnable";

        //Download.Thumbnail
        public static string ThumbnailSize { get; private set; } = "Download.Thumbnail.ThumbnailSize";

        public static string ThumbnailSuffix { get; private set; } = "Download.Thumbnail.ThumbnailSuffix";

        //Download.VideoInfo
        public static string VideoInfoType { get; private set; } = "Download.VideoInfo.VideoInfoType";

        //Download.Ichiba
        public static string IchibaInfoType { get; private set; } = "Download.Ichiba.IchibaInfoType";

        //Download.Memory
        public static string IsDownloadingVideoEnable { get; private set; } = "Download.Memory.IsDownloadingVideoDnabled";

        public static string IsDownloadingCommentEnable { get; private set; } = "Download.Memory.IsDownloadingCommentEnabled";

        public static string IsDownloadingKakoroguEnable { get; private set; } = "Download.Memory.IsDownloadingKakoroguEnabled";

        public static string IsDownloadingEasyEnable { get; private set; } = "Download.Memory.IsDownloadingEasyeEabled";

        public static string IsDownloadingThumbEnable { get; private set; } = "Download.Memory.IsDownloadingThumbEnabled";

        public static string IsDownloadingOwnerEnable { get; private set; } = "Download.Memory.IsDownloadingOwnerEnabled";

        public static string IsOverwriteEnable { get; private set; } = "Download.Memory.IsOverwriteEnabled";

        public static string IsSkipEnable { get; private set; } = "Download.Memory.IsSkipEnabled";

        public static string IsCopyEnable { get; private set; } = "Download.Memory.IsCopyEnabled";

        public static string IsDownloadingVideoInfoEnable { get; private set; } = "Download.Memory.IsDownloadingVideoInfoEnabled";

        public static string IsDownloadingVideoInfoInJsonEnable { get; private set; } = "Download.Memory.IsDownloadingVideoInfoInJsonEnabled";

        public static string IsDownloadingIchibaInfoEnable { get; private set; } = "Download.Memory.IsDownloadingIchibaInfoEnable";

        //Download.History
        public static string DisableDownloadSucceededHistory { get; private set; } = "Download.History.DisableDownloadSucceededHistory";

        public static string DisableDownloadFailedHistory { get; private set; } = "Download.History.DisableDownloadFailedHistory";

        public static string DisablePlaybackHistory { get; private set; } = "Download.History.DisablePlaybackHistory";

        //Network.Session
        public static string IsAutologinEnable { get; private set; } = "Network.Session.IsAutoLoginEnabled";

        public static string AutoLoginMode { get; private set; } = "Network.Session.AutoLoginMode";

        public static string FirefoxProfileName { get; private set; } = "Network.Session.FirefoxProfileName";

        //Network.Fetch
        public static string MaxParallelFetchCount { get; private set; } = "Network.Fetch.MaxParallelFetchCount";

        public static string FetchSleepInterval { get; private set; } = "Network.Fetch.FetchSleepInterval";

        //Network.Security
        public static string SkipSSLVerification { get; private set; } = "Network.Security.SkipSSLVerification";

        //Local
        public static string PlayerAPath { get; private set; } = "Local.PlayerAPath";

        public static string PlayerBPath { get; private set; } = "Local.PlayerBPath";

        public static string AppUrlPath { get; private set; } = "Local.SendToAppBPath";

        public static string AppIdPath { get; private set; } = "Local.SendToAppPath";

        public static string AppUrlParam { get; private set; } = "Local.SendToAppBParam";

        public static string AppIdParam { get; private set; } = "Local.SendToAppAParam";

        public static string VideoSearchDirectories { get; private set; } = "Local.VideoSearchDirectories";

        //Playlist
        public static string SaveTreePrevExpandedState { get; private set; } = "Playlist.SavePlaylistTreePreviousExpandedState";

        public static string ExpandTreeOnStartUp { get; private set; } = "Playlist.ExpandPlaylistTreeOnStartUp";

        public static string AutoRenamingAfterSetNetworkPlaylist { get; private set; } = "Playlist.AutoRenamingAfterSetNetworkPlaylist";

        //Videolist.TextBoxAndButtons
        public static string StoreOnlyNiconicoIDOnRegister { get; private set; } = "Videolist.TextBoxAndButtons.StoreOnlyNiconicoIDOnRegister";

        //Videolist.ListView

        public static string ReAllocateIfVideoisNotSaved { get; private set; } = "Videolist.ListView.SwitchOpenInAppCommandToSendToAppCommandWhenNotDownloaded";

        public static string VideoListItemdbClickAction { get; private set; } = "Videolist.ListView.VideoListItemDoubleClickAction";

        public static string IsRestoringColumnWidthDisabled { get; private set; } = "Videolist.ListView.IsRestoringColumnWidthDisabled";

        public static string DisableScrollRestore { get; private set; } = "Videolist.ListView.DisableScrollRestore";

        //Videolist.ListView.Width
        public static string VideoListCheckBoxColumnWidth { get; private set; } = "Videolist.ListView.Width.CheckBoxColumnWidth";

        public static string VideoListThumbnailColumnWidth { get; private set; } = "Videolist.ListView.Width.ThumbnailColumnWidth";

        public static string VideoListTitleColumnWidth { get; private set; } = "Videolist.ListView.Width.TitleColumnWidth";

        public static string VideoListUploadedDateTimeColumnWidth { get; private set; } = "Videolist.ListView.Width.UploadedDateTimeColumnWidth";

        public static string VideoListIsDownloadedColumnWidth { get; private set; } = "Videolist.ListView.Width.IsDownloadedColumnWidth";

        public static string VideoListViewCountColumnWidth { get; private set; } = "Videolist.ListView.Width.ViewCountColumnWidth";

        public static string VideoListCommentCountColumnWidth { get; private set; } = "Videolist.ListView.Width.CommentCountColumnWidth";
        public static string VideoListMylistCountColumnWidth { get; private set; } = "Videolist.ListView.Width.MylistCountColumnWidth";

        public static string VideoListLikeCountColumnWidth { get; private set; } = "Videolist.ListView.Width.LikeCountColumnWidth";

        public static string VideoListMessageColumnWidth { get; private set; } = "Videolist.ListView.MessageColumnWidth";

        //Videolist.File
        public static string SearchVideosExact { get; private set; } = "Videolist.File.SearchVideosExact";

        //Application
        public static string LimitWindowsToSingleton { get; private set; } = "Application.LimitWindowsToSingleton";

        public static string ApplicationTheme { get; private set; } = "Application.ApplicationTheme";

        public static string ConfirmIfDownloading { get; private set; } = "Application.ConfirmOnExitIfDownloading";

        public static string SnackbarDuration { get; private set; } = "Applicatoin.SnackbarDuration";

        public static string ShowDownloadTasksAsTab { get; private set; } = "Application.ShowDownloadTasksAsTab";

        public static string IsDebugMode { get; private set; } = "Application.IsDebugMode";

        //Application.Window
        public static string WindowTop { get; private set; } = "Application.Window.WindowTop";

        public static string WindowLeft { get; private set; } = "Application.Window.WindowLeft";

        public static string WindowWidth { get; private set; } = "Application.Window.WindowWidth";

        public static string WindowHeight { get; private set; } = "Application.Window.WindowHeight";

        //Addons
        public static string IsDeveloppersMode { get; private set; } = "Addons.IsDeveloppersModeEnabled";

    }
}
