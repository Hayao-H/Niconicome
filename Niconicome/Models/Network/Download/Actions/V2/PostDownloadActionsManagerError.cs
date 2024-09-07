using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Network.Download.Actions.V2
{
    public enum PostDownloadActionsManagerError
    {
        [ErrorEnum(ErrorLevel.Log, "コンピューターをシャットダウンします。")]
        ShutDown,
        [ErrorEnum(ErrorLevel.Log, "コンピューターを再起動します。")]
        Restart,
        [ErrorEnum(ErrorLevel.Log, "Windowsからログオフします。")]
        LogOff,
        [ErrorEnum(ErrorLevel.Log, "コンピューターを休止状態に移行します。")]
        Sleep,
    }
}
