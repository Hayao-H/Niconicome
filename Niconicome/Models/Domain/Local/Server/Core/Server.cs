using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.API.Comment.V1;
using Niconicome.Models.Domain.Local.Server.API.NG.V1;
using Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1;
using Niconicome.Models.Domain.Local.Server.API.Resource.V1;
using Niconicome.Models.Domain.Local.Server.API.VideoInfo.V1;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1;
using Niconicome.Models.Domain.Local.Server.RequestHandler.M3U8;
using Niconicome.Models.Domain.Local.Server.RequestHandler.NotFound;
using Niconicome.Models.Domain.Local.Server.RequestHandler.TS;
using Niconicome.Models.Domain.Local.Server.RequestHandler.UserChrome;
using Niconicome.Models.Domain.Local.Server.RequestHandler.Video;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public interface IServer
    {

        /// <summary>
        /// ポート番号
        /// </summary>
        int Port { get; }

        /// <summary>
        /// サーバーを起動
        /// </summary>
        void Start();

        /// <summary>
        /// サーバーをシャットダウン
        /// </summary>
        void ShutDown();
    }

    public class Server : IServer
    {
        public Server(IUrlHandler urlHandler, IVideoRequestHandler video, INotFoundRequestHandler notFound, IM3U8RequestHandler m3U8, ITSRequestHandler ts, IUserChromeRequestHandler userChrome, IErrorHandler errorHandler, IPortHandler portHandler, IWatchHandler watchHandler, ICommentRequestHandler commentRequestHandler, IRegacyHLSHandler regacyHLSHandler, IIPHandler iPHandler, IResourceHandler resourceHandler, IVideoInfoHandler videoInfoHandler,INGHandler nGHandler)
        {
            this._urlHandler = urlHandler;
            this._video = video;
            this._notFound = notFound;
            this._m3U8 = m3U8;
            this._ts = ts;
            this._userChrome = userChrome;
            this._errorHandler = errorHandler;
            this._portHandler = portHandler;
            this._watchHandler = watchHandler;
            this._commentRequestHandler = commentRequestHandler;
            this._regacyHLSHandler = regacyHLSHandler;
            this._resourceHandler = resourceHandler;
            this._videoInfoHandler = videoInfoHandler;
            this._nGHandler = nGHandler;
            this._iPHandler = iPHandler;
        }

        ~Server()
        {
            this._isRunning = false;
        }

        #region field

        private readonly IUrlHandler _urlHandler;

        private readonly IVideoRequestHandler _video;

        private readonly INotFoundRequestHandler _notFound;

        private readonly IM3U8RequestHandler _m3U8;

        private readonly ITSRequestHandler _ts;

        private readonly IUserChromeRequestHandler _userChrome;

        private readonly IErrorHandler _errorHandler;

        private readonly IPortHandler _portHandler;

        private readonly IWatchHandler _watchHandler;

        private readonly ICommentRequestHandler _commentRequestHandler;

        private readonly IRegacyHLSHandler _regacyHLSHandler;

        private readonly IResourceHandler _resourceHandler;

        private readonly IVideoInfoHandler _videoInfoHandler;

        private readonly INGHandler _nGHandler;

        private readonly IIPHandler _iPHandler;

        private readonly Queue<int> _ports = new();

        private bool _isRunning;

        private bool _isShutdowned;

        #endregion

        #region Props

        public int Port { get; private set; }

        #endregion

        #region Method

        public void Start()
        {
            if (this._isRunning || this._isShutdowned)
            {
                return;
            }

            this._isRunning = true;

            _ = Task.Run(() =>
            {
                try
                {
                    this.Port = this._portHandler.GetSettingValue();
                    if (!this._portHandler.IsPortAvailable(this.Port))
                    {
                        if (this._ports.Count == 0)
                        {

                            IAttemptResult<IEnumerable<int>> portResult = this._portHandler.GetAvailablePorts();
                            if (!portResult.IsSucceeded || portResult.Data is null)
                            {
                                return;
                            }

                            foreach (var p in portResult.Data)
                            {
                                this._ports.Enqueue(p);
                            }
                        }
                        this.Port = this._ports.Dequeue();
                    }

                    var listnner = new HttpListener();

                    listnner.Prefixes.Clear();
                    listnner.Prefixes.Add($"http://localhost:{this.Port}/");
                    listnner.Prefixes.Add($"http://127.0.0.1:{this.Port}/");

                    listnner.Start();
                    this._errorHandler.HandleError(ServerError.ServerStarted, this.Port);

                    while (this._isRunning)
                    {
                        HttpListenerContext context = listnner.GetContext();

                        _ = Task.Run(async () =>
                        {
                            HttpListenerRequest request = context.Request;
                            HttpListenerResponse response = context.Response;

                            this._errorHandler.HandleError(ServerError.RequestHandled, request.Url!.ToString(), request.UserAgent);

                            //CORS
                            response.Headers.Add("Access-Control-Allow-Origin", "*");

                            if (request.Url is null)
                            {
                                context.Response.Close();
                                return;
                            }

                            if (request.HttpMethod == "OPTIONS")
                            {
                                response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
                                response.StatusCode = (int)HttpStatusCode.OK;
                                response.Close();
                                return;
                            }

                            if (request.HttpMethod != "GET" && request.HttpMethod != "POST")
                            {
                                context.Response.Close();
                                return;
                            }

                            RequestType type = this._urlHandler.GetReqyestType(request.Url);
                            IAttemptResult? result = null;

                            if (type == RequestType.Video)
                            {
                                try
                                {
                                    result = this._video.Handle(request.Url, response);
                                }
                                catch { }
                            }

                            if (type == RequestType.M3U8)
                            {
                                try
                                {
                                    result = this._m3U8.Handle(response);
                                }
                                catch { }
                            }

                            if (type == RequestType.TS)
                            {
                                try
                                {
                                    result = this._ts.Handle(request.Url, response);
                                }
                                catch { }
                            }

                            if (type == RequestType.UserChrome)
                            {
                                try
                                {
                                    result = this._userChrome.Handle(response);
                                }
                                catch { }
                            }

                            if (type == RequestType.WatchAPI)
                            {
                                try
                                {
                                    result = this._watchHandler.Handle(response, request.Url.ToString(), this.Port);
                                }
                                catch { }
                            }

                            if (type == RequestType.CommentAPI)
                            {
                                try
                                {
                                    result = this._commentRequestHandler.Handle(request.Url.ToString(), response);
                                }
                                catch { }
                            }

                            if (type == RequestType.RegacyHLSAPI)
                            {
                                try
                                {
                                    result = await this._regacyHLSHandler.Handle(request.Url.ToString(), response, this.Port);
                                }
                                catch { }
                            }

                            if (type == RequestType.ResourceAPI)
                            {
                                try
                                {
                                    result = this._resourceHandler.Handle(request.Url.ToString(), response);
                                }
                                catch { }
                            }

                            if (type == RequestType.VideoInfoAPI)
                            {
                                try
                                {
                                    result = this._videoInfoHandler.Handle(this.Port, request.Url.ToString(), response);
                                }
                                catch { }
                            }

                            if (type == RequestType.NG)
                            {
                                try
                                {
                                    result = this._nGHandler.Handle(request, response);
                                }
                                catch { }
                            }

                            if (result is null || !result.IsSucceeded)
                            {
                                try
                                {
                                    this._notFound.Handle(request.Url, response, result?.Message);
                                }
                                catch { }
                            }


                            response.Close();
                        });


                    }

                    listnner.Close();
                    this._errorHandler.HandleError(ServerError.ServerStopped);
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(ServerError.ServerStoppedWithException, ex);
                    this._isRunning = false;

                    if (!this._isShutdowned)
                    {
                        this.Start();
                    }
                }
            });
        }

        public void ShutDown()
        {
            this._isRunning = false;
            this._isShutdowned = true;
        }


        #endregion
    }
}
