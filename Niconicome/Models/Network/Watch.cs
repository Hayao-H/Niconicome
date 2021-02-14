﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchInfo = Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Playlist;
using Niconicome.Models.Local;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Network
{
    public interface IVideoInfo
    {
        string Title { get; set; }
        string Id { get; set; }
        string FileName { get; set; }
        int ViewCount { get; set; }
        IEnumerable<string> Tags { get; set; }
        DateTime UploadedOn { get; set; }
        Uri LargeThumbUri { get; set; }
        Uri ThumbUri { get; set; }
        ITreeVideoInfo ConvertToTreeVideoInfo();
    }

    public interface IWatch
    {
        Task<IResult> TryGetVideoInfoAsync(string nicoId, IVideoInfo outInfo, WatchInfo::WatchInfoOptions options = WatchInfo::WatchInfoOptions.Default);
    }

    public interface IResult
    {
        bool IsSucceeded { get; }
        string Message { get; }
    }

    public class Watch : IWatch
    {
        public Watch(WatchInfo::IWatchInfohandler handler, ILocalSettingHandler settingHandler, INiconicoUtils utils)
        {
            this.handler = handler;
            this.settingHandler = settingHandler;
            this.utils = utils;
        }

        private readonly WatchInfo::IWatchInfohandler handler;

        private readonly ILocalSettingHandler settingHandler;

        private readonly INiconicoUtils utils;

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="nicoId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IResult> TryGetVideoInfoAsync(string nicoId, IVideoInfo info, WatchInfo::WatchInfoOptions options = WatchInfo::WatchInfoOptions.Default)
        {
            WatchInfo::IDomainVideoInfo retrieved;
            var result = new Result();
            string filenameFormat = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";

            try
            {
                retrieved = await this.handler.GetVideoInfoAsync(nicoId, options);
            }
            catch
            {
                result.IsSucceeded = false;
                result.Message = this.handler.State switch
                {
                    WatchInfo::WatchInfoHandlerState.HttpRequestFailure => "httpリクエストに失敗しました。(サーバーエラー・IDの指定間違い)",
                    WatchInfo::WatchInfoHandlerState.JsonParsingFailure => "視聴ページの解析に失敗しました。(サーバーエラー)",
                    WatchInfo::WatchInfoHandlerState.NoJsDataElement => "視聴ページの解析に失敗しました。(サーバーエラー・権利のない有料動画など)",
                    WatchInfo::WatchInfoHandlerState.OK => "取得完了",
                    _ => "不明なエラー"
                };

                return result;
            }

            //タイトル
            info.Title = retrieved.Title;

            //ID
            info.Id = retrieved.Id;

            //タグ
            info.Tags = retrieved.Tags;

            //ファイル名
            info.FileName = this.utils.GetFileName(filenameFormat, retrieved.DmcInfo, ".mp4");

            //再生回数
            info.ViewCount = retrieved.ViewCount;

            //投稿日時
            if (retrieved.DmcInfo is not null)
            {
                info.UploadedOn = retrieved.DmcInfo.UploadedOn;
            }

            //サムネイル
            if (retrieved.DmcInfo is not null && retrieved.DmcInfo.ThumbInfo is not null)
            {
                if (retrieved.DmcInfo.ThumbInfo.Large is not null)
                {
                    info.LargeThumbUri = new Uri(retrieved.DmcInfo.ThumbInfo.Large);
                }

                if (retrieved.DmcInfo.ThumbInfo.Normal is not null)
                {
                    info.ThumbUri = new Uri(retrieved.DmcInfo.ThumbInfo.Normal);
                }
            }

            result.IsSucceeded = true;
            result.Message = "取得成功";


            return result;

        }
    }

    /// <summary>
    /// 動画情報
    /// </summary>
    public class VIdeoInfo : IVideoInfo
    {
        public static string DeletedVideoThumb { get; private set; } = "https://nicovideo.cdn.nimg.jp/web/img/common/video_deleted.jpg";

        public string Title { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;


        public int ViewCount { get; set; }

        public IEnumerable<string> Tags { get; set; } = new List<string>();

        public DateTime UploadedOn { get; set; }

        public Uri LargeThumbUri { get; set; } = new Uri(VIdeoInfo.DeletedVideoThumb);

        public Uri ThumbUri { get; set; } = new Uri(VIdeoInfo.DeletedVideoThumb);

        /// <summary>
        /// Viewから参照可能な形式に変換する
        /// </summary>
        /// <returns></returns>
        public ITreeVideoInfo ConvertToTreeVideoInfo()
        {
            return new BindableTreeVideoInfo()
            {
                Title = this.Title,
                NiconicoId = this.Id,
                UploadedOn = this.UploadedOn,
                LargeThumbUrl = this.LargeThumbUri.AbsoluteUri,
                ThumbUrl = this.ThumbUri.AbsoluteUri,
                Tags = this.Tags,
                ViewCount = this.ViewCount,
                FileName = this.FileName,
            };
        }

    }

    public class Result : IResult
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }


}