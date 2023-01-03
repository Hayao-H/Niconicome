using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Const = Niconicome.Models.Const;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList
{
    public class MainVideoListViewModel : TabViewModelBase
    {
        public MainVideoListViewModel() : base("動画一覧", Const::LocalConstant.VideoListV2TabID)
        {

        }
    }
}
