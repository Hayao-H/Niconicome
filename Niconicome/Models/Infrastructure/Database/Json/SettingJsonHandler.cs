using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Infrastructure.Database.Json
{
    public class SettingJsonHandler : ISettingsStore
    {
        public SettingJsonHandler(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private bool _isInitialized;

        private bool _isEmpty;

        private readonly object _lockObj = new object();

        private Dictionary<string, object>? _cache;

        #endregion

        #region Method

        public IAttemptResult<ISettingInfo<T>> GetSetting<T>(string settingName) where T : notnull
        {
            IAttemptResult iResult = this.Initialize();
            if (!iResult.IsSucceeded) return AttemptResult<ISettingInfo<T>>.Fail(iResult.Message);

            IAttemptResult<Dictionary<string, object>> dResult = this.Read();
            if (!dResult.IsSucceeded || dResult.Data is null)
            {
                return AttemptResult<ISettingInfo<T>>.Fail(dResult.Message);
            }

            Dictionary<string, object> data = dResult.Data;

            if (!data.ContainsKey(settingName))
            {
                this._errorHandler.HandleError(SettingJSONError.SettingNotFound, settingName);
                return AttemptResult<ISettingInfo<T>>.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingNotFound, settingName));
            }

            dynamic value = data[settingName];

            if (value is int && typeof(T).IsEnum)
            {
                return AttemptResult<ISettingInfo<T>>.Succeeded(new SettingInfo<T>(settingName, (T)Enum.ToObject(typeof(T), value), this));
            }
            else if (value is T typedValue)
            {
                return AttemptResult<ISettingInfo<T>>.Succeeded(new SettingInfo<T>(settingName, typedValue, this));
            }
            else
            {
                this._errorHandler.HandleError(SettingJSONError.SettingTypeNotMatch, settingName, value.GetType().Name, typeof(T).Name);
                return AttemptResult<ISettingInfo<T>>.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingTypeNotMatch, settingName, value.GetType().Name, typeof(T).Name));
            }
        }

        public IAttemptResult SetSetting<T>(ISettingInfo<T> setting) where T : notnull
        {
            return this.SetSetting(setting.SettingName, setting.Value);
        }

        public IAttemptResult SetSetting<T>(string name, T value) where T : notnull
        {
            IAttemptResult iResult = this.Initialize();
            if (!iResult.IsSucceeded) return AttemptResult<T>.Fail(iResult.Message);

            IAttemptResult<Dictionary<string, object>> dResult = this.Read();
            if (!dResult.IsSucceeded || dResult.Data is null)
            {
                return AttemptResult<T>.Fail(dResult.Message);
            }

            Dictionary<string, object> data = dResult.Data;
            object settingValue;

            if (value.GetType().IsEnum)
            {
                settingValue = Convert.ToInt32(value);
            }
            else
            {
                settingValue = value;
            }

            if (data.ContainsKey(name))
            {
                data[name] = settingValue;
            }
            else
            {
                data.Add(name, settingValue);
            }

            return this.Update(data);
        }

        public bool Exists(string settingName)
        {
            IAttemptResult iResult = this.Initialize();
            if (!iResult.IsSucceeded) return false;

            IAttemptResult<Dictionary<string, object>> dResult = this.Read();
            if (!dResult.IsSucceeded || dResult.Data is null)
            {
                return false;
            }

            Dictionary<string, object> data = dResult.Data;

            return data.ContainsKey(settingName);
        }


        public IAttemptResult Flush()
        {
            this._cache = null;
            IAttemptResult<Dictionary<string, object>> result = this.Read();
            return result.IsSucceeded ? AttemptResult.Succeeded() : AttemptResult.Fail();
        }

        public IAttemptResult Clear()
        {
            IAttemptResult iResult = this.Initialize();
            if (!iResult.IsSucceeded)
            {
                return AttemptResult.Fail(iResult.Message);
            }

            IAttemptResult<Dictionary<string, object>> dResult = this.Read();
            if (!dResult.IsSucceeded || dResult.Data is null)
            {
                return AttemptResult.Fail(dResult.Message);
            }

            Dictionary<string, object> data = dResult.Data;

            IAttemptResult result = this.Update(data);
            if (result.IsSucceeded)
            {
                this._errorHandler.HandleError(SettingJSONError.SettingCleared);
            }

            return result;
        }



        #endregion

        #region Props

        public bool IsEmpty
        {
            get
            {
                if (!this.Initialize().IsSucceeded)
                {
                    return true;
                }

                return this._isEmpty;
            }
        }

        #endregion

        #region private

        /// <summary>
        /// 設定ファイルを初期化する
        /// </summary>
        /// <returns></returns>
        private IAttemptResult Initialize()
        {
            if (this._isInitialized) return AttemptResult.Succeeded();

            var fileInfo = new FileInfo(Path.Combine(AppContext.BaseDirectory, FileFolder.SettingJSONPath));

            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                this._isInitialized = true;
                return AttemptResult.Succeeded();
            }

            if (fileInfo.Directory is null)
            {
                this._errorHandler.HandleError(SettingJSONError.SettingDirectoryDetectionFailed);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingDirectoryDetectionFailed));
            }
            else if (!fileInfo.Directory.Exists)
            {
                try
                {
                    fileInfo.Directory.Create();
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(SettingJSONError.SettingDirectoryCreationFailed, ex);
                    return AttemptResult.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingDirectoryCreationFailed, ex));
                }
            }

            try
            {
                using var _ = fileInfo.Create();
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(SettingJSONError.SettingJsonCreationFailed, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingJsonCreationFailed, ex));
            }

            var initial = "{}";
            lock (this._lockObj)
            {
                try
                {
                    using FileStream fs = fileInfo.OpenWrite();
                    using var writer = new StreamWriter(fs);
                    writer.Write(initial);
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(SettingJSONError.SettingJsonInitializationFailed, ex);
                    return AttemptResult.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingJsonInitializationFailed, ex));
                }
            }

            this._isEmpty = true;
            this._isInitialized = true;

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 設定データを読み込む
        /// </summary>
        /// <returns></returns>
        private IAttemptResult<Dictionary<string, object>> Read(int retryCount = 0)
        {
            if (this._cache is not null) return AttemptResult<Dictionary<string, object>>.Succeeded(this._cache);

            string content;
            lock (this._lockObj)
            {
                try
                {
                    var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, FileFolder.SettingJSONPath));
                    content = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    if (retryCount < LocalConstant.MaxSettingsJsonRetry)
                    {
                        return this.Read(retryCount++);
                    }

                    this._errorHandler.HandleError(SettingJSONError.SettingJsonReadingFailed, ex);
                    return AttemptResult<Dictionary<string, object>>.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingJsonReadingFailed, ex));
                }
            }

            Dictionary<string, object> rawData;

            try
            {
                rawData = JsonParser.DeSerialize<Dictionary<string, object>>(content);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(SettingJSONError.SettingJsonDeserializationFailed, ex);
                return AttemptResult<Dictionary<string, object>>.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingJsonDeserializationFailed, ex));
            }

            var data = new Dictionary<string, object>();

            foreach (var x in rawData)
            {
                if (x.Value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Array)
                    {
                        var stringList = new List<string>();
                        foreach (JsonElement elm in element.EnumerateArray())
                        {
                            stringList.Add(element.ValueKind switch
                            {
                                JsonValueKind.String => element.GetString() ?? string.Empty,
                                _ => throw new NotSupportedException()
                            });
                        }


                        data.Add(x.Key, stringList);
                    } else
                    {
                        data.Add(x.Key, element.ValueKind switch
                        {
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Number => element.GetInt32(),
                            JsonValueKind.String => element.GetString() ?? string.Empty,
                            _ => throw new NotSupportedException()
                        });
                    }
                }
            }

            this._cache = data;
            return AttemptResult<Dictionary<string, object>>.Succeeded(data);
        }

        /// <summary>
        /// 設定データを更新する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IAttemptResult Update(Dictionary<string, object> data, int retryCount = 0)
        {
            data = data.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            string content;
            JsonSerializerOptions oprions = JsonParser.DefaultOption;
            oprions.WriteIndented = true;

            try
            {
                content = JsonParser.Serialize(data, oprions);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(SettingJSONError.SettingJsonSerializationFailed, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingJsonSerializationFailed, ex));
            }

            lock (this._lockObj)
            {
                try
                {
                    using var writer = new StreamWriter(Path.Combine(AppContext.BaseDirectory, FileFolder.SettingJSONPath), false);
                    writer.WriteLine(content);
                }
                catch (Exception ex)
                {
                    if (retryCount <= LocalConstant.MaxSettingsJsonRetry)
                    {
                        return this.Update(data, retryCount++);
                    }

                    this._errorHandler.HandleError(SettingJSONError.SettingJsonWritingFailed, ex);
                    return AttemptResult.Fail(this._errorHandler.GetMessageForResult(SettingJSONError.SettingJsonWritingFailed, ex));
                }
            }

            this._cache = data;

            return AttemptResult.Succeeded();
        }

        #endregion
    }
}
