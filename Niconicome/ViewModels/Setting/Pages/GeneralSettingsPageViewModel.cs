using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Auth;
using Niconicome.Models.Const;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels.Mainpage.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class GeneralSettingsPageViewModel : SettingaBase
    {
        public GeneralSettingsPageViewModel()
        {
            #region 自動ログイン
            this.IsAutologinEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.AutologinEnable);

            var normal = new ComboboxItem<string>(AutoLoginTypeString.Normal, "パスワードログイン");
            var wv2 = new ComboboxItem<string>(AutoLoginTypeString.Webview2, "Webview2とCookieを共有");
            var firefox = new ComboboxItem<string>(AutoLoginTypeString.Firefox, "FirefoxとCookieを共有");
            var storeFirefox = new ComboboxItem<string>(AutoLoginTypeString.StoreFirefox, "Store版FirefoxとCookieを共有");

            this.SelectableAutoLoginTypes = new List<ComboboxItem<string>>() { normal, wv2, firefox, storeFirefox };

            //自動ログインのタイプ
            this.SelectedAutoLoginType = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.AutologinMode).ToReactivePropertyAsSynchronized(x => x.Value, x => x switch
               {
                   AutoLoginTypeString.Normal => normal,
                   AutoLoginTypeString.Webview2 => wv2,
                   AutoLoginTypeString.Firefox => firefox,
                   AutoLoginTypeString.StoreFirefox => storeFirefox,
                   _ => normal,
               }, x => x.Value);


            this.SelectedAutoLoginType.Subscribe(x =>
            {
                this.SelectableFirefoxProfiles.Clear();
                if (x.Value == AutoLoginTypeString.Firefox)
                {
                    this.SelectableFirefoxProfiles.AddRange(WS::SettingPage.AutoLogin.GetFirefoxProfiles(AutoLoginType.Firefox).Select(y => y.ProfileName));
                }
                else if (x.Value == AutoLoginTypeString.StoreFirefox)
                {
                    this.SelectableFirefoxProfiles.AddRange(WS::SettingPage.AutoLogin.GetFirefoxProfiles(AutoLoginType.StoreFirefox).Select(y => y.ProfileName));
                }
            });
            var profile = WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.FFProfileName) ?? string.Empty;
            this.SelectedFirefoxProfileName = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.FFProfileName, this.SelectableFirefoxProfiles.FirstOrDefault(p => p == profile) ?? "");

            //Firefoxのプロファイル選択肢を表示するかどうか
            this.DisplayFirefoxPrifile = this.SelectedAutoLoginType
                .Select(value => value.Value == AutoLoginTypeString.Firefox || value.Value == AutoLoginTypeString.StoreFirefox)
                .ToReactiveProperty();
            #endregion


            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");
            var n5 = new ComboboxItem<int>(5, "5");

            this.SelectablefetchSleepInterval = new List<ComboboxItem<int>> { n1, n2, n3, n4, n5 };
            this.SelectableMaxParallelFetch = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };

            var maxFetchParallelCount = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.MaxFetchCount);
            var fetchSleepInterval = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.FetchSleepInterval);

            this.MaxFetchParallelCount = WS::SettingPage.SettingsContainer.GetReactiveIntSetting(SettingsEnum.MaxFetchCount).ToReactivePropertyAsSynchronized(x => x.Value, x => x switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                _ => n4,
            }, x => x.Value);

            this.FetchSleepInterval = WS::SettingPage.SettingsContainer.GetReactiveIntSetting(SettingsEnum.FetchSleepInterval).ToReactivePropertyAsSynchronized(x => x.Value, x => x switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                5 => n5,
                _ => n4,
            }, x => x.Value);

            this.IsSkippingSSLVerificationEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.SkipSSLVerification);
            this.IsExpandallPlaylistsEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.ExpandAll);
            this.IsSavePrevPlaylistExpandedStateEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.InheritExpandedState);
            this.IsStoreOnlyNiconicoIDEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.StoreOnlyNiconicoID);
            this.IsAutoRenamingRemotePlaylistEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.AutoRenameNetPlaylist);
            this.IsSingletonWindowsEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.SingletonWindows);
            this.IsConfirmngIfDownloadingEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.ConfirmIfDownloading);
            this.SnackbarDuration = WS::SettingPage.SettingsContainer.GetReactiveIntSetting(SettingsEnum.SnackbarDuration, null, value => value <= 0 ? LocalConstant.DefaultSnackbarDuration : value);
            this.IsShowingTasksAsTabEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.ShowTasksAsTab);
        }

        #region Props

        /// <summary>
        /// 選択可能な自動ログインの種別
        /// </summary>
        public List<ComboboxItem<string>> SelectableAutoLoginTypes { get; init; }

        /// <summary>
        /// 自動ログインの種別
        /// </summary>
        public ReactiveProperty<ComboboxItem<string>> SelectedAutoLoginType { get; init; }

        /// <summary>
        /// Firefoxのプロファイル
        /// </summary>
        public ReactiveProperty<string> SelectedFirefoxProfileName { get; init; }

        /// <summary>
        /// Firefoxのプロファイルを表示するかどうか
        /// </summary>
        public ReactiveProperty<bool> DisplayFirefoxPrifile { get; init; }

        /// <summary>
        /// タスク一覧をタブ表示
        /// </summary>
        public ReactiveProperty<bool> IsShowingTasksAsTabEnable { get; init; }

        /// <summary>
        /// 選択可能なFirefoxのプロファイル
        /// </summary>
        public ObservableCollection<string> SelectableFirefoxProfiles { get; init; } = new();

        /// <summary>
        /// 同時取得数
        /// </summary>
        public List<ComboboxItem<int>> SelectableMaxParallelFetch { get; init; }

        /// <summary>
        /// スリープ間隔
        /// </summary>
        public List<ComboboxItem<int>> SelectablefetchSleepInterval { get; init; }

        /// <summary>
        /// 自動ログイン
        /// </summary>
        public ReactiveProperty<bool> IsAutologinEnable { get; init; }

        /// <summary>
        /// 動画情報最大並列取得数
        /// </summary>
        public ReactiveProperty<ComboboxItem<int>> MaxFetchParallelCount { get; init; }

        /// <summary>
        /// 待機間隔
        /// </summary>
        public ReactiveProperty<ComboboxItem<int>> FetchSleepInterval { get; init; }

        /// <summary>
        /// SSL証明書の検証をスキップする
        /// </summary>
        public ReactiveProperty<bool> IsSkippingSSLVerificationEnable { get; init; }

        /// <summary>
        /// 展開状況を保存する
        /// </summary>
        public ReactiveProperty<bool> IsSavePrevPlaylistExpandedStateEnable { get; init; }

        /// <summary>
        /// すべて展開する
        /// </summary>
        public ReactiveProperty<bool> IsExpandallPlaylistsEnable { get; init; }

        /// <summary>
        /// ニコニコ動画のIDのみを保存する
        /// </summary>
        public ReactiveProperty<bool> IsStoreOnlyNiconicoIDEnable { get; init; }

        /// <summary>
        /// 自動リネーム
        /// </summary>
        public ReactiveProperty<bool> IsAutoRenamingRemotePlaylistEnable { get; init; }

        /// <summary>
        /// DL中は終了時に確認する
        /// </summary>
        public ReactiveProperty<bool> IsConfirmngIfDownloadingEnable { get; init; }


        /// <summary>
        /// マルチウィンドウ禁止
        /// </summary>
        public ReactiveProperty<bool> IsSingletonWindowsEnable { get; init; }

        /// <summary>
        /// スナックバー表示時間
        /// </summary>
        [RegularExpression(@"^\d$", ErrorMessage = "整数値を入力してください。")]
        public ReactiveProperty<int> SnackbarDuration { get; init; }

        #endregion

    }

    class GeneralSettingsPageViewModelD
    {

        public GeneralSettingsPageViewModelD()
        {
            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");
            var n5 = new ComboboxItem<int>(5, "5");

            this.SelectablefetchSleepInterval = new List<ComboboxItem<int>> { n1, n2, n3, n4, n5 };
            this.SelectableMaxParallelFetch = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };

            var al = new ComboboxItem<string>(AutoLoginTypeString.Normal, "パスワード");
            this.SelectableAutoLoginTypes = new List<ComboboxItem<string>>() { al };
            this.SelectedAutoLoginType = new ReactiveProperty<ComboboxItem<string>>(al);

            var ffp = "Hello_World";
            this.SelectableFirefoxProfiles = new List<string>() { ffp };
            this.SelectedFirefoxProfileName = new ReactiveProperty<string>(ffp);

            this.MaxFetchParallelCount = new ReactiveProperty<ComboboxItem<int>>(n3);
            this.FetchSleepInterval = new ReactiveProperty<ComboboxItem<int>>(n5);
        }

        public ReactiveProperty<bool> IsAutologinEnable { set; get; } = new(true);

        public ReactiveProperty<bool> IsSkippingSSLVerificationEnable { get; set; } = new(false);

        public ReactiveProperty<bool> IsSavePrevPlaylistExpandedStateEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsExpandallPlaylistsEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsStoreOnlyNiconicoIDEnable { get; set; } = new(true);

        public ReactiveProperty<bool> IsAutoRenamingRemotePlaylistEnable { get; set; } = new(true);

        public List<string> SelectableFirefoxProfiles { get; init; }

        public List<ComboboxItem<string>> SelectableAutoLoginTypes { get; init; }

        public ReactiveProperty<ComboboxItem<string>> SelectedAutoLoginType { get; init; }

        public ReactiveProperty<string> SelectedFirefoxProfileName { get; init; }

        public ReactiveProperty<bool> DisplayFirefoxPrifile { get; init; } = new(true);

        public ReactiveProperty<bool> IsSingletonWindowsEnable { get; init; } = new(true);

        public ReactiveProperty<bool> IsConfirmngIfDownloadingEnable { get; init; } = new(true);

        public ReactiveProperty<bool> IsShowingTasksAsTabEnable { get; init; } = new(true);

        public ReactiveProperty<int> SnackbarDuration { get; init; } = new();

        public List<ComboboxItem<int>> SelectableMaxParallelFetch { get; init; }

        public List<ComboboxItem<int>> SelectablefetchSleepInterval { get; init; }

        public ReactiveProperty<ComboboxItem<int>> MaxFetchParallelCount { get; set; }

        public ReactiveProperty<ComboboxItem<int>> FetchSleepInterval { get; set; }

    }
}
