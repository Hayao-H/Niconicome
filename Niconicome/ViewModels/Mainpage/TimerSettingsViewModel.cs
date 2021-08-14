using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Download.Actions;
using Niconicome.ViewModels.Mainpage.Utils;
using Prism.Events;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    class TimerSettingsViewModel : BindableBase
    {
        public TimerSettingsViewModel(IEventAggregator ea)
        {
            this.ea = ea;
            this.IsTimerEveryDayEnable = WS::Mainpage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.DlTimerEveryDay);

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

            #region timer

            void log()
            {
                if (!WS::Mainpage.DlTimer.IsEnabled.Value) return;

                DateTime dt = WS::Mainpage.DlTimer.TrigggeredDT;
                double delta = Math.Floor((dt - DateTime.Now).TotalMinutes * 10) / 10;
                if (delta < 0) return;

                string isEnabled = WS::Mainpage.DlTimer.IsEnabled.Value ? "有効" : "無効";
                WS::Mainpage.Messagehandler.AppendMessage($"タイマーを「{dt}({delta}分後)」に設定しました。（設定：{isEnabled}）"); ;
            }

            this.IsTImerEnabled = WS::Mainpage.DlTimer.IsEnabled.ToReactivePropertyAsSynchronized(x => x.Value);
            this.IsTImerEnabled.Skip(1).Subscribe(value =>
            {
                if (value)
                {
                    log();
                }
                else
                {
                    WS::Mainpage.Messagehandler.AppendMessage($"タイマーを解除しました。"); ;
                }
            });

            this.SelectedTime.Subscribe(value =>
            {

                WS::Mainpage.DlTimer.Set(value, () =>
                {
                    WS::Mainpage.Messagehandler.AppendMessage("タイマー処理を開始します。");
                    this.ea.GetEvent<PubSubEvent<MVVMEvent<VideoInfoViewModel>>>().Publish(new MVVMEvent<VideoInfoViewModel>(null, typeof(DownloadSettingsViewModel), EventType.Download));
                });

                log();

            });


            #endregion
        }

        #region field

        private readonly IEventAggregator ea;

        #endregion

        #region Props

        public ReactiveProperty<ComboboxItem<PostDownloadActions>> Action { get; init; }

        public List<ComboboxItem<PostDownloadActions>> SelectableActions { get; init; }

        public ReactiveProperty<DateTime> SelectedTime { get; init; } = new(DateTime.Now);

        public ReactiveProperty<bool> IsTimerEveryDayEnable { get; init; }

        public ReactiveProperty<bool> IsTImerEnabled { get; init; }

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

        public ReactiveProperty<DateTime> SelectedTime { get; init; } = new(DateTime.Now);

        public ReactiveProperty<bool> IsTImerEnabled { get; init; } = new(true);

        public ReactiveProperty<bool> IsTimerEveryDayEnable { get; init; } = new(true);

        public List<ComboboxItem<PostDownloadActions>> SelectableActions { get; init; }

    }
}
