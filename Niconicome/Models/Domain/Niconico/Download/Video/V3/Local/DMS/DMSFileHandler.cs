using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;
using System.IO;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.IO.V2;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.DMS
{
    public interface IDMSFileHandler
    {
        /// <summary>
        /// 動画ファイルの存在を確認
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        bool Exists(string id, string dirPath);

        /// <summary>
        /// エコノミーであるかどうかを確認
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        bool IsEconomy(string id, string dirPath);
    }

    public class DMSFileHandler : IDMSFileHandler
    {
        public DMSFileHandler(INiconicomeDirectoryIO directoryIO)
        {
            this._directoryIO = directoryIO;
        }

        private readonly INiconicomeDirectoryIO _directoryIO;

        public bool Exists(string id, string dirPath)
        {

            var videoDir = Path.Combine(dirPath, id, "video");
            var audioDir = Path.Combine(dirPath, id, "audio");

            if (!this._directoryIO.Exists(videoDir) || !this._directoryIO.Exists(audioDir)) return false;

            var videoSegments = this._directoryIO.GetDirectories(videoDir);
            var audioSegments = this._directoryIO.GetDirectories(audioDir);

            if (videoSegments.Data is null || audioSegments.Data is null) return false;

            return videoSegments.Data.Any() && audioSegments.Data.Any();
        }

        public bool IsEconomy(string id, string dirPath)
        {

            var videoDir = Path.Combine(dirPath, id, "video");

            var _360p = Path.Combine(videoDir, "360");
            var _144p = Path.Combine(videoDir, "144");


            if (this._directoryIO.Exists(_360p) || this._directoryIO.Exists(_144p))
            {
                var videoSegments = this._directoryIO.GetDirectories(videoDir);

                if (!videoSegments.IsSucceeded || videoSegments.Data is null) return false;

                return videoSegments.Data.Select(p => Path.GetFileName(p)).Where(p => p != "360" && p != "144").Any().Not();
            }
            else
            {
                return false;
            }
        }
    }

}
