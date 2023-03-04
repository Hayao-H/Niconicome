using System;
using System.Net;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.RequestHandler.NotFound;
using Niconicome.Models.Domain.Local.Server.RequestHandler.Video;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public interface IServer
    {
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
        public Server(IUrlHandler urlHandler, IVideoRequestHandler video, INotFoundRequestHandler notFound, IErrorHandler errorHandler)
        {
            this._urlHandler = urlHandler;
            this._video = video;
            this._notFound = notFound;
            this._errorHandler = errorHandler;
        }

        ~Server()
        {
            this._isRunning = false;
        }

        #region field

        private readonly IUrlHandler _urlHandler;

        private readonly IVideoRequestHandler _video;

        private readonly INotFoundRequestHandler _notFound;

        private readonly IErrorHandler _errorHandler;

        private bool _isRunning;


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
                    var listnner = new HttpListener();

                    listnner.Prefixes.Clear();
                    listnner.Prefixes.Add(@"http://localhost:2580/");

                    listnner.Start();
                    this._errorHandler.HandleError(ServerError.ServerStarted);

                    while (this._isRunning)
                    {
                        HttpListenerContext context = listnner.GetContext();

                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        if (request.Url is null)
                        {
                            context.Response.Close();
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
