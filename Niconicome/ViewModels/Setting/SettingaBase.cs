using System;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions.System.Diagnostics;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels.Mainpage.Utils;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting
{
    /// <summary>
    /// 設定の基底クラス
    /// </summary>
    class SettingaBase : BindableBase
    {
        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiels"></param>
        /// <param name="data"></param>
        /// <param name="settingname"></param>
        /// <param name="propertyname"></param>
        protected void Savesetting<T>(ref T fiels, T data, SettingsEnum setting, [CallerMemberName] string? propertyname = null)
        {
            if (data is bool boolData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(boolData, setting);
            }
            else if (data is string stringData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(stringData, setting);
            }
            else if (data is int intData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(intData, setting);
            }
            else
            {
                return;
            }

            fiels = data;
            this.OnPropertyChanged(propertyname);

        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiels"></param>
        /// <param name="data"></param>
        /// <param name="settingname"></param>
        /// <param name="propertyname"></param>
        protected void Savesetting<T>(T data, SettingsEnum setting, [CallerMemberName] string? propertyname = null)
        {
            if (data is bool boolData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(boolData, setting);
            }
            else if (data is string stringData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(stringData, setting);
            }
            else if (data is int intData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(intData, setting);
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiels"></param>
        /// <param name="data"></param>
        /// <param name="settingname"></param>
        /// <param name="propertyname"></param>
        protected void Savesetting<T>(ref ComboboxItem<T> field, ComboboxItem<T> data, SettingsEnum setting, [CallerMemberName] string? propertyname = null)
        {
            if (data.Value is bool boolData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(boolData, setting);
            }
            else if (data.Value is string stringData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(stringData, setting);
            }
            else if (data.Value is int intData)
            {
                WS::SettingPage.SettingHandler.SaveSetting(intData, setting);
            }
            else
            {
                return;
            }

            field = data;

            this.OnPropertyChanged(propertyname);

        }

        /// <summary>
        /// 列挙型設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="propertyname"></param>
        protected void SaveEnumSetting<T>(ref ComboboxItem<T> field, ComboboxItem<T> data, [CallerMemberName] string? propertyname = null) where T : Enum
        {
            WS::SettingPage.EnumSettingsHandler.SaveSetting(data.Value);

            field = data;

            this.OnPropertyChanged(propertyname);
        }

        /// <summary>
        /// 列挙型設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="propertyname"></param>
        protected void SaveEnumSetting<T>(ComboboxItem<T> data, [CallerMemberName] string? propertyname = null) where T : Enum
        {
            WS::SettingPage.EnumSettingsHandler.SaveSetting(data.Value);
        }
    }

    /// <summary>
    /// ハイパーリンクを機能させる
    /// </summary>
    class HyperlinkBehavior : Behavior<Hyperlink>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.RequestNavigate += this.OnRequest;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RequestNavigate -= this.OnRequest;
        }

        private void OnRequest(object? sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            ProcessEx.StartWithShell(e.Uri.AbsoluteUri);
        }
    }
}
