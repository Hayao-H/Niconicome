using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.General.ReplaceHandlerError;

namespace Niconicome.Models.Domain.Niconico.Download.General
{
    interface IReplaceHandler
    {
        /// <summary>
        /// 置き換えルールを追加
        /// </summary>
        /// <param name="replaceFrom"></param>
        /// <param name="replaceTo"></param>
        /// <returns></returns>
        IAttemptResult AddRule(string replaceFrom, string replaceTo);

        /// <summary>
        /// 置き換えルールを削除
        /// </summary>
        /// <param name="replaceFrom"></param>
        /// <param name="replaceTo"></param>
        /// <returns></returns>
        IAttemptResult RemoveRule(string replaceFrom, string replaceTo);

        /// <summary>
        /// 全ての置き換えルールを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<IReplaceRule>> GetRules();
    }

    public interface IReplaceRule
    {
        /// <summary>
        /// 置き換え対象の文字列
        /// </summary>
        string ReplaceFrom { get; }

        /// <summary>
        /// 置き換え先の文字列
        /// </summary>
        string ReplaceTo { get; }
    }


    public class ReplaceHandler : IReplaceHandler
    {
        public ReplaceHandler(ISettingsContainer settingsContainer, IErrorHandler errorHandler)
        {
            this._settingsContainer = settingsContainer;
            this._errorHandler = errorHandler;
        }

        #region private

        private readonly ISettingsContainer _settingsContainer;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<IEnumerable<IReplaceRule>> GetRules()
        {
            IAttemptResult<List<string>> result = this._settingsContainer.GetOnlyValue(SettingNames.ReplaceRules, Format.DefaultReplaceRules);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<IReplaceRule>>.Fail(result.Message);
            }

            IEnumerable<IReplaceRule> data = result.Data.Select(s =>
            {
                string[] splited = s.Split("to");
                return new ReplaceRule(splited[0], splited[1]);
            });

            return AttemptResult<IEnumerable<IReplaceRule>>.Succeeded(data);
        }

        public IAttemptResult AddRule(string replaceFrom, string replaceTo)
        {
            IAttemptResult<ISettingInfo<List<string>>> result = this._settingsContainer.GetSetting(SettingNames.ReplaceRules, Format.DefaultReplaceRules);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<IReplaceRule>>.Fail(result.Message);
            }

            var data = result.Data.Value;
            data.Add($"{replaceFrom}to{replaceTo}");

            result.Data.Value = data;

            return AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveRule(string replaceFrom, string replaceTo)
        {
            IAttemptResult<ISettingInfo<List<string>>> result = this._settingsContainer.GetSetting(SettingNames.ReplaceRules, Format.DefaultReplaceRules);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<IReplaceRule>>.Fail(result.Message);
            }

            var data = result.Data.Value;

            var target = $"{replaceFrom}to{replaceTo}";

            if (data.Contains(target))
            {
                data.Remove(target);
                result.Data.Value = data;
                return AttemptResult.Succeeded();
            }
            else
            {
                this._errorHandler.HandleError(Err.DataNotExist, replaceFrom, replaceTo);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.DataNotExist, replaceFrom, replaceTo));
            }
        }

        #endregion
    }

    public record ReplaceRule(string ReplaceFrom, string ReplaceTo) : IReplaceRule;
}
