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
using Niconicome.Models.Utils.InitializeAwaiter;

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
        /// <returns></returns>
        Task<IAttemptResult> DownloadThumbAsync(string niconicoID, string url);

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
        public ThumbnailUtility(INicoFileIO fileIO,IErrorHandler errorHandler,INicoHttp http)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
            this._http = http;
        }

        #region field

        private readonly INicoFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        private readonly INicoHttp _http;

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

        public async Task DownloadDeletedVideoThumbAsync()
        {
            if (this.IsThumbExists("0")) return;
            await this.DownloadThumbAsync("0", NetConstant.NiconicoDeletedVideothumb);
        }


        public async Task<IAttemptResult> DownloadThumbAsync(string niconicoID, string url)
        {
            bool uResult = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);
            if (!uResult)
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.ThumbUrlIsInvalid, url);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbUrlIsInvalid, url));
            }

            HttpResponseMessage res = await this._http.GetAsync(uri!);
            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.ThumbFetchFaild, url, (int)res.StatusCode);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbFetchFaild, url, (int)res.StatusCode));
            }

            byte[] data = await res.Content.ReadAsByteArrayAsync();

            string path = this.GetThumbPathInternal(niconicoID);
            try
            {
                using var stream = new FileStream(path, FileMode.OpenOrCreate);
                stream.Write(data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ThumbnailUtilityError.ThumbWritingFailed, ex);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThumbnailUtilityError.ThumbWritingFailed, ex));
            }

            return AttemptResult.Succeeded();
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
            string numberStr = Regex.Replace(niconicoID, @"/D", "");
            int number = int.Parse(numberStr);

            return Path.Combine(AppContext.BaseDirectory, "thumb", "cache", $"{number % 10}.jpg");
        }

        #endregion
    }
}
