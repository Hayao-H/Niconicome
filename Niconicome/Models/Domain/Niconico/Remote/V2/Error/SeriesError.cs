using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Error
{
    public enum SeriesError
    {
        [ErrorEnum(ErrorLevel.Log,"シリーズページの取得を開始しました。(id:{0})")]
        RetrievingHasStarted,
        [ErrorEnum(ErrorLevel.Log, "シリーズページの取得が完了しました。(id:{0})")]
        RetrievingHasCompleted,
        [ErrorEnum(ErrorLevel.Error,"シリーズページの取得に失敗しました。(URL:{0}, status:{1})")]
        FailedToRetrieveData,
        [ErrorEnum(ErrorLevel.Error, "シリーズページの解析に失敗しました。")]
        FailedToParseDocument,
        [ErrorEnum(ErrorLevel.Error, "シリーズページの解析に失敗しました。(id:{0})")]
        FailedToParseDocumentWithException,
    }
}
