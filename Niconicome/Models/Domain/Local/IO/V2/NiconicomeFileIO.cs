﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.IO.V2
{
    public interface INiconicomeFileIO
    {
        /// <summary>
        /// 動画ファイルの垂直方向の解像度を取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult<int> GetVerticalResolution(string path);

        /// <summary>
        /// ファイルに書き込む
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        IAttemptResult Write(string path, string content, Encoding? encoding = null);

        /// <summary>
        /// ファイルが存在するかどうかを確認する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Exist(string path);
    }
}
