using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Local
{


    public interface ILocalSettingHandler
    {
        bool GetBoolSetting(Settings setting);
        string? GetStringSetting(Settings setting);
        void SaveSetting<T>(T data, Settings setting);
    }

    public class LocalSettingHandler : ILocalSettingHandler
    {
        public LocalSettingHandler(ISettingHandler settingHandler)
        {
            this.settingHandler = settingHandler;
        }

        private readonly ISettingHandler settingHandler;

        /// <summary>
        /// 文字列の設定を取得する
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public string? GetStringSetting(Settings setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return null;

            if (this.settingHandler.Exists(settingname, SettingType.stringSetting))
            {
                return this.settingHandler.GetStringSetting(settingname).Data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 真偽値の設定を取得する
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool GetBoolSetting(Settings setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return false;

            if (this.settingHandler.Exists(settingname, SettingType.boolSetting))
            {
                return this.settingHandler.GetBoolSetting(settingname).Data;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="setting"></param>
        public void SaveSetting<T>(T data, Settings setting)
        {
            var settingname = this.GetSettingName(setting);
            if (settingname is null) return;

            if (data is bool boolData)
            {
                this.settingHandler.SaveBoolSetting(settingname, boolData);
            }
            else if (data is string stringData)
            {
                this.settingHandler.SaveStringSetting(settingname, stringData);
            }
        }

        /// <summary>
        /// 設定名を取得する
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private string? GetSettingName(Settings settings)
        {
            return settings switch
            {
            Settings.FileNameFormat => STypes::SettingNames.FileNameFormat,
            Settings.PlayerAPath => STypes::SettingNames.PlayerAPath,
            Settings.PlayerBPath => STypes::SettingNames.PlayerBPath,
            Settings.AppUrlPath => STypes::SettingNames.AppUrlPath,
            Settings.AppUrlParam => STypes::SettingNames.AppUrlParam,
            Settings.AppIdPath => STypes::SettingNames.AppIdPath,
            Settings.AppIdParam => STypes::SettingNames.AppIdParam,
            Settings.FfmpegPath => STypes::SettingNames.FFmpegPath,
            Settings.DefaultFolder=>STypes::SettingNames.DefaultFolder,
                _ => null
            };
        }
    }

    public enum Settings
    {
        FileNameFormat,
        PlayerAPath,
        PlayerBPath,
        AppUrlPath,
        AppIdPath,
        AppUrlParam,
        AppIdParam,
        FfmpegPath,
        DefaultFolder
    }
}
