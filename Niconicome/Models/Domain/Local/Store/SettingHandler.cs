using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Niconicome.Extensions;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Network;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Local.Store
{

    public enum SettingType
    {
        boolSetting,
        stringSetting,
        intSetting,
    }

    public interface ISettingData<T>
    {
        T Data { get; }
        string SettingName { get; }
    }

    public interface ISettingHandler
    {
        IAttemptResult<IEnumerable<ISettingData<bool>>> GetAllBoolSetting();
        IAttemptResult<IEnumerable<ISettingData<string>>> GetAllStringSetting();
        IAttemptResult<IEnumerable<ISettingData<int>>> GetAllIntSetting();

        ISettingData<bool> GetBoolSetting(string settingName);
        ISettingData<string> GetStringSetting(string settingName);
        ISettingData<int> GetIntSetting(string settingName);
        bool Exists(string settingName, SettingType settingType);
        void SaveBoolSetting(string settingName, bool data);
        void SaveStringSetting(string settingName, string data);
        void SaveIntSetting(string settingName, int data);
    }

    public class SettingHandler : ISettingHandler
    {
        public SettingHandler(IDataBase database,ILiteDBHandler db)
        {
            this.database = database;
            this._database = db;
        }

        private readonly IDataBase database;

        private readonly ILiteDBHandler _database;

        public IAttemptResult<IEnumerable<ISettingData<bool>>> GetAllBoolSetting()
        {
            IAttemptResult<IReadOnlyList<STypes::AppSettingBool>> result = this._database.GetAllRecords<STypes::AppSettingBool>(STypes::AppSettingBool.TableNameS);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<ISettingData<bool>>>.Fail(result.Message);
            }

            return AttemptResult<IEnumerable<ISettingData<bool>>>.Succeeded(result.Data.Where(r => r.SettingName is not null).Select(r => new SettingData<bool>(r.Value, r.SettingName!)));
        }

        public IAttemptResult<IEnumerable<ISettingData<string>>> GetAllStringSetting()
        {
            IAttemptResult<IReadOnlyList<STypes::AppSettingString>> result = this._database.GetAllRecords<STypes::AppSettingString>(STypes::AppSettingString.TableNameS);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<ISettingData<string>>>.Fail(result.Message);
            }

            return AttemptResult<IEnumerable<ISettingData<string>>>.Succeeded(result.Data.Where(r => r.SettingName is not null && r.Value is not null).Select(r => new SettingData<string>(r.Value ?? string.Empty, r.SettingName!)));
        }

        public IAttemptResult<IEnumerable<ISettingData<int>>> GetAllIntSetting()
        {
            IAttemptResult<IReadOnlyList<STypes::AppSettingInt>> result = this._database.GetAllRecords<STypes::AppSettingInt>(STypes::AppSettingInt.TableNameS);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<ISettingData<int>>>.Fail(result.Message);
            }

            return AttemptResult<IEnumerable<ISettingData<int>>>.Succeeded(result.Data.Where(r => r.SettingName is not null).Select(r => new SettingData<int>(r.Value, r.SettingName!)));
        }


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

            return new SettingData<bool>(this.database.GetRecord<STypes::AppSettingBool>(tableName, s => s.SettingName == settingName).Data!.Value, settingName);
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

            return new SettingData<string>(this.database.GetRecord<STypes::AppSettingString>(tableName, s => s.SettingName == settingName).Data!.Value ?? string.Empty, settingName);
        }

        /// <summary>
        /// 整数値設定を取得
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public ISettingData<int> GetIntSetting(string settingName)
        {
            string tableName = this.GetTableName(SettingType.intSetting);
            if (!this.Exists(settingName, SettingType.intSetting))
            {
                throw new InvalidOperationException($"設定名{settingName}はテーブル{tableName}に存在しません。");
            }

            return new SettingData<int>(this.database.GetRecord<STypes::AppSettingInt>(tableName, s => s.SettingName == settingName).Data!.Value, settingName);
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
            string tableName = STypes::AppSettingBool.TableNameS;
            var data = new STypes::AppSettingBool() { Value = value, SettingName = settingName };

            if (this.Exists(settingName, SettingType.boolSetting))
            {
                var setting = this.database.GetRecord<STypes::AppSettingBool>(tableName, s => s.SettingName == settingName);
                data.Id = setting.Data!.Id;
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
            string tableName = STypes::AppSettingString.TableNameS;
            var data = new STypes::AppSettingString() { Value = value, SettingName = settingName };

            if (this.Exists(settingName, SettingType.stringSetting))
            {
                var setting = this.database.GetRecord<STypes::AppSettingString>(tableName, s => s.SettingName == settingName);
                data.Id = setting.Data!.Id;
                this.database.Update(data, tableName);
            }
            else
            {
                this.database.Store(data, tableName);
            }
        }

        /// <summary>
        /// 整数値の設定を保存する
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="data"></param>
        public void SaveIntSetting(string settingName, int value)
        {
            string tableName = this.GetTableName(SettingType.intSetting);
            var data = new STypes::AppSettingInt() { Value = value, SettingName = settingName };

            if (this.Exists(settingName, SettingType.intSetting))
            {
                var setting = this.database.GetRecord<STypes::AppSettingInt>(tableName, s => s.SettingName == settingName);
                data.Id = setting.Data!.Id;
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
                SettingType.boolSetting => STypes::AppSettingBool.TableNameS,
                SettingType.stringSetting => STypes::AppSettingString.TableNameS,
                SettingType.intSetting => STypes::AppSettingInt.TableNameS,
                _ => STypes::AppSettingString.TableNameS,
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
