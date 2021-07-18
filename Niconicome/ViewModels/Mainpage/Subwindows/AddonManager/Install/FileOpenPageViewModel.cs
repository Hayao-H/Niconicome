using Microsoft.Win32;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Install
{
    class FileOpenPageViewModel : BindableBase
    {
        public FileOpenPageViewModel()
        {

            this.OpenFileCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    var dialog = new OpenFileDialog();
                    dialog.Filter = "アドオンファイル(*.zip)|*.zip|すべてのファイル|*";
                    if (dialog.ShowDialog() == true)
                    {
                        this.AddonPath.Value = dialog.FileName;
                    }
                });
        }

        #region field


        #endregion

        #region Prop

        public ReactiveProperty<string> AddonPath { get; init; } = new();

        #endregion

        #region Command

        public ReactiveCommand OpenFileCommand { get; init; }

        #endregion
    }

    class FileOpenPageViewModelD : InstallPageBase
    {
        public ReactiveProperty<string> AddonPath { get; init; } = new(@"path\to\addon");

        public ReactiveCommand OpenFileCommand { get; init; } = new();


    }
}
