using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Utils.Event
{
    public interface IEventManager
    {
        /// <summary>
        /// イベントを登録する
        /// </summary>
        /// <returns></returns>
        string Regster(Action eventAction, DateTime eventTtiggeredDT, Action<Exception>? onError = null);

        /// <summary>
        /// イベントを登録して呼び出し関数を取得する
        /// </summary>
        /// <param name="eventAction"></param>
        /// <param name="onError"></param>
        /// <returns>呼び出し関数</returns>
        Action RegisterAndGetCaller(Action eventAction, Action<Exception>? onError = null);

        /// <summary>
        /// 指定したイベントをキャンセルする
        /// </summary>
        void Cancel(string id);

        /// <summary>
        /// 全てのイベントをキャンセルする
        /// </summary>
        void CancelAll();
    }

    class EventManager : IEventManager
    {

        #region field

        private readonly Dictionary<string, IEvent> events = new();

        private readonly object lockObj = new();

        #endregion

        #region Methods

        public string Regster(Action eventAction, DateTime eventTtiggeredDT, Action<Exception>? onError = null)
        {
            lock (this.lockObj)
            {
                var ev = new Event(eventAction, id => this.Remove(id), eventTtiggeredDT, onError);
                this.events.Add(ev.ID, ev);

                return ev.ID;
            }
        }

        public Action RegisterAndGetCaller(Action eventAction, Action<Exception>? onError = null)
        {
            lock (this.lockObj)
            {
                var ev = new Event(eventAction, id => this.Remove(id), onError: onError);
                this.events.Add(ev.ID, ev);

                return () => ev.Invoke();
            }
        }

        public void Cancel(string id)
        {
            this.events.TryGetValue(id, out IEvent? value);

            if (value is not null)
            {
                value.IsCancelled = true;
                this.Remove(id);
            }
        }

        public void CancelAll()
        {
            lock (this.lockObj)
            {
                foreach (var ev in events.Values)
                {
                    ev.IsCancelled = true;
                }

                this.events.Clear();
            }
        }
        #endregion

        #region private

        /// <summary>
        /// 指定したイベントの参照を切る
        /// </summary>
        /// <param name="id"></param>
        private void Remove(string id)
        {
            if (this.events.ContainsKey(id))
            {
                this.events.Remove(id);
            }
        }

        #endregion
    }
}
