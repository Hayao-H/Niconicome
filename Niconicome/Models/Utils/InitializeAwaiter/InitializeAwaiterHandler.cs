using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Utils.InitializeAwaiter
{

    public interface IInitializeAwaiterHandler
    {
        /// <summary>
        /// ステップを登録
        /// </summary>
        /// <param name="awaiterName"></param>
        /// <param name="stepsType"></param>
        /// <returns></returns>
        IAttemptResult RegisterStep(string awaiterName, Type stepsType);

        /// <summary>
        /// ステップの完了を通知
        /// </summary>
        /// <param name="awaiterName"></param>
        /// <param name="stepstype"></param>

        void NotifyCompletedStep(string awaiterName, Type stepstype);

        /// <summary>
        /// 待機オブジェクトを取得
        /// </summary>
        /// <param name="awaiterName"></param>
        /// <returns></returns>
        Task GetAwaiter(string awaiterName); 
    }

    public class InitializeAwaiterHandler : IInitializeAwaiterHandler
    {
        #region field

        private Dictionary<string, IInitializeAwaiter> _awaiters = new();

        #endregion

        public IAttemptResult RegisterStep(string awaiterName, Type stepsType)
        {
            if (!this._awaiters.ContainsKey(awaiterName))
            {
                this._awaiters.Add(awaiterName, new InitializeAwaiter());
            }

            IInitializeAwaiter awaiter = this._awaiters[awaiterName];
            return awaiter.RegisterStep(stepsType);
        }

        public void NotifyCompletedStep(string awaiterName, Type stepsType)
        {
            if (!this._awaiters.ContainsKey(awaiterName))
            {
                return;
            }


            IInitializeAwaiter awaiter = this._awaiters[awaiterName];
            awaiter.NotifyCompletedStep(stepsType);

            if (awaiter.IsCompleted)
            {
                this._awaiters.Remove(awaiterName);
            }
        }

        public Task GetAwaiter(string awaiterName)
        {
            if (this._awaiters.ContainsKey(awaiterName))
            {
                return this._awaiters[awaiterName].Awaiter;
            }else
            {
                var tcs = new TaskCompletionSource();
                tcs.TrySetResult();
                return tcs.Task;
            }
        }



    }
}
