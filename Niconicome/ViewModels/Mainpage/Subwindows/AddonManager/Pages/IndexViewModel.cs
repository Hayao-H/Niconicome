using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Update;
using Niconicome.Models.Helper.Result;
using Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Shared;
using Timer = System.Timers;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Pages
{
    public class IndexViewModel : IDisposable
    {
        public IndexViewModel()
        {
            this.RegisterListEventHandlers();
            this.RefreshList();
        }

        #region field

        private List<Action> _listChangedHandler = new();

        private List<Action> _alertChangedHandler = new();

        private List<Action<ModalType>> _modalChangeHandler = new();

        private List<Timer::Timer> _timers = new();

        private enum AlertType { Danger, Info };

        private SynchronizationContext? _currentContext;

        #endregion

        #region Props

        public IEnumerable<AddonInfomationViewModel> LoadedAddons { get; private set; } = new List<AddonInfomationViewModel>();

        public IEnumerable<UpdateCheckInfomationViewModel> ToBeUpdatedAddons { get; private set; } = new List<UpdateCheckInfomationViewModel>();

        public IEnumerable<FailedResultViewModel> LoadFailedAddons { get; private set; } = new List<FailedResultViewModel>();

        /// <summary>
        /// アップデート情報
        /// </summary>
        public UpdateInfomationViewModel? UpdateInfomation { get; private set; }

        /// <summary>
        /// アンインストール予定のアドオン
        /// </summary>
        public AddonInfomationViewModel? ToBeUninstalledAddon { get; private set; }

        /// <summary>
        /// アラート部に表示するメッセージ
        /// </summary>
        public string AlertMessage { get; private set; } = string.Empty;

        /// <summary>
        /// アラートの種類
        /// </summary>
        public string AlertTypeClass { get; private set; } = string.Empty;

        /// <summary>
        /// アラート部の表示有無
        /// </summary>
        public bool DisplayAlertMessage { get; private set; }

        /// <summary>
        /// デバッグモード
        /// </summary>
        public bool IsDevModeEnable
        {
            get => WS::AddonPage.AddonManager.IsDevelopperMode;
            set => WS::AddonPage.AddonManager.IsDevelopperMode = value;
        }

        #endregion

        #region private

        /// <summary>
        /// アラート部を表示する
        /// </summary>
        private void ShowAlert(AlertType type = AlertType.Danger)
        {
            this.DisplayAlertMessage = true;
            this.AlertTypeClass = type switch
            {
                AlertType.Info => "alert-info",
                _ => "alert-danger"
            };

            foreach (var handler in this._alertChangedHandler)
            {
                this._currentContext?.Post(_ => handler(), null);
            }

            var timer = new Timer::Timer(5000);
            timer.AutoReset = false;
            timer.Elapsed += (_, _) =>
            {
                this.DisplayAlertMessage = false;
                foreach (var handler in this._alertChangedHandler)
                {
                    this._currentContext?.Post(_ => handler(), null);
                }
                this._timers.Remove(timer);
            };

            timer.Enabled = true;
            this._timers.Add(timer);
        }

        /// <summary>
        /// モーダル部を表示する
        /// </summary>
        private void ShowModal(ModalType type)
        {
            foreach (var handler in this._modalChangeHandler)
            {
                this._currentContext?.Post(_ => handler(type), null);
            }
        }

        /// <summary>
        /// リスト変更イベントを購読
        /// </summary>
        private void RegisterListEventHandlers()
        {
            WS::AddonPage.AddonStatusContainer.ListChanged += this.OnListChanged;
        }

        /// <summary>
        /// リスト変更イベントハンドラー
        /// </summary>
        /// <param name="_"></param>
        /// <param name="__"></param>
        private void OnListChanged(object? _, EventArgs __)
        {
            this.RefreshList();
            foreach (var handler in this._listChangedHandler) handler();
        }

        /// <summary>
        /// リスト更新関数
        /// </summary>
        private void RefreshList()
        {
            this.LoadedAddons = WS::AddonPage.AddonStatusContainer.LoadedAddons.Select(i => new AddonInfomationViewModel(i));
            this.ToBeUpdatedAddons = WS::AddonPage.AddonStatusContainer.ToBeUpdatedAddons.Select(i => new UpdateCheckInfomationViewModel(i));
            this.LoadFailedAddons = WS::AddonPage.AddonStatusContainer.LoadFailedAddons.Select(i => new FailedResultViewModel(i));
        }

        #endregion

        #region Method

        /// <summary>
        /// コンテキストを登録
        /// </summary>
        /// <param name="context"></param>
        public void SetContext(SynchronizationContext context)
        {
            this._currentContext = context;
        }

        /// <summary>
        /// リスト変更を通知する
        /// </summary>
        /// <param name="handler"></param>
        public void AddListHandler(Action handler)
        {
            this._listChangedHandler.Add(handler);
        }

        /// <summary>
        /// アラート部表示イベントハンドラ
        /// </summary>
        /// <param name="handler"></param>
        public void AddAlertHandler(Action handler)
        {
            this._alertChangedHandler.Add(handler);
        }

        /// <summary>
        /// モーダル部表示イベントハンドラ
        /// </summary>
        /// <param name="handler"></param>
        public void AddModalHandler(Action<ModalType> handler)
        {
            this._modalChangeHandler.Add(handler);
        }

        /// <summary>
        /// 更新ボタンがクリックされたとき
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task OnUpdateClick(string id)
        {
            IAttemptResult<UpdateInfomation> downloadResult = await WS::AddonPage.AddonInstallManager.DownloadUpdate(id);
            if (!downloadResult.IsSucceeded || downloadResult.Data is null)
            {
                this.AlertMessage = downloadResult.Message ?? "不明なエラーによりアップデートに失敗しました。";
                this.ShowAlert();
            }
            else if (downloadResult.Data.HasNewPermission)
            {
                this.UpdateInfomation = new UpdateInfomationViewModel(downloadResult.Data, id);
                this.ShowModal(ModalType.Update);
            }
            else
            {
                this.OnInstallUpdateClick(id, downloadResult.Data.archivePath);
            }
        }

        /// <summary>
        /// 更新実行ボタンがクリックされたとき
        /// </summary>
        /// <param name="id"></param>
        /// <param name="archivePath"></param>
        public void OnInstallUpdateClick(string id, string archivePath)
        {
            IAttemptResult result = WS::AddonPage.AddonInstallManager.UpdateAndLoad(id, archivePath, true);
            if (!result.IsSucceeded)
            {
                this.AlertMessage = result.Message ?? "不明なエラーによりアップデートに失敗しました。";
                this.ShowAlert();
            }
            else
            {
                this.AlertMessage = "更新が完了しました。";
                this.ShowAlert(AlertType.Info);
            }
        }

        /// <summary>
        /// 更新確認ボタンがクリックされたとき
        /// </summary>
        /// <returns></returns>
        public async Task OnCheckForUpdateClick()
        {
            this.AlertMessage = "更新を確認します。";
            this.ShowAlert(AlertType.Info);

            await WS::AddonPage.AddonManager.CheckForUpdates();

            this.AlertMessage = "更新を確認しました。";
            this.ShowAlert(AlertType.Info);
        }

        /// <summary>
        /// アンインストールリンクがクリックされたとき
        /// </summary>
        /// <param name="id"></param>
        public void OnUninstallClicled(AddonInfomationViewModel vm)
        {
            this.ToBeUninstalledAddon = vm;
            this.ShowModal(ModalType.Uninstall);
        }

        /// <summary>
        /// アンインストールの確認がされたとき
        /// </summary>
        public void OnUninstallConfirmed()
        {
            if (this.ToBeUninstalledAddon is null) return;

            IAttemptResult result = WS::AddonPage.AddonInstallManager.Uninstall(this.ToBeUninstalledAddon.ID);
            if (result.IsSucceeded)
            {
                this.AlertMessage = "アンインストールが完了しました。";
                this.ShowAlert(AlertType.Info);
            }
            else
            {
                this.AlertMessage = $"アンインストールに失敗しました。(詳細：{result.Message ?? "不明"})";
                this.ShowAlert();
            }

            this.ToBeUninstalledAddon = null;
        }

        #endregion


        public void Dispose()
        {
            WS::AddonPage.AddonStatusContainer.ListChanged -= this.OnListChanged;
            this._listChangedHandler.Clear();
            this._alertChangedHandler.Clear();
            this._modalChangeHandler.Clear();
        }
    }

    public enum ModalType { Update, Uninstall }
}
