using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Settings.Enum;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.External
{
    public interface IExternalDownloaderHandler
    {
        /// <summary>
        /// 外部ダウンローダーでDLすべきかどうかを判断する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        bool CheckCondition(IDomainVideoInfo videoInfo, IDownloadSettings settings);

        /// <summary>
        /// 外部ダウンローダーでDL
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="niconicoID"></param>
        /// <param name="outputPath"></param>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult> DownloadVideoByExtarnalDownloaderAsync(IDownloadSettings settings, string niconicoID, string outputPath, Action<string> onMessage, CancellationToken token);
    }

    public class ExternalDownloaderHandler : IExternalDownloaderHandler
    {
        public ExternalDownloaderHandler(ISettingsContainer settingsContainer, IProcessManager processManager)
        {
            this._settingsContainer = settingsContainer;
            this._processManager = processManager;
        }

        #region field

        private readonly ISettingsContainer _settingsContainer;

        private readonly IProcessManager _processManager;

        #endregion

        #region Method

        public bool CheckCondition(IDomainVideoInfo videoInfo, IDownloadSettings settings)
        {


            ExternalDownloaderConditionSetting setting = settings.ExternalDownloaderConditionSetting;

            if (setting == ExternalDownloaderConditionSetting.Always)
            {
                return true;
            }
            else if (setting == ExternalDownloaderConditionSetting.Encrypted)
            {
                return videoInfo.DmcInfo.IsEncrypted;
            }
            else if (setting == ExternalDownloaderConditionSetting.Official)
            {
                return videoInfo.DmcInfo.IsOfficial;
            }
            else
            {
                return false;
            }
        }

        public async Task<IAttemptResult> DownloadVideoByExtarnalDownloaderAsync(IDownloadSettings settings, string niconicoID, string outputPath, Action<string> onMessage, CancellationToken token)
        {

            string path = settings.ExternalDownloaderPath;
            string param = settings.ExternalDownloaderParam
                .Replace("<NiconicoID>", niconicoID)
                .Replace("<URL>", NetConstant.NiconicoWatchUrl + niconicoID)
                .Replace("<OutputPath>", outputPath);

            IAttemptResult<IProcessResult> result = await this._processManager.StartProcessAsync(path, param, true, onMessage, token);

            if (result.IsSucceeded)
            {
                return AttemptResult.Succeeded(result.Message);
            }
            else
            {
                return AttemptResult.Fail(result.Message);
            }

        }

        #endregion
    }
}
