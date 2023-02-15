using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.External.Error;

namespace Niconicome.Models.Local.External
{
    public interface IExternalAppUtilsV2
    {
        /// <summary>
        /// プレイヤーで開く(A)
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        IAttemptResult OpenInPlayerA(IVideoInfo videoInfo);

        /// <summary>
        /// プレイヤーで開く(B)
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        IAttemptResult OpenInPlayerB(IVideoInfo videoInfo);

        /// <summary>
        /// アプリに送る(A)
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        IAttemptResult SendToAppA(IVideoInfo videoInfo);

        /// <summary>
        /// アプリに送る(B)
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        IAttemptResult SendToAppB(IVideoInfo videoInfo);
    }

    public class ExternalAppUtilsV2 : IExternalAppUtilsV2
    {
        public ExternalAppUtilsV2(IErrorHandler errorHandler,ICommandExecuter commandExecuter,ISettingsContainer settingsContainer)
        {
            this._errorHandler = errorHandler;
            this._commandExecuter = commandExecuter;
            this._settingsContainer = settingsContainer;
        }


        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly ICommandExecuter _commandExecuter;

        private readonly ISettingsContainer _settingsContainer;

        #endregion

        #region Method

        public IAttemptResult OpenInPlayerA(IVideoInfo videoInfo)
        {
            if (CheckWhetherSwitchToSendCommand(videoInfo))
            {
                return this.SendToAppA(videoInfo);
            }

            IAttemptResult<ISettingInfo<string>> setting = this._settingsContainer.GetSetting(SettingNames.PlayerAPath, "");
            if (!setting.IsSucceeded || setting.Data is null)
            {
                return AttemptResult.Fail(setting.Message);
            }

            return this.OpenPlayer(setting.Data.Value, videoInfo);
        }

        public IAttemptResult OpenInPlayerB(IVideoInfo videoInfo)
        {
            if (CheckWhetherSwitchToSendCommand(videoInfo))
            {
                return this.SendToAppB(videoInfo);
            }

            IAttemptResult<ISettingInfo<string>> setting = this._settingsContainer.GetSetting(SettingNames.PlayerBPath, "");
            if (!setting.IsSucceeded || setting.Data is null)
            {
                return AttemptResult.Fail(setting.Message);
            }

            return this.OpenPlayer(setting.Data.Value, videoInfo);
        }

        public IAttemptResult SendToAppA(IVideoInfo videoInfo)
        {
            IAttemptResult<ISettingInfo<string>> path = this._settingsContainer.GetSetting(SettingNames.AppIdPath, "");
            if (!path.IsSucceeded || path.Data is null)
            {
                return AttemptResult.Fail(path.Message);
            }

            IAttemptResult<ISettingInfo<string>> param = this._settingsContainer.GetSetting(SettingNames.AppIdParam, "");
            if (!param.IsSucceeded || param.Data is null)
            {
                return AttemptResult.Fail(param.Message);
            }

            return this.SendToAppCommand(path.Data.Value, param.Data.Value, videoInfo);

        }

        public IAttemptResult SendToAppB(IVideoInfo videoInfo)
        {

            IAttemptResult<ISettingInfo<string>> path = this._settingsContainer.GetSetting(SettingNames.AppUrlParam, "");
            if (!path.IsSucceeded || path.Data is null)
            {
                return AttemptResult.Fail(path.Message);
            }

            IAttemptResult<ISettingInfo<string>> param = this._settingsContainer.GetSetting(SettingNames.AppUrlParam, "");
            if (!param.IsSucceeded || param.Data is null)
            {
                return AttemptResult.Fail(param.Message);
            }

            return this.SendToAppCommand(path.Data.Value, param.Data.Value, videoInfo);
        }

        #endregion

        #region private

        /// <summary>
        /// プレイヤーで開く
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private IAttemptResult OpenPlayer(string appPath, IVideoInfo videoInfo)
        {
            if (!videoInfo.IsDownloaded.Value)
            {
                this._errorHandler.HandleError(ExternalAppUtilsV2Error.VideoIsNotDownloaded, videoInfo.NiconicoId);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(ExternalAppUtilsV2Error.VideoIsNotDownloaded, videoInfo.NiconicoId));
            }

            var path = videoInfo.FilePath.Replace(@"\\?\", string.Empty)
                ;
            return this._commandExecuter.Execute(appPath, $"\"{path}\"");
        }

        /// <summary>
        /// アプリに送る
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="argBase"></param>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private IAttemptResult SendToAppCommand(string appPath, string argBase, IVideoInfo videoInfo)
        {
            var constructedArg = argBase
                .Replace("<url>", NetConstant.NiconicoWatchUrl + videoInfo.NiconicoId)
                .Replace("<url:short>", NetConstant.NiconicoShortUrl + videoInfo.NiconicoId)
                .Replace("<id>", videoInfo.NiconicoId)
                ;

            return this._commandExecuter.Execute(appPath, constructedArg);
        }

        /// <summary>
        /// 開く⇒送るの切り替えを判断する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private bool CheckWhetherSwitchToSendCommand(IVideoInfo videoInfo)
        {
            if (videoInfo.IsDownloaded.Value) return false;

            IAttemptResult<ISettingInfo<bool>> result = this._settingsContainer.GetSetting(SettingNames.ReAllocateIfVideoisNotSaved, false);
            if (!result.IsSucceeded||result.Data is null)
            {
                return false;
            }

            return result.Data.Value;
        }

        #endregion
    }
}
