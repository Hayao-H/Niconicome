using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.NotFound
{
    public interface INotFoundRequestHandler
    {
        /// <summary>
        /// 対応できないリクエストを処理する
        /// </summary>
        /// <param name="request"></param>
        /// <param name="res"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        IAttemptResult Handle(Uri request, HttpListenerResponse res, string? message);
    }

    public class NotFoundRequestHandler : INotFoundRequestHandler
    {
        public NotFoundRequestHandler(IStringHandler stringHandler, IErrorHandler errorHandler)
        {
            this._stringHandler = stringHandler;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IStringHandler _stringHandler;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult Handle(Uri request, HttpListenerResponse res, string? message)
        {

            if (message is null)
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                message = this._stringHandler.GetContent(NotFoundRequestHandlerStringContent.NotFound, request.AbsolutePath);
            } else
            {
                message = this._stringHandler.GetContent(NotFoundRequestHandlerStringContent.ErrorOccured, message);
            }

            try
            {
                byte[] content = Encoding.UTF8.GetBytes(message);
                res.OutputStream.Write(content, 0, content.Length);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(NotFoundRequestHandlerError.FailedToWriteMessage, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(NotFoundRequestHandlerError.FailedToWriteMessage, ex));
            }

            res.ContentType = "text/html";
            return AttemptResult.Succeeded();
        }

        #endregion

    }
}
