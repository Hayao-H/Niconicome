using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.Error
{
    public enum SegmentDownloaderError
    {
        [ErrorEnum(ErrorLevel.Error, "未初期化です。")]
        NotInitialized,
        [ErrorEnum(ErrorLevel.Error,"セグメントファイルのダウンロード処理がキャンセルされました。(id:{0})")]
        Canceled,
        [ErrorEnum(ErrorLevel.Error,"セグメントファイルのいずれかのダウンロードに失敗したため、処理を中止します。(id:{0})")]
        FailedInAny,
        [ErrorEnum(ErrorLevel.Error, "セグメント(idx:{0})の取得に失敗しました。(status:{1}, url:{2}, id:{3}")]
        FailedToFetch,
        [ErrorEnum(ErrorLevel.Log, "セグメント(idx:{0})を取得しました。(id:{1})")]
        SucceededToFetch,
    }
}
