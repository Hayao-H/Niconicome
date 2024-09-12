using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.API.Comment.V1.Local;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2.Manager;
using Err = Niconicome.Models.Domain.Local.Server.API.Comment.V1.Error.CommentRequestHandlerError;

namespace Niconicome.Models.Domain.Local.Server.API.Comment.V1
{
    public interface ICommentRequestHandler
    {
        /// <summary>
        /// リクエストを裁く
        /// </summary>
        /// <param name="url"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        IAttemptResult Handle(string url, HttpListenerResponse response);
    }

    public class CommentRequestHandler : ICommentRequestHandler
    {
        public CommentRequestHandler(IPlaylistManager playlistManager, IErrorHandler errorHandler, ICommentRetreiver commentRetreiver)
        {
            this._playlistManager = playlistManager;
            this._errorHandler = errorHandler;
            this._commentRetreiver = commentRetreiver;
        }

        #region field

        private readonly IPlaylistManager _playlistManager;

        private readonly IErrorHandler _errorHandler;

        private readonly ICommentRetreiver _commentRetreiver;

        #endregion

        public IAttemptResult Handle(string url, HttpListenerResponse response)
        {
            IAttemptResult<(int, string)> playlistAndVideoIDResult = this.GetPlaylistAndVideoID(url);
            if (!playlistAndVideoIDResult.IsSucceeded)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                this.WriteMessage(playlistAndVideoIDResult.Message ?? string.Empty, response.StatusCode, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            IAttemptResult<IPlaylistInfo> playlistResult = this._playlistManager.GetPlaylist(playlistAndVideoIDResult.Data.Item1);
            if (!playlistResult.IsSucceeded || playlistResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                this.WriteMessage(playlistResult.Message ?? string.Empty, response.StatusCode, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            IVideoInfo? video = playlistResult.Data.Videos.FirstOrDefault(v => v.NiconicoId == playlistAndVideoIDResult.Data.Item2);
            if (video is null)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                this.WriteMessage(this._errorHandler.HandleError(Err.NotFound), response.StatusCode, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            IAttemptResult<CommentType> commentResult = this._commentRetreiver.GetComment(playlistAndVideoIDResult.Data.Item2, playlistResult.Data.TemporaryFolderPath);
            if (!commentResult.IsSucceeded || commentResult.Data is null)
            {

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                this.WriteMessage(commentResult.Message ?? string.Empty, response.StatusCode, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            string data = JsonParser.Serialize(commentResult.Data);
            byte[] content = Encoding.UTF8.GetBytes(data);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/json";
            response.OutputStream.Write(content, 0, content.Length);

            return AttemptResult.Succeeded();
        }

        private IAttemptResult<(int, string)> GetPlaylistAndVideoID(string rawURL)
        {
            var url = new Uri(rawURL);
            string[] splited = url.AbsolutePath.TrimStart('/').Split("/");

            if (splited.Length < 5)
            {
                return AttemptResult<(int, string)>.Fail(this._errorHandler.HandleError(Err.CannotExtractPlaylistAndVideoID, rawURL));
            }

            return AttemptResult<(int, string)>.Succeeded((int.Parse(splited[4]), splited[5]));
        }

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
    }
}
