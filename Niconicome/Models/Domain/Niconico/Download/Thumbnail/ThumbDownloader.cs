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

namespace Niconicome.Models.Domain.Niconico.Download.Thumbnail
{



    public interface IThumbDownloader
    {
        Task<IDownloadResult> DownloadThumbnailAsync(IThumbDownloadSettings settings);
        Task<IDownloadResult> DownloadThumbnailAsync(IThumbDownloadSettings settings, IWatchSession session);
    }


    public interface IThumbDownloadSettings
    {
        string NiconicoId { get; }
        string FolderName { get; }
        string FileNameFormat { get; }
        bool IsOverwriteEnable { get; }
        bool IsReplaceStrictedEnable { get; }
    }

    /// <summary>
    /// 外部から触るAPIを提供する
    /// </summary>
    public class ThumbDownloader : IThumbDownloader
    {
        public ThumbDownloader(IWatchSession session, ILogger logger, INicoHttp http, INiconicoUtils niconicoUtils)
        {
            this.session = session;
            this.logger = logger;
            this.http = http;
            this.niconicoUtils = niconicoUtils;
        }

        public async Task<IDownloadResult> DownloadThumbnailAsync(IThumbDownloadSettings settings, IWatchSession session)
        {


            if (session.Video is null)
            {
                throw new InvalidOperationException("動画情報が未取得です。");
            }


            var generatedFIlename = this.niconicoUtils.GetFileName(settings.FileNameFormat, session.Video!.DmcInfo, ".jpg",settings.IsReplaceStrictedEnable);
            string fileName = generatedFIlename.IsNullOrEmpty() ? $"[{session.Video!.Id}]{session.Video!.Title}.jpg" : generatedFIlename;

            IOUtils.CreateDirectoryIfNotExist(settings.FolderName, fileName);


            string? thumbUrl;
            if (!(session.Video!.DmcInfo.ThumbInfo.Large?.IsNullOrEmpty() ?? true))
            {
                thumbUrl = session.Video!.DmcInfo.ThumbInfo.Large;
            }
            else if (!(session.Video!.DmcInfo.ThumbInfo.Normal?.IsNullOrEmpty() ?? true))
            {
                thumbUrl = session.Video!.DmcInfo.ThumbInfo.Normal;
            }
            else
            {
                return new DownloadResult() { Issucceeded = false, Message = "サムネイルのURLを取得できませんでした。" };
            }


            byte[] data;
            try
            {
                data = await this.DownloadAsync(thumbUrl);
            }
            catch (Exception e)
            {
                this.logger.Error($"サムネイルの取得に失敗しました。", e);
                return new DownloadResult() { Issucceeded = false, Message = $"サムネイルの取得に失敗しました。(詳細: {e.Message})" };
            }

            try
            {
                this.WriteThumb(data, settings.FolderName, fileName, settings.IsOverwriteEnable);
            }
            catch (Exception e)
            {
                this.logger.Error($"サムネイルの保存に失敗しました。", e);
                return new DownloadResult() { Issucceeded = false, Message = $"サムネイルの保存に失敗しました。(詳細: {e.Message})" };
            }

            return new DownloadResult() { Issucceeded = true };
        }

        public async Task<IDownloadResult> DownloadThumbnailAsync(IThumbDownloadSettings settings)
        {
            return await this.DownloadThumbnailAsync(settings, this.session);
        }

        private readonly IWatchSession session;

        private readonly ILogger logger;

        private readonly INicoHttp http;

        private readonly INiconicoUtils niconicoUtils;

        /// <summary>
        /// サムネイルをダウンロードする
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<byte[]> DownloadAsync(string url)
        {
            var res = await this.http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"サムネイルの取得に失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase}, url: {url})");
            }
            else
            {
                return await res.Content.ReadAsByteArrayAsync();
            }
        }

        /// <summary>
        /// サムネイルを書き込む
        /// </summary>
        /// <param name="data"></param>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <param name="isOverwriteEnable"></param>
        private void WriteThumb(byte[] data, string folderName, string fileName, bool isOverwriteEnable)
        {
            folderName = this.GetFolderName(folderName);
            fileName = this.GetFilePath(fileName, folderName, isOverwriteEnable);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            using var fs = File.Create(fileName);
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


        /// <summary>
        /// フォルダー名を取得する
        /// </summary>
        /// <param name="foldername"></param>
        /// <returns></returns>
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
    }

    /// <summary>
    /// ダウンロード設定
    /// </summary>
    public class ThumbDownloadSettings : IThumbDownloadSettings
    {
        public string NiconicoId { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public bool IsOverwriteEnable { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }


    }

}
