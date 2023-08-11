using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.OS;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.Models.Infrastructure.IO
{

    public class WindowsClipboardManager : IClipbordManager
    {
        public WindowsClipboardManager(IErrorHandler errorHandler)
        {
            this.IsMonitoring = this._isMonitoringSource.AsReadOnly();
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly BindableProperty<bool> _isMonitoringSource = new(false);

        private event Action<string>? _changeHandler;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Props

        public IReadonlyBindablePperty<bool> IsMonitoring { get; init; }

        #endregion

        #region Method

        public IAttemptResult<string> GetClipboardContent()
        {
            string data;

            try
            {
                data = Clipboard.GetText();
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsClipboardManagerError.FailedToGetClipboardData, ex);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(WindowsClipboardManagerError.FailedToGetClipboardData, ex));
            }

            return AttemptResult<string>.Succeeded(data);
        }

        public IAttemptResult SetToClipBoard(string content)
        {
            try
            {
                Clipboard.SetText(content);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsClipboardManagerError.FailedToSetDataToClipboard, ex);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(WindowsClipboardManagerError.FailedToSetDataToClipboard, ex));
            }

            return AttemptResult.Succeeded();

        }


        public void RegisterClipboardChangeHandler(Action<string> handler)
        {
            this._changeHandler += handler;
        }

        public void UnRegisterClipboardChangeHandler(Action<string> handler)
        {
            this._changeHandler -= handler;
        }


        public IAttemptResult StartMonitoring()
        {
            //二重監視防止
            if (this.IsMonitoring.Value)
            {
                this._errorHandler.HandleError(WindowsClipboardManagerError.AlreadyMonitoring);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsClipboardManagerError.AlreadyMonitoring));
            }

            //OSバージョンチェック
            if (!Compatibility.IsOsVersionLargerThan(10, 0, 10240))
            {
                this._errorHandler.HandleError(WindowsClipboardManagerError.OSVersionOlderThanMinimum);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsClipboardManagerError.OSVersionOlderThanMinimum));
            }

            try
            {
                Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += this.OnContentChanged;
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsClipboardManagerError.FailedToStartMonitoringClipboard, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsClipboardManagerError.FailedToStartMonitoringClipboard, ex));
            }

            this._isMonitoringSource.Value = true;

            return AttemptResult.Succeeded();
        }

        public void StopMonitoring()
        {
            //監視していないなら終了できない
            if (!this.IsMonitoring.Value)
            {
                return;
            }

            try
            {
                Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged -= this.OnContentChanged;
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsClipboardManagerError.FailedToStopMonitoringClipboard, ex);
                return;
            }

            this._isMonitoringSource.Value = false;

            return;
        }



        #endregion

        #region private

        /// <summary>
        /// クリップボード変更イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentChanged(object? sender, object? e)
        {
            IAttemptResult<string> result = this.GetClipboardContent();
            if (result.IsSucceeded && result.Data is not null)
            {
                this._changeHandler?.Invoke(result.Data);
            }
        }

        #endregion
    }
}
