using System;
using Niconicome.Extensions.System;
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
        /// <summary>
        /// 動画DLフラグ
        /// </summary>
        bool Video { get; }

        /// <summary>
        /// コメントDLフラグ
        /// </summary>
        bool Comment { get; }

        /// <summary>
        /// サムネDLフラグ
        /// </summary>
        bool Thumbnail { get; }

        /// <summary>
        /// 上書きフラグ
        /// </summary>
        bool Overwrite { get; }

        /// <summary>
        /// 他フォルダーからコピー
        /// </summary>
        bool FromAnotherFolder { get; }

        /// <summary>
        /// 非上書きフラグ
        /// </summary>
        bool Skip { get; }

        /// <summary>
        /// かんたんコメントDLフラグ
        /// </summary>
        bool DownloadEasy { get; }

        /// <summary>
        /// 過去ログDLフラグ
        /// </summary>
        bool DownloadLog { get; }

        /// <summary>
        /// 投コメDLフラグ
        /// </summary>
        bool DownloadOwner { get; }

        /// <summary>
        /// 動画情報DLフラグ
        /// </summary>
        bool DownloadVideoInfo { get; }

        /// <summary>
        /// 市場情報DLフラグ
        /// </summary>
        bool DownloadIchibaInfo { get; }

        /// <summary>
        /// 禁則文字置き換えフラグ
        /// </summary>
        bool IsReplaceStrictedEnable { get; }

        /// <summary>
        /// ファイル作成日時上書きフラグ
        /// </summary>
        bool OverrideVideoFileDateToUploadedDT { get; }

        /// <summary>
        /// レジュームフラグ
        /// </summary>
        bool ResumeEnable { get; }

        /// <summary>
        /// 高速コメントDLフラグ
        /// </summary>
        bool EnableUnsafeCommentHandle { get; }

        /// <summary>
        /// TS保存フラグ
        /// </summary>
        bool SaveWithoutEncode { get; }

        /// <summary>
        /// 実験中のコメDLシステム利用フラグ
        /// </summary>
        bool EnableExperimentalCommentSafetySystem { get; }

        /// <summary>
        /// エコノミーファイル削除フラグ
        /// </summary>
        bool DeleteExistingEconomyFile { get; }

        /// <summary>
        /// エコノミーフラグ
        /// </summary>
        bool IsEconomy { get; }

        /// <summary>
        /// 成功履歴を保存する
        /// </summary>
        bool SaveSucceededHistory { get; }

        /// <summary>
        /// 失敗履歴を保存する
        /// </summary>
        bool SaveFailedHistory { get; }


        /// <summary>
        /// 動画ID
        /// </summary>
        string NiconicoId { get; }

        /// <summary>
        /// 保存フォルダーパス
        /// </summary>
        string FolderPath { get; }

        /// <summary>
        /// フォーマット
        /// </summary>
        string FileNameFormat { get; }

        /// <summary>
        /// 動画ファイルの拡張子
        /// </summary>
        string VideoInfoExt { get; }

        /// <summary>
        /// 市場情報の拡張子
        /// </summary>
        string IchibaInfoExt { get; }

        /// <summary>
        /// サムネイルの拡張子
        /// </summary>
        string ThumbnailExt { get; }

        /// <summary>
        /// 市場情報ファイルの接尾辞
        /// </summary>
        string IchibaInfoSuffix { get; }

        /// <summary>
        /// 動画情報ファイルの接尾辞
        /// </summary>
        string VideoInfoSuffix { get; }

        /// <summary>
        /// サムネの接尾辞
        /// </summary>
        string ThumbSuffix { get; }

        /// <summary>
        /// 投コメの接尾辞
        /// </summary>
        string OwnerComSuffix { get; }

        /// <summary>
        /// エコノミーファイルの接尾辞
        /// </summary>
        string EconomySuffix { get; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// コマンドのフォーマット
        /// </summary>
        string CommandFormat { get; }

        /// <summary>
        /// 解像度
        /// </summary>
        uint VerticalResolution { get; }

        /// <summary>
        /// プレイリストのID
        /// </summary>
        int PlaylistID { get; }

        /// <summary>
        /// 最大コメ数
        /// </summary>
        int MaxCommentsCount { get; }

        /// <summary>
        /// コメント取得時の待機時間
        /// </summary>
        int CommentFetchWaitSpan { get; }

        /// <summary>
        /// コメントのオフセット
        /// </summary>
        int CommentOffset { get; }

        /// <summary>
        /// 最大同時セグメントDL数
        /// </summary>
        int MaxParallelSegmentDLCount { get; }

        /// <summary>
        /// 市場情報の形式
        /// </summary>
        IchibaInfoTypeSettings IchibaInfoType { get; }

        /// <summary>
        /// 動画情報の形式
        /// </summary>
        VideoInfoTypeSettings VideoInfoType { get; }

        /// <summary>
        /// サムネのサイズ
        /// </summary>
        VideoInfo::ThumbSize ThumbSize { get; }
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

        public bool DeleteExistingEconomyFile { get; set; }

        public bool IsEconomy { get; set; }

        public bool SaveSucceededHistory { get; set; }

        public bool SaveFailedHistory { get; set; }

        public uint VerticalResolution { get; set; }

        public int PlaylistID { get; set; }

        public int MaxCommentsCount { get; set; }

        public int CommentFetchWaitSpan { get; set; }

        public int CommentOffset { get; set; }

        public int MaxParallelSegmentDLCount { get; set; }

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

        public string FilePath { get; set; } = string.Empty;

        public string EconomySuffix { get; set; } = String.Empty;

        public IchibaInfoTypeSettings IchibaInfoType { get; set; }

        public VideoInfoTypeSettings VideoInfoType { get; set; }

        public VideoInfo::ThumbSize ThumbSize { get; set; }

    }


}
