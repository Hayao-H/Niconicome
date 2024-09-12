using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Niconicome.Extensions.System;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist.V2.Migration;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class MigrationViewModel
    {
        public MigrationViewModel(NavigationManager navigation)
        {
            this._navigation = navigation;

            this.Bindables = new Bindables();
            this.IsMigrating = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.IsMigrated = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.IsMigrationFailed = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.Message = new BindableProperty<string>("").AddTo(this.Bindables);
            this.CurrentlyMigratingData = new BindableProperty<string>("").AddTo(this.Bindables);
        }

        ~MigrationViewModel()
        {
            this.Bindables.Dispose();
        }

        #region field

        private readonly NavigationManager _navigation;

        #endregion

        #region Method

        public async Task Migrate()
        {
            this.IsMigrating.Value = true;

            SynchronizationContext? ctx = SynchronizationContext.Current;

            await Task.Run(() =>
             {
                 IAttemptResult<MigrationResult> result = WS::Mainpage.VideoAndPlayListMigration.Migrate(x =>
                 {
                     if (ctx is not null)
                     {
                         ctx.Post(_ => this.CurrentlyMigratingData.Value = x, null);
                     }
                 });

                 if (!result.IsSucceeded || result.Data is null)
                 {
                     this.IsMigrationFailed.Value = true;
                     this.Message.Value = result.Message ?? "None";
                 }
                 else
                 {
                     this.MigrationResult = result.Data;
                     this.IsMigrated.Value = true;

                     WS::Mainpage.PlaylistManager.Initialize();
                 }
             });

            this.IsMigrating.Value = false;
        }

        public void OnDone()
        {
            WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/videos");
            this._navigation.NavigateTo("/videos");
        }

        #endregion

        #region Props

        /// <summary>
        /// 移行結果
        /// </summary>
        public MigrationResult? MigrationResult { get; private set; }


        /// <summary>
        /// 移行中フラグ
        /// </summary>
        public IBindableProperty<bool> IsMigrating { get; init; }

        /// <summary>
        /// 移行完了フラグ
        /// </summary>
        public IBindableProperty<bool> IsMigrated { get; init; }

        /// <summary>
        /// 移行失敗フラグ
        /// </summary>
        public IBindableProperty<bool> IsMigrationFailed { get; init; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public IBindableProperty<string> Message { get; init; }

        /// <summary>
        /// 移行中のデータ
        /// </summary>
        public IBindableProperty<string> CurrentlyMigratingData { get; init; }

        /// <summary>
        /// 変更通知
        /// </summary>
        public Bindables Bindables { get; init; }

        #endregion
    }
}
