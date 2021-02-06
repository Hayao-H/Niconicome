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
        void Write(byte[] data, IDownloadTask task);
        string FolderName { get; }
        string FolderNameAbs { get; }
        IEnumerable<string> FilesPath { get; }
        IEnumerable<string> FilesPathAbs { get; }
    }

    class SegmentWriter : ISegmentWriter
    {

        public SegmentWriter()
        {
            this.FolderName = Path.Combine("tmp", Guid.NewGuid().ToString("D"));
            if (!Directory.Exists(this.FolderName))
            {
                Directory.CreateDirectory(this.FolderName);
            }
        }

        /// <summary>
        /// ファイル名のリスト
        /// </summary>
        private readonly List<string> innerFileNames = new();

        /// <summary>
        /// 書き込む
        /// </summary>
        /// <param name="data"></param>
        /// <param name="task"></param>
        public void Write(byte[] data, IDownloadTask task)
        {
            this.innerFileNames.Add(task.FileName);
            this.innerFileNames.Sort();
            string targetPath = Path.Combine(this.FolderName, task.FileName);
            using var fs = File.Create(targetPath);
            fs.Write(data);
        }

        /// <summary>
        /// フォルダー名
        /// </summary>
        public string FolderName { get; init; }

        /// <summary>
        /// フォルダー名(絶対)
        /// </summary>
        public string FolderNameAbs
        {
            get => Path.Combine(AppContext.BaseDirectory, this.FolderName);
        }

        /// <summary>
        /// ファイル名のリスト
        /// </summary>
        public IEnumerable<string> FilesPath => this.innerFileNames.Select(p => Path.Combine(this.FolderName, p));

        /// <summary>
        /// ファイル名のリスト(絶対)
        /// </summary>
        public IEnumerable<string> FilesPathAbs => this.innerFileNames.OrderBy(p=>int.Parse(p.Substring(0,p.IndexOf('.')))).Select(p => Path.Combine(this.FolderNameAbs, p));
    }
}
