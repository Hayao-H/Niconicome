using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface IVideoFileStore
    {
        /// <summary>
        /// ファイルパスを取得
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        IAttemptResult<string> GetFilePath(string niconicoID, uint verticalResolution);

        /// <summary>
        /// ファイルを追加
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="filePath"></param>
        /// <param name="verticalResolution"></param>
        /// <returns></returns>
        IAttemptResult AddFile(string niconicoID, string filePath);

        /// <summary>
        /// 指定したディレクトリ内に存在するファイルを追加
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        IAttemptResult<int> AddFilesFromDirectoryList(IEnumerable<string> directories);

        /// <summary>
        /// ファイルの存在を確認
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="verticalResolution"></param>
        /// <returns></returns>
        bool Exist(string niconicoID, uint verticalResolution);
    }
}
