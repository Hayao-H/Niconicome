using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.External.Software.FFmpeg.FFmpeg;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode.VideoEncoderError;
using SC = Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode.VideoEncoderSC;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode
{
    public interface IVideoEncoder
    {
        /// <summary>
        /// 動画ファイルをエンコード
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult> EncodeAsync(IEncodeOption settings, Action<string> onMessage, CancellationToken token);
    }

    public class VideoEncoder : IVideoEncoder
    {
        public VideoEncoder(IErrorHandler errorHandler, INiconicomeFileIO fileIO, IFFmpegManager fFmpegManager, IStringHandler stringHandler)
        {
            this._errorHandler = errorHandler;
            this._fileIO = fileIO;
            this._fFmpegManager = fFmpegManager;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IFFmpegManager _fFmpegManager;

        private readonly IStringHandler _stringHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult> EncodeAsync(IEncodeOption settings, Action<string> onMessage, CancellationToken token)
        {
            string targetFilePath = Path.Combine(settings.FolderPath, "combined.ts");

            string? targetFolderPath = Path.GetDirectoryName(settings.FilePath);
            if (targetFolderPath is null)
            {
                this._errorHandler.HandleError(Err.FailedToGetTargetFolderPath, settings.FilePath);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.FailedToGetTargetFolderPath, settings.FilePath));
            }

            if (token.IsCancellationRequested)
            {
                this._errorHandler.HandleError(Err.Canceled);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.Canceled));
            }

            onMessage(this._stringHandler.GetContent(SC.StartTSConcat));

            IAttemptResult result = await Task.Run(() =>
               {
                   return this.Merge(settings.TsFilePaths, targetFilePath);

               });

            if (!result.IsSucceeded)
            {
                return result;
            }

            onMessage(this._stringHandler.GetContent(SC.CompleteTSConcat));

            if (token.IsCancellationRequested)
            {
                this._errorHandler.HandleError(Err.Canceled);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.Canceled));
            }

            if (settings.IsStoreRawTSFileEnable)
            {
                onMessage(this._stringHandler.GetContent(SC.StartCopyFile));

                IAttemptResult cResult = this._fileIO.Copy(targetFolderPath, settings.FilePath);

                if (!cResult.IsSucceeded)
                {
                    return cResult;
                }

                onMessage(this._stringHandler.GetContent(SC.CompleteCopyFile));
            }
            else
            {
                onMessage(this._stringHandler.GetContent(SC.StartEncode));


                IAttemptResult ffmpegResult = await this._fFmpegManager.EncodeAsync(targetFilePath, settings.FilePath, settings.CommandFormat, token);

                if (!ffmpegResult.IsSucceeded)
                {
                    return ffmpegResult;
                }

                onMessage(this._stringHandler.GetContent(SC.CompleteEncode));

            }

            if (settings.IsOverrideDTEnable)
            {

                return this._fileIO.SetLastWriteTime(settings.FilePath, settings.UploadedOn);
            }
            else
            {
                return AttemptResult.Succeeded();
            }

        }


        #endregion

        #region private

        /// <summary>
        /// セグメントファイルを結合
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <param name="targetFilePath"></param>
        /// <returns></returns>
        public IAttemptResult Merge(IEnumerable<string> sourceFiles, string targetFilePath)
        {
            try
            {
                using var fsTarget = new FileStream(targetFilePath, FileMode.OpenOrCreate);
                foreach (var file in sourceFiles.OrderBy(p =>
                {
                    p = Path.GetFileName(p) ?? string.Empty;
                    return int.Parse(p.Contains(".") ? p[0..p.LastIndexOf(".")] : p);
                }))
                {
                    using var fsSource = new FileStream(file, FileMode.Open);
                    int count;
                    byte[] buffer = new byte[1024 * 1024 * 10];
                    while ((count = fsSource.Read(buffer, 0, buffer.Length)) > 0)
                    {

                        fsTarget.Write(buffer, 0, count);
                    }
                }
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Err.FailedToConcatTS, targetFilePath);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.FailedToConcatTS, ex, targetFilePath));
            }

            return AttemptResult.Succeeded();
        }


        #endregion
    }
}
