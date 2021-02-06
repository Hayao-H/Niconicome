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
    public record UrlSetting:IStorable,ISetting
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
    /// アプリケーションの設定(真偽値)
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

    }
}
