using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils;
using Err = Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Local.Application;
using Niconicome.ViewModels;
using Niconicome.Views;
using Niconicome.Views.AddonPage.V2;
using Niconicome.Views.Controls.MVVM;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Niconicome.Models.Domain.Utils.StringHandler;

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
            containerRegistry.RegisterDialog<CommonMessageBox>(nameof(CommonMessageBox));
            containerRegistry.RegisterDialog<DownloadTasksWindows>(nameof(DownloadTasksWindows));
            containerRegistry.RegisterDialog<MainManager>(nameof(MainManager));
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

            if (e.Exception is not null)
            {
                Err::IErrorHandler errorHandler = DIFactory.Resolve<Err::IErrorHandler>();
                errorHandler.HandleError(AppError.UIThreadError, e.Exception);
            }

            if (this.ConfirmUnhandledException(DIFactory.Resolve<IStringHandler>().GetContent(AppString.UIThread)))
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

            if (e.Exception is not null)
            {
                Err::IErrorHandler errorHandler = DIFactory.Resolve<Err::IErrorHandler>();
                errorHandler.HandleError(AppError.BackgroundTaskError, e.Exception);
            }

            if (this.ConfirmUnhandledException(DIFactory.Resolve<IStringHandler>().GetContent(AppString.Background)))
            {
                e?.SetObserved();
            }
            else
            {
                Environment.Exit(1);
            }
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

            Err::IErrorHandler errorHandler = DIFactory.Resolve<Err::IErrorHandler>();
            string message;

            if (e.ExceptionObject is Exception exception)
            {
                errorHandler.HandleError(AppError.UnhandledError, exception);
                message = errorHandler.GetMessageForResult(AppError.UnhandledError, exception);
            }
            else
            {
                message = errorHandler.GetMessageForResult(AppError.UnhandledError);
            }

            MessageBox.Show(message, DIFactory.Resolve<IStringHandler>().GetContent(AppString.MessageBoxCaptionUnknown), MessageBoxButton.OK, MessageBoxImage.Stop);
            Environment.Exit(1);
        }

        /// <summary>
        /// 実行を継続するかどうかを選択できる場合の未処理例外を処理
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        bool ConfirmUnhandledException(string captionDetail)
        {
            IStringHandler stringHandler = DIFactory.Resolve<IStringHandler>();
            string message = stringHandler.GetContent(AppString.Confirm);
            string caption = stringHandler.GetContent(AppString.MessageBoxCaption, captionDetail);

            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }
    }

    public enum AppError
    {
        [Err::ErrorEnum(Err::ErrorLevel.Error, "UIスレッドでエラーが発生しました。")]
        UIThreadError,
        [Err::ErrorEnum(Err::ErrorLevel.Error, "非同期タスクの実行中にエラーが発生しました。")]
        BackgroundTaskError,
        [Err::ErrorEnum(Err::ErrorLevel.Error, "致命的なエラーが発生しました。")]
        UnhandledError,
    }

    public enum AppString
    {
        [StringEnum("予期せぬエラーが発生しました。続けて発生する場合は開発者に報告してください。\nプログラムの実行を継続しますか？")]
        Confirm,
        [StringEnum("バックグランドタスク")]
        Background,
        [StringEnum("UIスレッド")]
        UIThread,
        [StringEnum("未処理例外 ({0})")]
        MessageBoxCaption,
        [StringEnum("未処理例外")]
        MessageBoxCaptionUnknown,
    }

}
