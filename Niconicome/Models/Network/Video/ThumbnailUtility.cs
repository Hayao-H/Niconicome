using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video.Error;
using Windows.Networking.Proximity;

namespace Niconicome.Models.Network.Video
{
    public interface IThumbnailUtility
    {
        /// <summary>
        /// サムネイルのパスを取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        IAttemptResult<string> GetThumbPath(string niconicoID);

        /// <summary>
        /// 削除された動画のサムネイルを取得する
        /// </summary>
        /// <returns></returns>
        string GetDeletedVideoThumb();

        /// <summary>
        /// サムネイルをダウンロードする
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void DownloadThumb(string niconicoID, string url, Action<IAttemptResult<string>> callback);

        /// <summary>
        /// 削除動画のサムネを取得する
        /// </summary>
        /// <returns></returns>
        Task DownloadDeletedVideoThumbAsync();

        /// <summary>
        /// サムネイルが存在するかどうかをチェックする
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        bool IsThumbExists(string niconicoID);
    }

    public class ThumbnailUtility : IThumbnailUtility
    {
        public ThumbnailUtility(INicoFileIO fileIO, IErrorHandler errorHandler, INicoHttp http, INicoDirectoryIO directoryIO)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
            this._http = http;
            this._directoryIO = directoryIO;
        }

        #region field

        private readonly INicoFileIO _fileIO;

        private readonly INicoDirectoryIO _directoryIO;

        private readonly IErrorHandler _errorHandler;

        private readonly INicoHttp _http;

        private readonly Queue<ThumbData> _queue = new();

        private bool _isProcessing;

        #endregion

        #region Method

        public IAttemptResult<string> GetThumbPath(string niconicoID)
        {
            if (!this.IsThumbExists(niconicoID))
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.ThumbNotExist, niconicoID);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbNotExist, niconicoID));
            }

            return AttemptResult<string>.Succeeded(this.GetThumbPathInternal(niconicoID));
        }

        public string GetDeletedVideoThumb()
        {
            return this.GetThumbPathInternal("0");
        }

        public bool IsThumbExists(string niconicoID)
        {
            string path = this.GetThumbPathInternal(niconicoID);
            return this._fileIO.Exists(path);
        }

        public Task DownloadDeletedVideoThumbAsync()
        {
            if (this.IsThumbExists("0")) return Task.CompletedTask;

            var tsc = new TaskCompletionSource();

            this.DownloadThumb("0", NetConstant.NiconicoDeletedVideothumb, _ => tsc.SetResult());

            return tsc.Task;
        }


        public void DownloadThumb(string niconicoID, string url, Action<IAttemptResult<string>> callback)
        {
            if (this._queue.Any(x => x.niconicoID == niconicoID))
            {
                this._queue.First(x => x.niconicoID == niconicoID).Callbacks.Add(callback);
            }
            else
            {
                this._queue.Enqueue(new ThumbData(niconicoID, url, new List<Action<IAttemptResult<string>>>() { callback }));
            }

            if (!this._isProcessing)
            {
                _ = this.StartDownloadThumbAsyncInternal();
            }
        }



        #endregion

        #region private

        /// <summary>
        /// サムネのパスを導出
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        private string GetThumbPathInternal(string niconicoID)
        {
            string numberStr = Regex.Replace(niconicoID, @"\D", "");
            int number = int.Parse(numberStr);

            return Path.Combine(AppContext.BaseDirectory, "cache", "thumb", $"{number % 20}",$"{niconicoID}.jpeg");
        }

        /// <summary>
        /// サムネイルのDLを開始
        /// </summary>
        /// <returns></returns>
        private async Task StartDownloadThumbAsyncInternal()
        {
            if (this._queue.Count == 0)
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.QueueIsEMpty);
                return;
            }

            ThumbData thumbData = this._queue.Dequeue();
            string niconicoID = thumbData.niconicoID;
            string url = thumbData.Url;
            List<Action<IAttemptResult<string>>> callbacks = thumbData.Callbacks;

            void OnCompleted(IAttemptResult<string> result)
            {
                foreach (var callback in callbacks)
                {
                    callback(result);
                }

                if (this._queue.Count > 0)
                {
                    _ = this.StartDownloadThumbAsyncInternal();
                }
                else
                {
                    this._isProcessing = false;
                }
            }

            this._isProcessing = true;

            bool uResult = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);
            if (!uResult)
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.ThumbUrlIsInvalid, url);
                OnCompleted(AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbUrlIsInvalid, url)));
                return;
            }

            HttpResponseMessage res = await this._http.GetAsync(uri!);
            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.ThumbFetchFaild, url, (int)res.StatusCode);
                OnCompleted(AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbFetchFaild, url, (int)res.StatusCode)));
                return;
            }

            byte[] data = await res.Content.ReadAsByteArrayAsync();

            string path = this.GetThumbPathInternal(niconicoID);

            string? dirPath = Path.GetDirectoryName(path);
            if (dirPath is not null && !this._directoryIO.Exists(dirPath))
            {
                try
                {
                    this._directoryIO.Create(dirPath);
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(ThumbnailUtilityError.ThumbDirectoryCreationFailed, ex, dirPath);
                    OnCompleted(AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbDirectoryCreationFailed, ex, dirPath)));
                    return;
                }
            }

            try
            {
                using var stream = new FileStream(path, FileMode.OpenOrCreate);
                stream.Write(data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.ThumbWritingFailed, ex);
                OnCompleted(AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbWritingFailed, ex)));
                return;
            }

            OnCompleted(AttemptResult<string>.Succeeded(path));
        }

        private record ThumbData(string niconicoID, string Url, List<Action<IAttemptResult<string>>> Callbacks);

        #endregion
    }
}
