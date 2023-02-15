using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent
{
    public enum OutputViewModelStringContent
    {
        [StringEnum("クリップボードへの書き込みに成功しました。")]
        SettingOutputToClipboardSucceeded,
        [StringEnum("クリップボードへの書き込みに失敗しました。")]
        SettingOutputToClipboardFailed,
        [StringEnum("選択した動画は未ダウンロードです。")]
        VideoIsNotDownloaded,
        [StringEnum("外部プレイヤーの起動に失敗しました。")]
        FailedToOpenInPlayer,
        [StringEnum("外部アプリの起動に失敗しました。")]
        FailedToLaunchApp,
    }
}
