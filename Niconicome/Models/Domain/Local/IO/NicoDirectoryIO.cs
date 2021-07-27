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
        void DeleteAll(Predicate<string> predicate, bool recurse = true);

        /// <summary>
        /// すべてのファイルを移動する
        /// </summary>
        /// <param name="sourceDir">移動元ディレクトリ</param>
        /// <param name="targetDir">移動先ディレクトリ</param>
        void MoveAllFiles(string sourceDir, string targetDir);
        List<string> GetFiles(string path, string pattern = "*", bool recurse = false);
        List<string> GetDirectorys(string path, string pattern = "*", bool recurse = false);

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

        /// <summary>
        /// 指定した条件でディレクトリを削除する
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="recurse"></param>
        public void DeleteAll(Predicate<string> predicate, bool recurse = true)
        {
            var dirs = this.GetDirectorys(@"/").Where(p => predicate(p));

            foreach (var directory in dirs)
            {
                this.Delete(directory, recurse);
            }
        }

        public void MoveAllFiles(string sourceDir, string targetDir)
        {
            List<string> files = this.GetFiles(sourceDir, recurse: true);

            foreach (var file in files)
            {
                var targetPath = new FileInfo(Path.Combine(targetDir, file));
                if (targetPath.Directory is not null && !targetPath.Directory.Exists)
                {
                    targetPath.Directory.Create();
                }

                File.Move(file, targetPath.FullName, true);
            }
        }


        /// <summary>
        /// ディレクトリ内のファイル一覧を取得する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public List<string> GetFiles(string path, string pattern = "*", bool recurse = false)
        {
            var option = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(path, pattern, option);

            return new List<string>(files);
        }

        /// <summary>
        /// ディレクトリの一覧を取得する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public List<string> GetDirectorys(string path, string pattern = "*", bool recurse = false)
        {
            var option = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetDirectories(path, pattern, option);

            return new List<string>(files);
        }


    }
}
