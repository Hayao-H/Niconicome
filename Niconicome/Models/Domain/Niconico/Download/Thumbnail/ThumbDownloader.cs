using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Extensions.System;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;

namespace Niconicome.Models.Domain.Niconico.Download.Thumbnail
{
    public interface IThumbDownloader
    {
        Task<IAttemptResult> DownloadThumbnailAsync(IDownloadSettings settings, IWatchSession session);
    }

    public class ThumbDownloader : IThumbDownloader
    {
        public ThumbDownloader(ILogger logger, INicoHttp http, INiconicoUtils niconicoUtils, IPathOrganizer pathOrganizer)
        {
            this._logger = logger;
            this._http = http;
            this._niconicoUtils = niconicoUtils;
            this._pathOrganizer = pathOrganizer;
        }

        #region field

        private readonly ILogger _logger;

        private readonly INicoHttp _http;

        private readonly INiconicoUtils _niconicoUtils;

        private readonly IPathOrganizer _pathOrganizer;
        #endregion

        #region Method

        public async Task<IAttemptResult> DownloadThumbnailAsync(IDownloadSettings settings, IWatchSession session)
        {
            if (session.Video is null)
            {
                return AttemptResult.Fail("動画情報が未取得です。");
            }

            var filepath = this._pathOrganizer.GetFilePath(settings.FileNameFormat, session.Video!.DmcInfo, settings.ThumbnailExt, settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite, settings.ThumbSuffix);

            string? thumbUrl;

            try
            {
                thumbUrl = session.Video!.DmcInfo.ThumbInfo.GetSpecifiedThumbnail(settings.ThumbSize);
            }
            catch (Exception e)
            {
                this._logger.Error($"サムネイルURLの取得に失敗しました。", e);
                return AttemptResult.Fail("サムネイルのURLを取得できませんでした。");
            }


            byte[] data;
            try
            {
                data = await this.DownloadAsync(thumbUrl);
            }
            catch (Exception e)
            {
                this._logger.Error($"サムネイルの取得に失敗しました。", e);
                return AttemptResult.Fail($"サムネイルの取得に失敗しました。(詳細: {e.Message})");
            }

            try
            {
                this.WriteThumb(data, filepath);
            }
            catch (Exception e)
            {
                this._logger.Error($"サムネイルの保存に失敗しました。", e);
                return AttemptResult.Fail($"サムネイルの保存に失敗しました。(詳細: {e.Message})");
            }

            return AttemptResult.Succeeded();
        }

        #endregion

        #region private

        private async Task<byte[]> DownloadAsync(string url)
        {
            var res = await this._http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"サムネイルの取得に失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase}, url: {url})");
            }
            else
            {
                return await res.Content.ReadAsByteArrayAsync();
            }
        }

        private void WriteThumb(byte[] data, string filePath)
        {
            IOUtils.CreateDirectoryIfNotExist(filePath);
            using var fs = File.Create(filePath);
            fs.Write(data);
        }

        private string GetFilePath(string filePath, string foldername, bool overwrite)
        {
            foldername = this.GetFolderName(foldername);
            filePath = Path.Combine(foldername, filePath);
            if (!overwrite)
            {
                filePath = IOUtils.CheclFileExistsAndReturnNewFilename(filePath);
            }
            return filePath;
        }

        private string GetFolderName(string foldername)
        {
            if (Path.IsPathRooted(foldername))
            {
                return foldername;
            }
            else
            {
                return Path.Combine(AppContext.BaseDirectory, foldername);
            }
        }

        #endregion
    }


}
