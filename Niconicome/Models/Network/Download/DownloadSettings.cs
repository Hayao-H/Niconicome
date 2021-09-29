using System;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Cdl = Niconicome.Models.Domain.Niconico.Download.Comment;
using DDL = Niconicome.Models.Domain.Niconico.Download.Description;
using IDl = Niconicome.Models.Domain.Niconico.Download.Ichiba;
using Tdl = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using Vdl = Niconicome.Models.Domain.Niconico.Download.Video;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Network.Download
{
    public interface IDownloadSettings
    {
        bool Video { get; }
        bool Comment { get; }
        bool Thumbnail { get; }
        bool Overwrite { get; }
        bool FromAnotherFolder { get; }
        bool Skip { get; }
        bool DownloadEasy { get; }
        bool DownloadLog { get; }
        bool DownloadOwner { get; }
        bool DownloadVideoInfo { get; }
        bool DownloadIchibaInfo { get; }
        bool IsReplaceStrictedEnable { get; }
        bool OverrideVideoFileDateToUploadedDT { get; }
        bool ResumeEnable { get; }
        bool EnableUnsafeCommentHandle { get; }
        bool SaveWithoutEncode { get; }
        bool EnableExperimentalCommentSafetySystem { get; }
        string NiconicoId { get; }
        string FolderPath { get; }
        string FileNameFormat { get; }
        string VideoInfoExt { get; }
        string IchibaInfoExt { get; }
        string ThumbnailExt { get; }
        string IchibaInfoSuffix { get; }
        string VideoInfoSuffix { get; }
        string ThumbSuffix { get; }
        string OwnerComSuffix { get; }
        string? EconomySuffix { get; }
        uint VerticalResolution { get; }
        int PlaylistID { get; }
        int MaxCommentsCount { get; }
        int CommentFetchWaitSpan { get; }
        IchibaInfoTypeSettings IchibaInfoType { get; }
        VideoInfo::ThumbSize ThumbSize { get; }
        Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(bool autodispose, int maxParallelDLCount);
        Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting();
        Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(int commentOffset);
        DDL::IDescriptionSetting ConvertToDescriptionDownloadSetting(bool dlInJson, bool dlInXml, bool dlInText);
        IDl::IIchibaInfoDownloadSettings ConvertToIchibaInfoDownloadSettings();
    }


    /// <summary>
    /// DL設定
    /// </summary>
    public record DownloadSettings : IDownloadSettings
    {
        public bool Video { get; set; }

        public bool Comment { get; set; }

        public bool Thumbnail { get; set; }

        public bool Overwrite { get; set; }

        public bool FromAnotherFolder { get; set; }

        public bool Skip { get; set; }

        public bool DownloadEasy { get; set; }

        public bool DownloadLog { get; set; }

        public bool DownloadOwner { get; set; }

        public bool DownloadVideoInfo { get; set; }

        public bool DownloadIchibaInfo { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }

        public bool OverrideVideoFileDateToUploadedDT { get; set; }

        public bool ResumeEnable { get; set; }

        public bool EnableUnsafeCommentHandle { get; set; }

        public bool EnableExperimentalCommentSafetySystem { get; set; }

        public bool SaveWithoutEncode { get; set; }

        public uint VerticalResolution { get; set; }

        public int PlaylistID { get; set; }

        public int MaxCommentsCount { get; set; }

        public int CommentFetchWaitSpan { get; set; }

        public string NiconicoId { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public string VideoInfoExt { get; set; } = string.Empty;

        public string IchibaInfoExt { get; set; } = string.Empty;

        public string ThumbnailExt { get; set; } = string.Empty;

        public string IchibaInfoSuffix { get; set; } = string.Empty;

        public string VideoInfoSuffix { get; set; } = string.Empty;

        public string ThumbSuffix { get; set; } = string.Empty;

        public string OwnerComSuffix { get; set; } = string.Empty;

        public string CommandFormat { get; set; } = string.Empty;

        public string? EconomySuffix { get; set; }


        public IchibaInfoTypeSettings IchibaInfoType { get; set; }

        public VideoInfo::ThumbSize ThumbSize { get; set; }

        public Vdl::IVideoDownloadSettings ConvertToVideoDownloadSettings(bool autodispose, int maxParallelDLCount)
        {
            return new Vdl::VideoDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FileNameFormat = this.FileNameFormat,
                FolderName = this.FolderPath,
                IsAutoDisposingEnable = autodispose,
                IsOverwriteEnable = this.Overwrite,
                VerticalResolution = this.VerticalResolution,
                MaxParallelDownloadCount = maxParallelDLCount,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
                IsOvwrridingFileDTEnable = this.OverrideVideoFileDateToUploadedDT,
                IsResumeEnable = this.ResumeEnable,
                IsNoEncodeEnable = this.SaveWithoutEncode,
                CommandFormat = this.CommandFormat,
                EconomySuffix = this.EconomySuffix,
            };
        }

        public Tdl::IThumbDownloadSettings ConvertToThumbDownloadSetting()
        {
            return new Tdl::ThumbDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FolderName = this.FolderPath,
                FileNameFormat = this.FileNameFormat,
                IsOverwriteEnable = this.Overwrite,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
                Extension = this.ThumbnailExt,
                ThumbSize = this.ThumbSize,
                Suffix = this.ThumbSuffix,
            };
        }

        public Cdl::ICommentDownloadSettings ConvertToCommentDownloadSetting(int commentOffset)
        {
            return new Cdl::CommentDownloadSettings()
            {
                NiconicoId = this.NiconicoId,
                FolderName = this.FolderPath,
                FileNameFormat = this.FileNameFormat,
                IsOverwriteEnable = this.Overwrite,
                IsDownloadingEasyCommentEnable = this.DownloadEasy,
                IsDownloadingLogEnable = this.DownloadLog,
                IsDownloadingOwnerCommentEnable = this.DownloadOwner,
                CommentOffset = commentOffset,
                IsReplaceStrictedEnable = this.IsReplaceStrictedEnable,
                MaxcommentsCount = this.MaxCommentsCount,
                IsUnsafeHandleEnable = this.EnableUnsafeCommentHandle,
                OwnerSuffix = this.OwnerComSuffix,
                IsExperimentalSafetySystemEnable = this.EnableExperimentalCommentSafetySystem,
                FetchWaitSpan = this.CommentFetchWaitSpan,
            };
        }

        public DDL::IDescriptionSetting ConvertToDescriptionDownloadSetting(bool dlInJson, bool dlInXml, bool dlInText)
        {
            return new DDL::DescriptionSetting()
            {
                IsOverwriteEnable = this.Overwrite,
                FolderName = this.FolderPath,
                IsReplaceRestrictedEnable = this.IsReplaceStrictedEnable,
                Format = this.FileNameFormat,
                IsSaveInJsonEnabled = dlInJson,
                IsSaveInXmlEnabled = dlInXml,
                IsSaveInTextEnabled = dlInText,
                Suffix = this.VideoInfoSuffix,
            };
        }

        public IDl::IIchibaInfoDownloadSettings ConvertToIchibaInfoDownloadSettings()
        {
            return new IDl::IchibaInfoDownloadSettings()
            {
                IsReplacingStrictedEnabled = this.IsReplaceStrictedEnable,
                IsXml = this.IchibaInfoType == IchibaInfoTypeSettings.Xml,
                IsJson = this.IchibaInfoType == IchibaInfoTypeSettings.Json,
                IsHtml = this.IchibaInfoType == IchibaInfoTypeSettings.Html,
            };
        }



    }


}
