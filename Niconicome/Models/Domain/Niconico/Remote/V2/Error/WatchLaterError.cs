using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Error
{
    public enum WatchLaterError
    {
        [ErrorEnum(ErrorLevel.Error, "「あとで見る」の取得に失敗しました。(url:{0} status_code:{1})")]
        FailedToRetrieveData,
        [ErrorEnum(ErrorLevel.Error, "「あとで見る」データの解析に失敗しました。(page:{1})")]
        DataAnalysisFailed,
        [ErrorEnum(ErrorLevel.Log, "「あとで見る」の取得を開始しました。")]
        FetchStarted,
        [ErrorEnum(ErrorLevel.Log, "APIへのアクセスを試行します。(url:{0})")]
        AccessToAPI,
        [ErrorEnum(ErrorLevel.Log, "{0}件の動画を「あとで見る」から取得しました。")]
        FetchCompleted,
    }
}
