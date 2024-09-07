using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Network.Download.Actions;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;
using SC = Niconicome.ViewModels.Mainpage.Tabs.VideoList.BottomPanel.StringContent.TimerVMSC;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.BottomPanel
{
    public class TimerViewModel
    {
        public TimerViewModel()
        {
            this.IsTimerEnable = WS.DlTimer.IsEnabled.Subscribe(x =>
            {
                if (!x)
                {
                    var message = WS.StringHandler.GetContent(SC.TimerWasUnset);
                    WS.SnackbarHandler.Enqueue(message);
                    WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                }

            }).AddTo(this.Bindables);
            this.IsRepeatByDayEnable = WS.DlTimer.IsRepeatByDayEnable.AddTo(this.Bindables);
            this.PostDownloadAction = WS.PostDownloadActionsManager.PostDownloadAction.AddTo(this.Bindables);

            var now = DateTime.Now;
            this.Year = new BindableProperty<int>(now.Year).AddTo(this.Bindables);
            this.Month = new BindableProperty<int>(now.Month).AddTo(this.Bindables);
            this.Day = new BindableProperty<int>(now.Day).AddTo(this.Bindables);
            this.Hour = new BindableProperty<int>(now.Hour).AddTo(this.Bindables);
            this.Minute = new BindableProperty<int>(now.Minute).AddTo(this.Bindables);
        }

        #region 時間

        /// <summary>
        /// 年
        /// </summary>
        public IBindableProperty<int> Year { get; init; }

        /// <summary>
        /// 月
        /// </summary>
        public IBindableProperty<int> Month { get; init; }

        /// <summary>
        /// 日
        /// </summary>
        public IBindableProperty<int> Day { get; init; }

        /// <summary>
        /// 時間
        /// </summary>
        public IBindableProperty<int> Hour { get; init; }

        /// <summary>
        /// 分
        /// </summary>
        public IBindableProperty<int> Minute { get; init; }

        #endregion

        /// <summary>
        /// タイマーが有効かどうか
        /// </summary>
        public IBindableProperty<bool> IsTimerEnable { get; init; }

        /// <summary>
        /// 24時間ごとに繰り返すかどうか
        /// </summary>
        public IBindableProperty<bool> IsRepeatByDayEnable { get; init; }

        public IBindableProperty<PostDownloadActions> PostDownloadAction { get; init; }

        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// タイマー時刻を保存
        /// </summary>
        public void OnSetTimerClick()
        {
            var now = DateTime.Now;
            if (this.Year.Value < 2024)
            {
                this.Year.Value = now.Year;
            }
            var dt = new DateTime(this.Year.Value, this.Month.Value, this.Day.Value, this.Hour.Value, this.Minute.Value, 0);

            if (dt < now)
            {
                return;
            }

            WS.DlTimer.Set(dt, () => WS.DownloadManager.StartDownloadAsync(m => WS.SnackbarHandler.Enqueue(m), m => WS.MessageHandler.AppendMessage(m, LocalConstant.SystemMessageDispacher)));

            var isEnabled = WS.DlTimer.IsEnabled.Value ? WS.StringHandler.GetContent(SC.TimerEnable) : WS.StringHandler.GetContent(SC.TimerDisable);
            var delta = Math.Floor((dt - now).TotalMinutes);

            var message = WS.StringHandler.GetContent(SC.TimerWasSet, dt.ToString("yyyy/MM//dd HH:mm"), delta.ToString(), isEnabled);
            WS.SnackbarHandler.Enqueue(message);
            WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);

        }
    }
}
