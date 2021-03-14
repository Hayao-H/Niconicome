using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesign = MaterialDesignThemes.Wpf;

namespace Niconicome.Models.Local.State
{
    interface ISnackbarHandler
    {
        MaterialDesign.ISnackbarMessageQueue Queue { get;}

        void Enqueue(string message);
    }

    class SnackbarHandler : ISnackbarHandler
    {
        public MaterialDesign::ISnackbarMessageQueue Queue { get; init; } = new MaterialDesign::SnackbarMessageQueue();

        /// <summary>
        /// キューに追加する
        /// </summary>
        /// <param name="message"></param>
        public void Enqueue(string message)
        {
            this.Queue.Enqueue(message);
        }
    }
}
