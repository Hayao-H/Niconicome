using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.Diagnostics;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.External.Error;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist.V2.Manager;

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

        /// <summary>
        /// エクスプローラーで開く
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult OpenExplorer(string path);
    }

    public class ExternalAppUtilsV2 : IExternalAppUtilsV2
    {
        public ExternalAppUtilsV2(IErrorHandler errorHandler, ICommandExecuter commandExecuter, ISettingsContainer settingsContainer, IVideoStore videoStore, IPlaylistManager playlistManager,ILocalState localState)
        {
            this._errorHandler = errorHandler;
            this._commandExecuter = commandExecuter;
            this._settingsContainer = settingsContainer;
            this._videoStore = videoStore;
            this._playlistManager = playlistManager;
            this._localState = localState;
        }


        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly ICommandExecuter _commandExecuter;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IVideoStore _videoStore;

        private readonly IPlaylistManager _playlistManager;

        private readonly ILocalState _localState;

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

            IAttemptResult<ISettingInfo<string>> path = this._settingsContainer.GetSetting(SettingNames.AppUrlPath, "");
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

        public IAttemptResult OpenExplorer(string path)
        {
            try
            {
                ProcessEx.StartWithShell("explorer.exe", path);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ExternalAppUtilsV2Error.FailedToOpenExplorer, ex, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(ExternalAppUtilsV2Error.FailedToOpenExplorer, ex, path));
            }

            return AttemptResult.Succeeded();
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

            string path;

            if (videoInfo.IsDMS)
            {
                path = $"http://localhost:{this._localState.Port}/niconicome/watch/v1/{videoInfo.PlaylistID}/{videoInfo.NiconicoId}/main.m3u8";
            }
            else
            {
                path = videoInfo.FilePath.Replace(@"\\?\", string.Empty);
            }

            this.AddVideoToHistory(videoInfo.NiconicoId, path);

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
                .Replace("<url:watch>", $"http://localhost:{this._localState.Port}/niconicome/watch/v1/{videoInfo.PlaylistID}/{videoInfo.NiconicoId}/main.m3u8")
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
            if (!result.IsSucceeded || result.Data is null)
            {
                return false;
            }

            return result.Data.Value;
        }

        /// <summary>
        /// 履歴に残す
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="filePath"></param>
        private void AddVideoToHistory(string niconicoID, string filePath)
        {
            if (this._settingsContainer.GetOnlyValue(SettingNames.DisablePlaybackHistory, false).Data)
            {
                return;
            }

            IAttemptResult<IPlaylistInfo> pResult = this._playlistManager.GetSpecialPlaylistByType(SpecialPlaylists.PlaybackHistory);
            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return;
            }

            IPlaylistInfo playlist = pResult.Data;

            if (this._videoStore.Exist(niconicoID, playlist.ID))
            {
                return;
            }

            IAttemptResult cResult = this._videoStore.Create(niconicoID, playlist.ID);
            if (!cResult.IsSucceeded)
            {
                return;
            }

            IAttemptResult<IVideoInfo> vResult = this._videoStore.GetVideo(niconicoID, playlist.ID);
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return;
            }

            vResult.Data.FilePath = filePath;

            playlist.AddVideo(vResult.Data);
        }

        #endregion
    }
}
