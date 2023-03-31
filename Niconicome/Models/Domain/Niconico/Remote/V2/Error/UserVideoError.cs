using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Error
{
    public enum UserVideoError
    {
        [ErrorEnum(ErrorLevel.Error, "投稿動画一覧の取得に失敗しました。(url:{0}, status:{1})")]
        FailedToRetrieveData,
        [ErrorEnum(ErrorLevel.Error, "投稿動画一覧の解析に失敗しました。(id:{0}, page:{1})")]
        FailedToAnalysis,
    }
}
