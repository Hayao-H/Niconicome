using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Network.Download.DLTask.StringContent
{
    public enum DownloadTaskStringContent
    {
        [StringEnum("DLをキャンセル")]
        TaskCancelled,
        [StringEnum("待機中...")]
        IsWaiting,
        [StringEnum("キャンセルされています。")]
        AlreadyCancelled,
        [StringEnum("ダウンロードされています。")]
        AlreadyDownloaded,
        [StringEnum("初期化されていません")]
        NotInitialized,
        [StringEnum("{0}のダウンロード処理を開始しました。")]
        DownloadStarted,
        [StringEnum("{0}のダウンロード処理に失敗しました。")]
        DownloadFailed,
        [StringEnum("詳細：{0}")]
        DownloadFailedDetailed,
        [StringEnum("DL失敗")]
        DownloadFailedMessage,
        [StringEnum("{0}のダウンロードに成功しました。")]
        DownloadSucceeded,
        [StringEnum("ダウンロード完了{0}")]
        DownloadCOmpletedMessage,
        [StringEnum("{0}コメ")]
        CommentCount,
        [StringEnum("vertical:{0}px")]
        VerticalResolution,
    }
}
