using System;
using System.Collections.Generic;
using System.IO;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Description
{
    public interface IDescriptionSetting
    {
        string FolderName { get; set; }
        string Format { get; set; }
        bool IsOverwriteEnable { get; set; }
        bool IsReplaceRestrictedEnable { get; set; }
    }

    interface IDescriptionDownloader
    {
        IDownloadResult DownloadVideoInfo(IDescriptionSetting setting, IWatchSession session, Action<string> onMessage);
    }

    public class DescriptionSetting : IDescriptionSetting
    {
        public string FolderName { get; set; } = string.Empty;

        public string Format { get; set; } = string.Empty;

        public bool IsOverwriteEnable { get; set; }

        public bool IsReplaceRestrictedEnable { get; set; }
    }

    class DescriptionDownloader : IDescriptionDownloader
    {
        public DescriptionDownloader(INiconicoUtils niconicoUtils, ILogger logger, IDownloadMessenger messenger)
        {
            this.utils = niconicoUtils;
            this.logger = logger;
            this.messenger = messenger;
        }

        private readonly INiconicoUtils utils;

        private readonly ILogger logger;

        private readonly IDownloadMessenger messenger;

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

            var fileName = this.utils.GetFileName(setting.Format, session.Video.DmcInfo, ".txt", setting.IsReplaceRestrictedEnable);
            var filePath = Path.Combine(setting.FolderName, fileName);
            var folderpath = Path.GetDirectoryName(filePath);

            if (folderpath is null)
            {
                this.messenger.SendMessage($"動画情報ファイルの保存フォルダー取得に失敗しました。");
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult() { Message = "動画情報ファイルの保存フォルダー取得に失敗しました。" };
            }

            var content = this.GetContent(session.Video.DmcInfo);

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


        private string GetContent(IDmcInfo info)
        {
            var list = new List<string>()
            {
                "[name]",
                info.Id,
                Environment.NewLine,
                "[post]",
                info.UploadedOn.ToString("yyyy/MM/dd HH:mm:ss"),
                Environment.NewLine,
                "[title]",
                info.Title,
                Environment.NewLine,
                "[comment]",
                info.Description,
                Environment.NewLine,
                "[tags]",
                String.Join(Environment.NewLine,info.Tags),
                Environment.NewLine,
                "[view_counter]",
                info.ViewCount.ToString(),
                Environment.NewLine,
                "[comment_num]",
                info.CommentCount.ToString(),
                Environment.NewLine,
                "[mylist_counter]",
                info.MylistCount.ToString(),
                Environment.NewLine,
                "[length]",
                $"{Math.Floor((double)(info.Duration/60))}分{info.Duration%60}秒",
                Environment.NewLine,
                "[owner_id]",
                info.OwnerID.ToString(),
                Environment.NewLine,
                "[owner_nickname]",
                info.Owner
            };

            return string.Join(Environment.NewLine, list);
        }
    }
}
