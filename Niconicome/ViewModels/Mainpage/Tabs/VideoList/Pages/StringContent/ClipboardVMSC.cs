using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent
{
    public enum ClipboardVMSC
    {
        [StringEnum("クリップボードの監視を開始します。")]
        StartMonitoring,
        [StringEnum("クリップボードの監視を停止します。")]
        StopMonitoring,
        [StringEnum("クリップボードの監視に失敗しました。")]
        MonitoringFailed,
    }
}
