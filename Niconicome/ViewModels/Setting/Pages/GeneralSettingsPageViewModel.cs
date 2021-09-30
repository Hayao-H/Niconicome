using System;
using System.Collections.Generic;
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
            this.isAutologinEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.AutologinEnable);

            this.SelectableFirefoxProfiles = WS::SettingPage.AutoLogin.GetFirefoxProfiles().Select(p => p.ProfileName).ToList();
            var profile = WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.FFProfileName) ?? string.Empty;
            this.SelectedFirefoxProfileName = new ReactiveProperty<string?>(this.SelectableFirefoxProfiles.FirstOrDefault(p => p == profile));
            this.SelectedFirefoxProfileName.Subscribe(value =>
            {
                if (value is not null)
                {
                    this.SaveSetting(value, SettingsEnum.FFProfileName);
                }
            });

            var normal = new ComboboxItem<string>(AutoLoginTypeString.Normal, "パスワードログイン");
            var wv2 = new ComboboxItem<string>(AutoLoginTypeString.Webview2, "Webview2とCookieを共有");
            var firefox = new ComboboxItem<string>(AutoLoginTypeString.Firefox, "FirefoxとCookieを共有");

            this.SelectableAutoLoginTypes = new List<ComboboxItem<string>>() { normal, wv2, firefox };

            //自動ログインのタイプ
            var type = WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.AutologinMode);
            this.SelectedAutoLoginType = type switch
            {
                AutoLoginTypeString.Normal => new ReactiveProperty<ComboboxItem<string>>(normal),
                AutoLoginTypeString.Webview2 => new ReactiveProperty<ComboboxItem<string>>(wv2),
                AutoLoginTypeString.Firefox => new ReactiveProperty<ComboboxItem<string>>(firefox),
                _ => new ReactiveProperty<ComboboxItem<string>>(normal),
            };
            this.SelectedAutoLoginType.Subscribe(value =>
            {
                this.SaveSetting(value.Value, SettingsEnum.AutologinMode);
            }).AddTo(this.disposables);

            //Firefoxのプロファイル選択肢を表示するかどうか
            this.DisplayFirefoxPrifile = this.SelectedAutoLoginType
                .Select(value => value.Value == AutoLoginTypeString.Firefox)
                .ToReactiveProperty();
            #endregion

            this.isSkippingSSLVerificationEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.SkipSSLVerification);

            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");
            var n5 = new ComboboxItem<int>(5, "5");

            this.SelectablefetchSleepInterval = new List<ComboboxItem<int>> { n1, n2, n3, n4, n5 };
            this.SelectableMaxParallelFetch = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };


            var maxFetchParallelCount = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.MaxFetchCount);
            var fetchSleepInterval = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.FetchSleepInterval);

            this.maxFetchParallelCountField = maxFetchParallelCount switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                _ => n4,
            };

            this.fetchSleepIntervalFIeld = fetchSleepInterval switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                5 => n5,
                _ => n4,
            };

            this.isExpandallPlaylistsEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.ExpandAll);
            this.isSavePrevPlaylistExpandedStateEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.InheritExpandedState);
            this.isStoreOnlyNiconicoIDEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.StoreOnlyNiconicoID);
            this.isAutoRenamingRemotePlaylistEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.AutoRenameNetPlaylist);

            this.IsSingletonWindowsEnable = new ReactiveProperty<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.SingletonWindows));
            this.IsSingletonWindowsEnable.Subscribe(value => WS::SettingPage.SettingHandler.SaveSetting(value, SettingsEnum.SingletonWindows)).AddTo(this.disposables);

            this.IsConfirmngIfDownloadingEnable = new ReactiveProperty<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.ConfirmIfDownloading));
            this.IsConfirmngIfDownloadingEnable.Subscribe(value => WS::SettingPage.SettingHandler.SaveSetting(value, SettingsEnum.ConfirmIfDownloading)).AddTo(this.disposables);

            this.SnackbarDuration = WS::SettingPage.SettingsContainer.GetReactiveIntSetting(SettingsEnum.SnackbarDuration, null, value => value <= 0 ? LocalConstant.DefaultSnackbarDuration : value);
        }

        #region field
        private bool isAutologinEnableField;

        private bool isSkippingSSLVerificationEnableField;

        private bool isSavePrevPlaylistExpandedStateEnableField;

        private bool isExpandallPlaylistsEnableField;

        private bool isStoreOnlyNiconicoIDEnableField;

        private bool isAutoRenamingRemotePlaylistEnableField;

        private string empty = string.Empty;

        private ComboboxItem<int> maxFetchParallelCountField;

        private ComboboxItem<int> fetchSleepIntervalFIeld;
        #endregion

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
        public ReactiveProperty<string?> SelectedFirefoxProfileName { get; init; }

        /// <summary>
        /// Firefoxのプロファイルを表示するかどうか
        /// </summary>
        public ReactiveProperty<bool> DisplayFirefoxPrifile { get; init; }

        /// <summary>
        /// 選択可能なFirefoxのプロファイル
        /// </summary>
        public List<string> SelectableFirefoxProfiles { get; init; }


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
        public bool IsAutologinEnable { get => this.isAutologinEnableField; set => this.Savesetting(ref this.isAutologinEnableField, value, SettingsEnum.AutologinEnable); }

        /// <summary>
        /// 動画情報最大並列取得数
        /// </summary>
        public ComboboxItem<int> MaxFetchParallelCount
        {
            get => this.maxFetchParallelCountField;
            set => this.Savesetting(ref this.maxFetchParallelCountField, value, SettingsEnum.MaxFetchCount);
        }

        /// <summary>
        /// 待機間隔
        /// </summary>
        public ComboboxItem<int> FetchSleepInterval { get => this.fetchSleepIntervalFIeld; set => this.Savesetting(ref this.fetchSleepIntervalFIeld, value, SettingsEnum.FetchSleepInterval); }

        /// <summary>
        /// SSL証明書の検証をスキップする
        /// </summary>
        public bool IsSkippingSSLVerificationEnable { get => this.isSkippingSSLVerificationEnableField; set => this.Savesetting(ref this.isSkippingSSLVerificationEnableField, value, SettingsEnum.SkipSSLVerification); }

        /// <summary>
        /// 展開状況を保存する
        /// </summary>
        public bool IsSavePrevPlaylistExpandedStateEnable { get => this.isSavePrevPlaylistExpandedStateEnableField; set => this.Savesetting(ref this.isSavePrevPlaylistExpandedStateEnableField, value, SettingsEnum.InheritExpandedState); }

        /// <summary>
        /// すべて展開する
        /// </summary>
        public bool IsExpandallPlaylistsEnable { get => this.isExpandallPlaylistsEnableField; set => this.Savesetting(ref this.isExpandallPlaylistsEnableField, value, SettingsEnum.ExpandAll); }

        /// <summary>
        /// ニコニコ動画のIDのみを保存する
        /// </summary>
        public bool IsStoreOnlyNiconicoIDEnable { get => this.isStoreOnlyNiconicoIDEnableField; set => this.Savesetting(ref this.isStoreOnlyNiconicoIDEnableField, value, SettingsEnum.StoreOnlyNiconicoID); }

        /// <summary>
        /// 自動リネーム
        /// </summary>
        public bool IsAutoRenamingRemotePlaylistEnable { get => this.isAutoRenamingRemotePlaylistEnableField; set => this.Savesetting(ref this.isAutoRenamingRemotePlaylistEnableField, value, SettingsEnum.AutoRenameNetPlaylist); }

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

            this.MaxFetchParallelCount = n3;
            this.FetchSleepInterval = n5;
        }

        public bool IsAutologinEnable { set; get; } = true;

        public bool IsSkippingSSLVerificationEnable { get; set; } = false;

        public bool IsSavePrevPlaylistExpandedStateEnable { get; set; } = true;

        public bool IsExpandallPlaylistsEnable { get; set; } = true;

        public bool IsStoreOnlyNiconicoIDEnable { get; set; } = true;

        public bool IsAutoRenamingRemotePlaylistEnable { get; set; } = true;

        public List<string> SelectableFirefoxProfiles { get; init; }

        public List<ComboboxItem<string>> SelectableAutoLoginTypes { get; init; }

        public ReactiveProperty<ComboboxItem<string>> SelectedAutoLoginType { get; init; }

        public ReactiveProperty<string> SelectedFirefoxProfileName { get; init; }

        public ReactiveProperty<bool> DisplayFirefoxPrifile { get; init; } = new(true);

        public ReactiveProperty<bool> IsSingletonWindowsEnable { get; init; } = new(true);

        public ReactiveProperty<bool> IsConfirmngIfDownloadingEnable { get; init; } = new();

        public ReactiveProperty<int> SnackbarDuration { get; init; } = new();

        public List<ComboboxItem<int>> SelectableMaxParallelFetch { get; init; }

        public List<ComboboxItem<int>> SelectablefetchSleepInterval { get; init; }

        public ComboboxItem<int> MaxFetchParallelCount { get; set; }

        public ComboboxItem<int> FetchSleepInterval { get; set; }

    }
}
