using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno
{
    public interface IXenoImportResult
    {
        /// <summary>
        /// 動画数
        /// </summary>
        int SucceededVideosCount { get; }

        /// <summary>
        /// プレイリスト数
        /// </summary>
        int SucceededPlaylistsCount { get; }
    }

    public record XenoImportResult(int SucceededVideosCount, int SucceededPlaylistsCount) : IXenoImportResult;
}
