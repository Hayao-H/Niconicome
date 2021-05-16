using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Niconico.Video.Ichiba;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Ichiba
{
    public interface IIchibaInfoDownloadSettings
    {
        bool IsHtml { get; }
        bool IsJson { get; }
        bool IsXml { get; }
        bool IsReplacingStrictedEnabled { get; }
        string FilePath { get; }

    }

    interface IIchibaInfoDownloader
    {
        Task<IAttemptResult> DownloadIchibaInfo(IWatchSession session, IIchibaInfoDownloadSettings settings, Action<string> onMessage, IDownloadContext context);
    }

    class IchibaInfoDownloadSettings : IIchibaInfoDownloadSettings
    {
        public bool IsJson { get; set; }

        public bool IsXml { get; set; }

        public bool IsHtml { get; set; }

        public bool IsReplacingStrictedEnabled { get; set; }

        public string FilePath { get; set; } = string.Empty;
    }

    class IchibaInfoDownloader : IIchibaInfoDownloader
    {
        public IchibaInfoDownloader(INiconicoIchibaHandler niconicoIchibaHandler, ILogger logger)
        {
            this.niconicoIchibaHandler = niconicoIchibaHandler;
            this.logger = logger;
        }

        #region DI
        private readonly INiconicoIchibaHandler niconicoIchibaHandler;

        private readonly ILogger logger;
        #endregion

        /// <summary>
        /// 市場情報をDLする
        /// </summary>
        /// <param name="session"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<IAttemptResult> DownloadIchibaInfo(IWatchSession session, IIchibaInfoDownloadSettings settings, Action<string> onMessage, IDownloadContext context)
        {
            if (session.Video is null)
            {
                return new AttemptResult() { Message = "セッション情報のVideoがnullです。" };
            }

            onMessage($"市場APIへのアクセスを開始します。({session.Video.Id})");
            var getResult = await this.niconicoIchibaHandler.GetIchibaInfo(session.Video.Id);
            if (!getResult.IsSucceeded || getResult.Data is null)
            {
                onMessage($"市場APIからの情報取得に失敗しました。({session.Video.Id})");
                if (getResult.Exception is not null)
                {
                    this.logger.Error($"市場情報の取得に失敗しました。(詳細:{getResult.Message} ,ctx:{context.GetLogContent()})", getResult.Exception);
                }
                else
                {
                    this.logger.Error($"市場情報の取得に失敗しました。(詳細:{getResult.Message} ,ctx:{context.GetLogContent()})");
                }
                return new AttemptResult() { Message = getResult.Message };
            }

            onMessage($"市場APIからの情報取得を取得しました。({session.Video.Id})");

            string content;

            //if (!settings.IsHtml)
            //{
            try
            {
                content = this.GetContent(getResult.Data, settings.IsXml);
            }
            catch (Exception e)
            {
                this.logger.Error($"市場情報のシリアル化に失敗しました。(詳細:{getResult.Message} ,ctx:{context.GetLogContent()})", e);
                onMessage($"市場情報のシリアル化に失敗しました。({session.Video.Id})");
                return new AttemptResult() { Message = "市場情報のシリアル化に失敗しました。" };
            }
            //}

            try
            {
                using var fs = new StreamWriter(settings.FilePath);
                fs.Write(content);
            }
            catch (Exception e)
            {
                this.logger.Error($"市場情報ファイルの書き込みに失敗しました。(ctx:{context.GetLogContent()})", e);
                onMessage($"市場情報ファイルの書き込みに失敗しました。({session.Video.Id})");
                return new AttemptResult() { Message = $"市場情報ファイルの書き込みに失敗しました。(詳細:{e.Message})" };
            }

            onMessage($"市場情報ファイルの保存が完了しました。({session.Video.Id})");
            return new AttemptResult() { IsSucceeded = true };


        }

        #region private
        /// <summary>
        /// Json・Xmlを取得する
        /// </summary>
        /// <param name="niconicoIchibaInfo"></param>
        /// <returns></returns>
        private string GetContent(INiconicoIchibaInfo niconicoIchibaInfo, bool isXml)
        {
            var info = new MarketInformation();

            foreach (var item in niconicoIchibaInfo.IchibaItems)
            {
                var marketItem = new MarketItem()
                {
                    Category = item.Category,
                    LinkUrl = item.LinkUrl,
                    Price = item.Price,
                    Name = item.Name,
                    ThumbUrl = item.ThumbUrl,
                };
                info.Items.Add(marketItem);
            }
            string serialized;

            if (isXml)
            {
                serialized = Xmlparser.Serialize(info);
            }
            else
            {
                serialized = JsonParser.Serialize(info);
            }

            return serialized;
        }
        #endregion

    }
}
