using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Integrate
{
    public enum VideoDownloaderSC
    {
        [StringEnum("視聴セッションを確立中...")]
        EnsureSession,
        [StringEnum("ストリーム情報を取得中...")]
        FetchingStreamInfo,
        [StringEnum("DLをレジューム")]
        Resume,
        [StringEnum("完了: {0}/{1} {2}px")]
        SegmentDownloadCompleted,
        [StringEnum("エンコード中...")]
        Encode,
        [StringEnum("動画のダウンロードが完了")]
        Completed,
        [StringEnum("エコノミー設定によりスキップ")]
        SkipEconomy,
    }
}
