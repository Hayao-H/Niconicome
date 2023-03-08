using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe
{
    public interface IFFprobeHandler
    {
        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IAttemptResult<IFFprobeResult>> GetVideoInfomationAsync(string path);
    }

    public class FFprobeHandler : IFFprobeHandler
    {
        public FFprobeHandler(INiconicomeFileIO fileIO, IProcessManager processManager, ISettingsContainer settingsContainer, IErrorHandler errorHandler)
        {
            this._fileIO = fileIO;
            this._processManager = processManager;
            this._settingsContainer = settingsContainer;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IProcessManager _processManager;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<IFFprobeResult>> GetVideoInfomationAsync(string path)
        {
            IAttemptResult<ISettingInfo<bool>> useShellResult = this._settingsContainer.GetSetting(SettingNames.UseShellWhenLaunchingFFmpeg, false);
            IAttemptResult<ISettingInfo<string>> ffprobePathResult = this._settingsContainer.GetSetting(SettingNames.FFprobePath, Format.FFprobePath);

            if (!this.CheckIfSetingSucceeded(useShellResult))
            {
                return AttemptResult<IFFprobeResult>.Fail(useShellResult.Message);
            }

            if (!this.CheckIfSetingSucceeded(ffprobePathResult))
            {
                return AttemptResult<IFFprobeResult>.Fail(ffprobePathResult.Message);
            }

            string ffprobePath = this.GetffprobePath(ffprobePathResult.Data!.Value);
            string command = $"ffprobe -i \"{path}\" -v error -show_streams -of json";

            IAttemptResult<IProcessResult> result = await this._processManager.StartProcessAsync(ffprobePath, command, useShellResult.Data!.Value);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IFFprobeResult>.Fail(result.Message);
            }

            if (result.Data.ExitCode != 0)
            {
                string error = this.GetErrorMessage(result.Data.ErrorOutput);
                this._errorHandler.HandleError(FFprobeHandlerError.FailedToRunFFprobe, error);
                return AttemptResult<IFFprobeResult>.Fail(this._errorHandler.GetMessageForResult(FFprobeHandlerError.FailedToRunFFprobe, error));
            }

            IAttemptResult<Response> parseResult = this.ParseOutput(result.Data.StandardOutput);
            if (!parseResult.IsSucceeded || parseResult.Data is null)
            {
                return AttemptResult<IFFprobeResult>.Fail(parseResult.Message);
            }

            IAttemptResult<int> heightResult = this.GetHeigt(parseResult.Data);
            if (!heightResult.IsSucceeded)
            {
                return AttemptResult<IFFprobeResult>.Fail(heightResult.Message);
            }

            return AttemptResult<IFFprobeResult>.Succeeded(new FFprobeResult(heightResult.Data));

        }

        #endregion

        #region private

        /// <summary>
        /// 設定取得が成功したかどうかを確認
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool CheckIfSetingSucceeded<T>(IAttemptResult<ISettingInfo<T>> result) where T : notnull
        {
            if (!result.IsSucceeded)
            {
                return false;
            }

            if (result.Data is null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ffprobeのパスを取得
        /// </summary>
        /// <param name="settingValue"></param>
        /// <returns></returns>
        private string GetffprobePath(string settingValue)
        {
            if (settingValue != Format.FFprobePath)
            {
                return settingValue;
            }

            return Path.Combine(AppContext.BaseDirectory, settingValue);
        }

        /// <summary>
        /// 出力を解析
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private IAttemptResult<Response> ParseOutput(StreamReader stream)
        {
            string content;

            try
            {
                content = stream.ReadToEnd();
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(FFprobeHandlerError.FailedToReadResponse, ex);
                return AttemptResult<Response>.Fail(this._errorHandler.GetMessageForResult(FFprobeHandlerError.FailedToReadResponse, ex));
            }

            try
            {
                var data = JsonParser.DeSerialize<Response>(content);
                return AttemptResult<Response>.Succeeded(data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(FFprobeHandlerError.FailedToDeserializeData, ex);
                return AttemptResult<Response>.Fail(this._errorHandler.GetMessageForResult(FFprobeHandlerError.FailedToDeserializeData, ex));
            }
        }

        /// <summary>
        /// エラーメッセージを取得
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string GetErrorMessage(StreamReader stream)
        {
            string content;

            try
            {
                content = stream.ReadToEnd();
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(FFprobeHandlerError.FailedToReadError, ex);
                return string.Empty;
            }

            return content;
        }

        //動画の高さを取得
        private IAttemptResult<int> GetHeigt(Response data)
        {
            Stream? video = data.Streams.FirstOrDefault(s => s.CodecType == "video");
            if (video is null)
            {
                this._errorHandler.HandleError(FFprobeHandlerError.NoCodecTypeVideo);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(FFprobeHandlerError.NoCodecTypeVideo));
            }

            return AttemptResult<int>.Succeeded(video.Height);
        }

        #endregion
    }
}
