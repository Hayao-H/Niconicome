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

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.Video
{
    public interface IVideoRequestHandler
    {
        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="request"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        IAttemptResult Handle(Uri request, HttpListenerResponse res);
    }

    public class VideoRequestHandler : IVideoRequestHandler
    {
        public VideoRequestHandler(INiconicomeFileIO fileIO, IVideoStore videoStore, IErrorHandler errorHandler)
        {
            this._fileIO = fileIO;
            this._store = videoStore;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IVideoStore _store;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult Handle(Uri request, HttpListenerResponse res)
        {
            IAttemptResult<VideoIDInfo> idResult = this.GetVideoID(request.AbsolutePath);
            if (!idResult.IsSucceeded || idResult.Data is null)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                return AttemptResult.Fail(idResult.Message);
            }

            VideoIDInfo id = idResult.Data;

            IAttemptResult<IVideoInfo> vResult = this._store.GetVideo(id.NiconicoID, id.PlaylistID);
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                return AttemptResult.Fail(vResult.Message);
            }

            IVideoInfo video = vResult.Data;
            if (!video.IsDownloaded.Value)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                this._errorHandler.HandleError(VideoRequestHandlerError.VideoIsNotDownloaded, id.PlaylistID, id.NiconicoID);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoRequestHandlerError.VideoIsNotDownloaded, id.PlaylistID, id.NiconicoID));
            }

            res.ContentType = "video/mp4";
            IAttemptResult wResult = this.WriteToStream(res.OutputStream, video.Mp4FilePath);

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
        private IAttemptResult<VideoIDInfo> GetVideoID(string request)
        {
            string[] splited = request.Split("/");
            if (splited.Length != 6)
            {
                this._errorHandler.HandleError(VideoRequestHandlerError.RequestUrlInalid, request);
                return AttemptResult<VideoIDInfo>.Fail(this._errorHandler.GetMessageForResult(VideoRequestHandlerError.RequestUrlInalid, request));
            }

            if (!int.TryParse(splited[3], out int playlistID))
            {
                this._errorHandler.HandleError(VideoRequestHandlerError.RequestUrlInalid, request);
                return AttemptResult<VideoIDInfo>.Fail(this._errorHandler.GetMessageForResult(VideoRequestHandlerError.RequestUrlInalid, request));
            }

            return AttemptResult<VideoIDInfo>.Succeeded(new VideoIDInfo(playlistID, splited[4]));
        }

        /// <summary>
        /// ストリームに書き込み
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="videoFilePath"></param>
        /// <returns></returns>
        private IAttemptResult WriteToStream(Stream stream, string videoFilePath)
        {
            if (!this._fileIO.Exists(videoFilePath))
            {
                this._errorHandler.HandleError(VideoRequestHandlerError.VideoDoesNotExist, videoFilePath);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoRequestHandlerError.VideoDoesNotExist, videoFilePath));
            }

            try
            {
                using var video = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
                video.CopyTo(stream);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(VideoRequestHandlerError.FailedToOpenVodeo, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoRequestHandlerError.FailedToOpenVodeo, ex));
            }

            return AttemptResult.Succeeded();
        }

        private record VideoIDInfo(int PlaylistID, string NiconicoID);

        #endregion
    }
}
