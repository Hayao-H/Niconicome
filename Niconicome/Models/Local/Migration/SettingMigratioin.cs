using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niconicome.Extensions;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Local.Migration
{
    public interface ISettingMigratioin
    {
        /// <summary>
        /// 設定ファイルの移行が必要かどうかを確認する
        /// </summary>
        bool IsMigrationNeeded { get; }

        /// <summary>
        /// 設定ファイルを移行する
        /// </summary>
        /// <returns>移行に失敗した設定のリスト</returns>
        IAttemptResult<IReadOnlyList<string>> Migrate();
    }
    public class SettingMigratioin : ISettingMigratioin
    {
        public SettingMigratioin(ISettingsStore store, ISettingHandler settingHandler)
        {
            this._store = store;
            this._settingHandler = settingHandler;
        }

        #region field

        private readonly ISettingsStore _store;

        private readonly ISettingHandler _settingHandler;

        #endregion

        #region Props

        public bool IsMigrationNeeded => this._store.IsEmpty;

        #endregion

        #region Method

        public IAttemptResult<IReadOnlyList<string>> Migrate()
        {
            //{設定名：プロパティー名}
            Dictionary<string, string> oldSettingNames = typeof(STypes::SettingNames).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(string)).ToDictionary(p => p.GetValue(p)!.As<string>(), p => p.Name);

            //{プロパティー名：設定名}
            Dictionary<string, string> newSettingNames = typeof(SettingNames).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(string)).ToDictionary(p => p.Name, p => p.GetValue(p)!.As<string>());

            var failed = new List<string>();

            //bool
            IAttemptResult<IEnumerable<ISettingData<bool>>> boolResult = this._settingHandler.GetAllBoolSetting();
            if (!boolResult.IsSucceeded || boolResult.Data is null)
            {
                return AttemptResult<IReadOnlyList<string>>.Fail(boolResult.Message);
            }

            foreach (var boolSetting in boolResult.Data)
            {
                if (!oldSettingNames.TryGetValue(boolSetting.SettingName, out string? propName))
                {
                    failed.Add(boolSetting.SettingName);
                    continue;
                }
                if (!newSettingNames.TryGetValue(propName, out string? settingName))
                {
                    failed.Add(boolSetting.SettingName);
                    continue;
                }

                this._store.SetSetting(settingName, boolSetting.Data);

            }

            //int
            IAttemptResult<IEnumerable<ISettingData<int>>> intResult = this._settingHandler.GetAllIntSetting();
            if (!intResult.IsSucceeded || intResult.Data is null)
            {
                return AttemptResult<IReadOnlyList<string>>.Fail(intResult.Message);
            }

            foreach (var intSetting in intResult.Data)
            {
                if (!oldSettingNames.TryGetValue(intSetting.SettingName, out string? propName))
                {
                    failed.Add(intSetting.SettingName);
                    continue;
                }
                if (!newSettingNames.TryGetValue(propName, out string? settingName))
                {
                    failed.Add(intSetting.SettingName);
                    continue;
                }

                this._store.SetSetting(settingName, intSetting.Data);
            }

            //string
            IAttemptResult<IEnumerable<ISettingData<string>>> stringResult = this._settingHandler.GetAllStringSetting();
            if (!stringResult.IsSucceeded || stringResult.Data is null)
            {
                return AttemptResult<IReadOnlyList<string>>.Fail(stringResult.Message);
            }

            foreach (var stringSetting in stringResult.Data)
            {
                if (!oldSettingNames.TryGetValue(stringSetting.SettingName, out string? propName))
                {
                    failed.Add(stringSetting.SettingName);
                    continue;
                }
                if (!newSettingNames.TryGetValue(propName, out string? settingName))
                {
                    failed.Add(stringSetting.SettingName);
                    continue;
                }

                this._store.SetSetting(settingName, stringSetting.Data);
            }

            var setting = this._store.GetSetting<VideoInfoTypeSettings>(SettingNames.VideoInfoType);

            return AttemptResult<IReadOnlyList<string>>.Succeeded(failed.AsReadOnly());
        }

        #endregion

    }
}
