using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.External.Software.FFmpeg.FFmpeg
{
    public interface IFFmpegManager
    {
        /// <summary>
        /// ファイルをエンコード
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="commandFormat"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult> EncodeAsync(string inputFilePath, string outputFilePath, string commandFormat, CancellationToken token);
    }


    public class FFmpegManager : IFFmpegManager
    {
        public FFmpegManager(IProcessManager processManager, ISettingsContainer settingsContainer, IErrorHandler errorHandler)
        {
            this._processManager = processManager;
            this._settingsContainer = settingsContainer;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IProcessManager _processManager;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult> EncodeAsync(string inputFilePath, string outputFilePath, string commandFormat, CancellationToken token)
        {
            IAttemptResult<bool> useShellResult = this._settingsContainer.GetOnlyValue(SettingNames.UseShellWhenLaunchingFFmpeg, false);
            IAttemptResult<string> ffmpegPathResult = this._settingsContainer.GetOnlyValue(SettingNames.FFmpegPath, Format.FFmpegPath);

            if (!useShellResult.IsSucceeded)
            {
                return AttemptResult.Fail(useShellResult.Message);
            }

            if (!ffmpegPathResult.IsSucceeded || ffmpegPathResult.Data is null)
            {
                return AttemptResult.Fail(ffmpegPathResult.Message);
            }

            string ffmpeg = this.GetffmpegPath(ffmpegPathResult.Data);
            string command = commandFormat
                .Replace("<source>", inputFilePath)
                .Replace("<output>", outputFilePath);

            IAttemptResult<IProcessResult> result = await this._processManager.StartProcessAsync(ffmpeg, command, useShellResult.Data, token);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }

            if (result.Data.ExitCode != 0)
            {
                this._errorHandler.HandleError(FFmpegManagerError.FailedToRunFFmpeg, result.Data.ErrorOutput);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(FFmpegManagerError.FailedToRunFFmpeg, result.Data.ErrorOutput));
            }

            return AttemptResult.Succeeded();
        }


        #endregion

        #region private

        /// <summary>
        /// ffmpegのパスを取得
        /// </summary>
        /// <param name="settingValue"></param>
        /// <returns></returns>
        private string GetffmpegPath(string settingValue)
        {
            if (settingValue != Format.FFmpegPath)
            {
                return settingValue;
            }

            return Path.Combine(AppContext.BaseDirectory, settingValue);
        }

        #endregion
    }
}
