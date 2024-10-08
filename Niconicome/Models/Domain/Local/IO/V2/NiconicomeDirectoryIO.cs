﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.IO.V2
{
    public interface INiconicomeDirectoryIO
    {
        /// <summary>
        /// ディレクトリを作成
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult CreateDirectory(string path);

        /// <summary>
        /// path配下のディレクトリを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns>ディレクトリの完全パス一覧</returns>
        IAttemptResult<IEnumerable<string>> GetDirectories(string path);

        /// <summary>
        /// path配下のファイルを取得
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        IAttemptResult<IEnumerable<string>> GetFiles(string path, string searchPattern = "*");

        /// <summary>
        /// ディレクトリを削除
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        IAttemptResult Delete(string path, bool recursive = true, bool recycle = false);

        /// <summary>
        /// ディレクトリを移動
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        IAttemptResult Move(string source, string destination, bool overwrite = true);

        /// <summary>
        /// ディレクトリの存在を確認
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Exists(string path);
    }
}
