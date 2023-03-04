using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Error
{
    public enum ChannelError
    {
        [ErrorEnum(ErrorLevel.Log,"チャンネル情報の取得を開始しました。(id:{0})")]
        ChannnelInfomationRetrievingHasStarted,
        [ErrorEnum(ErrorLevel.Log, "チャンネル情報の取得が完了しました。(id:{0})")]
        ChannnelInfomationRetrievingHasCompleted,
        [ErrorEnum(ErrorLevel.Error, "ページの取得に失敗しました。(url:{0}, status:{1})")]
        FailedToRetrievingPage,
        [ErrorEnum(ErrorLevel.Error, "ページの解析に失敗しました。")]
        FailedToAnalysis,
        [ErrorEnum(ErrorLevel.Error, "wrapper要素を発見できませんでした。")]
        FailedToAnalysisForNoWrapperElement,
    }
}
