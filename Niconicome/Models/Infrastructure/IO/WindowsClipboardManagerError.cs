using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.IO
{
    public enum WindowsClipboardManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "クリップボードへの書き込みに失敗しました。")]
        FailedToSetDataToClipboard,
        [ErrorEnum(ErrorLevel.Error, "クリップボードの取得に失敗しました。")]
        FailedToGetClipboardData,
        [ErrorEnum(ErrorLevel.Error, "すでにクリップボードを監視しています。")]
        AlreadyMonitoring,
        [ErrorEnum(ErrorLevel.Error, "この機能は'Windows10 1507'以降のOSでのみ利用可能です。")]
        OSVersionOlderThanMinimum,
        [ErrorEnum(ErrorLevel.Error, "クリップボード監視の開始に失敗しました。")]
        FailedToStartMonitoringClipboard,
        [ErrorEnum(ErrorLevel.Error, "クリップボード監視の終了に失敗しました。")]
        FailedToStopMonitoringClipboard,
    }
}
