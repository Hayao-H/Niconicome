using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent
{
    public enum IndexViewModelStringContent
    {
        [StringEnum($"ブラウザでの視聴に失敗しました。")]
        FailedToOpenVideoInBrowser,
        [StringEnum("ブラウザで視聴を開始しました。")]
        VideoOpenedInBrowser,
    }
}
