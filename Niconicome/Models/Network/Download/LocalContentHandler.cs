using System;
using System.IO;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Network.Download
{
    public interface ILocalContentHandler
    {
        ILocalContentInfo GetLocalContentInfo(string folderPath, string format, IDmcInfo dmcInfo, uint verticalResolution, bool replaceStricted, string videoInfoExt, string ichibaInfoExt, string thumbnailExt, string ichibaSuffix, string videoInfosuffix);
        IDownloadResult MoveDownloadedFile(string niconicoId, string downloadedFilePath, string destinationPath);
    }

    public interface ILocalContentInfo
    {
        bool CommentExist { get; init; }
        bool ThumbExist { get; init; }
        bool VideoExist { get; init; }
        bool VIdeoExistInOnotherFolder { get; init; }
        bool VideoInfoExist { get; init; }
        bool IchibaInfoExist { get; init; }
        string? LocalPath { get; init; }
    }

    /// <summary>
    /// ローカルデータの処理
    /// </summary>
    public class LocalContentHandler : ILocalContentHandler
    {
        public LocalContentHandler(INiconicoUtils niconicoUtils, IVideoFileStore fileStore, ILogger logger)
        {
            this._niconicoUtils = niconicoUtils;
            this.logger = logger;
            this._videoFileStore = fileStore;
        }

        private readonly INiconicoUtils _niconicoUtils;

        private readonly IVideoFileStore _videoFileStore;

        private readonly ILogger logger;

        /// <summary>
        /// ローカルの保存状況を取得する
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="format"></param>
        /// <param name="dmcInfo"></param>
        /// <returns></returns>
        public ILocalContentInfo GetLocalContentInfo(string folderPath, string format, IDmcInfo dmcInfo, uint verticalResolution, bool replaceStricted, string videoInfoExt, string ichibaInfoExt, string thumbnailExt, string ichibaSuffix, string videoInfosuffix)
        {
            string videoFIlename = this._niconicoUtils.GetFileName(format, dmcInfo, ".mp4", replaceStricted);
            string commentFIlename = this._niconicoUtils.GetFileName(format, dmcInfo, ".xml", replaceStricted);
            string thumbFIlename = this._niconicoUtils.GetFileName(format, dmcInfo, thumbnailExt, replaceStricted);
            string videoInfoFilename = this._niconicoUtils.GetFileName(format, dmcInfo, videoInfoExt, replaceStricted, videoInfosuffix);
            string ichibaInfoFilename = this._niconicoUtils.GetFileName(format, dmcInfo, ichibaInfoExt, replaceStricted, ichibaSuffix);
            bool videoExist = this._videoFileStore.Exist(dmcInfo.Id, verticalResolution);
            string? localPath = null;

            if (videoExist)
            {
                IAttemptResult<string> result = this._videoFileStore.GetFilePath(dmcInfo.Id, verticalResolution);
                if (result.IsSucceeded&&result.Data is not null)
                {
                    localPath = result.Data;
                }
            }

            return new LocalContentInfo()
            {
                VideoExist = File.Exists(Path.Combine(folderPath, videoFIlename)),
                CommentExist = File.Exists(Path.Combine(folderPath, commentFIlename)),
                ThumbExist = File.Exists(Path.Combine(folderPath, thumbFIlename)),
                VideoInfoExist = File.Exists(Path.Combine(folderPath, videoInfoFilename)),
                VIdeoExistInOnotherFolder = videoExist,
                IchibaInfoExist = File.Exists(Path.Combine(folderPath, ichibaInfoFilename)),
                LocalPath = localPath,
            };
        }

        /// <summary>
        /// ダウンロード済のファイルをコピーする
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="downloadedFilePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public IDownloadResult MoveDownloadedFile(string niconicoId, string downloadedFilePath, string destinationPath)
        {
            if (!File.Exists(downloadedFilePath))
            {
                return new DownloadResult() { Message = "そのようなファイルは存在しません。" };
            }

            if (!Directory.Exists(destinationPath))
            {
                try
                {
                    Directory.CreateDirectory(destinationPath);
                }
                catch (Exception e)
                {
                    this.logger.Error("移動先フォルダーの作成に失敗しました。", e);
                    return new DownloadResult() { Message = "移動先フォルダーの作成に失敗しました。" };
                }
            }

            string filename = Path.GetFileName(downloadedFilePath);
            try
            {
                File.Copy(downloadedFilePath, Path.Combine(destinationPath, filename));
            }
            catch (Exception e)
            {
                this.logger.Error("ファイルのコピーに失敗しました。", e);
                return new DownloadResult() { Message = $"ファイルのコピーに失敗しました。" };
            }

            this._videoFileStore.AddFile(niconicoId, Path.Combine(destinationPath, filename));

            return new DownloadResult() { IsSucceeded = true };

        }

    }

    /// <summary>
    /// ローカル情報
    /// </summary>
    public class LocalContentInfo : ILocalContentInfo
    {
        public LocalContentInfo(string? localPath)
        {
            this.LocalPath = localPath;
        }

        public LocalContentInfo() : this(null) { }

        public bool VideoExist { get; init; }

        public bool VIdeoExistInOnotherFolder { get; init; }

        public bool CommentExist { get; init; }

        public bool ThumbExist { get; init; }

        public bool VideoInfoExist { get; init; }

        public bool IchibaInfoExist { get; init; }

        public string? LocalPath { get; init; }
    }
}
