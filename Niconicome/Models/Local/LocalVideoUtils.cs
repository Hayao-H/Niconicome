using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Local
{
    public interface ILocalVideoUtils
    {
        string GetFilePath(IListVideoInfo video, string folderPath, string format, bool replaceStricted, bool searchByID);
        void ClearCache();
    }

    class LocalVideoUtils : ILocalVideoUtils
    {
        public LocalVideoUtils(INiconicoUtils niconicoUtils, INicoDirectoryIO directoryIO)
        {
            this.niconicoUtils = niconicoUtils;
            this.directoryIO = directoryIO;
        }

        #region field

        private readonly INiconicoUtils niconicoUtils;

        private readonly INicoDirectoryIO directoryIO;

        private List<string>? cachedFiles;
        #endregion

        /// <summary>
        /// ローカルファイルの取得を試行する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public string GetFilePath(IListVideoInfo video, string folderPath, string format, bool replaceStricted, bool searchByID)
        {

            if (!Path.IsPathRooted(folderPath))
            {
                folderPath = Path.Combine(AppContext.BaseDirectory, folderPath);
            }

            if (this.cachedFiles is null)
            {
                this.cachedFiles = new List<string>();
                if (this.directoryIO.Exists(folderPath))
                {
                    this.cachedFiles.AddRange(this.directoryIO.GetFiles(folderPath, $"*{FileFolder.Mp4FileExt}", true).Select(p => Path.Combine(folderPath, p)).ToList());
                    this.cachedFiles.AddRange(this.directoryIO.GetFiles(folderPath, $"*{FileFolder.TsFileExt}", true).Select(p => Path.Combine(folderPath, p)).ToList());
                }
                
            }

            bool SearchFunc(string currentPath,string targetPath)
            {
                if (searchByID)
                {
                    return currentPath.Contains(video.NiconicoId.Value);
                } else
                {
                    return currentPath == targetPath;
                }
            }

            var fn = this.niconicoUtils.GetFileName(format, video, FileFolder.Mp4FileExt, replaceStricted);
            var path = IOUtils.GetPrefixedPath(Path.Combine(folderPath, fn));

            string? firstMp4 = this.cachedFiles.FirstOrDefault(p => SearchFunc(p,path));
            //.mp4ファイルを確認
            if (firstMp4 is not null)
            {
                return firstMp4;
            }
            else
            //.tsファイルを確認
            {
                fn = this.niconicoUtils.GetFileName(format, video, FileFolder.TsFileExt, replaceStricted);
                path = IOUtils.GetPrefixedPath(Path.Combine(folderPath, fn));
                string? firstTS = this.cachedFiles.FirstOrDefault(p => SearchFunc(p, path));
                if (firstTS is not null) return firstTS;
            }

            return Path.GetDirectoryName(path) ?? folderPath;

        }

        /// <summary>
        /// キャッシュをクリアする
        /// </summary>
        public void ClearCache()
        {
            this.cachedFiles = null;
        }


    }
}
