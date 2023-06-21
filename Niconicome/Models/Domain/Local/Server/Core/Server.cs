using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.RequestHandler.M3U8;
using Niconicome.Models.Domain.Local.Server.RequestHandler.NotFound;
using Niconicome.Models.Domain.Local.Server.RequestHandler.TS;
using Niconicome.Models.Domain.Local.Server.RequestHandler.UserChrome;
using Niconicome.Models.Domain.Local.Server.RequestHandler.Video;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

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
        public Server(IUrlHandler urlHandler, IVideoRequestHandler video, INotFoundRequestHandler notFound, IErrorHandler errorHandler, IM3U8RequestHandler m3U8, ITSRequestHandler ts, IUserChromeRequestHandler userChrome, IPortHandler portHandler)
        {
            this._urlHandler = urlHandler;
            this._video = video;
            this._notFound = notFound;
            this._errorHandler = errorHandler;
            this._m3U8 = m3U8;
            this._ts = ts;
            this._userChrome = userChrome;
            this._portHandler = portHandler;
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

        private readonly Queue<int> _ports = new();

        private bool _isRunning;


        #endregion

        #region Props

        public int Port { get; private set; }

        #endregion

        #region Method

        public void Start()
        {
            if (this._isRunning)
            {
                return;
            }

            this._isRunning = true;

            _ = Task.Run(() =>
            {
                try
                {
                    this.Port = 2580;
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

                    listnner.Start();
                    this._errorHandler.HandleError(ServerError.ServerStarted,this.Port);

                    while (this._isRunning)
                    {
                        HttpListenerContext context = listnner.GetContext();

                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        //CORS
                        response.Headers.Add("Access-Control-Allow-Origin", "*");

                        if (request.Url is null)
                        {
                            context.Response.Close();
                            continue;
                        }

                        if (request.HttpMethod == "OPTIONS")
                        {
                            response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.Close();
                            continue;
                        }

                        if (request.HttpMethod != "GET")
                        {
                            context.Response.Close();
                            continue;
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

                        if (result is null || !result.IsSucceeded)
                        {
                            try
                            {
                                this._notFound.Handle(request.Url, response, result?.Message);
                            }
                            catch { }
                        }

                        response.Close();
                    }

                    listnner.Close();
                    this._errorHandler.HandleError(ServerError.ServerStopped);
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(ServerError.ServerStoppedWithException, ex);
                    this._isRunning = false;

                    this.Start();
                }
            });
        }

        public void ShutDown()
        {
            this._isRunning = false;
        }


        #endregion
    }
}
