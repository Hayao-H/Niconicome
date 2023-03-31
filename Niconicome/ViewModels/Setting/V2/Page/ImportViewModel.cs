using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.V2.Page.StringContent;
using Niconicome.ViewModels.Shared;
using WS = Niconicome.Workspaces.SettingPageV2;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class ImportViewModel : AlertViewModel
    {
        public ImportViewModel()
        {
            this.Bindables.Add(this._alertBindables);
            this.IsProcessing = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.ImpottPathInput = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.XenoMessage = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.XenoRootFilePathInput = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
        }

        #region Method

        public async Task OnExportButtonClickASync()
        {
            if (this.IsProcessing.Value)
            {
                return;
            }

            this.IsProcessing.Value = true;

            IAttemptResult<string> result = await WS.ImportExportManager.ExportDataAsync();
            if (!result.IsSucceeded || result.Data is null)
            {
                this.ShowAlert(WS.StringHandler.GetContent(ImportVMSC.ExportFailed), AlertType.Error);
                WS.MessageHandler.AppendMessage(WS.StringHandler.GetContent(ImportVMSC.ExportFailedDetal, result.Message), LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }
            else
            {
                string message = WS.StringHandler.GetContent(ImportVMSC.ExportSucceeded, result.Data);
                this.ShowAlert(message, AlertType.Info);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
            }

            this.IsProcessing.Value = false;
        }

        public async Task OnImportButtonClickAsync()
        {
            if (this.IsProcessing.Value)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.ImpottPathInput.Value))
            {
                this.ShowAlert(WS.StringHandler.GetContent(ImportVMSC.ImportPathIsEmpty), AlertType.Error);
                return;
            }

            this.IsProcessing.Value = true;

            IAttemptResult<ImportResult> result = await WS.ImportExportManager.ImportDataAsync(this.ImpottPathInput.Value);
            if (!result.IsSucceeded || result.Data is null)
            {
                this.ShowAlert(WS.StringHandler.GetContent(ImportVMSC.ImportFailed), AlertType.Error);
                WS.MessageHandler.AppendMessage(WS.StringHandler.GetContent(ImportVMSC.ImportFailedDetail, result.Message), LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }
            else
            {
                string message = WS.StringHandler.GetContent(ImportVMSC.ImportSucceeded, result.Data.ImportSucceededPlaylistsCount, result.Data.ImpotySucceededVideosCount);
                this.ShowAlert(message, AlertType.Info);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
            }

            this.ImpottPathInput.Value = string.Empty;
            this.IsProcessing.Value = false;

        }

        public async Task OnXenoImportButtonClickAsync()
        {
            if (this.IsProcessing.Value)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.XenoRootFilePathInput.Value))
            {
                this.ShowAlert(WS.StringHandler.GetContent(ImportVMSC.ImportPathIsEmpty), AlertType.Error);
                return;
            }

            this.IsProcessing.Value = true;

            IAttemptResult<IXenoImportResult> result = await WS.XenoImportManager.ImportDataAsync(this.XenoRootFilePathInput.Value, m => this.XenoMessage.Value = m);
            if (!result.IsSucceeded || result.Data is null)
            {
                this.ShowAlert(WS.StringHandler.GetContent(ImportVMSC.ImportFailed), AlertType.Error);
                WS.MessageHandler.AppendMessage(WS.StringHandler.GetContent(ImportVMSC.ImportFailedDetail, result.Message), LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }
            else
            {
                string message = WS.StringHandler.GetContent(ImportVMSC.ImportSucceeded, result.Data.SucceededPlaylistsCount, result.Data.SucceededVideosCount);
                this.ShowAlert(message, AlertType.Info);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
            }

            this.XenoRootFilePathInput.Value = string.Empty;
            this.IsProcessing.Value = false;
        }

        #endregion

        #region Props

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<bool> IsProcessing { get; init; }

        public IBindableProperty<string> XenoMessage { get; init; }

        public IBindableProperty<string> ImpottPathInput { get; init; }

        public IBindableProperty<string> XenoRootFilePathInput { get; init; }

        #endregion
    }
}
