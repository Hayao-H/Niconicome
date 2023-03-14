using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent
{
    public enum SearchVMSC
    {
        [StringEnum("入力文字列が空です。")]
        InputIsEmpty,
        [StringEnum("検索失敗しました。(詳細:{0})")]
        FailedToSearch,
        [StringEnum("検索結果が空です。")]
        ListIsEmpty,
    }
}
