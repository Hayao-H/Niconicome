using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.BottomPanel.StringContent
{
    public enum TimerVMSC
    {
        [StringEnum("有効")]
        TimerEnable,
        [StringEnum("無効")]
        TimerDisable,
        [StringEnum("タイマーを「{0}({1}分後)」に設定しました。（設定：{2}）")]
        TimerWasSet,
        [StringEnum("タイマーを解除しました。")]
        TimerWasUnset,
    }
}
