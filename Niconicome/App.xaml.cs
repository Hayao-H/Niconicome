using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Application;
using Niconicome.ViewModels;
using Niconicome.Views.AddonPage;
using Niconicome.Views.AddonPage.Install;
using Niconicome.Views.Controls.MVVM;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using AM = Niconicome.Views.AddonPage;

namespace Niconicome
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {
            ///ソースコードはhttps://mseeeen.msen.jp/wpf-handle-unhandled-exceptions/様よりお借りしました。
            DispatcherUnhandledException += this.App_DispatcherUnhandledException;
#pragma warning disable CS8622
            TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;
#pragma warning restore CS8622
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
            this.RunStartUpTask();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<AddonManagerWindow>(nameof(AddonManagerWindow));
            containerRegistry.RegisterDialog<AddonInstallWindow>(nameof(AddonInstallWindow));
            containerRegistry.RegisterDialog<CommonMessageBox>(nameof(CommonMessageBox));
            containerRegistry.RegisterForNavigation<FileOpenPage>();
            containerRegistry.RegisterForNavigation<AddonLoadPage>();
            containerRegistry.RegisterForNavigation<AddonInstallPage>();
            containerRegistry.RegisterForNavigation<AM::Pages.MainPage>();
        }

        /// <summary>
        /// ViewModelAttricute属性でVMを定義できるようにする
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(vType =>
            {
                var vm = vType.GetTypeInfo().GetCustomAttribute<ViewModelAttribute>();
                return vm?.ViewModelType;
            });
        }

        public void RunStartUpTask()
        {
            var setup = DIFactory.Provider.GetRequiredService<IStartUp>();
            setup.RunStartUptasks();
        }

        /// <summary>
        /// UI スレッドで発生した未処理例外を処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //taskcanceledは握りつぶす
            if (e.Exception is TaskCanceledException) return;
            var logger = DIFactory.Provider.GetRequiredService<ILogger>();
            var exception = e.Exception as Exception;
            if (exception is not null)logger.Error("UIスレッドで例外が発生しました。", exception);
            if (this.ConfirmUnhandledException(exception, "UI スレッド"))
            {
                e.Handled = true;
            }
            else
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// バックグラウンドタスクで発生した未処理例外を処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //taskcanceledは握りつぶす
            if (e.Exception?.InnerException is TaskCanceledException) return;
            var logger = DIFactory.Provider.GetRequiredService<ILogger>();
            var exception = e?.Exception?.InnerException as Exception;
            if (exception is not null) logger.Error("バックグラウンドタスクの実行中にエラーが発生しました。", exception);
            if (this.ConfirmUnhandledException(exception, "バックグラウンドタスク"))
            {
                e?.SetObserved();
            }
            else
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// 実行を継続するかどうかを選択できる場合の未処理例外を処理
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        bool ConfirmUnhandledException(Exception? e, string sourceName)
        {
            var message = $"予期せぬエラーが発生しました。続けて発生する場合は開発者に報告してください。\nプログラムの実行を継続しますか？";
            if (e is not null) message += $"\n({e.Message} @ {e.TargetSite?.Name})";
            var result = MessageBox.Show(message, $"未処理例外 ({sourceName})", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// 最終的に処理されなかった未処理例外を処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //taskcanceledは握りつぶす
            if (e.ExceptionObject is TaskCanceledException) return;
            var logger = DIFactory.Provider.GetRequiredService<ILogger>();
            var message = $"致命的なエラーが発生しました。続けて発生する場合は開発者に報告してください。";
            if (e.ExceptionObject is Exception exception)
            {
                message += $"\n({exception.Message} @ {exception.TargetSite?.Name})";
                logger.Error("致命的なエラーが発生しました。", exception);
            }
            MessageBox.Show(message, "未処理例外", MessageBoxButton.OK, MessageBoxImage.Stop);
            Environment.Exit(1);
        }
    }
}
