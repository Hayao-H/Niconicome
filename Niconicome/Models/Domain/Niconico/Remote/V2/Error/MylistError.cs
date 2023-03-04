using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Error
{
    public enum MylistError
    {
        [ErrorEnum(ErrorLevel.Error, "マイリストデータの取得に失敗しました。(url:{0} status_code:{1})")]
        FailedToRetrieveData,
        [ErrorEnum(ErrorLevel.Error, "マイリストデータの解析に失敗しました。(id:{0}, page:{1})")]
        DataAnalysisFailed,
        [ErrorEnum(ErrorLevel.Log, "マイリストの取得を開始しました。(id:{0})")]
        FetchStarted,
        [ErrorEnum(ErrorLevel.Log,"API(v1)へのアクセスを試行します。(url:{0})")]
        AccessToAPIV1,
        [ErrorEnum(ErrorLevel.Log, "API(v2)へのアクセスを試行します。(url:{0})")]
        AccessToAPIV2,
        [ErrorEnum(ErrorLevel.Log, "{0}件の動画をマイリスト(id:{1})から取得しました。")]
        FetchCompleted,
    }
}
