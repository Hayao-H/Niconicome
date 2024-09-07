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
    public class SideMenuViewModel
    {
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
                SettingPages.Style => "/settings/style",
                SettingPages.Debug => "/settings/debug",
                SettingPages.AppInfo => "/settings/appinfo",
                _ => "/settings"
            };

            WS.BlazorPageManager.RequestBlazorToNavigate(page);
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
        Style,
        Debug,
        AppInfo,
    }

}
