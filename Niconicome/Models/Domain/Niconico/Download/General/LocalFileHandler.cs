using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.General
{
    public interface ILocalFileHandler
    {
        /// <summary>
        /// 保存済みのファイル情報を取得
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="niconicoID"></param>
        /// <param name="ignoreEconomy"></param>
        /// <param name="verticalResolution"></param>
        /// <param name="thumbnailExt"></param>
        /// <param name="ichibaSuffix"></param>
        /// <param name="videoInfosuffix"></param>
        /// <param name="economySuffix"></param>
        /// <returns></returns>
        IAttemptResult<ILocalFileInfo> GetLocalContentInfo(string folderPath, string niconicoID, bool ignoreEconomy, uint verticalResolution, string thumbnailExt, string ichibaSuffix, string videoInfosuffix, string economySuffix);

        /// <summary>
        /// ダウンロード済みの動画ファイルを移動
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        IAttemptResult MoveDownloadedVideoFile(string sourcePath, string destinationPath);
    }

    public class LocalFileHandler : ILocalFileHandler
    {
        public LocalFileHandler(INiconicomeFileIO fileIO, INiconicomeDirectoryIO directoryIO, IVideoFileStore fileStore)
        {
            this._fileIO = fileIO;
            this._directoryIO = directoryIO;
            this._videoFileStore = fileStore;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly IVideoFileStore _videoFileStore;

        #endregion

        #region Method

        public IAttemptResult<ILocalFileInfo> GetLocalContentInfo(string folderPath, string niconicoID, bool ignoreEconomy, uint verticalResolution, string thumbnailExt, string ichibaSuffix, string videoInfosuffix, string economySuffix)
        {
            IAttemptResult<IEnumerable<string>> filesResult = this._directoryIO.GetFiles(folderPath);

            if (!filesResult.IsSucceeded || filesResult.Data is null)
            {
                return AttemptResult<ILocalFileInfo>.Fail(filesResult.Message);
            }

            IEnumerable<string> files = filesResult.Data.Where(f=>f.Contains(niconicoID));

            bool videoExist = files.Any(f => this.GetFileType(f, thumbnailExt, ichibaSuffix, videoInfosuffix) == FileType.Video);
            bool commentExist = files.Any(f => this.GetFileType(f, thumbnailExt, ichibaSuffix, videoInfosuffix) == FileType.Comment);
            bool ichibaInfoExist = files.Any(f => this.GetFileType(f, thumbnailExt, ichibaSuffix, videoInfosuffix) == FileType.Ichiba);
            bool videoInfoExist = files.Any(f => this.GetFileType(f, thumbnailExt, ichibaSuffix, videoInfosuffix) == FileType.VideoInfo);
            bool thumbExist = files.Any(f => this.GetFileType(f, thumbnailExt, ichibaSuffix, videoInfosuffix) == FileType.Thumbnail);

            bool videoExistsInAnotherFolder = this._videoFileStore.Exist(niconicoID, verticalResolution);
            string videoFilePath = string.Empty;
            if (videoExistsInAnotherFolder)
            {
                IAttemptResult<string> result = this._videoFileStore.GetFilePath(niconicoID, verticalResolution);
                if (result.IsSucceeded && !string.IsNullOrEmpty(result.Data))
                {
                    videoFilePath = result.Data;
                }
            }

            if (videoExist && ignoreEconomy)
            {
                string p = files.First(f => this.GetFileType(f, thumbnailExt, ichibaSuffix, videoInfosuffix) == FileType.Video);
                if (p.Contains(economySuffix))
                {
                    videoExist = false;
                }
            }


            var data = new LocalFileInfo()
            {
                VideoExists = videoExist,
                CommentExists = commentExist,
                VideoInfoExist = videoInfoExist,
                IchibaInfoExist = ichibaInfoExist,
                ThumbExists = thumbExist,
                VideoExistInOnotherFolder = videoExistsInAnotherFolder,
                VideoFilePath = videoFilePath,
            };

            return AttemptResult<ILocalFileInfo>.Succeeded(data);
        }

        public IAttemptResult MoveDownloadedVideoFile(string sourcePath, string destinationPath)
        {
            return this._fileIO.Copy(sourcePath, destinationPath);
        }

        #endregion

        #region private

        private FileType GetFileType(string fileName, string thumbnailExt, string ichibaSuffix, string videoInfosuffix)
        {
            if (fileName.Contains(FileFolder.Mp4FileExt) || fileName.Contains(FileFolder.TsFileExt))
            {
                return FileType.Video;
            }
            else if (fileName.Contains(ichibaSuffix))
            {
                return FileType.Ichiba;
            }
            else if (fileName.Contains(videoInfosuffix))
            {
                return FileType.VideoInfo;
            }
            else if (fileName.Contains(thumbnailExt))
            {
                return FileType.Thumbnail;
            }
            else if (fileName.Contains(".xml"))
            {
                return FileType.Comment;
            }
            else
            {
                return FileType.Unknown;
            }
        }

        private enum FileType
        {
            Unknown,
            Video,
            Comment,
            Ichiba,
            VideoInfo,
            Thumbnail,
        }

        #endregion
    }
}
