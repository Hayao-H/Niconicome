using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;

namespace Niconicome.Models.Domain.Niconico.Download.Video.Resume
{

    public interface ISegmentsDirectoryHandler
    {
        ISegmentsDirectoryInfo GetSegmentsDirectoryInfo(string niconicoID);
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
            var dirs = this.directoryIO.GetDirectorys(@"\tmp\", $"{niconicoID}-*");

            if (dirs.Count == 0) throw new IOException($"{niconicoID}のセグメントファイルは保存されていません。");

            var dir = dirs.First();
            var splitedDirName = dir.Split("-");

            if (splitedDirName.Length < 3) throw new InvalidOperationException($"セグメントファイルディレクトリ名が不正です({dir})");

            var parseResult = uint.TryParse(splitedDirName[1], out uint resolution);
            if (!parseResult) throw new InvalidOperationException($"セグメントファイルディレクトリ名が不正です({dir})");


            var files = this.directoryIO.GetFiles(dir, "*.ts");
            files.Remove("combined.ts");

            var info = new SegmntsDirectoryInfo(niconicoID, resolution, dir);
            info.ExistsFileNames.AddRange(files);

            return info;
        }

    }
}
