using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Niconicome.Models.Domain.Local.IO.Media.Audio;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Error = Niconicome.Models.Infrastructure.IO.Media.Audio.NaudioHandlerError;

namespace Niconicome.Models.Infrastructure.IO.Media.Audio
{
    public class NaudioHandler : IAudioPlayer
    {
        public NaudioHandler(INiconicomeFileIO fileIO, IErrorHandler errorHandler)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        private readonly Dictionary<string, OutputStream> _streams = new();

        #endregion

        #region Method

        public IAttemptResult Play(string filePath)
        {
            try
            {
                return this.PlayInternal(filePath);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Error.FailedToPlay, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Error.FailedToPlay, ex));
            }
        }

        #endregion

        #region private

        /// <summary>
        /// 内部処理
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private IAttemptResult PlayInternal(string filePath)
        {
            if (!this._fileIO.Exists(filePath))
            {
                this._errorHandler.HandleError(Error.FileNotExist, filePath);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Error.FileNotExist, filePath));
            }

            var key = Guid.NewGuid().ToString("D");
            var output = new WaveOutEvent();
            var audio = new AudioFileReader(filePath);

            output.Init(audio);
            output.PlaybackStopped += (_, _) => this.OnPlayBackStopped(key);

            this._streams.Add(key, new OutputStream(output, audio));

            output.Play();

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 再生終了時
        /// </summary>
        /// <param name="key"></param>
        private void OnPlayBackStopped(string key)
        {
            if (!this._streams.ContainsKey(key)) return;

            this._streams[key].AudioFileReader.Dispose();
            this._streams[key].WaveOutEvent.Dispose();

            this._streams.Remove(key);
        }

        #endregion

        private record OutputStream(WaveOutEvent WaveOutEvent, AudioFileReader AudioFileReader);
    }
}
