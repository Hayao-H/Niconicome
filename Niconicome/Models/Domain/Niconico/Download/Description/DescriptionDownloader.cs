using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Const = Niconicome.Models.Const;
using Xml = Niconicome.Models.Domain.Niconico.Net.Xml.API.Obsoleted;

namespace Niconicome.Models.Domain.Niconico.Download.Description
{
    public interface IDescriptionSetting
    {
        string FolderName { get; set; }
        string Format { get; set; }
        string Suffix { get; }
        bool IsOverwriteEnable { get; set; }
        bool IsReplaceRestrictedEnable { get; set; }
        bool IsSaveInJsonEnabled { get; set; }
        bool IsSaveInXmlEnabled { get; set; }
        bool IsSaveInTextEnabled { get; set; }
    }

    interface IDescriptionDownloader
    {
        IDownloadResult DownloadVideoInfo(IDescriptionSetting setting, IWatchSession session, Action<string> onMessage);
    }
    public interface IVideoInfoContentProducer
    {
        string GetContent(IDmcInfo info);
        string GetJsonContent(IDmcInfo info);
        string GetXmlContent(IDmcInfo info);
    }

    public class DescriptionSetting : IDescriptionSetting
    {
        public string FolderName { get; set; } = string.Empty;

        public string Format { get; set; } = string.Empty;

        public string Suffix { get; set; } = string.Empty;

        public bool IsOverwriteEnable { get; set; }

        public bool IsReplaceRestrictedEnable { get; set; }

        public bool IsSaveInJsonEnabled { get; set; }

        public bool IsSaveInXmlEnabled { get; set; }

        public bool IsSaveInTextEnabled { get; set; }
    }

    class DescriptionDownloader : IDescriptionDownloader
    {
        public DescriptionDownloader(INiconicoUtils niconicoUtils, ILogger logger, IDownloadMessenger messenger, IVideoInfoContentProducer producer)
        {
            this.utils = niconicoUtils;
            this.logger = logger;
            this.messenger = messenger;
            this.producer = producer;
        }

        private readonly INiconicoUtils utils;

        private readonly ILogger logger;

        private readonly IDownloadMessenger messenger;

        private readonly IVideoInfoContentProducer producer;

        /// <summary>
        /// 動画情報ファイルを保存する
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="dmcInfo"></param>
        /// <returns></returns>
        public IDownloadResult DownloadVideoInfo(IDescriptionSetting setting, IWatchSession session, Action<string> onMessage)
        {
            this.messenger.AddHandler(onMessage);

            if (session.Video is null)
            {
                this.messenger.RemoveHandler(onMessage);
                throw new InvalidOperationException("動画情報が未取得です。");
            }

            this.messenger.SendMessage($"動画情報の保存を開始します。");
            this.logger.Log($"{session.Video.Id}の動画情報保存を開始");

            string fileName;

            if (setting.IsSaveInJsonEnabled)
            {
                fileName = this.utils.GetFileName(setting.Format, session.Video.DmcInfo, ".json", setting.IsReplaceRestrictedEnable, setting.Suffix);
            }
            else if (setting.IsSaveInXmlEnabled)
            {
                fileName = this.utils.GetFileName(setting.Format, session.Video.DmcInfo, ".xml", setting.IsReplaceRestrictedEnable, setting.Suffix);
            }
            else
            {
                fileName = this.utils.GetFileName(setting.Format, session.Video.DmcInfo, ".txt", setting.IsReplaceRestrictedEnable, setting.Suffix);
            }

            var filePath = Path.Combine(setting.FolderName, fileName);
            var folderpath = Path.GetDirectoryName(filePath);

            if (folderpath is null)
            {
                this.messenger.SendMessage($"動画情報ファイルの保存フォルダー取得に失敗しました。");
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Message = "動画情報ファイルの保存フォルダー取得に失敗しました。" };
            }

            string content;

            if (setting.IsSaveInJsonEnabled)
            {
                content = this.producer.GetJsonContent(session.Video.DmcInfo);
            }
            else if (setting.IsSaveInXmlEnabled)
            {
                content = this.producer.GetXmlContent(session.Video.DmcInfo);
            }
            else
            {
                content = this.producer.GetContent(session.Video.DmcInfo);
            }

            try
            {
                using var fs = new StreamWriter(filePath);
                fs.Write(content);
            }
            catch (Exception e)
            {
                this.messenger.SendMessage("動画情報ファイルの書き込みに失敗しました。");
                this.logger.Error("動画情報ファイルの書き込みに失敗しました。", e);
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Message = $"動画情報ファイルの書き込みに失敗しました。(詳細:{e.Message})" };
            }

