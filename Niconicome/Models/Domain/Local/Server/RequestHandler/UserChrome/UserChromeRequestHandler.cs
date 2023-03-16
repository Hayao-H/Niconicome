using System;
using System.IO;
using System.Net;
using System.Text;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.UserChrome
{
    public interface IUserChromeRequestHandler
    {
        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        IAttemptResult Handle(HttpListenerResponse res);
    }

    public class UserChromeRequestHandler : IUserChromeRequestHandler
    {
        public UserChromeRequestHandler(INiconicomeFileIO fileIO, IErrorHandler errorHandler, IStringHandler stringHandler)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        #endregion

        #region Method

        public IAttemptResult Handle(HttpListenerResponse res)
        {

            res.ContentType = "text/css";

            IAttemptResult wResult = this.WriteToStream(res.OutputStream);

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
        /// ストリームに書き込み
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="videoFilePath"></param>
        /// <returns></returns>
        private IAttemptResult WriteToStream(Stream stream)
        {
            string path = Path.Join(AppContext.BaseDirectory, FileFolder.UserChromePath);

            try
            {
                if (!this._fileIO.Exists(path))
                {
                    byte[] content = Encoding.UTF8.GetBytes(this._stringHandler.GetContent(UserChromeRequestHandlerStringContent.DefaultContent));
                    stream.Write(content, 0, content.Length);
                }
                else
                {
                    using var content = new FileStream(path, FileMode.Open, FileAccess.Read);
                    content.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(UserChromeRequestHandlerError.FailedToAccessCSS, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(UserChromeRequestHandlerError.FailedToAccessCSS, ex));
            }

            return AttemptResult.Succeeded();
        }

        private record VideoIDInfo(int PlaylistID, string NiconicoID);

        #endregion
    }
}
