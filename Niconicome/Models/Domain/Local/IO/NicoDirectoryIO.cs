using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Niconicome.Extensions.System;

namespace Niconicome.Models.Domain.Local.IO
{
    public interface INicoDirectoryIO
    {
        bool Exists(string path);
        void Create(string path);
        void Delete(string path, bool recurse = true);
        void DeleteAll(Predicate<string> predicate, bool recurse = true);

        /// <summary>
        /// ファイルを移動する
        /// </summary>
        /// <param name="source">移動元フォルダーパス</param>
        /// <param name="destination">移動先フォルダー名</param>
        void Move(string source, string destination);

        /// <summary>
        /// すべてのファイルを移動する
        /// </summary>
        /// <param name="sourceDir">移動元ディレクトリ</param>
        /// <param name="targetDir">移動先ディレクトリ</param>
        void MoveAllFiles(string sourceDir, string targetDir);

        /// <summary>
        /// すべてのファイルを移動する
        /// </summary>
        /// <param name="sourceDir">移動元ディレクトリ</param>
        /// <param name="targetDir">移動先ディレクトリ</param>
        /// <param name="excludePattern">除外するファイルを表す正規表現</param>
        void MoveAllFiles(string sourceDir, string targetDir, string excludePattern);


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
            this.MoveAllFiles(sourceDir, targetDir, string.Empty);
        }

        public void MoveAllFiles(string sourceDir, string targetDir, string excludePattern)
        {

            List<string> files = this.GetFiles(sourceDir, recurse: true);

            foreach (var file in files)
            {
                if (!excludePattern.IsNullOrEmpty() && Regex.IsMatch(file, excludePattern))
                {
                    continue;
                }

                string targetPath = Path.Combine(targetDir, file.Replace($"{sourceDir}\\", ""));
                if (!Path.IsPathRooted(targetPath))
                {
                    targetPath = Path.Combine(AppContext.BaseDirectory, targetPath);
                }

                var targetFile = new FileInfo(targetPath);
                if (targetFile.Directory is not null && !targetFile.Directory.Exists)
                {
                    targetFile.Directory.Create();
                }

                File.Move(file, targetFile.FullName, true);
            }
        }


        public void Move(string source, string destination)
        {
            Directory.Move(source, destination);
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
