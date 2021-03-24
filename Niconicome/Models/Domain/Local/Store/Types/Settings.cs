using System;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
    }
}
