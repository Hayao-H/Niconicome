using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Niconicome.Models.Local.State;
using WS = Niconicome.Workspaces.SettingPageV2;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class SideMenuViewModel : IDisposable
    {
        public SideMenuViewModel(NavigationManager naviation)
        {
            WS.BlazorPageManager.RegisterNavigationManager(BlazorWindows.Settings, naviation);
        }

        public void OnLinkCkick(SettingPages pages)
        {
            string page = pages switch
            {
                SettingPages.Import => "/settings/import",
                SettingPages.Restore => "/settings/restore",
                SettingPages.General => "/settings/general",
                SettingPages.File => "/settings/file",
                SettingPages.Download => "/settings/download",
                SettingPages.ExternalSoftware => "/settings/external",
                SettingPages.VideoList => "/settings/videolist",
                _ => "/settings"
            };

            WS.BlazorPageManager.RequestBlazorToNavigate(page, BlazorWindows.Settings);
        }

        public void Dispose()
        {
            WS.BlazorPageManager.UnRegisterNavigationManager(BlazorWindows.Settings);
        }
    }

    public enum SettingPages
    {
        Import,
        Restore,
        General,
        File,
        Download,
        ExternalSoftware,
        VideoList,
    }

}
