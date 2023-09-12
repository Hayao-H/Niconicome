using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3.Error
{
    public enum CommentClientError
    {
        [ErrorEnum(ErrorLevel.Error, "コメント取得でエラーが発生しました。")]
        SomeErrorOccured,
        [ErrorEnum(ErrorLevel.Error, "デフォルトスレッドを取得できませんでした。")]
        FailedToGetDefaultThread,
        [ErrorEnum(ErrorLevel.Error, "コメントサーバーへのリクエストに失敗しました。（url:{0}, status:{1}）")]
        FailedToFetch,
        [ErrorEnum(ErrorLevel.Error, "コメントサーバーへのリクエストに失敗しました。（url:{0}")]
        ExceptionOccuredWhileFetch,
        [ErrorEnum(ErrorLevel.Error, "コメントの解析に失敗しました。")]
        FailedToDeserializeData,
        [ErrorEnum(ErrorLevel.Error, "ダウンロード処理がキャンセルされました。")]
        DownloadCanceled,
    }
}
