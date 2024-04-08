using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1.SegmentCreator;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1.Error.RegacyHLSHandlerError;

namespace Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1
{
    public interface IRegacyHLSHandler
    {
        /// <summary>
        /// リクエストを処理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task<IAttemptResult> Handle(string url, HttpListenerResponse response, int port);
    }


    public class RegacyHLSHandler : IRegacyHLSHandler
    {
        public RegacyHLSHandler(IHLSManager hLSManager, INiconicomeFileIO fileIO, IErrorHandler error)
        {
            this._hLSManager = hLSManager;
            this._fileIO = fileIO;
            this._error = error;
        }

        #region field

        private readonly IHLSManager _hLSManager;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _error;


        #endregion

        public async Task<IAttemptResult> Handle(string url, HttpListenerResponse response, int port)
        {
            RequestType type = this.GetRequestType(url);
            if (type == RequestType.Unknown)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                this.WriteMessage(this._error.HandleError(Err.InvalidRequest, url), (int)HttpStatusCode.NotFound, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            IAttemptResult<(int, string)> pResult = this.GetPlaylistAndVideoID(url);
            if (!pResult.IsSucceeded)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                this.WriteMessage(pResult.Message ?? string.Empty, (int)HttpStatusCode.BadRequest, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            (int playlistID, string niconicoID) = pResult.Data!;
            if (type == RequestType.Create)
            {
                IAttemptResult result = await this._hLSManager.CreateFilesAsync(niconicoID, playlistID);
                if (result.IsSucceeded)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/html";
                    this.WriteMessage($"http://localhost:{port}/niconicome/api/regacyhls/v1/{playlistID}/{niconicoID}/master.m3u8", (int)HttpStatusCode.OK, response.OutputStream);
                }
                else
                {
                    response.ContentType = "text/html";
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    this.WriteMessage(result.Message ?? string.Empty, (int)HttpStatusCode.InternalServerError, response.OutputStream);
                }
            }
            else if (type == RequestType.M3U8)
            {
                this.HandleM3U8(response, niconicoID, playlistID.ToString());
            }
            else
            {
                this.HandleTS(response, niconicoID, playlistID.ToString(), Path.GetFileName(url));
            }

            return AttemptResult.Succeeded();
        }

        #region private

        /// <summary>
        /// マスタープレイリストを返す
        /// </summary>
        /// <param name="response"></param>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        private IAttemptResult HandleM3U8(HttpListenerResponse response, string niconicoID, string playlistID)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "data", "tmp", "hls", playlistID, niconicoID, "playlist.m3u8");

            if (!this._fileIO.Exists(path))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "text/html";
                this.WriteMessage(this._error.HandleError(Err.FileNotExists, niconicoID, playlistID), (int)HttpStatusCode.NotFound, response.OutputStream);
                return AttemptResult.Fail();
            }

            IAttemptResult<byte[]> readResult = this._fileIO.ReadByte(path);
            if (!readResult.IsSucceeded || readResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                this.WriteMessage(readResult.Message ?? string.Empty, (int)HttpStatusCode.InternalServerError, response.OutputStream);
                return AttemptResult.Fail();
            }

            response.ContentType = "application/vnd.apple.mpegurl";
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Write(readResult.Data, 0, readResult.Data.Length);

            return AttemptResult.Succeeded();

        }

        /// <summary>
        /// セグメントファイルを返す
        /// </summary>
        /// <param name="response"></param>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private IAttemptResult HandleTS(HttpListenerResponse response, string niconicoID, string playlistID, string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "data", "tmp", "hls", playlistID, niconicoID, fileName);

            if (!this._fileIO.Exists(path))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "text/html";
                this.WriteMessage(this._error.HandleError(Err.FileNotExists, niconicoID, playlistID), (int)HttpStatusCode.NotFound, response.OutputStream);
                return AttemptResult.Fail();
            }

            IAttemptResult<byte[]> readResult = this._fileIO.ReadByte(path);
            if (!readResult.IsSucceeded || readResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                this.WriteMessage(readResult.Message ?? string.Empty, (int)HttpStatusCode.InternalServerError, response.OutputStream);
                return AttemptResult.Fail();
            }

            response.ContentType = "video/MP2T";
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Write(readResult.Data, 0, readResult.Data.Length);

            return AttemptResult.Succeeded();
        }

        private IAttemptResult<(int, string)> GetPlaylistAndVideoID(string rawURL)
        {
            var url = new Uri(rawURL);
            string[] splited = url.AbsolutePath.TrimStart('/').Split("/");

            if (splited.Length < 5)
            {
                return AttemptResult<(int, string)>.Fail(this._error.HandleError(Err.InvalidRequest, rawURL));
            }

            return AttemptResult<(int, string)>.Succeeded((int.Parse(splited[4]), splited[5]));
        }


        /// <summary>
        /// レスポンスを書き込む
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="output"></param>
        private void WriteMessage(string message, int status, Stream output)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine($"<title>Niconicome | {status}</title>");
            builder.AppendLine("<meta charset=\"utf-8\">");
            builder.AppendLine("</head>");
            builder.AppendLine("<body>");
            builder.AppendLine(message);
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            byte[] content = Encoding.UTF8.GetBytes(builder.ToString());
            output.Write(content, 0, content.Length);
        }

        /// <summary>
        /// リクエストを判別
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private RequestType GetRequestType(string url)
        {
            if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/api/regacyhls/v1/\d+/.+/create"))
            {
                return RequestType.Create;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/api/regacyhls/v1/\d+/.+/master\.m3u8"))
            {
                return RequestType.M3U8;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/api/regacyhls/v1/\d+/.+/.+\.ts"))
            {
                return RequestType.TS;
            }
            else
            {
                return RequestType.Unknown;
            }
        }


        private enum RequestType
        {
            Unknown,
            Create,
            TS,
            M3U8,
        }

        #endregion
    }
}
