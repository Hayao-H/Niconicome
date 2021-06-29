using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Utils
{
    public interface IPathOrganizer
    {
        string GetFIlePath(string format, IDmcInfo dmcInfo, string extension, string folderName, bool replaceStricted, bool overWrite, string? suffix = null);
    }

    public class PathOrganizer : IPathOrganizer
    {
        public PathOrganizer(INiconicoUtils niconicoUtils)
        {
            this.niconicoUtils = niconicoUtils;
        }

        #region DI
        private readonly INiconicoUtils niconicoUtils;
        #endregion

        public string GetFIlePath(string format, IDmcInfo dmcInfo, string extension, string folderName, bool replaceStricted, bool overWrite, string? suffix = null)
        {
            var filename = this.niconicoUtils.GetFileName(format, dmcInfo, extension, replaceStricted, suffix);
            var filePath = Path.Combine(folderName, filename);

            if (!overWrite)
            {
                filePath = IOUtils.CheclFileExistsAndReturnNewFilename(filePath);
            }

            return filePath;

        }
    }
}
