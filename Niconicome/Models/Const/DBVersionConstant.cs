using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Const
{
    /// <summary>
    /// 上から新->旧
    /// </summary>
    public static class DBVersionConstant
    {
        /// <summary>
        /// 現在のDBバージョン
        /// </summary>
        public static Version CurrentDBVersion = new("1.0.0");

        /// <summary>
        /// 動画・プレイリストをV2に移行
        /// </summary>
        public static Version VideosAndPlaylistMigrated= new("1.0.0");
    }
}
