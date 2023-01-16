using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.Reactive
{
    public interface IBindable : IDisposable
    {
        /// <summary>
        /// 値の変更を監視する
        /// </summary>
        /// <param name="handler"></param>
        void RegisterPropertyChangeHandler(Action handler);

        /// <summary>
        /// 値の変更の監視を停止する
        /// </summary>
        /// <param name="handler"></param>
        void UnRegisterPropertyChangeHandler(Action handler);

    }

    public class BindablePropertyBase : IBindable
    {
        #region field

        private readonly List<Action> _nonParamHandlers = new();

        #endregion

        #region Method

        public void RegisterPropertyChangeHandler(Action handler)
        {
            this._nonParamHandlers.Add(handler);
        }

        public void UnRegisterPropertyChangeHandler(Action handler)
        {
            this._nonParamHandlers.Remove(handler);
        }


        #endregion

        #region private

        protected void OnPropertyChanged()
        {
            try
            {
                foreach (var handler in this._nonParamHandlers)
                {
                    handler();
                }
            }
            catch { }
        }

        #endregion

        public virtual void Dispose()
        {
            this._nonParamHandlers.Clear();
        }

    }
}
