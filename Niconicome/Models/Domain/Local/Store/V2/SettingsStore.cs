using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface ISettingsStore
    {
        /// <summary>
        /// 設定を取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        IAttemptResult<ISettingInfo<T>> GetSetting<T>(string name) where T : notnull;

        /// <summary>
        /// 設定を変更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IAttemptResult SetSetting<T>(ISettingInfo<T> setting) where T : notnull;

        /// <summary>
        /// 設定名を指定して設定を変更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IAttemptResult SetSetting<T>(string name, T value) where T : notnull;


        /// <summary>
        /// 設定ファイルを再読み込みしてキャッシュを更新する
        /// </summary>
        /// <returns></returns>
        IAttemptResult Flush();

        /// <summary>
        /// 設定情報が空であるかどうかを返す
        /// </summary>
        bool IsEmpty { get; }

    }
}
