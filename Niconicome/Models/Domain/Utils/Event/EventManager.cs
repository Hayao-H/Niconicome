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
        /// <returns>イベントID</returns>
        string Regster(Action eventAction, DateTime eventTtiggeredDT, Action<Exception>? onError = null);

        /// <summary>
        /// 指定したイベントをキャンセルする
        /// </summary>
        void Cancel(string id);

        /// <summary>
        /// 全てのイベントをキャンセルする
        /// </summary>
        void CancelAll();
    }

    public class EventManager : IEventManager
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
