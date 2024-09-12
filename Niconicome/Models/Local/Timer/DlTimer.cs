using System;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.BackgroundTask;
using Niconicome.Models.Domain.Utils.Event;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Local.Timer
{
    interface IDlTimer
    {
        /// <summary>
        /// 有効フラグ
        /// </summary>
        IBindableProperty<bool> IsEnabled { get; }

        /// <summary>
        /// 24時間ごとに繰り返すかどうか
        /// </summary>
        IBindableProperty<bool> IsRepeatByDayEnable { get; }

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
        public DlTimer(ISettingsContainer settingsContainer, IBackgroundTaskManager backgroundTaskManager)
        {
            this._backgroundTaskManager = backgroundTaskManager;
            this.settingsContainer = settingsContainer;

            this.IsEnabled.Subscribe(value => this.ChangeState(value));
            this.IsRepeatByDayEnable = new BindableProperty<bool>(this.settingsContainer.GetOnlyValue(SettingNames.IsDlTImerEveryDayEnable, false).Data)
                .Subscribe(value =>
                {
                    if (this._isRepeatByDayEnable is null)
                    {
                        var result = this.settingsContainer.GetSetting(SettingNames.IsDlTImerEveryDayEnable, false);
                        if (!result.IsSucceeded || result.Data is null) return;
                        this._isRepeatByDayEnable = result.Data;
                    }

                    if (this._isRepeatByDayEnable.Value == value) return;

                    this._isRepeatByDayEnable.Value = value;

                    this.IsRepeatByDayEnable!.Value = value;
                });
        }

        #region field

        private readonly IBackgroundTaskManager _backgroundTaskManager;

        private readonly ISettingsContainer settingsContainer;

        private ISettingInfo<bool>? _isRepeatByDayEnable;

        private BackgroundTask? _task;

        private DateTime dt = DateTime.Now;

        private Action? _dlAction;

        #endregion

        #region Props

        public IBindableProperty<bool> IsEnabled { get; init; } = new BindableProperty<bool>(false);

        public DateTime TrigggeredDT => this.dt;

        public IBindableProperty<bool> IsRepeatByDayEnable { get; init; }

        #endregion

        #region Methods

        public void Set(DateTime dt, Action dlAction)
        {
            this.dt = dt;
            this._dlAction = dlAction;
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
            if (this._dlAction is null)
            {
                return;
            }

            if (isEnabled)
            {
                if (this.dt < DateTime.Now)
                {
                    this.dt = DateTime.Now + TimeSpan.FromDays(1);
                }

                this._task = this._backgroundTaskManager.AddTimerTask(() =>
                {
                    this._dlAction();

                    if (this.IsRepeatByDayEnable.Value)
                    {
                        this.Set(this.dt + TimeSpan.FromDays(1), this._dlAction);
                    }
                    else
                    {
                        this.IsEnabled.Value = false;
                    }
                }, this.dt);
            }
            else if (this._task is not null)
            {
                this._backgroundTaskManager.CancelTask(this._task.TaskID);
                this._task = null;
            }
        }

        /// <summary>
        /// 発行したイベントをキャンセルする
        /// </summary>
        private void Reset()
        {
            if (this._task is null) return;

            this._backgroundTaskManager.CancelTask(this._task.TaskID);
            this._task = null;
        }

        #endregion
    }
}
