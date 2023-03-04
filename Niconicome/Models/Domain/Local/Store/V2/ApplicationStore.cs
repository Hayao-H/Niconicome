using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface IApplicationStore
    {
        /// <summary>
        /// データベースのバージョンを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<Version> GetDBVersion();

        /// <summary>
        /// データベースのバージョンを定義
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        IAttemptResult SetDBVersion(Version version);
    }
}
