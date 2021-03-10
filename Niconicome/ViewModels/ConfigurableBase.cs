using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels
{
    class ConfigurableBase : BindableBase
    {
        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiels"></param>
        /// <param name="data"></param>
        /// <param name="settingname"></param>
        /// <param name="propertyname"></param>
        protected void Savesetting<T>(ref T fiels, T data, Settings setting, [CallerMemberName] string? propertyname = null)
        {
            if (data is bool boolData)
            {
                WS::Mainpage.SettingHandler.SaveSetting(boolData, setting);
            }
            else if (data is string stringData)
            {
                WS::Mainpage.SettingHandler.SaveSetting(stringData, setting);
            }
            else if (data is int intData)
            {
                WS::Mainpage.SettingHandler.SaveSetting(intData, setting);
            }
            else
            {
                return;
            }

            fiels = data;
            this.OnPropertyChanged(propertyname);

        }
    }
}
