using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Network.Download.DLTask.StringContent
{
    public enum DownloadManagerSC
    {
        [StringEnum("動画のダウンロードを開始します。({0}件)")]
        DownloadHasStarted,
        [StringEnum("ダウンロード中にエラーが発生しました")]
        Error,
        [StringEnum("ダウンロード中にエラーが発生しました（詳細：{0}）")]
        ErrorD,
        [StringEnum("動画を1件もダウンロード出来ませんでした。")]
        CannotDownloadAny,
        [StringEnum("{0}件の動画をダウンロードしました。")]
        DownloadedMany,
        [StringEnum("{0}件の動画のダウンロードに失敗しました。")]
        SomeDownloadHasFailed,
        [StringEnum("{0}をダウンロードしました。")]
        DownloadedOne,
    }
}
