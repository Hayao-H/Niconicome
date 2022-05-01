using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
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
        public SnackbarHandler(ILocalSettingsContainer container)
        {
            this._settingsContainer = container;
            this.Initialize();
        }

        #region field

        private readonly ILocalSettingsContainer _settingsContainer;

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
            return new SnackbarHandler(this._settingsContainer);
        }

        #region private

        private void Initialize()
        {
            if (this._isInitialized) return;

            this._settingsContainer.GetReactiveIntSetting(SettingsEnum.SnackbarDuration, null, value => value < 0 ? LocalConstant.DefaultSnackbarDuration : value).Subscribe(this.OnDurationChanged);

            this._isInitialized = true;
        }

        private void OnDurationChanged(int value)
        {
            int settingVal = value;
            if (settingVal <= 0)
            {
                settingVal = LocalConstant.DefaultSnackbarDuration;
            }

            this._snackBarduration = settingVal;
        }

        #endregion

    }
}
