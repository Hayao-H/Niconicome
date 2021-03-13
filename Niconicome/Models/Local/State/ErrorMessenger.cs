using System;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.ViewModels.Controls;

namespace Niconicome.Models.Local.State
{
    public interface IErrorMessanger
    {
        void FireError(string message);
        event EventHandler<IErrorEventArgs> Error;
    }

    public interface IErrorEventArgs
    {
        string Message { get; }
    }

    /// <summary>
    /// エラー通知
    /// </summary>
    class ErrorMessenger : IErrorMessanger
    {

        public ErrorMessenger(Func<string, Task<MaterialMessageBoxResult>> showMessageBoxFunc)
        {
            this.showMessageBox = showMessageBoxFunc;
            this.Error += this.OnError;
        }

        public ErrorMessenger() : this((message) => MaterialMessageBox.Show(message, MessageBoxButtons.OK, MessageBoxIcons.Error)) { }

        public event EventHandler<IErrorEventArgs> Error;

        /// <summary>
        /// メッセージボックス(DI)
        /// </summary>
        private readonly Func<string, Task<MaterialMessageBoxResult>> showMessageBox;

        /// <summary>
        /// エラーを通知する
        /// </summary>
        /// <param name="message"></param>
        public void FireError(string message)
        {
            this.Error?.Invoke(this, new ErrorEventArgs(message));
        }

        public void OnError(object? sender, IErrorEventArgs e)
        {
            this.showMessageBox(e.Message);
        }

    }


    public class ErrorEventArgs : EventArgs, IErrorEventArgs
    {
        public ErrorEventArgs(string e)
        {
            this.Message = e;
        }

        public string Message { get; private set; }

    }
}
