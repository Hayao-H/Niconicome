using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Error
{
    public enum SearchError
    {
        [ErrorEnum(ErrorLevel.Error, "スナップショット検索APIへのアクセスに失敗しました。(url:{0}, status:{1})")]
        FailedToRetrievingData,
        [ErrorEnum(ErrorLevel.Error, "検索結果の解析に失敗しました。")]
        FailedToAnalysisOfData,
    }
}
