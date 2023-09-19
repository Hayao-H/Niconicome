using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Niconicome.Models.Local.State;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Mainpage.Subwindows.DownloadTask
{
    public class ToolbarViewModel
    {
        public ToolbarViewModel(NavigationManager navigation)
        {
            WS.BlazorPageManager.RegisterNavigationManager(BlazorWindows.DLTask, navigation);
        }

        #region Method

        public void Navigate(PageType page)
        {
            WS.BlazorPageManager.RequestBlazorToNavigate(page switch
            {
                PageType.Stage => "/downloadtask/stage",
                _ => "/downloadtask/download",
            },BlazorWindows.DLTask);
        }

        #endregion
    }

    public enum PageType
    {
        Stage,
        Download
    }
}
