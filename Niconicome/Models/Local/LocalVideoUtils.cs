using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Local
{
    public interface ILocalVideoUtils
    {
        string GetFilePath(IListVideoInfo video, string folderPath, string format, bool replaceStricted);
    }

    class LocalVideoUtils : ILocalVideoUtils
    {
        public LocalVideoUtils(INiconicoUtils niconicoUtils)
        {
            this.niconicoUtils = niconicoUtils;
        }

        private readonly INiconicoUtils niconicoUtils;

        /// <summary>
        /// ローカルファイルの取得を試行する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public string GetFilePath(IListVideoInfo video, string folderPath, string format, bool replaceStricted)
        {
            if (!Path.IsPathRooted(folderPath))
            {
                folderPath = Path.Combine(AppContext.BaseDirectory, folderPath);
            }
            var fn = this.niconicoUtils.GetFileName(format, video, FileFolder.Mp4FileExt, replaceStricted);
            var path = IOUtils.GetPrefixedPath(Path.Combine(folderPath, fn));

            //.mp4ファイルを確認
            if (File.Exists(path))
            {
                return path;
            }
            else
            //.tsファイルを確認
            {
                fn = this.niconicoUtils.GetFileName(format, video, FileFolder.TsFileExt, replaceStricted);
                path = IOUtils.GetPrefixedPath(Path.Combine(folderPath, fn));
                if (File.Exists(path)) return path;
            }

            return Path.GetDirectoryName(path) ?? folderPath;

        }

    }
}
