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

        #endregion

        #region Props

        public IEnumerable<AddonInfomationViewModel> LoadedAddons { get; private set; } = new List<AddonInfomationViewModel>();

        public IEnumerable<UpdateCheckInfomationViewModel> ToBeUpdatedAddons { get; private set; } = new List<UpdateCheckInfomationViewModel>();

        #endregion

        #region private

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

        #endregion


        public void Dispose()
        {
            WS::AddonPage.AddonStatusContainer.ListChanged -= this.OnListChanged;
            this._listChangedHandler.Clear();
        }
    }
}
