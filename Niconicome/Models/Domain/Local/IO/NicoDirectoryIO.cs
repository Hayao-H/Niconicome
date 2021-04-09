using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.IO
{
    public interface INicoDirectoryIO
    {
        bool Exists(string path);
        void Create(string path);
        void Delete(string path, bool recurse = true);

    }

    class NicoDirectoryIO : INicoDirectoryIO
    {
        /// <summary>
        /// ディレクトリの存在をチェックする
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// ディレクトリを作成する
        /// </summary>
        /// <param name="path"></param>
        public void Create(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// ディレクトリを削除する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recurse"></param>
        public void Delete(string path, bool recurse = true)
        {
            Directory.Delete(path, recurse);
        }
    }
}
