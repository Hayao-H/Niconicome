using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using Niconicome.Models.Auth;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.ViewModels.Setting.Utils;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class GeneralSettingsPageViewModel
    {
        public GeneralSettingsPageViewModel()
        {
            #region 自動ログイン
            this.IsAutologinEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.IsAutologinEnable,false), false);

            var normal = new ComboboxItem<string>(AutoLoginTypeString.Normal, "パスワードログイン");
            var wv2 = new ComboboxItem<string>(AutoLoginTypeString.Webview2, "Webview2とCookieを共有");
            var firefox = new ComboboxItem<string>(AutoLoginTypeString.Firefox, "FirefoxとCookieを共有");
            var storeFirefox = new ComboboxItem<string>(AutoLoginTypeString.StoreFirefox, "Store版FirefoxとCookieを共有");

            this.SelectableAutoLoginTypes = new List<ComboboxItem<string>>() { normal, wv2, firefox, storeFirefox };

            //自動ログインのタイプ
            this.SelectedAutoLoginType = new ConvertableSettingInfoViewModel<string, ComboboxItem<string>>(WS::SettingPage.SettingsConainer.GetSetting<string>(SettingNames.AutoLoginMode,AutoLoginTypeString.Normal), normal, x => x switch
               {
                   AutoLoginTypeString.Normal => normal,
                   AutoLoginTypeString.Webview2 => wv2,
                   AutoLoginTypeString.Firefox => firefox,
                   AutoLoginTypeString.StoreFirefox => storeFirefox,
                   _ => normal,
               }, x => x.Value);


            this.SelectedAutoLoginType.RegisterPropChangeHandler(x =>
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
            this.SelectedFirefoxProfileName = new ConvertableSettingInfoViewModel<string, string>(WS::SettingPage.SettingsConainer.GetSetting<string>(SettingNames.FirefoxProfileName,""), "", profile => this.SelectableFirefoxProfiles.FirstOrDefault(p => p == profile) ?? "", x => x);

            //Firefoxのプロファイル選択肢を表示するかどうか
            this.SelectedAutoLoginType.RegisterPropChangeHandler(value =>
            {
                if (value.Value == AutoLoginTypeString.Firefox || value.Value == AutoLoginTypeString.StoreFirefox)
                {
                    this.DisplayFirefoxPrifile.Value = true;
                }
                else
                {
                    this.DisplayFirefoxPrifile.Value = false;
                }
            });
            #endregion


            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");
            var n5 = new ComboboxItem<int>(5, "5");

            this.SelectablefetchSleepInterval = new List<ComboboxItem<int>> { n1, n2, n3, n4, n5 };
            this.SelectableMaxParallelFetch = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };

            this.MaxFetchParallelCount = new ConvertableSettingInfoViewModel<int, ComboboxItem<int>>(WS::SettingPage.SettingsConainer.GetSetting<int>(SettingNames.MaxParallelFetchCount,4), n4, x => x switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                _ => n4,
            }, x => x.Value);

            this.FetchSleepInterval = new ConvertableSettingInfoViewModel<int, ComboboxItem<int>>(WS::SettingPage.SettingsConainer.GetSetting<int>(SettingNames.FetchSleepInterval,4), n4, x => x switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                5 => n5,
                _ => n4,
            }, x => x.Value);

            this.IsSkippingSSLVerificationEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.SkipSSLVerification,false), false);
            this.IsExpandallPlaylistsEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.ExpandTreeOnStartUp,false), false);
            this.IsSavePrevPlaylistExpandedStateEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.SaveTreePrevExpandedState, false), false);
            this.IsStoreOnlyNiconicoIDEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.StoreOnlyNiconicoIDOnRegister, false), false);
            this.IsAutoRenamingRemotePlaylistEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.AutoRenamingAfterSetNetworkPlaylist, false), false);
            this.IsSingletonWindowsEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.LimitWindowsToSingleton, false), false);
            this.IsConfirmngIfDownloadingEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.ConfirmIfDownloading, false), false);
            this.SnackbarDuration = new ConvertableSettingInfoViewModel<int, int>(WS::SettingPage.SettingsConainer.GetSetting<int>(SettingNames.SnackbarDuration, LocalConstant.DefaultSnackbarDuration), LocalConstant.DefaultSnackbarDuration, value => value <= 0 ? LocalConstant.DefaultSnackbarDuration : value, value => value <= 0 ? LocalConstant.DefaultSnackbarDuration : value);
            this.IsShowingTasksAsTabEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.ShowDownloadTasksAsTab, false), false);
        }

        #region Props

        /// <summary>
        /// 選択可能な自動ログインの種別
        /// </summary>
        public List<ComboboxItem<string>> SelectableAutoLoginTypes { get; init; }

        /// <summary>
        /// 自動ログインの種別
        /// </summary>
        public ConvertableSettingInfoViewModel<string, ComboboxItem<string>> SelectedAutoLoginType { get; init; }

        /// <summary>
        /// Firefoxのプロファイル
        /// </summary>
        public ConvertableSettingInfoViewModel<string, string> SelectedFirefoxProfileName { get; init; }

        /// <summary>
        /// Firefoxのプロファイルを表示するかどうか
        /// </summary>
        public ReactiveProperty<bool> DisplayFirefoxPrifile { get; init; } = new();

        /// <summary>
        /// タスク一覧をタブ表示
        /// </summary>
        public SettingInfoViewModel<bool> IsShowingTasksAsTabEnable { get; init; }

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
        public SettingInfoViewModel<bool> IsAutologinEnable { get; init; }

        /// <summary>
        /// 動画情報最大並列取得数
        /// </summary>
        public ConvertableSettingInfoViewModel<int, ComboboxItem<int>> MaxFetchParallelCount { get; init; }

        /// <summary>
        /// 待機間隔
        /// </summary>
        public ConvertableSettingInfoViewModel<int, ComboboxItem<int>> FetchSleepInterval { get; init; }

        /// <summary>
        /// SSL証明書の検証をスキップする
        /// </summary>
        public SettingInfoViewModel<bool> IsSkippingSSLVerificationEnable { get; init; }

        /// <summary>
        /// 展開状況を保存する
        /// </summary>
        public SettingInfoViewModel<bool> IsSavePrevPlaylistExpandedStateEnable { get; init; }

        /// <summary>
        /// すべて展開する
        /// </summary>
        public SettingInfoViewModel<bool> IsExpandallPlaylistsEnable { get; init; }

        /// <summary>
        /// ニコニコ動画のIDのみを保存する
        /// </summary>
        public SettingInfoViewModel<bool> IsStoreOnlyNiconicoIDEnable { get; init; }

        /// <summary>
        /// 自動リネーム
        /// </summary>
        public SettingInfoViewModel<bool> IsAutoRenamingRemotePlaylistEnable { get; init; }

        /// <summary>
        /// DL中は終了時に確認する
        /// </summary>
        public SettingInfoViewModel<bool> IsConfirmngIfDownloadingEnable { get; init; }


        /// <summary>
        /// マルチウィンドウ禁止
        /// </summary>
        public SettingInfoViewModel<bool> IsSingletonWindowsEnable { get; init; }

        /// <summary>
        /// スナックバー表示時間
        /// </summary>
        [RegularExpression(@"^\d$", ErrorMessage = "整数値を入力してください。")]
        public ConvertableSettingInfoViewModel<int, int> SnackbarDuration { get; init; }

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
            this.SelectedAutoLoginType = new ConvertableSettingInfoViewModelD<ComboboxItem<string>>(al);

            var ffp = "Hello_World";
            this.SelectableFirefoxProfiles = new List<string>() { ffp };
            this.SelectedFirefoxProfileName = new  SettingInfoViewModelD<string>(ffp);

            this.MaxFetchParallelCount =new ConvertableSettingInfoViewModelD<ComboboxItem<int>>(n3);
            this.FetchSleepInterval = new ConvertableSettingInfoViewModelD<ComboboxItem<int>>(n5);
        }

        public SettingInfoViewModelD<bool> IsAutologinEnable { set; get; } = new(true);

        public SettingInfoViewModelD<bool> IsSkippingSSLVerificationEnable { get; set; } = new(false);

        public SettingInfoViewModelD<bool> IsSavePrevPlaylistExpandedStateEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsExpandallPlaylistsEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsStoreOnlyNiconicoIDEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsAutoRenamingRemotePlaylistEnable { get; set; } = new(true);

        public List<string> SelectableFirefoxProfiles { get; init; }

        public List<ComboboxItem<string>> SelectableAutoLoginTypes { get; init; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<string>> SelectedAutoLoginType { get; init; }

        public SettingInfoViewModelD<string> SelectedFirefoxProfileName { get; init; }

        public SettingInfoViewModelD<bool> DisplayFirefoxPrifile { get; init; } = new(true);

        public SettingInfoViewModelD<bool> IsSingletonWindowsEnable { get; init; } = new(true);

        public SettingInfoViewModelD<bool> IsConfirmngIfDownloadingEnable { get; init; } = new(true);

        public SettingInfoViewModelD<bool> IsShowingTasksAsTabEnable { get; init; } = new(true);

        public SettingInfoViewModelD<int> SnackbarDuration { get; init; } = new(10000);

        public List<ComboboxItem<int>> SelectableMaxParallelFetch { get; init; }

        public List<ComboboxItem<int>> SelectablefetchSleepInterval { get; init; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<int>> MaxFetchParallelCount { get; set; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<int>> FetchSleepInterval { get; set; }

    }
}
