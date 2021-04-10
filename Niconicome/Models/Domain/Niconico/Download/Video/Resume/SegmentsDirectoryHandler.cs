using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.IO;

namespace Niconicome.Models.Domain.Niconico.Download.Video.Resume
{

    public interface ISegmentsDirectoryHandler
    {
        ISegmentsDirectoryInfo GetSegmentsDirectoryInfo(string niconicoID);
        ISegmentsDirectoryInfo GetSegmentsDirectoryInfoWithDirectoryPath(string path);
    }

    public class SegmentsDirectoryHandler : ISegmentsDirectoryHandler
    {
        public SegmentsDirectoryHandler(INicoDirectoryIO directoryIO)
        {
            this.directoryIO = directoryIO;
        }

        /// <summary>
        /// ディレクトリを操作する
        /// </summary>
        private readonly INicoDirectoryIO directoryIO;

        /// <summary>
        /// セグメントファイルの情報を取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        public ISegmentsDirectoryInfo GetSegmentsDirectoryInfo(string niconicoID)
        {
            var dirs = this.directoryIO.GetDirectorys(@"tmp\", $"{niconicoID}-*");

            if (dirs.Count == 0) throw new IOException($"{niconicoID}のセグメントファイルは保存されていません。");

            var dir = dirs.First();

            var info = this.GetSegmentsDirectoryInfoInternal(dir);
            return info;
        }

        /// <summary>
        /// ディレクトリのパスから取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ISegmentsDirectoryInfo GetSegmentsDirectoryInfoWithDirectoryPath(string path)
        {
            return this.GetSegmentsDirectoryInfoInternal(path);
        }


        #region private

        /// <summary>
        /// セグメントディレクトリの情報を取得する
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private ISegmentsDirectoryInfo GetSegmentsDirectoryInfoInternal(string dirPath)
        {
            if (!this.directoryIO.Exists(dirPath)) throw new IOException($"指定したディレクトリは存在しません。({dirPath})");


            var dirName = Path.GetFileName(dirPath);

            if (string.IsNullOrEmpty(dirName)) throw new IOException($"ディレクトリ名を取得できませんでした。({dirName})");

            var splitedDirName = dirName.Split("-");

            if (splitedDirName.Length < 5) throw new InvalidOperationException($"セグメントファイルディレクトリ名が不正です({dirName})");

            var parseResult = uint.TryParse(splitedDirName[1], out uint resolution);
            if (!parseResult) throw new InvalidOperationException($"セグメントファイルディレクトリ名が不正です({dirName})");

            var dt = DateTime.ParseExact(string.Join("-", splitedDirName[2..5]), "yyyy-MM-dd", CultureInfo.InvariantCulture);


            var files = this.directoryIO.GetFiles(dirPath, "*.ts").Select(p => Path.GetFileName(p) ?? string.Empty).Where(p => !p.IsNullOrEmpty() && p != "combined.ts");

            var info = new SegmntsDirectoryInfo(splitedDirName[0], resolution, dirName);
            info.ExistsFileNames.AddRange(files);
            info.StartedOn = dt;

            return info;
        }
        #endregion

    }
}
