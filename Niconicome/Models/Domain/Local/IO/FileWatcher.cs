using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.IO
{
    public interface IFileWatcher
    {
        void UnWatch();
        void Watch(string filePath, NotifyFilters filters, Action<FileSystemEventArgs> handler);
    }

    public class FileWatcher : IFileWatcher
    {
        #region field

        private FileSystemWatcher? watcher;

        #endregion

        /// <summary>
        /// 監視する
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filters"></param>
        /// <param name="handler"></param>
        public void Watch(string filePath, NotifyFilters filters, Action<FileSystemEventArgs> handler)
        {
            if (this.watcher is not null) throw new InvalidOperationException("すでに監視しています。");

            string dirPath = Path.GetDirectoryName(filePath)!;
            string fileName = Path.GetFileName(filePath)!;

            this.watcher = new FileSystemWatcher(dirPath);
            this.watcher.NotifyFilter = filters;
            this.watcher.Filter = fileName;
            this.watcher.Changed += (_, e) => handler(e);
            this.watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 監視を停止する
        /// </summary>
        public void UnWatch()
        {
            if (this.watcher is null) return;
            this.watcher.EnableRaisingEvents = false;
            this.watcher.Dispose();
            this.watcher = null;
        }
    }
}
