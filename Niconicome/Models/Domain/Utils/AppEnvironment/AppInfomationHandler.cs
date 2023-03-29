using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Utils.AppEnvironment
{
    public interface IAppInfomationHandler
    {
        /// <summary>
        /// アプリケーションバージョンを取得
        /// </summary>
        Version ApplicationVersion { get; }

        /// <summary>
        /// 64bitプロセスであるかどうかを確認
        /// </summary>
        bool Is64BitProcess { get; }

    }
}
