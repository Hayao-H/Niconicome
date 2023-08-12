using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;

namespace NiconicomeTest.Stabs.Models.Domain.Local.Settings
{
    internal class SettingsContainerStub : ISettingsContainer
    {
        public Dictionary<string, object> Settings { get; init; } = new();

        public IAttemptResult<ISettingInfo<T>> GetSetting<T>(string settingName, T defaultValue) where T : notnull
        {
            if (this.Settings.ContainsKey(settingName))
            {
                var value = this.Settings[settingName];
                if (value is T typedValue)
                {
                    return AttemptResult<ISettingInfo<T>>.Succeeded(new SettingInfoStub<T>(settingName, typedValue));
                }
                else
                {
                    return AttemptResult<ISettingInfo<T>>.Succeeded(new SettingInfoStub<T>(settingName, defaultValue));
                }
            }
            else
            {
                this.Settings.Add(settingName, defaultValue);
                return AttemptResult<ISettingInfo<T>>.Succeeded(new SettingInfoStub<T>(settingName, defaultValue));
            }
        }

        public IAttemptResult<T> GetOnlyValue<T>(string settingName, T defaultValue) where T : notnull
        {
            if (this.Settings.ContainsKey(settingName))
            {
                var value = this.Settings[settingName];
                if (value is T typedValue)
                {
                    return AttemptResult<T>.Succeeded(typedValue);
                }
                else
                {
                    return AttemptResult<T>.Succeeded(defaultValue);
                }
            }
            else
            {
                this.Settings.Add(settingName, defaultValue);
                return AttemptResult<T>.Succeeded(defaultValue);
            }

        }

        public IAttemptResult ClearSettings()
        {
            return AttemptResult.Succeeded();
        }
    }
}
