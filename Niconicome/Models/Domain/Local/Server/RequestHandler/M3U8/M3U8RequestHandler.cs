using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.M3U8
{
    public interface IM3U8RequestHandler
    {
        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        IAttemptResult Handle(HttpListenerResponse res);
    }

    public class M3U8RequestHandler : IM3U8RequestHandler
    {
        public M3U8RequestHandler(INiconicomeFileIO fileIO, IErrorHandler errorHandler)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult Handle(HttpListenerResponse res)
        {
            if (!this._fileIO.Exists(@"tmp\hls\playlisyt.m3u8"))
            {
                this._errorHandler.HandleError(M3U8RequestHandlerError.PlaylistDoesNotExist);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(M3U8RequestHandlerError.PlaylistDoesNotExist));
            }

            IAttemptResult wResult = this.WriteToStream(res.OutputStream);

            if (!wResult.IsSucceeded)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                return AttemptResult.Fail(wResult.Message);
            }

            res.StatusCode = (int)HttpStatusCode.OK;
            res.ContentType = "application/x-mpegURL";
            return AttemptResult.Succeeded();

        }


        #endregion

        #region private

        /// <summary>
        /// ストリームに書き込み
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="videoFilePath"></param>
        /// <returns></returns>
        private IAttemptResult WriteToStream(Stream stream)
        {
            try
            {
                using var video = new FileStream(@"tmp\hls\playlist.m3u8", FileMode.Open, FileAccess.Read);
                video.CopyTo(stream);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(M3U8RequestHandlerError.FailedToAccessPlaylist, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(M3U8RequestHandlerError.FailedToAccessPlaylist, ex));
            }

            return AttemptResult.Succeeded();
        }

        private record VideoIDInfo(int PlaylistID, string NiconicoID);

        #endregion
    }
}
