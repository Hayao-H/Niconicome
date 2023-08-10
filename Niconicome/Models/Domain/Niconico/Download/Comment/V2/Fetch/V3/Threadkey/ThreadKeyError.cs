using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3.Threadkey
{
    public enum ThreadKeyError
    {
        [ErrorEnum(ErrorLevel.Error,"APIへのアクセスに失敗しました。(url：{0}, status：{1})")]
        FailedToGetData,
        [ErrorEnum(ErrorLevel.Error,"データの解析に失敗しました。(詳細：{0})")]
        FailedToLoadData,
        [ErrorEnum(ErrorLevel.Log,"スレッドキーを取得しました。（id:{0}, key:{1}）")]
        GetThreadKey,
    }
}
