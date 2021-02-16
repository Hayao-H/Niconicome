using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Extensions.System.Windows;
using Niconicome.Models.Domain.Utils;
using MaterialDesign = MaterialDesignThemes.Wpf;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting
{
    class SettingMainViewModel : BindableBase
    {

        public SettingMainViewModel()
        {
            this.pageUriField = new Uri("/Views/Setting/Pages/EmptyPage.xaml", UriKind.Relative);
            this.SnackbarMessageQueue = WS::SettingPage.SnackbarMessageQueue;
        }

        private Uri pageUriField;

        /// <summary>
        /// ページのURI
        /// </summary>
        public Uri PageUri { get => this.pageUriField; set => this.SetProperty(ref this.pageUriField, value); }

        /// <summary>
        /// ページ遷移
        /// </summary>
        /// <param name="page"></param>
        public void Navigate(SettingPages page)
        {
            string uri = page switch
            {
                SettingPages.FileSettings => "/Views/Setting/Pages/FileSettingsPage.xaml",
                SettingPages.ExternalSoftwareSettings => "/Views/Setting/Pages/ExternalSoftwareSettingsPage.xaml",
                SettingPages.DebugSettings => "/Views/Setting/Pages/DebugSettingsPage.xaml",
                SettingPages.Restore => "/Views/Setting/Pages/RestorePage.xaml",
                SettingPages.ApplicationInfo => "/Views/Setting/Pages/AppinfoPage.xaml",
                SettingPages.Import => "/Views/Setting/Pages/ImportPage.xaml",
                _ => "/Views/Setting/Pages/EmptyPage.xaml",
            };
            this.PageUri = new Uri(uri, UriKind.Relative);
        }

        public MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }
    }

    /// <summary>
    /// ナビゲーションバーのビヘビアー
    /// </summary>
    class SettingNavigationBehavior : Behavior<StackPanel>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseLeftButtonDown += this.OnClick;

        }

        private void OnClick(object? sender, MouseEventArgs e)
        {
            if (sender is null || sender.AsNullable<StackPanel>() is not StackPanel panel || panel is null) return;

            //クリックした要素を取得
            Point pos = e.GetPosition(panel);
            var target = Utils.HitTest<TextBlock>(panel, pos);
            if (target is null) return;
            var targetBorder = target.GetParent<Border>();
            if (targetBorder is null) return;
            if (panel.DataContext is not SettingMainViewModel vm) return;
            if (WS::SettingPage.State.IsImportingFromXeno)
            {
                vm.SnackbarMessageQueue.Enqueue("インポート作業中のためページ遷移できません。");
                return;
            }

            this.ResetSelectedItem();
            targetBorder.BorderBrush = Utils.ConvertToBrush("#757575");
            this.currentSelected = targetBorder;

            vm.Navigate(this.GetPages(target.Text));
        }

        private Border? currentSelected;

        private void ResetSelectedItem()
        {
            if (this.currentSelected is null) return;
            this.currentSelected.BorderBrush = Brushes.Transparent;
        }

        /// <summary>
        /// 設定ページを取得する
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        private SettingPages GetPages(string settingName)
        {
            return settingName switch
            {
                "ファイル設定" => SettingPages.FileSettings,
                "外部ソフト設定" => SettingPages.ExternalSoftwareSettings,
                "URL設定" => SettingPages.UrlSettings,
                "デバッグ設定" => SettingPages.DebugSettings,
                "アプリ情報" => SettingPages.ApplicationInfo,
                "回復" => SettingPages.Restore,
                "インポート" => SettingPages.Import,
                _ => SettingPages.None
            };
        }

    }

    /// <summary>
    /// フレームのビヘビアー
    /// </summary>
    class SettingFrameBehavior : Behavior<Frame>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Navigated += this.OnNavigate;
        }

        private void OnNavigate(object? sender, EventArgs e)
        {
            if (this.AssociatedObject is null) return;
            while (this.AssociatedObject.CanGoBack)
            {
                this.AssociatedObject.RemoveBackEntry();
            }
        }
    }

    enum SettingPages
    {
        None,
        FileSettings,
        ExternalSoftwareSettings,
        UrlSettings,
        DebugSettings,
        ApplicationInfo,
        Import,
        Restore,
    }
}