using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Utils.InitializeAwaiter
{
    public interface IInitializeAwaiter
    {

        /// <summary>
        /// 指定した名前の待機タスクを取得
        /// </summary>
        /// <param name="awaiterName"></param>
        /// <returns></returns>
        Task Awaiter { get; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 待機するクラスを追加
        /// </summary>
        /// <param name="stepsType"></param>

        IAttemptResult RegisterStep(Type stepsType);

        /// <summary>
        /// 処理の完了を通知
        /// </summary>
        /// <param name="awaiterName"></param>
        /// <param name="stepsType"></param>

        void NotifyCompletedStep(Type stepsType);
    }

    public class InitializeAwaiter : IInitializeAwaiter
    {
        #region field

        private readonly List<Type> _steps = new();

        private TaskCompletionSource _tcs = new();

        #endregion

        #region Props

        public Task Awaiter
        {
            get
            {
                this.CheckIfCompleted();
                return this._tcs.Task;
            }
        }

        public bool IsCompleted { get; private set; } = false;

        #endregion

        #region Method

        public IAttemptResult RegisterStep(Type stepsType)
        {
            if (this.Contains(stepsType))
            {
                return AttemptResult.Fail("すでにステップが登録されています。");
            }

            this._steps.Add(stepsType);

            return AttemptResult.Succeeded();
        }

        public void NotifyCompletedStep(Type stepsType)
        {
            if (!this.Contains(stepsType))
            {
                return;
            }

            this._steps.RemoveAll(s => s == stepsType);

            this.CheckIfCompleted();

        }

        #endregion

        #region private

        /// <summary>
        /// タスクが完了していた場合、プロパティーを書き換える
        /// </summary>
        private void CheckIfCompleted()
        {
            if (this._steps.Count == 0)
            {
                this.IsCompleted = true;
                this._tcs.TrySetResult();
            }
        }

        /// <summary>
        /// ステップを検索
        /// </summary>
        /// <param name="stepsType"></param>
        /// <returns></returns>
        private bool Contains(Type stepsType)
        {
            return this._steps.Contains(stepsType);
        }

        #endregion
    }
}
