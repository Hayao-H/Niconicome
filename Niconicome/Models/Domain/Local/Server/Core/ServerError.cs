using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public enum ServerError
    {
        [ErrorEnum(ErrorLevel.Log,"ローカルサーバーを起動しました。")]
        ServerStarted,
        [ErrorEnum(ErrorLevel.Error, "サーバーの実行中に例外が発生しました。")]
        ServerStoppedWithException,
        [ErrorEnum(ErrorLevel.Log,"ローカルサーバーをシャットダウンしました。")]
        ServerStopped,
    }
}
