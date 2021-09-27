using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Settings;
using MaterialDesign = MaterialDesignThemes.Wpf;

namespace Niconicome.Models.Local.State
{
    public interface ISnackbarHandler
    {
        /// <summary>
        /// キュー
        /// </summary>
        MaterialDesign.ISnackbarMessageQueue Queue { get; }

        /// <summary>
        /// キューに追加する
        /// </summary>
        /// <param name="message"></param>
        void Enqueue(string message);

        /// <summary>
        /// アクション付きでキューに追加する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="action"></param>
        /// <param name="actionFunc"></param>
        void Enqueue(string message, string action, Action actionFunc);

        /// <summary>
        /// 新しいインスタンスを返す
        /// </summary>
        /// <returns></returns>
        ISnackbarHandler CreateNewHandler();
    }

    public class SnackbarHandler : ISnackbarHandler
    {
        public SnackbarHandler(ILocalSettingHandler settingHandler)
        {
            this._settingHandler = settingHandler;
            this.Initialize();
        }

        #region field

        private readonly ILocalSettingHandler _settingHandler;

        private bool _isInitialized;

        private double _snackBarduration;

        #endregion

        #region Props
        public MaterialDesign::ISnackbarMessageQueue Queue { get; init; } = new MaterialDesign::SnackbarMessageQueue();

        #endregion

        public void Enqueue(string message)
        {
            this.Queue.Enqueue(message, null, null, null, false, false, TimeSpan.FromMilliseconds(this._snackBarduration));
        }

        public void Enqueue(string message, string action, Action actionFunc)
        {
            this.Queue.Enqueue(message, action, _ => actionFunc(), null, false, false, TimeSpan.FromMilliseconds(this._snackBarduration));
        }

        public ISnackbarHandler CreateNewHandler()
        {
            return new SnackbarHandler(this._settingHandler);
        }

        #region private

        private void Initialize()
        {
            if (this._isInitialized) return;

            int settingVal = this._settingHandler.GetIntSetting(SettingsEnum.SnackbarDuration);
            if (settingVal <= 0)
            {
                settingVal = 1000;
            }
            this._snackBarduration = settingVal;

            this._isInitialized = true;
        }

        #endregion

    }
}
