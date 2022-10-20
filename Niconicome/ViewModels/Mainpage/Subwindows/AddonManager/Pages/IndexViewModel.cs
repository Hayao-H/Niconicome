using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Shared;
using WS = Niconicome.Workspaces;
using System.Collections.Specialized;
using Niconicome.Extensions;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Update;
using System.Timers;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;

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

        private List<Action> _modalChangeHandler = new();

        private List<Timer> _timers = new();

        #endregion

        #region Props

        public IEnumerable<AddonInfomationViewModel> LoadedAddons { get; private set; } = new List<AddonInfomationViewModel>();

        public IEnumerable<UpdateCheckInfomationViewModel> ToBeUpdatedAddons { get; private set; } = new List<UpdateCheckInfomationViewModel>();

        /// <summary>
        /// アップデート情報
        /// </summary>
        public UpdateInfomationViewModel? UpdateInfomation { get; private set; }

        /// <summary>
        /// アラート部に表示するメッセージ
        /// </summary>
        public string AlertMessage { get; private set; } = string.Empty;

        /// <summary>
        /// アラート部の表示有無
        /// </summary>
        public bool DisplayAlertMessage { get; private set; }

        #endregion

        #region private

        /// <summary>
        /// アラート部を表示する
        /// </summary>
        private void ShowAlert()
        {
            this.DisplayAlertMessage = true;
            foreach (var handler in this._alertChangedHandler)
            {
                handler();
            }

            var timer = new Timer(5000);
            timer.AutoReset = false;
            timer.Elapsed += (_, _) =>
            {
                this.DisplayAlertMessage = false;
                foreach (var handler in this._alertChangedHandler)
                {
                    handler();
                }
                this._timers.Remove(timer);
            };

            timer.Enabled = true;
            this._timers.Add(timer);
        }

        /// <summary>
        /// モーダル部を表示する
        /// </summary>
        private void ShowModal()
        {
            foreach(var handler in this._modalChangeHandler)
            {
                handler();
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

            this.UpdateInfomation = new UpdateInfomationViewModel(new UpdateInfomation("", new List<Permission>() { Permissions.Hooks, Permissions.Hooks, Permissions.DownloadSettings },true, WS::AddonPage.AddonStatusContainer.LoadedAddons.First(), "", ""));
        }

        #endregion

        #region Method

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
        public void AddModalHandler(Action handler)
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
                this.UpdateInfomation = new UpdateInfomationViewModel(downloadResult.Data);
                this.ShowModal();
            } else
            {
                this.OnInstallUpdateClick(id, downloadResult.Data.archivePath);
            }
        }

        /// <summary>
        /// 更新確認ボタンがクリックされたとき
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
        }

        #endregion


        public void Dispose()
        {
            WS::AddonPage.AddonStatusContainer.ListChanged -= this.OnListChanged;
            this._listChangedHandler.Clear();
        }
    }
}
