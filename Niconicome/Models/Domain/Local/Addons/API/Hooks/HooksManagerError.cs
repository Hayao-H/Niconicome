using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Addons.API.Hooks
{
    public enum HooksManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "視聴ページ解析関数が登録されていません。")]
        PageAnalyzeFunctionNotRegistered,
        [ErrorEnum(ErrorLevel.Error, "視聴ページ解析に失敗しました。")]
        FailedToAnalyzeWatchPage,
        [ErrorEnum(ErrorLevel.Error,"動画情報取得関数が登録されていません。")]
        VideoInfoFunctionNotRegistered,
        [ErrorEnum(ErrorLevel.Error,"動画情報の取得に失敗しました。")]
        FailedToFetchVideoInfo,
        [ErrorEnum(ErrorLevel.Error, "セッション確立関数が登録されていません。")]
        SessionFunctionNotRegistered,
        [ErrorEnum(ErrorLevel.Error, "セッションの確立に失敗しました。")]
        FailedToEnsureSession,
    }
}
