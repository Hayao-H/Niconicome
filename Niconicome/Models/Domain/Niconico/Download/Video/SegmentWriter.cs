using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video
{

    public interface ISegmentWriter
    {
        void Write(byte[] data, IParallelDownloadTask task, string segmentDirectoryName);
        string FolderNameAbs { get; }
        IEnumerable<string> FilesPath { get; }
        IEnumerable<string> FilesPathAbs { get; }
    }

    class SegmentWriter : ISegmentWriter
    {

        /// <summary>
        /// ファイル名のリスト
        /// </summary>
        private readonly List<string> innerFileNames = new();

        /// <summary>
        /// 書き込む
        /// </summary>
        /// <param name="data"></param>
        /// <param name="task"></param>
        public void Write(byte[] data, IParallelDownloadTask task, string segmentDirectoryName)
        {
            this.folderName = Path.Combine("tmp", segmentDirectoryName);

            if (!Directory.Exists(this.folderName))
            {
                Directory.CreateDirectory(this.folderName);
            }

            this.innerFileNames.Add(task.FileName);
            this.innerFileNames.Sort();
            string targetPath = Path.Combine(this.folderName, task.FileName);
            using var fs = File.Create(targetPath);
            fs.Write(data);
        }

        /// <summary>
        /// フォルダー名
        /// </summary>
        private string? folderName;

        /// <summary>
        /// 完全なフォルダー名
        /// </summary>
        public string FolderNameAbs { get => Path.Combine(AppContext.BaseDirectory, this.folderName ?? string.Empty); }


        /// <summary>
        /// ファイル名のリスト
        /// </summary>
        public IEnumerable<string> FilesPath => this.innerFileNames.Select(p => Path.Combine(this.folderName ?? "tmp", p));

        /// <summary>
        /// ファイル名のリスト(絶対)
        /// </summary>
        public IEnumerable<string> FilesPathAbs => this.innerFileNames.OrderBy(p => int.Parse(p.Substring(0, p.IndexOf('.')))).Select(p => Path.Combine(this.FolderNameAbs, p));
    }
}
