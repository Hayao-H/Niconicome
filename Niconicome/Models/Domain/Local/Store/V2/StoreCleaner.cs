using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface IStoreCleaner
    {
        /// <summary>
        /// 不要なプレイリストを削除
        /// </summary>
        /// <returns></returns>
        IAttemptResult CleanPlaylists();

        /// <summary>
        /// 不要な動画を削除
        /// </summary>
        /// <returns></returns>
        IAttemptResult CleanVideos();
    }
}
