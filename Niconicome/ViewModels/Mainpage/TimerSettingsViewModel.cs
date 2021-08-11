using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Network.Download.Actions;
using Niconicome.ViewModels.Mainpage.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    class TimerSettingsViewModel : BindableBase
    {
        public TimerSettingsViewModel()
        {
            #region actions

            var none = new ComboboxItem<PostDownloadActions>(PostDownloadActions.None, "何もしない");
            var shutdown = new ComboboxItem<PostDownloadActions>(PostDownloadActions.Shutdown, "シャットダウン");
            var restart = new ComboboxItem<PostDownloadActions>(PostDownloadActions.Restart, "再起動");
            var logoff = new ComboboxItem<PostDownloadActions>(PostDownloadActions.LogOff, "ログオフ");
            var sleep = new ComboboxItem<PostDownloadActions>(PostDownloadActions.Sleep, "休止状態");

            this.Action = WS::Mainpage.PostDownloadTasksManager.PostDownloadAction.ToReactivePropertyAsSynchronized(x => x.Value, x => x switch
               {
                   PostDownloadActions.Shutdown => shutdown,
                   PostDownloadActions.Restart => restart,
                   PostDownloadActions.LogOff => logoff,
                   PostDownloadActions.Sleep => sleep,
                   _ => none,
               }, x => x.Value);
            this.SelectableActions = new List<ComboboxItem<PostDownloadActions>>() { none, shutdown, restart, logoff, sleep };

            #endregion
        }

        #region Props

        public ReactiveProperty<ComboboxItem<PostDownloadActions>> Action { get; init; }

        public List<ComboboxItem<PostDownloadActions>> SelectableActions { get; init; }

        #endregion
    }

    class TimerSettingsViewModelD
    {

        public TimerSettingsViewModelD()
        {
            #region actions

            var none = new ComboboxItem<PostDownloadActions>(PostDownloadActions.None, "何もしない");
            var shutdown = new ComboboxItem<PostDownloadActions>(PostDownloadActions.Shutdown, "シャットダウン");
            var restart = new ComboboxItem<PostDownloadActions>(PostDownloadActions.Restart, "再起動");
            var logoff = new ComboboxItem<PostDownloadActions>(PostDownloadActions.LogOff, "ログオフ");
            var sleep = new ComboboxItem<PostDownloadActions>(PostDownloadActions.Sleep, "休止状態");

            this.Action = new ReactiveProperty<ComboboxItem<PostDownloadActions>>(none);
            this.SelectableActions = new List<ComboboxItem<PostDownloadActions>>() { none, shutdown, restart, logoff, sleep };

            #endregion
        }


        public ReactiveProperty<ComboboxItem<PostDownloadActions>> Action { get; init; }

        public List<ComboboxItem<PostDownloadActions>> SelectableActions { get; init; }

    }
}
