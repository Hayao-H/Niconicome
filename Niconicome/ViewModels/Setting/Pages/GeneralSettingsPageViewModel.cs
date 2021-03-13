using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class GeneralSettingsPageViewModel:SettingaBase
    {
        public GeneralSettingsPageViewModel()
        {
            this.isAutologinEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.AutologinEnable);
        }

        private bool isAutologinEnableField;

        /// <summary>
        /// 自動ログイン
        /// </summary>
        public bool IsAutologinEnable { get => this.isAutologinEnableField; set => this.Savesetting(ref this.isAutologinEnableField, value, Settings.AutologinEnable); }
    }

    class GeneralSettingsPageViewModelD
    {
        public bool IsAutologinEnable { set; get; } = true;

    }
}
