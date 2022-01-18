using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Niconico.Video.Ichiba;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Network.Download;

namespace Niconicome.Models.Domain.Niconico.Download.Ichiba
{

    interface IIchibaInfoDownloader
    {
        /// <summary>
        /// 市場情報をDLする
        /// </summary>
        /// <param name="session"></param>
        /// <param name="settings"></param>
        /// <param name="onMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<IAttemptResult> DownloadIchibaInfo(IWatchSession session, IDownloadSettings settings, Action<string> onMessage, IDownloadContext context);
    }

    class IchibaInfoDownloader : IIchibaInfoDownloader
    {
        public IchibaInfoDownloader(INiconicoIchibaHandler niconicoIchibaHandler, ILogger logger,INicoFileIO fileIO,IPathOrganizer path)
        {
            this._niconicoIchibaHandler = niconicoIchibaHandler;
            this._logger = logger;
            this._fileIO = fileIO;
            this._path = path;
        }

        #region field

        private readonly INiconicoIchibaHandler _niconicoIchibaHandler;

        private readonly ILogger _logger;

        private readonly INicoFileIO _fileIO;

        private readonly IPathOrganizer _path;

        #endregion

        #region Method

        public async Task<IAttemptResult> DownloadIchibaInfo(IWatchSession session, IDownloadSettings settings, Action<string> onMessage, IDownloadContext context)
        {
            if (session.Video is null)
            {
                return new AttemptResult() { Message = "セッション情報のVideoがnullです。" };
            }

            onMessage($"市場APIへのアクセスを開始します。({session.Video.Id})");
            var getResult = await this._niconicoIchibaHandler.GetIchibaInfo(session.Video.Id);
            if (!getResult.IsSucceeded || getResult.Data is null)
            {
                onMessage($"市場APIからの情報取得に失敗しました。({session.Video.Id})");
                if (getResult.Exception is not null)
                {
                    this._logger.Error($"市場情報の取得に失敗しました。(詳細:{getResult.Message} ,{context.GetLogContent()})", getResult.Exception);
                }
                else
                {
                    this._logger.Error($"市場情報の取得に失敗しました。(詳細:{getResult.Message} ,{context.GetLogContent()})");
                }
                return new AttemptResult() { Message = getResult.Message };
            }

            onMessage($"市場APIからの情報取得を取得しました。({session.Video.Id})");

            string content;

            if (settings.IchibaInfoType != IchibaInfoTypeSettings.Html)
            {
                try
                {
                    content = this.GetContent(getResult.Data, settings.IchibaInfoType == IchibaInfoTypeSettings.Xml);
                }
                catch (Exception e)
                {
                    this._logger.Error($"市場情報のシリアル化に失敗しました。(詳細:{getResult.Message} ,{context.GetLogContent()})", e);
                    onMessage($"市場情報のシリアル化に失敗しました。({session.Video.Id})");
                    return new AttemptResult() { Message = "市場情報のシリアル化に失敗しました。" };
                }
            }
            else
            {

                try
                {
                    content = this.GetHtmlContent(getResult.Data, session.Video.Id);
                }
                catch (Exception e)
                {
                    this._logger.Error($"市場情報のシリアル化に失敗しました。(詳細:{getResult.Message} ,{context.GetLogContent()})", e);
                    onMessage($"市場情報のシリアル化に失敗しました。({session.Video.Id})");
                    return new AttemptResult() { Message = "市場情報のシリアル化に失敗しました。" };
                }
            }


            string filePath = this._path.GetFilePath(settings.FileNameFormat, session.Video!.DmcInfo, settings.IchibaInfoExt, settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite, settings.IchibaInfoSuffix);

            try
            {
                this._fileIO.Write(filePath, content);
            }
            catch (Exception e)
            {
                this._logger.Error($"市場情報ファイルの書き込みに失敗しました。({context.GetLogContent()})", e);
                onMessage($"市場情報ファイルの書き込みに失敗しました。({session.Video.Id})");
                return new AttemptResult() { Message = $"市場情報ファイルの書き込みに失敗しました。(詳細:{e.Message})" };
            }

            onMessage($"市場情報ファイルの保存が完了しました。({session.Video.Id})");
            return new AttemptResult() { IsSucceeded = true };


        }

        #endregion

        #region private

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
        private string GetHtmlContent(INiconicoIchibaInfo niconicoIchibaInfo, string niconicoId)
        {
            var page = IchibaTemplate.GetReplacedString(niconicoId);
            var items = new List<string>();
            foreach (var item in niconicoIchibaInfo.IchibaItems)
            {
                var htmlItem = IchibaItemTemplate.GetReplacedString(item);
                items.Add(htmlItem);
            }

            var combined = string.Join("", items);
            return page.Replace("{items}", combined);
        }
        #endregion

    }
}
