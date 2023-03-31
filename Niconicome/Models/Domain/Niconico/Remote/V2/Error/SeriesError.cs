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
        [ErrorEnum(ErrorLevel.Log,"シリーズの取得を開始しました。(id:{0})")]
        RetrievingHasStarted,
        [ErrorEnum(ErrorLevel.Log, "APIへのアクセスを試行します。(url:{0})")]
        AccessToAPI,
        [ErrorEnum(ErrorLevel.Error, "シリーズの取得に失敗しました。(URL:{0}, status:{1})")]
        FailedToRetrieveData,
        [ErrorEnum(ErrorLevel.Error, "シリーズのデータの解析に失敗しました。(id:{0},page:{1})")]
        DataAnalysisFailed,
        [ErrorEnum(ErrorLevel.Log, "{0}件の動画を「{1}」から取得しました。")]
        RetrievingHasCompleted,
    }
}
