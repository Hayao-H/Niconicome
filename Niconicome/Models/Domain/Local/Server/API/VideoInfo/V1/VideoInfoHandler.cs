using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.API.VideoInfo.V1.Types;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist.V2.Manager;
using NetConst = Niconicome.Models.Const.NetConstant;
using Err = Niconicome.Models.Domain.Local.Server.API.VideoInfo.V1.Error.VideoInfoHandlerError;
using System.Text.RegularExpressions;

namespace Niconicome.Models.Domain.Local.Server.API.VideoInfo.V1
{
    public interface IVideoInfoHandler
    {
        /// <summary>
        /// 動画情報JSONを取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        string GetVideoInfoJson(int port, string niconicoID, int playlistID);

        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        IAttemptResult Handle(int port, string url, HttpListenerResponse response);
    }

    public class VideoInfoHandler(IPlaylistManager playlistManager, IErrorHandler errorHandler) : IVideoInfoHandler
    {
        #region field

        private readonly IPlaylistManager _playlistManager = playlistManager;

        private readonly IErrorHandler _errorHandler = errorHandler;

        #endregion

        public string GetVideoInfoJson(int port, string niconicoID, int playlistID)
        {
            IAttemptResult<IPlaylistInfo> pResult = this._playlistManager.GetPlaylist(playlistID);
            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                var info = new JsWatchInfo();
                info.Meta.Status = (int)HttpStatusCode.NotFound;
                info.Meta.Message = pResult.Message ?? string.Empty;
                return JsonParser.Serialize(info);
            }

            IVideoInfo? video = pResult.Data.Videos.FirstOrDefault(v => v.NiconicoId == niconicoID);
            if (video is null)
            {
                var info = new JsWatchInfo();
                info.Meta.Status = (int)HttpStatusCode.NotFound;
                info.Meta.Message = this._errorHandler.HandleError(Err.VideoNotFound, playlistID, niconicoID);
                return JsonParser.Serialize(info);
            }

            JsWatchInfo converted = this.Convert(port, video, pResult.Data);
            return JsonParser.Serialize(converted);
        }

        public IAttemptResult Handle(int port, string url, HttpListenerResponse response)
        {
            RequestType type = this.GetRequestType(url);
            if (type == RequestType.None)
            {
                this.WriteMessage(this._errorHandler.HandleError(Err.InvalidRequest, url), (int)HttpStatusCode.BadRequest, response);
                return AttemptResult.Succeeded();
            }

            var urlResult = this.GetPlaylistIDAndNiconicoID(url);
            if (!urlResult.IsSucceeded)
            {
                this.WriteMessage(this._errorHandler.HandleError(Err.InvalidRequest, url), (int)HttpStatusCode.BadRequest, response);
                return AttemptResult.Succeeded();
            }

            var (playlistID, niconicoID) = urlResult.Data;
            string data = this.GetVideoInfoJson(port, niconicoID, playlistID);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/json";
            byte[] content = Encoding.UTF8.GetBytes(data);
            response.OutputStream.Write(content, 0, content.Length);
            return AttemptResult.Succeeded();
        }



        #region private

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

        private JsWatchInfo Convert(int port, IVideoInfo video, IPlaylistInfo playlist)
        {
            var info = new JsWatchInfo();
            info.Media.IsDownloaded = video.IsDownloaded.Value;
            info.Meta.Status = (int)HttpStatusCode.OK;
            info.Media.IsDMS = video.IsDMS;

            info.API.BaseUrl = $"http://localhost:{port}/niconicome/api/videoinfo/v1/{playlist.ID}/";

            if (video.IsDMS && video.IsDownloaded.Value)
            {
                info.Media.ContentUrl = string.Format(NetConst.WatchAddressV1, port, playlist.ID, video.NiconicoId);
            }
            else
            {
                info.Media.ContentUrl = string.Format(NetConst.HLSAddressV1, port, playlist.ID, video.NiconicoId);
                info.Media.CreateUrl = string.Format(NetConst.HLSCreateAddressV1, port, playlist.ID, video.NiconicoId);
            }

            info.Comment.ContentUrl = string.Format(NetConst.CommentAddressV1, port, playlist.ID, video.NiconicoId);
            info.Comment.CommentNGAPIBaseUrl = string.Format(NetConst.NGAPIAddressV1, port);
            info.Thumbnail.ContentUrl = string.Format(NetConst.ThumbnailAddressV1, port, video.NiconicoId);

            info.Video.NiconicoID = video.NiconicoId;
            info.Video.Title = video.Title;
            info.Video.UploadedAt = video.UploadedOn;
            info.Video.Tags = video.Tags.Select(tag => new Tag() { Name = tag.Name, IsNicodicExists = tag.IsNicodicExist }).ToList();
            info.Video.Owner.Name = video.OwnerName;
            info.Video.Owner.ID = video.OwnerID;
            info.Video.Description = video.Description;
            info.Video.Duration = video.Duration;
            info.Video.Count.View = video.ViewCount;
            info.Video.Count.Comment = video.CommentCount;
            info.Video.Count.Mylist = video.MylistCount;
            info.Video.Count.Like = video.LikeCount;

            info.PlaylistVideos = playlist.Videos.Select(v => new PlaylistVideo() { NiconicoID = v.NiconicoId, Title = v.Title, UploadedAt = v.UploadedOn, ThumbnailURL = string.Format(NetConst.ThumbnailAddressV1, port, v.NiconicoId), Duration = v.Duration, ViewCount = v.ViewCount }).ToList();

            return info;
        }

        private IAttemptResult<(int, string)> GetPlaylistIDAndNiconicoID(string url)
        {
            string[] splitted = url.Split('/');
            string niconicoID = splitted[^1];
            string playlistID = splitted[^2];

            if (!niconicoID.EndsWith(".json"))
            {
                return AttemptResult<(int, string)>.Fail(this._errorHandler.HandleError(Err.InvalidRequest, url));
            }

            niconicoID = niconicoID.Replace(".json", string.Empty);

            if (!int.TryParse(playlistID, out int playlistIDInt))
            {
                return AttemptResult<(int, string)>.Fail(this._errorHandler.HandleError(Err.InvalidRequest, url));
            }

            return AttemptResult<(int, string)>.Succeeded((playlistIDInt, niconicoID));
        }

        private RequestType GetRequestType(string url)
        {
            if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/api/videoinfo/v1/\d+/.+\.json"))
            {
                return RequestType.VideoInfo;
            }

            return RequestType.None;
        }

        private enum RequestType
        {
            None,
            VideoInfo,
        }

        #endregion
    }
}
