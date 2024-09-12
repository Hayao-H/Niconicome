using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video;
using Err = Niconicome.Models.Domain.Local.Server.API.Resource.V1.Error.ResourceHandlerError;
using Niconicome.Models.Domain.Utils.Error;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Niconicome.Models.Domain.Local.Server.API.Resource.V1
{
    public interface IResourceHandler
    {
        /// <summary>
        /// リクエストを処理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        IAttemptResult Handle(string url, HttpListenerResponse response);
    }

    public class ResourceHandler(IThumbnailUtility thumbnailUtility, INiconicomeFileIO fileIO, IErrorHandler errorHandler) : IResourceHandler
    {
        #region field

        private readonly IThumbnailUtility _thumbnailUtility = thumbnailUtility;

        private readonly INiconicomeFileIO _fileIO = fileIO;

        private readonly IErrorHandler _errorHandler = errorHandler;

        #endregion

        public IAttemptResult Handle(string url, HttpListenerResponse response)
        {
            RequestType type = this.GetRequestType(url);
            if (type == RequestType.None)
            {
                this.WriteMessage(this._errorHandler.HandleError(Err.InvalidUrl, url), (int)HttpStatusCode.BadRequest, response);
                return AttemptResult.Succeeded();
            }

            if (type == RequestType.Thumbnail)
            {
                this.HandleThumb(url, response);
                return AttemptResult.Succeeded();
            } else if (type == RequestType.Favicon)
            {
                this.HandleFavicon(response);
                return AttemptResult.Succeeded();
            }

            return AttemptResult.Succeeded();
        }


        #region private

        private void HandleThumb(string url, HttpListenerResponse response)
        {

            string[] splitted = url.Split('/');
            string niconicoID = splitted[^2];
            string path;

            var thumbnail = this._thumbnailUtility.GetThumbPath(niconicoID);
            if (!thumbnail.IsSucceeded || thumbnail.Data is null)
            {
                path = this._thumbnailUtility.GetDeletedVideoThumb();
            }
            else if (!File.Exists(thumbnail.Data))
            {
                path = this._thumbnailUtility.GetDeletedVideoThumb();
            }
            else
            {
                path = thumbnail.Data;
            }

            IAttemptResult<byte[]> readResult = this._fileIO.ReadByte(path);
            if (!readResult.IsSucceeded)
            {
                this.WriteMessage(readResult.Message ?? string.Empty, (int)HttpStatusCode.InternalServerError, response);
                return;
            }

            byte[] content = readResult.Data!;
            response.OutputStream.Write(content, 0, content.Length);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "image/jpeg";
            return;
        }

        private void HandleFavicon(HttpListenerResponse response)
        {
            response.ContentType = "image/x-icon";
            Icon icon = Properties.Resources.favicon;
            icon.Save(response.OutputStream);
        }

        private void WriteMessage(string message, int statusCode, HttpListenerResponse response)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine($"<title>Niconicome | {statusCode}</title>");
            builder.AppendLine("<meta charset=\"utf-8\">");
            builder.AppendLine("</head>");
            builder.AppendLine("<body>");
            builder.AppendLine(message);
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            byte[] content = Encoding.UTF8.GetBytes(builder.ToString());
            response.OutputStream.Write(content, 0, content.Length);
            response.StatusCode = statusCode;
            response.ContentType = "text/html";
        }

        private RequestType GetRequestType(string url)
        {
            if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/resource/v1/thumb/.+/thumb\.jpg"))
            {
                return RequestType.Thumbnail;
            } else if (Regex.IsMatch(url, @"https?://.+:\d+/favicon\.ico"))
            {
                return RequestType.Favicon;
            }

            return RequestType.None;
        }



        #endregion

        private enum RequestType
        {
            None,
            Thumbnail,
            Favicon,
        }
    }
}
