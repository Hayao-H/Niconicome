using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;


namespace Niconicome.Models.Network.Download
{
    interface IDownloadSettingsHandler
    {
        bool IsCopyFromAnotherFolderEnable { get; set; }
        bool IsDownloadingCommentEnable { get; set; }
        bool IsDownloadingCommentLogEnable { get; set; }
        bool IsDownloadingEasyComment { get; set; }
        bool IsDownloadingOwnerComment { get; set; }
        bool IsDownloadingThumbEnable { get; set; }
        bool IsDownloadingVideoEnable { get; set; }
        bool IsDownloadingVideoInfoEnable { get; set; }
        bool IsLimittingCommentCountEnable { get; set; }
        bool IsOverwriteEnable { get; set; }
        bool IsSkippingEnable { get; set; }
        int MaxCommentsCount { get; set; }

        DownloadSettings CreateDownloadSettings();
    }

    class DownloadSettingsHandler : IDownloadSettingsHandler
    {
        public DownloadSettingsHandler(ILocalSettingHandler settingHandler, ICurrent current)
        {
            this.settingHandler = settingHandler;
            this.current = current;
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

            if (this.current.SelectedPlaylist is null) throw new InvalidOperationException("");

            var replaceStricted = this.settingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB);
            var overrideVideoDT = this.settingHandler.GetBoolSetting(SettingsEnum.OverrideVideoFileDTToUploadedDT);
            var resumeEnable = this.settingHandler.GetBoolSetting(SettingsEnum.EnableResume);
            var unsafeHandle = this.settingHandler.GetBoolSetting(SettingsEnum.UnsafeCommentHandle);
            string folderPath = this.current.SelectedPlaylist.Folderpath.IsNullOrEmpty() ? this.settingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? "downloaded" : this.current.SelectedPlaylist.Folderpath;

            return new DownloadSettings
            {
                Video = this.IsDownloadingVideoEnable,
                Thumbnail = this.IsDownloadingThumbEnable,
                Overwrite = this.IsOverwriteEnable,
                Comment = this.IsDownloadingCommentEnable,
                DownloadLog = this.IsDownloadingCommentLogEnable,
                DownloadEasy = this.IsDownloadingEasyComment,
                DownloadOwner = this.IsDownloadingOwnerComment,
                FromAnotherFolder = this.IsCopyFromAnotherFolderEnable,
                Skip = this.IsSkippingEnable,
                FolderPath = folderPath,
                VerticalResolution = this.Resolution.Vertical,
                PlaylistID = this.current.SelectedPlaylist?.Id ?? 0,
                IsReplaceStrictedEnable = replaceStricted,
                OverrideVideoFileDateToUploadedDT = overrideVideoDT,
                MaxCommentsCount = this.IsLimittingCommentCountEnable ? this.MaxCommentsCount : 0,
                DownloadVideoInfo = this.IsDownloadingVideoInfoEnable,
                ResumeEnable = resumeEnable,
                EnableUnsafeCommentHandle = unsafeHandle,

            };
        }

        /// <summary>
        /// 動画DL
        /// </summary>
        public bool IsDownloadingVideoEnable { get; set; }

        /// <summary>
        /// コメントDL
        /// </summary>
        public bool IsDownloadingCommentEnable { get; set; }

        /// <summary>
        /// 過去ログ
        /// </summary>
        public bool IsDownloadingCommentLogEnable { get; set; }

        /// <summary>
        /// 投コメ
        /// </summary>
        public bool IsDownloadingOwnerComment { get; set; }

        /// <summary>
        /// かんたんコメント
        /// </summary>
        public bool IsDownloadingEasyComment { get; set; }

        /// <summary>
        /// サムネ
        /// </summary>
        public bool IsDownloadingThumbEnable { get; set; }

        /// <summary>
        /// 上書き
        /// </summary>
        public bool IsOverwriteEnable { get; set; }

        /// <summary>
        /// DL済みをスキップ
        /// </summary>
        public bool IsSkippingEnable { get; set; }

        /// <summary>
        /// 他フォルダーからコピー
        /// </summary>
        public bool IsCopyFromAnotherFolderEnable { get; set; }

        /// <summary>
        /// 最大コメ数
        /// </summary>
        public bool IsLimittingCommentCountEnable { get; set; }

        /// <summary>
        /// 動画情報
        /// </summary>
        public bool IsDownloadingVideoInfoEnable { get; set; }

        /// <summary>
        /// 最大コメ数
        /// </summary>
        public int MaxCommentsCount { get; set; }

        /// <summary>
        /// 解像度
        /// </summary>
        public VideoInfo::IResolution Resolution = new VideoInfo::Resolution("1920x1080");
    }
}
