using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Niconicome.Models.Domain.Utils;
using MaterialDesign = MaterialDesignThemes.Wpf;

namespace Niconicome.ViewModels.Controls
{
    static class MaterialMessageBox
    {
        /// <summary>
        /// メッセージボックスを表示する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static async Task<MaterialMessageBoxResult> Show(string message, MessageBoxButtons buttons, MessageBoxIcons icon, string identifier = "default")
        {
            var dialog = new Niconicome.Views.Controls.MessageBox()
            {
                DataContext = new MessageBoxViewModel(message, buttons, icon)
            };

            object? result = await DialogHost.Show(dialog, identifier);

            return (MaterialMessageBoxResult?)result ?? MaterialMessageBoxResult.No;
        }
    }

    class MessageBoxViewModel : BindableBase
    {
        public MessageBoxViewModel()
        {
            this.Message = "これはメッセージボックスのテストです。";
            this.IconKind = MaterialDesign::PackIconKind.HelpCircleOutline;
            this.IconColor = Utils.ConvertToBrush("#42a5f5");
            this.OKVisibility = Visibility.Visible;
            this.YesVisibility = Visibility.Collapsed;
            this.NoVisibility = Visibility.Collapsed;
            this.CancelVisibility = Visibility.Collapsed;
        }

        public MessageBoxViewModel(string message, MessageBoxButtons messageBoxButtons, MessageBoxIcons messageBoxIcons)
        {
            this.Message = message;

            this.IconKind = messageBoxIcons switch
            {
                MessageBoxIcons.Question => MaterialDesign::PackIconKind.HelpCircleOutline,
                MessageBoxIcons.Caution => MaterialDesign::PackIconKind.AlertOutline,
                _ => MaterialDesign::PackIconKind.AlertCircleOutline
            };

            this.IconColor = messageBoxIcons switch
            {

                MessageBoxIcons.Question => Utils.ConvertToBrush("#42a5f5"),
                MessageBoxIcons.Caution => Utils.ConvertToBrush("#ffa500"),
                _ => Utils.ConvertToBrush("#d4000"),
            };

            if (messageBoxButtons.HasFlag(MessageBoxButtons.Yes))
            {
                this.YesVisibility = Visibility.Visible;
            }
            else
            {
                this.YesVisibility = Visibility.Collapsed;
            }

            if (messageBoxButtons.HasFlag(MessageBoxButtons.No))
            {
                this.NoVisibility = Visibility.Visible;
            }
            else
            {
                this.NoVisibility = Visibility.Collapsed;
            }

            if (messageBoxButtons.HasFlag(MessageBoxButtons.OK))
            {
                this.OKVisibility = Visibility.Visible;
            }
            else
            {
                this.OKVisibility = Visibility.Collapsed;
            }

            if (messageBoxButtons.HasFlag(MessageBoxButtons.Cancel))
            {
                this.CancelVisibility = Visibility.Visible;
            }
            else
            {
                this.CancelVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// メニューアイコン
        /// </summary>
        public MaterialDesign::PackIconKind IconKind { get; init; }

        public System.Windows.Media.Brush IconColor { get; init; }

        /// <summary>
        /// Yesボタン
        /// </summary>
        public Visibility YesVisibility { get; init; }

        /// <summary>
        /// Noボタン
        /// </summary>
        public Visibility NoVisibility { get; init; }

        /// <summary>
        /// OKボタン
        /// </summary>
        public Visibility OKVisibility { get; init; }

        /// <summary>
        /// Cancelボタン
        /// </summary>
        public Visibility CancelVisibility { get; init; }

    }

    [Flags]
    enum MessageBoxButtons
    {
        Yes = 1,
        No = 2,
        OK = 4,
        Cancel = 8,
    }

    enum MessageBoxIcons
    {
        Question,
        Caution,
        Error
    }

    enum MaterialMessageBoxResult
    {
        Yes,
        No,
        OK,
        Cancel
    }
}
