using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.Utils;
using WS = Niconicome.Workspaces.SettingPageV2;
using Const = Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Download.General;
using System.Reflection.Emit;
using System.Collections.ObjectModel;
using Niconicome.Models.Const;
using Niconicome.Models.Helper.Result;
using SC = Niconicome.ViewModels.Setting.V2.Page.StringContent.FileVMSC;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.ViewModels.Shared;
using Niconicome.Extensions.System.List;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class FileViewModel : AlertViewModel
    {
        public FileViewModel()
        {
            this.DefaultFolder = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.DefaultFolder, Const::FileFolder.DefaultDownloadDir), Const::FileFolder.DefaultDownloadDir).AddTo(this.Bindables);
            this.FileFormat = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.FileNameFormat, string.Empty), string.Empty).AddTo(this.Bindables);
            this.IsReplaceSBToSBEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.ReplaceSingleByteToMultiByte, false), false).AddTo(this.Bindables);
            this.HtmlFileExt = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.HtmlFileExtension, Const::FileFolder.DefaultHtmlFileExt), Const::FileFolder.DefaultHtmlFileExt).AddTo(this.Bindables);
            this.JpegFileExt = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.JpegFileExtension, Const::FileFolder.DefaultJpegFileExt), Const::FileFolder.DefaultJpegFileExt).AddTo(this.Bindables);
            this.VideoInfoSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.VideoInfoSuffix, Const::Format.DefaultFileNameFormat), Const::Format.DefaultFileNameFormat).AddTo(this.Bindables);
            this.ThumbnailSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.ThumbnailSuffix, Const::Format.DefaultThumbnailSuffix), Const::Format.DefaultThumbnailSuffix).AddTo(this.Bindables);
            this.OwnerCommentSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.OwnerCommentSuffix, Const::Format.DefaultOwnerCommentSuffix), Const::Format.DefaultOwnerCommentSuffix).AddTo(this.Bindables);
            this.IchibaSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.IchibaSuffix, Const::Format.DefaultIchibaSuffix), Const::Format.DefaultIchibaSuffix).AddTo(this.Bindables);
            this.EconomySuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.EnonomyQualitySuffix, Const::Format.DefaultEconomyVideoSuffix), Const::Format.DefaultEconomyVideoSuffix).AddTo(this.Bindables);
            this.IsSearchingVideosExactEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.SearchVideosExact, false), false).AddTo(this.Bindables);

            this._replaceRules = new ObservableCollection<IReplaceRule>(WS.ReplaceHandler.GetRules().Data ?? WS.ReplaceHandler.Convert(Format.DefaultReplaceRules));
            this.ReplaceRules = new BindableCollection<ReplaceRuleViewModel, IReplaceRule>(this._replaceRules, r =>
            {
                var vm = new ReplaceRuleViewModel(r);
                this.Bindables.Add(vm.IsSelected);
                return vm;
            });
            this.Bindables.Add(this.ReplaceRules);
            this.ReplaceFromInput = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.ReplaceToInput = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
        }

        #region field

        private readonly ObservableCollection<IReplaceRule> _replaceRules;

        #endregion

        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// 置き換えルール
        /// </summary>
        public BindableCollection<ReplaceRuleViewModel, IReplaceRule> ReplaceRules { get; init; }

        /// <summary>
        /// デフォルトのDLパス
        /// </summary>
        public IBindableSettingInfo<string> DefaultFolder { get; init; }

        /// <summary>
        /// ファイル名のフォーマット
        /// </summary>
        public IBindableSettingInfo<string> FileFormat { get; init; }

        /// <summary>
        /// 禁則文字を2バイト文字に置き換える
        /// </summary>
        public IBindableSettingInfo<bool> IsReplaceSBToSBEnable { get; init; }

        /// <summary>
        /// htmlファイル
        /// </summary>
        public IBindableSettingInfo<string> HtmlFileExt { get; init; }

        /// <summary>
        /// jpegファイル
        /// </summary>
        public IBindableSettingInfo<string> JpegFileExt { get; init; }

        /// <summary>
        /// 動画情報の接尾辞
        /// </summary>
        public IBindableSettingInfo<string> VideoInfoSuffix { get; init; }

        /// <summary>
        /// 市場情報の接尾辞
        /// </summary>
        public IBindableSettingInfo<string> IchibaSuffix { get; init; }

        /// <summary>
        /// 動画をIDで探索する
        /// </summary>
        public IBindableSettingInfo<bool> IsSearchingVideosExactEnable { get; init; }

        /// <summary>
        /// サムネイルの接尾辞
        /// </summary>
        public IBindableSettingInfo<string> ThumbnailSuffix { get; init; }

        /// <summary>
        /// 投コメの接尾辞
        /// </summary>
        public IBindableSettingInfo<string> OwnerCommentSuffix { get; init; }


        /// <summary>
        /// エコノミー動画の接尾辞
        /// </summary>
        public IBindableSettingInfo<string> EconomySuffix { get; init; }

        /// <summary>
        /// 置き換え元
        /// </summary>
        public IBindableProperty<string> ReplaceToInput { get; init; }

        /// <summary>
        /// 置き換え元
        /// </summary>
        public IBindableProperty<string> ReplaceFromInput { get; init; }

        /// <summary>
        /// 置き換えルールを追加
        /// </summary>
        public void AddReplaceRule()
        {
            if (string.IsNullOrEmpty(this.ReplaceFromInput.Value))
            {
                return;
            }

            IAttemptResult result = WS.ReplaceHandler.AddRule(this.ReplaceFromInput.Value, this.ReplaceToInput.Value);

            if (!result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.FailedToAddRule);
                string messageD = WS.StringHandler.GetContent(SC.Detail, result.Message);
                this.ShowAlert(message, AlertType.Error);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }
            else
            {
                this._replaceRules.Add(new ReplaceRule(this.ReplaceFromInput.Value, this.ReplaceToInput.Value));
                this.ReplaceFromInput.Value = string.Empty;
                this.ReplaceToInput.Value = string.Empty;
            }
        }

        /// <summary>
        /// 置き換えルールを削除
        /// </summary>
        public void RemoveReplaceRule()
        {
            var targets = this.ReplaceRules.Where(r => r.IsSelected.Value);
            if (targets.Count() == 0)
            {
                return;
            }

            foreach (var target in targets)
            {
                IAttemptResult result = WS.ReplaceHandler.RemoveRule(target.ReplaceFrom,target.ReplaceTo);

                if (!result.IsSucceeded)
                {
                    string message = WS.StringHandler.GetContent(SC.FailedToRemovedRule);
                    string messageD = WS.StringHandler.GetContent(SC.Detail, result.Message);
                    this.ShowAlert(message, AlertType.Error);
                    WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                }
                else
                {
                    this._replaceRules.RemoveAll(r => r.ReplaceFrom == target.ReplaceFrom);
                }
            }
        }
    }

    public class ReplaceRuleViewModel
    {
        public ReplaceRuleViewModel(IReplaceRule rule)
        {
            this.IsSelected = new BindableProperty<bool>(false);
            this.ReplaceFrom = rule.ReplaceFrom;
            this.ReplaceTo = rule.ReplaceTo;
        }

        /// 選択フラグ
        /// </summary>
        public IBindableProperty<bool> IsSelected { get; init; }

        /// <summary>
        /// 置き換え元
        /// </summary>
        public string ReplaceFrom { get; init; }

        /// <summary>
        /// 置き換え先
        /// </summary>
        public string ReplaceTo { get; init; }
    }
}
