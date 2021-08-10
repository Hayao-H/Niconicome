using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Niconicome.Models.Domain.Utils.Event
{
    public interface IEvent
    {

        /// <summary>
        /// イベントID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        bool IsCancelled { get; set; }

        /// <summary>
        /// イベントを実行する
        /// </summary>
        void Invoke();
    }

    class Event : IEvent
    {
        public Event(Action eventAction, Action<string> postTriggered, DateTime eventTiggeredTime = default, Action<Exception>? onError = null)
        {
            this.eventAction = eventAction;
            this.onError = onError;
            this.postTriggered = postTriggered;
            this.ID = Guid.NewGuid().ToString("D");

            if (eventTiggeredTime != default)
            {
                this.timer = new Timer((eventTiggeredTime - DateTime.Now).TotalMilliseconds);
                this.timer.Elapsed += (_, _) => this.Invoke();
                this.timer.Enabled = true;
            }
        }

        #region field

        private readonly Action eventAction;

        private Action<string> postTriggered;

        private readonly Action<Exception>? onError;

        private readonly Timer? timer;

        private bool iscanceledField;

        #endregion

        #region Props

        public string ID { get; init; }

        public bool IsCancelled
        {
            get => this.iscanceledField;
            set
            {
                if (value)
                {
                    this.Cancel();
                }
            }
        }

        #endregion

        #region Methods
        public void Invoke()
        {

            if (this.IsCancelled) return;

            try
            {
                this.eventAction();
            }
            catch (Exception ex)
            {
                this.onError?.Invoke(ex);
            }

            this.postTriggered(this.ID);
        }

        #endregion

        #region private

        private void Cancel()
        {
            if (this.timer is not null)
            {
                this.timer.Enabled = false;
                this.timer.Dispose();
            }
            this.iscanceledField = true;
        }

        #endregion
    }
}
