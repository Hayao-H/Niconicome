using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public interface IReplaceHandler
    {
        /// <summary>
        /// 置き換えルール
        /// </summary>
        ReadOnlyObservableCollection<IReplaceRule> ReplaceRules { get; }

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
        /// 変換
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<IReplaceRule> Convert(IEnumerable<string> source);
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
            this.ReplaceRules = new ReadOnlyObservableCollection<IReplaceRule>(this._replaceRules);
            this.Refresh();
        }

        #region private

        private readonly ISettingsContainer _settingsContainer;

        private readonly IErrorHandler _errorHandler;

        private readonly ObservableCollection<IReplaceRule> _replaceRules = new();

        #endregion

        #region Props

        public ReadOnlyObservableCollection<IReplaceRule> ReplaceRules { get; init; }

        #endregion

        #region Method



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

            this.Refresh();

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
                this.Refresh();
                return AttemptResult.Succeeded();
            }
            else
            {
                this._errorHandler.HandleError(Err.DataNotExist, replaceFrom, replaceTo);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.DataNotExist, replaceFrom, replaceTo));
            }
        }

        public IEnumerable<IReplaceRule> Convert(IEnumerable<string> source)
        {
            return source.Select(s =>
            {
                string[] splited = s.Split("to");

                var to = splited.Length == 1 ? string.Empty : splited[1];

                return new ReplaceRule(splited[0], to);
            });
        }


        #endregion

        private void Refresh()
        {
            IAttemptResult<List<string>> result = this._settingsContainer.GetOnlyValue(SettingNames.ReplaceRules, Format.DefaultReplaceRules);
            if (!result.IsSucceeded || result.Data is null)
            {
                return;
            }

            IEnumerable<IReplaceRule> data = this.Convert(result.Data);

            this._replaceRules.Clear();
            this._replaceRules.AddRange(data);
        }
    }

    public record ReplaceRule(string ReplaceFrom, string ReplaceTo) : IReplaceRule;
}
