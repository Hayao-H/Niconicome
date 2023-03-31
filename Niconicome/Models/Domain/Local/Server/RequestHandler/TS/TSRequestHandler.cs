using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.TS
{
    public interface ITSRequestHandler
    {
        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="request"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        IAttemptResult Handle(Uri request, HttpListenerResponse res);
    }

    public class TSRequestHandler : ITSRequestHandler
    {
        public TSRequestHandler(INiconicomeFileIO fileIO, IErrorHandler errorHandler)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult Handle(Uri request, HttpListenerResponse res)
        {
            IAttemptResult<string> fileResult = this.GetFileName(request.AbsolutePath);
            if (!fileResult.IsSucceeded || fileResult.Data is null)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                return AttemptResult.Fail(fileResult.Message);
            }


            string videoFilePath = Path.Join(AppContext.BaseDirectory, "tmp", "hls", fileResult.Data);

            if (!this._fileIO.Exists(videoFilePath))
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                this._errorHandler.HandleError(TSRequestHandlerError.VideoDoesNotExist, videoFilePath);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(TSRequestHandlerError.VideoDoesNotExist, videoFilePath));
            }

            res.ContentType = "video/MP2T";
            IAttemptResult wResult = this.WriteToStream(res.OutputStream, videoFilePath);

            if (!wResult.IsSucceeded)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                return AttemptResult.Fail(wResult.Message);
            }

            res.StatusCode = (int)HttpStatusCode.OK;
            return AttemptResult.Succeeded();

        }


        #endregion

        #region private

        /// <summary>
        /// プレイリスト・動画のIDを取得
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private IAttemptResult<string> GetFileName(string request)
        {
            string[] splited = request.Split("/");
            if (splited.Length != 4)
            {
                this._errorHandler.HandleError(TSRequestHandlerError.RequestUrlInalid, request);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(TSRequestHandlerError.RequestUrlInalid, request));
            }

            return AttemptResult<string>.Succeeded(splited[3]);
        }

        /// <summary>
        /// ストリームに書き込み
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="videoFilePath"></param>
        /// <returns></returns>
        private IAttemptResult WriteToStream(Stream stream, string videoFilePath)
        {
            try
            {
                using var video = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
                video.CopyTo(stream);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(TSRequestHandlerError.FailedToOpenVodeo, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(TSRequestHandlerError.FailedToOpenVodeo, ex));
            }

            return AttemptResult.Succeeded();
        }

        #endregion
    }
}
