using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Niconicome.Models.Domain.Utils;
using Niconicome.Views.Controls.MVVM;
using Prism.Services.Dialogs;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Controls
{
    class CommonMessageBoxViewModel : BindableBase, IDialogAware
    {
        public CommonMessageBoxViewModel()
        {
            this.CloseCommand = new ReactiveCommand<CommonMessageBoxButtons>()
                .WithSubscribe(param =>
                {
                    this.CloseWindows(param);
                });
        }

        #region private

        private void CloseWindows(CommonMessageBoxButtons button)
        {
            ButtonResult result = button switch
            {
                CommonMessageBoxButtons.Yes => ButtonResult.Yes,
                CommonMessageBoxButtons.No => ButtonResult.No,
                CommonMessageBoxButtons.OK => ButtonResult.OK,
                CommonMessageBoxButtons.Cancel => ButtonResult.Cancel,
                _ => ButtonResult.None
            };
            var dialogResult = new DialogResult(result);
            this.RequestClose?.Invoke(dialogResult);
        }

        #endregion

        #region IDialogAware

        public string Title { get; private set; } = "メッセージ";

        public event Action<IDialogResult>? RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            this.Message.Value = parameters.GetValue<string>(CommonMessageBoxAPI.MessageKey);
            this.IconKind.Value = parameters.GetValue<CommonMessageBoxAPI.MessageType>(CommonMessageBoxAPI.TypeKey) switch
            {
                CommonMessageBoxAPI.MessageType.Question => PackIconKind.HelpCircleOutline,
                CommonMessageBoxAPI.MessageType.Warinng => PackIconKind.AlertOutline,
                _ => PackIconKind.HelpCircleOutline,
            };
            this.IconColor.Value = parameters.GetValue<CommonMessageBoxAPI.MessageType>(CommonMessageBoxAPI.TypeKey) switch
            {
                CommonMessageBoxAPI.MessageType.Question => Utils.ConvertToBrush("#42a5f5"),
                CommonMessageBoxAPI.MessageType.Warinng => Utils.ConvertToBrush("#ffa500"),
                _ => Utils.ConvertToBrush("#42a5f5")
            };

            CommonMessageBoxButtons buttons = parameters.GetValue<CommonMessageBoxButtons>(CommonMessageBoxAPI.ButtonKey);

            if (buttons.HasFlag(CommonMessageBoxButtons.Yes))
            {
                this.YesVisibility.Value = true;
            }
            if (buttons.HasFlag(CommonMessageBoxButtons.No))
            {
                this.NoVisibility.Value = true;
            }
            if (buttons.HasFlag(CommonMessageBoxButtons.OK))
            {
                this.OKVisibility.Value = true;
            }
            if (buttons.HasFlag(CommonMessageBoxButtons.Cancel))
            {
                this.CancelVisibility.Value = true;
            }

            if (parameters.GetValue<CommonMessageBoxAPI.MessageType>(CommonMessageBoxAPI.TypeKey) == CommonMessageBoxAPI.MessageType.Warinng)
            {
                SystemSounds.Exclamation.Play();
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<CommonMessageBoxButtons> CloseCommand { get; init; }

        #endregion


        #region Props

        public ReactiveProperty<string> Message { get; init; } = new();

        public ReactiveProperty<PackIconKind> IconKind { get; init; } = new();

        public ReactiveProperty<Brush> IconColor { get; init; } = new();

        public ReactiveProperty<bool> YesVisibility { get; init; } = new();

        public ReactiveProperty<bool> NoVisibility { get; init; } = new();

        public ReactiveProperty<bool> OKVisibility { get; init; } = new();

        public ReactiveProperty<bool> CancelVisibility { get; init; } = new();
        #endregion
    }

    class CommonMessageBoxViewModelD
    {

        public ReactiveCommand<CommonMessageBoxButtons> CloseCommand { get; init; } = new();

        public ReactiveProperty<string> Message { get; init; } = new("テストメッセージ");

        public ReactiveProperty<PackIconKind> IconKind { get; init; } = new(PackIconKind.HelpCircleOutline);

        public ReactiveProperty<Brush> IconColor { get; init; } = new(Utils.ConvertToBrush("#42a5f5"));

        public ReactiveProperty<bool> YesVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> NoVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> OKVisibility { get; init; } = new(true);

        public ReactiveProperty<bool> CancelVisibility { get; init; } = new(true);
    }

    static class CommonMessageBoxAPI
    {
        /// <summary>
        /// メッセージボックスを表示する
        /// </summary>
        /// <param name="service">ダイアログサービス</param>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="type">ダイアログのタイプ</param>
        /// <param name="buttons">ボタン一覧</param>
        /// <returns></returns>
        public static IDialogResult Show(IDialogService service, string message, MessageType type, CommonMessageBoxButtons buttons)
        {
            var param = new DialogParameters();
            param.Add(MessageKey, message);
            param.Add(TypeKey, type);
            param.Add(ButtonKey, buttons);

            IDialogResult result = new DialogResult();

            service.ShowDialog(nameof(CommonMessageBox), param, r => result = r);

            return result;
        }

        public static readonly string MessageKey = "message";

        public static readonly string TypeKey = "type";

        public static readonly string ButtonKey = "button";

        public enum MessageType
        {
            Error,
            Warinng,
            Infomation,
            Question,
        }
    }

    [Flags]
    public enum CommonMessageBoxButtons
    {
        None = 0,
        Yes = 2,
        No = 4,
        OK = 8,
        Cancel = 16,

    }

}
