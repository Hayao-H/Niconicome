using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.Reactive.State
{
    public interface IStateChangeNotifyer : IBindable
    {
        /// <summary>
        /// 変更を監視
        /// </summary>
        /// <param name="handler"></param>
        void Subscribe(Action handler);

        /// <summary>
        /// 変更通知
        /// </summary>
        void RaiseChange();
    }

    public class StateChangeNotifyer : IStateChangeNotifyer
    {
        #region field

        private Action? _handler;

        #endregion

        #region Method

        public void RegisterPropertyChangeHandler(Action handler)
        {
            this._handler += handler;
        }

        public void UnRegisterPropertyChangeHandler(Action handler)
        {
            this._handler -= handler;
        }

        public void Subscribe(Action handler)
        {
            this.RegisterPropertyChangeHandler(handler);
        }

        public void RaiseChange()
        {
            try
            {
                this._handler?.Invoke();
            }
            catch { }
        }

        #endregion

        public void Dispose()
        {
            this._handler = null;
        }
    }
}
