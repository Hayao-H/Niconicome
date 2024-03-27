using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.HLS;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.HLS.AES;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.LocalFile;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.Session;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2.Manager;
using Err = Niconicome.Models.Domain.Local.Server.API.Watch.V1.Error.WatchHandlerError;
using SC = Niconicome.Models.Domain.Local.Server.API.Watch.V1.StringContent.WatchHandlerStringContent;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1
{
    public interface IWatchHandler
    {
        IAttemptResult Handle(HttpListenerResponse response, string url, int port);
    }

    public class WatchHandler : IWatchHandler
    {
        public WatchHandler(ISessionManager sessionManager, IPlaylistCreator playlistCreator, ILocalFileInfoHandler localFileInfoHandler, IPlaylistManager playlistManager, IStringHandler stringHandler, IErrorHandler error, INiconicomeFileIO fileIO, IDecryptor decryptor)
        {
            this._sessionManager = sessionManager;
            this._playlistCreator = playlistCreator;
            this._localFileInfoHandler = localFileInfoHandler;
            this._playlistManager = playlistManager;
            this._stringHandler = stringHandler;
            this._error = error;
            this._fileIO = fileIO;
            this._decryptor = decryptor;
        }

        #region field

        private readonly ISessionManager _sessionManager;

        private readonly IPlaylistCreator _playlistCreator;

        private readonly ILocalFileInfoHandler _localFileInfoHandler;

        private readonly IPlaylistManager _playlistManager;

        private readonly IStringHandler _stringHandler;

        private readonly IErrorHandler _error;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IDecryptor _decryptor;

        #endregion

        public IAttemptResult Handle(HttpListenerResponse response, string url, int port)
        {
            RequestType type = this.GetRequestType(url);

            //不明なリクエスト
            if (type == RequestType.Invalid)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.InvalidRequest), (int)HttpStatusCode.BadRequest, response.OutputStream);
                return AttemptResult.Succeeded();
            }
            else if (type == RequestType.MasterPlaylist)
            {
                this.HandleMaster(response, url, port);
                return AttemptResult.Succeeded();
            }

            IAttemptResult<string> sidResult = this.GetSessionID(url);
            if (!sidResult.IsSucceeded || sidResult.Data is null)
            {
                response.ContentType = "text/html";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                this.WriteMessage(this._stringHandler.GetContent(SC.InvalidRequest), (int)HttpStatusCode.BadRequest, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            if (!this._sessionManager.Exists(sidResult.Data))
            {
                response.ContentType = "text/html";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                this.WriteMessage(this._stringHandler.GetContent(SC.SessionNotExist), (int)HttpStatusCode.BadRequest, response.OutputStream);
                return AttemptResult.Succeeded();
            }

            ISession session = this._sessionManager.GetSession(sidResult.Data);

            if (type == RequestType.VideoPlaylist || type == RequestType.AudioPlaylist)
            {
                this.HandlePlaylist(response, type, session);
                return AttemptResult.Succeeded();
            }
            else if (type == RequestType.VideoMap || type == RequestType.AudioMap)
            {
                this.HandleMap(response, type, session);
                return AttemptResult.Succeeded();
            }
            else
            {
                this.HandleSegment(response, type, session, url);
                return AttemptResult.Succeeded();
            }


        }

        #region private
        private void HandleMaster(HttpListenerResponse response, string url, int port)
        {
            string sid = this._sessionManager.CreateSession();

            IAttemptResult<(int, string)> videoIDResult = this.GetPlaylistAndVideoID(url);
            if (!videoIDResult.IsSucceeded)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.FailedToExtractVideoID), (int)HttpStatusCode.BadRequest, response.OutputStream);
                return;
            }

            IAttemptResult<IPlaylistInfo> playlistResult = this._playlistManager.GetPlaylist(videoIDResult.Data!.Item1);
            if (!playlistResult.IsSucceeded || playlistResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.NotFound), (int)HttpStatusCode.NotFound, response.OutputStream);
                return;
            }

            IVideoInfo? video = playlistResult.Data.Videos.FirstOrDefault(v => v.NiconicoId == videoIDResult.Data!.Item2);
            if (video is null)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.NotFound), (int)HttpStatusCode.NotFound, response.OutputStream);
                return;
            }

            IAttemptResult<ILocalFileInfo> fileResult = this._localFileInfoHandler.GetLocalFileInfo(video.FilePath);
            if (!fileResult.IsSucceeded || fileResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.NotFound), (int)HttpStatusCode.NotFound, response.OutputStream);
                return;
            }

            ILocalFileInfo file = fileResult.Data;
            if (file.Streams.Count == 0)
            {
                this._error.HandleError(Err.StreamNotFound);
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.NotFound), (int)HttpStatusCode.NotFound, response.OutputStream);
                return;
            }

            string folderPath = Path.GetDirectoryName(video.FilePath)!;
            var (resolution, stream) = file.Streams.OrderByDescending(s => s.Key).First();
            HLSPlaylist playlist = this._playlistCreator.GetPlaylist(stream, sid, port);

            this._sessionManager.EditSession(sid, folderPath, stream.VideoSegments, stream.AudioSegments, stream.VideoKey, stream.AudioKey, stream.VideoMapFileName, stream.AudioMapFileName, playlist.Video, playlist.Audio, resolution, stream.VideoIV, stream.AudioIV);
            try
            {
                response.ContentType = "application/vnd.apple.mpegurl";
                response.OutputStream.Write(Encoding.UTF8.GetBytes(playlist.Master));
            }
            catch (Exception ex)
            {
                this._error.HandleError(Err.FailedToWriteStream, ex);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.FailedToWriteStream), (int)HttpStatusCode.InternalServerError, response.OutputStream);
                return;
            }

            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private void HandlePlaylist(HttpListenerResponse response, RequestType type, ISession session)
        {
            string playlistStr;

            if (type == RequestType.VideoPlaylist)
            {
                playlistStr = session.VideoPlaylist;
            }
            else
            {
                playlistStr = session.AudioPlaylist;
            }

            try
            {
                response.ContentType = "application/vnd.apple.mpegurl";
                response.OutputStream.Write(Encoding.UTF8.GetBytes(playlistStr));
            }
            catch (Exception ex)
            {
                this._error.HandleError(Err.FailedToWriteStream, ex);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.FailedToWriteStream), (int)HttpStatusCode.InternalServerError, response.OutputStream);
                return;
            }

            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private void HandleMap(HttpListenerResponse response, RequestType type, ISession session)
        {
            string mapFileName;
            string contentType;

            if (type == RequestType.VideoMap)
            {
                mapFileName = Path.Combine(session.Resolution.ToString(), "video", session.VideoMapFileName);
                contentType = "video/mp4";
            }
            else
            {
                mapFileName = Path.Combine(session.Resolution.ToString(), "audio", session.AudioMapFileName);
                contentType = "audio/mp4";
            }

            string path = Path.Combine(session.FolderPath, mapFileName);

            IAttemptResult<byte[]> readResult = this._fileIO.ReadByte(path);
            if (!readResult.IsSucceeded || readResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.FailedToReadStream), (int)HttpStatusCode.InternalServerError, response.OutputStream);
                return;
            }

            try
            {
                response.OutputStream.Write(readResult.Data);
            }
            catch (Exception ex)
            {
                this._error.HandleError(Err.FailedToWriteStream, ex);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.FailedToWriteStream), (int)HttpStatusCode.InternalServerError, response.OutputStream);
                return;
            }

            response.ContentType = contentType;
            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private void HandleSegment(HttpListenerResponse response, RequestType type, ISession session, string url)
        {
            string fileName = Path.GetFileName(url);
            string path;
            string contentType;
            string key;
            string iv;

            if (type == RequestType.VideoSegment)
            {
                path = Path.Combine(session.FolderPath, session.Resolution.ToString(), "video", fileName);
                contentType = "video/mp4";
                key = session.VideoKey;
                iv = session.VideoIV;
            }
            else
            {
                path = Path.Combine(session.FolderPath, session.Resolution.ToString(), "audio", fileName);
                contentType = "audio/mp4";
                key = session.AudioKey;
                iv = session.AudioIV;
            }

            IAttemptResult<byte[]> readResult = this._fileIO.ReadByte(path);
            if (!readResult.IsSucceeded || readResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                this.WriteMessage(this._stringHandler.GetContent(SC.FailedToReadStream), (int)HttpStatusCode.InternalServerError, response.OutputStream);
                return;
            }

            IAttemptResult<byte[]> decryptResult = this._decryptor.Decrypt(readResult.Data, new AESInfomation(Convert.FromBase64String(key), this.ToBytes(iv)));

            if (!decryptResult.IsSucceeded || decryptResult.Data is null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                response.OutputStream.Write(Encoding.UTF8.GetBytes(this._stringHandler.GetContent(SC.FailedToReadStream)));
                return;
            }

            try
            {
                response.OutputStream.Write(decryptResult.Data);
            }
            catch (Exception ex)
            {
                this._error.HandleError(Err.FailedToWriteStream, ex);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/html";
                return;
            }

            response.ContentType = contentType;
            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private RequestType GetRequestType(string url)
        {

            if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/watch/v1/\d+/.+/main\.m3u8"))
            {
                return RequestType.MasterPlaylist;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/watch/v1/.+/audio\/playlist.m3u8"))
            {
                return RequestType.AudioPlaylist;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/watch/v1/.+/video/playlist\.m3u8"))
            {
                return RequestType.VideoPlaylist;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/watch/v1/.+/video/init\.cmfv"))
            {
                return RequestType.VideoMap;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/watch/v1/.+/audio/init\.cmfa"))
            {
                return RequestType.AudioMap;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/watch/v1/.+/video/.+\.cmfv"))
            {
                return RequestType.VideoSegment;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/watch/v1/.+/audio/.+\.cmfa"))
            {
                return RequestType.AudioSegment;
            }
            else
            {
                return RequestType.Invalid;
            }
        }

        private IAttemptResult<string> GetSessionID(string rawURL)
        {
            var url = new Uri(rawURL);
            string[] splited = url.AbsolutePath.TrimStart('/').Split("/");

            if (splited.Length < 4)
            {
                return AttemptResult<string>.Fail(this._error.HandleError(Err.CannotExtractSessionID, rawURL));
            }

            return AttemptResult<string>.Succeeded(splited[3]);
        }

        private IAttemptResult<(int, string)> GetPlaylistAndVideoID(string rawURL)
        {
            var url = new Uri(rawURL);
            string[] splited = url.AbsolutePath.TrimStart('/').Split("/");

            if (splited.Length < 5)
            {
                return AttemptResult<(int, string)>.Fail(this._error.HandleError(Err.CannotExtractSessionID, rawURL));
            }

            return AttemptResult<(int, string)>.Succeeded((int.Parse(splited[3]), splited[4]));
        }

        private byte[] ToBytes(string str)
        {
            var bs = new List<byte>();

            for (var i = 2; i < str.Length - 1; i += 2)
            {
                bs.Add(Convert.ToByte(str.Substring(i, 2), 16));
            }

            return bs.ToArray();
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

        #endregion

        private enum RequestType
        {
            Invalid,
            MasterPlaylist,
            AudioPlaylist,
            VideoPlaylist,
            VideoSegment,
            AudioSegment,
            VideoMap,
            AudioMap,
        }

    }
}
