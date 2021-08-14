using System;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.Event;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Local.Timer
{
    interface IDlTimer
    {
        /// <summary>
        /// 有効フラグ
        /// </summary>
        ReactiveProperty<bool> IsEnabled { get; }

        /// <summary>
        /// イベント発火時刻
        /// </summary>
        DateTime TrigggeredDT { get; }

        /// <summary>
        /// 時間を設定する
        /// </summary>
        /// <param name="dt"></param>
        void Set(DateTime dt, Action dlAction);
    }

    class DlTimer : BindableBase, IDlTimer
    {
        public DlTimer(IEventManager manager, ILogger logger)
        {
            this.manager = manager;
            this.logger = logger;
            this.IsEnabled.Subscribe(value => this.ChangeState(value));

        }

        #region field

        private readonly IEventManager manager;

        private readonly ILogger logger;

        private string? eventID;

        private DateTime dt = DateTime.Now;

        private Action? dlAction;

        #endregion

        #region Props

        public ReactiveProperty<bool> IsEnabled { get; init; } = new();

        public DateTime TrigggeredDT => this.dt;

        #endregion

        #region Methods

        public void Set(DateTime dt, Action dlAction)
        {
            this.dt = dt;
            this.dlAction = dlAction;
            this.Reset();
            this.ChangeState(this.IsEnabled.Value);
        }

        #endregion

        #region private

        /// <summary>
        /// 状態を変更する
        /// </summary>
        /// <param name="isEnabled"></param>
        private void ChangeState(bool isEnabled)
        {
            if (this.dlAction is null)
            {
                return;
            }

            if (isEnabled)
            {
                if (this.dt < DateTime.Now)
                {
                    this.dt = DateTime.Now + TimeSpan.FromDays(1);
                }
                this.eventID = this.manager.Regster(this.dlAction, this.dt, ex =>
                {
                    this.logger.Error("タイマーDLに失敗しました。", ex);
                });
            }
            else if (this.eventID is not null)
            {
                this.manager.Cancel(this.eventID);
            }
        }

        /// <summary>
        /// 発行したイベントをキャンセルする
        /// </summary>
        private void Reset()
        {
            if (this.eventID is not null)
            {
                this.manager.Cancel(this.eventID);
            }
        }

        #endregion
    }
}
