using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Domain.Utils;
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
        public ClipbordManager(ILogger logger)
        {
            this._logger = logger;
            this.IsMonitoring = this._isMonitoringSource.ToReadOnlyReactiveProperty();
        }

        #region field

        private readonly ILogger _logger;

        private readonly ReactiveProperty<bool> _isMonitoringSource = new();

        private event Action<string>? _changeHandler;

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
                this._logger.Error("クリップボードからのデータ取得に失敗しました。", ex);
                return AttemptResult<string>.Fail($"クリップボードからのデータ取得に失敗しました。(詳細:{ex.Message})");
            }

            return AttemptResult<string>.Succeeded(data);
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
                return AttemptResult.Fail("すでに監視しています");
            }

            //OSバージョンチェック
            if (!Compatibility.IsOsVersionLargerThan(10, 0, 10240))
            {
                return AttemptResult.Fail("この機能は'Windows10 1507'以降のOSでのみ利用可能です。");
            }

            try
            {
                Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += this.OnContentChanged;
            }
            catch (Exception ex)
            {
                this._logger.Error("クリップボードの監視に失敗しました。", ex);
                return AttemptResult.Fail($"クリップボードの監視に失敗しました。(詳細:{ex.Message})");
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
                this._logger.Error("クリップボードの監視終了に失敗しました。", ex);
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
