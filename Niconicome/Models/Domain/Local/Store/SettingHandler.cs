using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions;
using Niconicome.Models.Network;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Local.Store
{

    public enum SettingType
    {
        boolSetting,
        stringSetting,
    }

    public interface ISettingData<T>
    {
        T Data { get; }
        string SettingName { get; }
    }

    public interface ISettingHandler
    {
        ISettingData<bool> GetBoolSetting(string settingName);
        ISettingData<string> GetStringSetting(string settingName);
        bool Exists(string settingName, SettingType settingType);
        void SaveBoolSetting(string settingName, bool data);
        void SaveStringSetting(string settingName, string data);
    }

   public class SettingHandler : ISettingHandler
    {
        public SettingHandler(IDataBase database)
        {
            this.database = database;
        }

        private readonly IDataBase database;

        /// <summary>
        /// 真偽値の設定を取得する
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public ISettingData<bool> GetBoolSetting(string settingName)
        {
            string tableName = this.GetTableName(SettingType.boolSetting);
            if (!this.Exists(settingName, SettingType.boolSetting))
            {
                throw new InvalidOperationException($"設定名{settingName}はテーブル{tableName}に存在しません。");
            }

            return new SettingData<bool>(this.database.GetRecord<STypes::AppSettingBool>(tableName, s => s.SettingName == settingName).Value, settingName);
        }

        /// <summary>
        /// 文字列の設定を取得する
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public ISettingData<string> GetStringSetting(string settingName)
        {
            string tableName = this.GetTableName(SettingType.stringSetting);
            if (!this.Exists(settingName, SettingType.stringSetting))
            {
                throw new InvalidOperationException($"設定名{settingName}はテーブル{tableName}に存在しません。");
            }

            return new SettingData<string>(this.database.GetRecord<STypes::AppSettingString>(tableName, s => s.SettingName == settingName).Value ?? string.Empty, settingName);
        }

        /// <summary>
        /// 設定の存在を確かめる
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="settingType"></param>
        /// <returns></returns>
        public bool Exists(string settingName, SettingType settingType)
        {
            string tableName = this.GetTableName(settingType);
            return settingType switch
            {
                SettingType.boolSetting => this.database.Exists<STypes::AppSettingBool>(tableName, s => s.SettingName == settingName),
                SettingType.stringSetting => this.database.Exists<STypes::AppSettingString>(tableName, s => s.SettingName == settingName),
                _ => this.database.Exists<STypes::AppSettingString>(tableName, s => s.SettingName == settingName),
            };
        }

        /// <summary>
        /// 真偽値の設定を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        public void SaveBoolSetting(string settingName, bool value)
        {
            string tableName = STypes::AppSettingBool.TableName;
            var data = new STypes::AppSettingBool() { Value = value, SettingName = settingName };

            if (this.Exists(settingName, SettingType.boolSetting))
            {
                var setting = this.database.GetRecord<STypes::AppSettingBool>(tableName, s => s.SettingName == settingName);
                data.Id = setting.Id;
                this.database.Update(data, tableName);
            }
            else
            {
                this.database.Store(data, tableName);
            }
        }

        /// <summary>
        /// 文字列の設定を保存する
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        public void SaveStringSetting(string settingName, string value)
        {
            string tableName = STypes::AppSettingString.TableName;
            var data = new STypes::AppSettingString() { Value = value, SettingName = settingName };

            if (this.Exists(settingName, SettingType.stringSetting))
            {
                var setting = this.database.GetRecord<STypes::AppSettingString>(tableName, s => s.SettingName == settingName);
                data.Id = setting.Id;
                this.database.Update(data, tableName);
            }
            else
            {
                this.database.Store(data, tableName);
            }
        }

        /// <summary>
        /// テーブル名を取得する
        /// </summary>
        /// <param name="settingType"></param>
        /// <returns></returns>
        private string GetTableName(SettingType settingType)
        {
            return settingType switch
            {
                SettingType.boolSetting => STypes::AppSettingBool.TableName,
                SettingType.stringSetting => STypes::AppSettingString.TableName,
                _ => STypes::AppSettingString.TableName,
            };
        }
    }

    /// <summary>
    /// 設定情報
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SettingData<T> : ISettingData<T>
    {
        public SettingData(T data, string settingName)
        {
            this.Data = data;
            this.SettingName = settingName;
        }

        public T Data { get; init; }

        public string SettingName { get; init; }

    }
}
