using System.IO;
using System.Windows.Automation;

namespace Niconicome.Models.Domain.Local.IO
{
    public interface INicoFileIO
    {
        bool Exists(string path);
        void Delete(string path);
        string OpenRead(string path);
        void Write(string path, string content, bool append = false);
    }

    public class NicoFileIO : INicoFileIO
    {

        /// <summary>
        /// ファイルの存在をチェックする
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            return File.Exists(path);
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
            using var fs = new StreamReader(path);
            return fs.ReadToEnd();
        }

        /// <summary>
        /// 文字列を書き込む
        /// </summary>
        /// <param name="content"></param>
        /// <param name="append"></param>
        public void Write(string path, string content, bool append = false)
        {
            using var fs = new StreamWriter(path, append);
            fs.Write(content);
        }
    }
}
