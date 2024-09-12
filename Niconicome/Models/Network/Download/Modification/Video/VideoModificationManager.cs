using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;

namespace Niconicome.Models.Network.Download.Modification.Video
{
    public interface IVideoModificationManager
    {
        /// <summary>
        /// ダウンロードの後処理を行う
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        /// <param name="videoFilePath"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task ModifyVideo(string niconicoID, string playlistID, string videoFilePath, Action<string> onMessage, CancellationToken ct);
    }

    public class VideoModificationManager : IVideoModificationManager
    {
        public VideoModificationManager(ISettingsContainer settingsContainer, ILocalState localState, IProcessManager processManager)
        {
            this._settingsContainer = settingsContainer;
            this._localState = localState;
            this._processManager = processManager;
        }

        private readonly ISettingsContainer _settingsContainer;

        private readonly ILocalState _localState;

        private readonly IProcessManager _processManager;

        public async Task ModifyVideo(string niconicoID, string playlistID, string videoFilePath, Action<string> onMessage, CancellationToken ct)
        {
            IAttemptResult<string> softwarePathResult = this._settingsContainer.GetOnlyValue(SettingNames.VideoModificationSoftwarePath, string.Empty);
            if (!softwarePathResult.IsSucceeded || string.IsNullOrEmpty(softwarePathResult.Data))
            {
                return;
            }

            IAttemptResult<string> argsResult = this._settingsContainer.GetOnlyValue(SettingNames.VideoModificationSoftwareParam, string.Empty);
            if (!argsResult.IsSucceeded || string.IsNullOrEmpty(argsResult.Data))
            {
                return;
            }

            string url = string.Format(NetConstant.WatchAddressV1, this._localState.Port, playlistID, niconicoID);

            string arg = argsResult.Data
                .Replace("<ServerURL>", url)
                .Replace("<FilePath>", videoFilePath);

            await this._processManager.StartProcessAsync(softwarePathResult.Data, arg, false, onMessage, ct);
        }
    }
}
