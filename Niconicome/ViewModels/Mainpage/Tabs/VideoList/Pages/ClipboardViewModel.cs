using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class ClipboardViewModel : IDisposable
    {
        public ClipboardViewModel()
        {
            WS.ClipbordManager.RegisterClipboardChangeHandler(this.OnChange);
        }

        #region Props

        public Bindables Bindables { get; init; } = new();

        public IReadonlyBindablePperty<bool> IsMonitoring => WS.ClipbordManager.IsMonitoring;

        public IBindableProperty<string> ClipboardContent { get; init; } = new BindableProperty<string>(string.Empty);

        #endregion

        #region Method

        public void SwitchMonitoring()
        {
            if (this.IsMonitoring.Value)
            {
                WS.ClipbordManager.StopMonitoring();

                string message = WS.StringHandler.GetContent(ClipboardVMSC.StopMonitoring);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                WS.SnackbarHandler.Enqueue(message);
            }
            else
            {
                IAttemptResult result = WS.ClipbordManager.StartMonitoring();
                if (!result.IsSucceeded)
                {
                    string message = WS.StringHandler.GetContent(ClipboardVMSC.MonitoringFailed);
                    string messageD = result.Message ?? string.Empty;

                    WS.MessageHandler.AppendMessage(messageD, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                    WS.SnackbarHandler.Enqueue(message);
                }
                else
                {
                    string message = WS.StringHandler.GetContent(ClipboardVMSC.StartMonitoring);
                    WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                    WS.SnackbarHandler.Enqueue(message);
                }
            }
        }

        #endregion

        #region private

        private void OnChange(string x)
        {
            if (string.IsNullOrEmpty(x))
            {
                return;
            }

            this.ClipboardContent.Value = x;

        }

        #endregion

        public void Dispose()
        {
            WS.ClipbordManager.UnRegisterClipboardChangeHandler(this.OnChange);
        }
    }
}
