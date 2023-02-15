using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils;
using Reactive.Bindings;

namespace Niconicome.Models.Local.OS
{
    public interface IClipbordManager
    {
        /// <summary>
        /// クリップボード監視フラグ
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsMonitoring { get; }

        /// <summary>
        /// クリップボードから情報を取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<string> GetClipboardContent();

        /// <summary>
        /// クリップボードに書き込む
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        IAttemptResult SetToClipBoard(string content);

        /// <summary>
        /// クリップボード監視を開始
        /// </summary>
        IAttemptResult StartMonitoring();

        /// <summary>
        /// クリップボード監視を終了
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// クリップボード変更ハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        void RegisterClipboardChangeHandler(Action<string> handler);
    }

    public class ClipbordManager : IClipbordManager
    {
        public ClipbordManager(IErrorHandler errorHandler)
        {
            this.IsMonitoring = this._isMonitoringSource.ToReadOnlyReactiveProperty();
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly ReactiveProperty<bool> _isMonitoringSource = new();

        private event Action<string>? _changeHandler;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Props

        public ReadOnlyReactiveProperty<bool> IsMonitoring { get; init; }

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
                this._errorHandler.HandleError(ClipboardManagerError.FailedToGetClipboardData, ex);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ClipboardManagerError.FailedToGetClipboardData, ex));
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
                this._errorHandler.HandleError(ClipboardManagerError.FailedToSetDataToClipboard, ex);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ClipboardManagerError.FailedToSetDataToClipboard, ex));
            }

            return AttemptResult.Succeeded();

        }


        public void RegisterClipboardChangeHandler(Action<string> handler)
        {
            this._changeHandler += handler;
        }

        public IAttemptResult StartMonitoring()
        {
            //二重監視防止
            if (this.IsMonitoring.Value)
            {
                this._errorHandler.HandleError(ClipboardManagerError.AlreadyMonitoring);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(ClipboardManagerError.AlreadyMonitoring));
            }

            //OSバージョンチェック
            if (!Compatibility.IsOsVersionLargerThan(10, 0, 10240))
            {
                this._errorHandler.HandleError(ClipboardManagerError.OSVersionOlderThanMinimum);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(ClipboardManagerError.OSVersionOlderThanMinimum));
            }

            try
            {
                Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += this.OnContentChanged;
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ClipboardManagerError.FailedToStartMonitoringClipboard, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(ClipboardManagerError.FailedToStartMonitoringClipboard, ex));
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
                this._errorHandler.HandleError(ClipboardManagerError.FailedToStopMonitoringClipboard, ex);
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
