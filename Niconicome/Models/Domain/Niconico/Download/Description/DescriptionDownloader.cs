using System;
using System.IO;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Network.Download;

namespace Niconicome.Models.Domain.Niconico.Download.Description
{

    interface IDescriptionDownloader
    {
        IAttemptResult DownloadVideoInfoAsync(IDownloadSettings setting, IDomainVideoInfo videoInfo, Action<string> onMessage);
    }

    class DescriptionDownloader : IDescriptionDownloader
    {
        public DescriptionDownloader(ILogger logger, INicoFileIO fileIO, IVideoInfoContentProducer producer, IPathOrganizer pathOrganizer)
        {
            this._logger = logger;
            this._producer = producer;
            this._fileIO = fileIO;
            this._pathOrganizer = pathOrganizer;
        }
        #region field

        private readonly ILogger _logger;

        private readonly IVideoInfoContentProducer _producer;

        private readonly INicoFileIO _fileIO;

        private readonly IPathOrganizer _pathOrganizer;

        #endregion

        /// <summary>
        /// 動画情報ファイルを保存する
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="dmcInfo"></param>
        /// <returns></returns>
        public IAttemptResult DownloadVideoInfoAsync(IDownloadSettings setting, IDomainVideoInfo videoInfo, Action<string> onMessage)
        {
            onMessage($"動画情報の保存を開始します。");
            this._logger.Log($"{videoInfo.Id}の動画情報保存を開始");

            string filePath = this._pathOrganizer.GetFilePath(setting.FileNameFormat, videoInfo.DmcInfo, setting.VideoInfoExt, setting.FolderPath, setting.IsReplaceStrictedEnable, setting.Overwrite, setting.VideoInfoSuffix);


            string content = setting.VideoInfoType switch
            {
                VideoInfoTypeSettings.Json => this._producer.GetJsonContent(videoInfo.DmcInfo),
                VideoInfoTypeSettings.Xml => this._producer.GetXmlContent(videoInfo.DmcInfo),
                _ => this._producer.GetContent(videoInfo.DmcInfo),
            };

            try
            {
                IOUtils.CreateDirectoryIfNotExist(filePath);
                this._fileIO.Write(filePath, content);
            }
            catch (Exception e)
            {
                onMessage("動画情報ファイルの書き込みに失敗しました。");
                this._logger.Error("動画情報ファイルの書き込みに失敗しました。", e);
                return AttemptResult.Fail($"動画情報ファイルの書き込みに失敗しました。(詳細:{e.Message})");
            }

            onMessage($"動画情報ファイルを保存しました。");
            return AttemptResult.Succeeded();
        }



    }

}
