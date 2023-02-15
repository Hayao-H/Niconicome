using System;
using System.IO;
using System.Text;
using System.Windows.Automation;
using System.Windows.Input;
using Niconicome.Models.Domain.Utils;
using Windows.Foundation;

namespace Niconicome.Models.Domain.Local.IO
{
    public interface INicoFileIO
    {
        bool Exists(string path);
        void Delete(string path);
        void Move(string path, string destPath, bool overwrite = false);
        string OpenRead(string path);

        /// <summary>
        /// テキストファイルに文字列を追記する
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="text">追記する文字列</param>
        void AppendText(string path, string text);

        /// <summary>
        /// 文字列を書き込む
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="append"></param>
        void Write(string path, string content, bool append = false, Encoding? encoding = null);
    }

    public class NicoFileIO : INicoFileIO
    {
        public NicoFileIO(INicoDirectoryIO directoryIO)
        {
            this.directoryIO = directoryIO;
        }

        #region field

        private readonly INicoDirectoryIO directoryIO;

        #endregion

        /// <summary>
        /// ファイルの存在をチェックする
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            return File.Exists(IOUtils.GetPrefixedPath(path));
        }

        /// <summary>
        /// ファイルを削除する
        /// </summary>
        /// <param name="path"></param>
        public void Delete(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// ファイルを開いて読み込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string OpenRead(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fs);
            return reader.ReadToEnd();
        }

        public void Move(string path, string destPath, bool overwrite = false)
        {
            File.Move(path, destPath, overwrite);
        }

        public void Write(string path, string content, bool append = false, Encoding? encoding = null)
        {
            string? dirPath = Path.GetDirectoryName(path);
            if (dirPath is not null)
            {
                bool exists = this.directoryIO.Exists(dirPath);
                if (!exists)
                {
                    this.directoryIO.Create(dirPath);
                }
            }

            using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fs, encoding ?? Encoding.UTF8);
            writer.Write(content);
        }

        public void AppendText(string path, string text)
        {
            File.AppendAllText(path, text);
        }

    }
}