            this.messenger.SendMessage($"動画情報ファイルを保存しました。");
            this.messenger.RemoveHandler(onMessage);
            return new DownloadResult() { Issucceeded = true };
        }



    }


    public class VideoInfoContentProducer : IVideoInfoContentProducer
    {
        /// <summary>
        /// 動画情報本文を取得する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetContent(IDmcInfo info)
        {
            var ownerNameTitle = info.ChannelName.IsNullOrEmpty() ? "[owner_nickname]" : "[channel_name]";
            var ownerIDTitle = info.ChannelName.IsNullOrEmpty() ? "[owner_id]" : "[channel_id]";
            var ownerName = info.ChannelName.IsNullOrEmpty() ? info.Owner : info.ChannelName;
            var ownerID = info.ChannelName.IsNullOrEmpty() ? info.OwnerID.ToString() : info.ChannelID;

            var list = new List<string>()
            {
                "[name]",
                info.Id+Environment.NewLine,
                "[post]",
                info.UploadedOn.ToString("yyyy/MM/dd HH:mm:ss")+Environment.NewLine,
                "[title]",
                info.Title+Environment.NewLine,
                "[comment]",
                info.Description+Environment.NewLine,
                "[tags]",
                String.Join(Environment.NewLine,info.Tags)+Environment.NewLine,
                "[view_counter]",
                info.ViewCount.ToString()+Environment.NewLine,
                "[comment_num]",
                info.CommentCount.ToString()+Environment.NewLine,
                "[mylist_counter]",
                info.MylistCount.ToString()+Environment.NewLine,
                "[length]",
                $"{Math.Floor((double)(info.Duration/60))}分{info.Duration%60}秒"+Environment.NewLine,
                ownerIDTitle,
                ownerID+Environment.NewLine,
                ownerNameTitle,
                ownerName
            };

            return string.Join(Environment.NewLine, list);
        }

        /// <summary>
        /// JSON文字列を取得する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetJsonContent(IDmcInfo info)
        {
            var data = new VideoInfoJson(info);
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false,
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            return JsonParser.Serialize(data, options);
        }

        /// <summary>
        /// XML文字列を取得する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetXmlContent(IDmcInfo info)
        {
            var data = new Xml::GetThumbInfoApiResponse();
            data.Thumb.VideoId = info.Id;
            data.Thumb.Title = info.Title;
            data.Thumb.Description = info.Description;
            data.Thumb.ThumbnailUrl = info.ThumbInfo.GetSpecifiedThumbnail(ThumbSize.Large);
            data.Thumb.FirstRetrieve = new DateTimeOffset(info.UploadedOn, TimeSpan.FromHours(9)).ToString("yyyy-MM-ddTHH:mm:sszzz");
            data.Thumb.Length = $"{Math.Floor((double)info.Duration / 60).ToString().PadLeft(2, '0')}:{info.Duration % 60}";
            data.Thumb.ViewCounter = info.ViewCount;
            data.Thumb.CommentNum = info.CommentCount;
            data.Thumb.LikeCounter = info.LikeCount;
            data.Thumb.MylistCounter = info.MylistCount;
            data.Thumb.WatchUrl = Const::Net.NiconicoWatchUrl + info.Id;
            data.Thumb.Tags.Tag.AddRange(info.Tags.Select(t => new Xml::Tag() { Text = t }));
            data.Thumb.UserId = info.ChannelName.IsNullOrEmpty() ? info.OwnerID.ToString() : null;
            data.Thumb.UserNickname = info.ChannelName.IsNullOrEmpty() ? info.Owner : null;
            data.Thumb.ChName = info.ChannelName.IsNullOrEmpty() ? null : info.ChannelName;
            data.Thumb.ChId = info.ChannelName.IsNullOrEmpty() ? null : info.ChannelID;

            return Xmlparser.Serialize(data);
        }


        class VideoInfoJson
        {
            public VideoInfoJson(IDmcInfo info)
            {
                this.Title = info.Title;
                this.Post = info.UploadedOn.ToString("yyyy/MM/ddTHH:mm:ss");
                this.ID = info.Id;
                this.Description = info.Description;
                this.Tags = new List<string>();
                this.Tags.AddRange(info.Tags);
                this.ViewCount = info.ViewCount;
                this.MylistCount = info.MylistCount;
                this.CommentCount = info.CommentCount;
                this.LikeCount = info.LikeCount;
                this.Owner = info.ChannelName.IsNullOrEmpty() ? info.Owner : null;
                this.OwnerID = info.ChannelName.IsNullOrEmpty() ? info.OwnerID.ToString() : null;
                this.ChannelName = info.ChannelName.IsNullOrEmpty() ? null : info.ChannelName;
                this.ChannelID = info.ChannelName.IsNullOrEmpty() ? null : info.ChannelID;
            }

            public string Title { get; init; }

            public string Post { get; init; }

            public string ID { get; init; }

            public string Description { get; init; }

            public List<string> Tags { get; init; }

            public int ViewCount { get; init; }

            public int CommentCount { get; init; }

            public int MylistCount { get; init; }

            public int LikeCount { get; init; }

            public string? Owner { get; init; }

            public string? OwnerID { get; init; }

            public string? ChannelName { get; init; }

            public string? ChannelID { get; init; }
        }
    }
}
