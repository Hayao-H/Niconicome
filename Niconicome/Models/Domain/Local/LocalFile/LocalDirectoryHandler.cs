using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Local.LocalFile
{
    interface ILocalDirectoryHandler
    {
        IEnumerable<string> GetVideoIdsFromDirectory(string directoryPath, bool searchSubfolder = true);
    }

    class LocalDirectoryHandler : ILocalDirectoryHandler
    {
        public LocalDirectoryHandler(ILogger logger, INiconicoUtils niconicoUtils)
        {
            this.logger = logger;
            this.niconicoUtils = niconicoUtils;
        }

        private readonly ILogger logger;

        private readonly INiconicoUtils niconicoUtils;

        /// <summary>
        /// ローカルディレクトリーからID一覧を取得する
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="searchSubfolder"></param>
        /// <returns></returns>
        public IEnumerable<string> GetVideoIdsFromDirectory(string directoryPath, bool searchSubfolder = true)
        {
            var option = searchSubfolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = new List<string>();

            try
            {
                files.AddRange(Directory.GetFiles(directoryPath, "*.mp4", option));
            }
            catch (Exception e)
            {
                this.logger.Error("ローカルファイルの探索に失敗しました。", e);
                throw new IOException("ローカルファイルの探索に失敗しました。");
            }

            return files.Select(f => this.niconicoUtils.GetIdFromFIleName(f)).Where(f => !f.IsNullOrEmpty());

        }
    }
}
