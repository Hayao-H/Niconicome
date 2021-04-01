using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Local
{
    interface ILocalVideoUtils
    {
        string GetFilePath(IVideoListInfo video, string folderPath, string format, bool replaceStricted);
    }

    class LocalVideoUtils : ILocalVideoUtils
    {
        public LocalVideoUtils(INiconicoUtils niconicoUtils, IVideoFileStorehandler videoFileStorehandler)
        {
            this.niconicoUtils = niconicoUtils;
            this.videoFileStorehandler = videoFileStorehandler;
        }

        private readonly INiconicoUtils niconicoUtils;

        private readonly IVideoFileStorehandler videoFileStorehandler;


        /// <summary>
        /// ローカルファイルの取得を試行する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public string GetFilePath(IVideoListInfo video, string folderPath, string format, bool replaceStricted)
        {
            if (!Path.IsPathRooted(folderPath))
            {
                folderPath = Path.Combine(AppContext.BaseDirectory, folderPath);
            }
            var fn = this.niconicoUtils.GetFileName(format, video, ".mp4", replaceStricted);
            var path = IOUtils.GetPrefixedPath(Path.Combine(folderPath, fn));

            if (File.Exists(path)) return path;

            return string.Empty;

        }

    }
}
