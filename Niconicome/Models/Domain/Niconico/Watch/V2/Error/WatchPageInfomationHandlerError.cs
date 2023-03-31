using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Watch.V2.Error
{
    public enum WatchPageInfomationHandlerError
    {
        [ErrorEnum(ErrorLevel.Log,"視聴ページへのアクセスを開始しました。")]
        RetrievingWatchPageHasStarted,
        [ErrorEnum(ErrorLevel.Log,"視聴ページへのアクセスが完了しました。")]
        RetrievingWatchPageHasCompleted,
        [ErrorEnum(ErrorLevel.Error,"視聴ページへのアクセスに失敗しました。(url:{0}, status:{1})")]
        FailedToRetrieveWatchPage,
        [ErrorEnum(ErrorLevel.Error, "ページ解析プラグイン、またはそれに相当するアドオンがインストールされていないか、初期化が完了していません。")]
        PluginNotFount,
    }
}
