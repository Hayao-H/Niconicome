using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Auth;
using Niconicome.Models.Local;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class GeneralSettingsPageViewModel : SettingaBase
    {
        public GeneralSettingsPageViewModel()
        {
            this.isAutologinEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.AutologinEnable);

            var normal = new AutoLoginTypes(AutoLoginTypeString.Normal, "パスワードログイン");
            var wv2 = new AutoLoginTypes(AutoLoginTypeString.Webview2, "Webview2とCookieを共有");

            this.SelectableAutoLoginTypes = new List<AutoLoginTypes>() { normal, wv2 };

            var type = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AutologinMode);
            this.selectedAutoLoginTypeField = type switch
            {
                AutoLoginTypeString.Normal => normal,
                AutoLoginTypeString.Webview2 => wv2,
                _ => normal
            };
        }

        private bool isAutologinEnableField;

        private string empty = string.Empty;

        private AutoLoginTypes selectedAutoLoginTypeField;

        /// <summary>
        /// 選択可能な自動ログインの種別
        /// </summary>
        public List<AutoLoginTypes> SelectableAutoLoginTypes { get; init; }

        /// <summary>
        /// 自動ログインの種別
        /// </summary>
        public AutoLoginTypes SelectedAutoLoginType
        {
            get => this.selectedAutoLoginTypeField; set
            {
                this.Savesetting(ref this.empty, value.Value, Settings.AutologinMode);
                this.SetProperty(ref this.selectedAutoLoginTypeField, value);
            }
        }

        /// <summary>
        /// 自動ログイン
        /// </summary>
        public bool IsAutologinEnable { get => this.isAutologinEnableField; set => this.Savesetting(ref this.isAutologinEnableField, value, Settings.AutologinEnable); }
    }

    class GeneralSettingsPageViewModelD
    {
        public bool IsAutologinEnable { set; get; } = true;

        /// <summary>
        /// 選択可能な自動ログインの種別
        /// </summary>
        public List<AutoLoginTypes> SelectableAutoLoginTypes { get; init; } = new();

        /// <summary>
        /// 自動ログインの種別
        /// </summary>
        public AutoLoginTypes SelectedAutoLoginType { get; init; } = new AutoLoginTypes(AutoLoginTypeString.Normal, "パスワード");

    }

    class AutoLoginTypes
    {
        public AutoLoginTypes(string value, string displayvalue)
        {
            this.Value = value;
            this.DisplayValue = displayvalue;
        }

        public string Value { get; init; }

        public string DisplayValue { get; init; }


    }
}
