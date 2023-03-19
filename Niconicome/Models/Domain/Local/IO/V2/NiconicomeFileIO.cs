using System;
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
        Task<IAttemptResult<int>> GetVerticalResolutionAsync(string path);

        /// <summary>
        /// ファイルに書き込む
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        IAttemptResult Write(string path, string content, Encoding? encoding = null);

        /// <summary>
        /// ファイルを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult<string> Read(string path);

        /// <summary>
        /// ファイルを削除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult Delete(string path);

        /// <summary>
        /// ファイルをコピー
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IAttemptResult Copy(string source, string target);

        /// <summary>
        /// ファイルを列挙してアクションを実行
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="enumAction"></param>
        /// <param name="searchSubDirectory"></param>
        void EnumerateFiles(string path, string searchPattern, Action<string> enumAction, bool searchSubDirectory);

        /// <summary>
        /// ファイルを列挙してアクションを実行
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="enumAction"></param>
        /// <param name="searchSubDirectory"></param>
        Task EnumerateFilesAsync(string path, string searchPattern, Func<string, Task> enumAction, bool searchSubDirectory);

        /// <summary>
        /// ファイルが存在するかどうかを確認する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Exists(string path);
    }
}
