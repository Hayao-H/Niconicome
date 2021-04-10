using System.IO;

namespace Niconicome.Models.Domain.Local.IO
{
    public interface INicoFileIO
    {
        bool Exists(string path);
        void Delete(string path);
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
    }
}
