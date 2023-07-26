using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Auth;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.ViewModels.Setting.Utils;
using Niconicome.ViewModels.Setting.V2.Utils;
using WS = Niconicome.Workspaces.SettingPageV2;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class GeneralViewModel : IDisposable
    {

        public GeneralViewModel()
        {

            this.IsAutologinEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsAutologinEnable, false), false).AddTo(this.Bindables);

            var normal = new SelectBoxItem<string>("パスワードログイン", AutoLoginTypeString.Normal);
            var wv2 = new SelectBoxItem<string>("Webview2とCookieを共有", AutoLoginTypeString.Webview2);
            var firefox = new SelectBoxItem<string>("FirefoxとCookieを共有", AutoLoginTypeString.Firefox);
            var storeFirefox = new SelectBoxItem<string>("Store版FirefoxとCookieを共有", AutoLoginTypeString.StoreFirefox);

            this.SelectableAutoLoginType.AddRange(new[] { normal, wv2, firefox, storeFirefox });

            this.SelectedAutoLoginType = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.AutoLoginMode, AutoLoginTypeString.Normal), AutoLoginTypeString.Normal).AddTo(this.Bindables);

            this.DisplayFirefoxPrifile = this.SelectedAutoLoginType.Select(t => t == AutoLoginTypeString.Firefox || t == AutoLoginTypeString.StoreFirefox).AsReadOnly().AddTo(this.Bindables);

            this.SelectableFirefoxProfiles = new BindableCollection<string, string>(this._selectableFirefoxProfiles, p => p);
            this.Bindables.Add(this.SelectableFirefoxProfiles);

            Action<bool> action = v =>
            {
                if (!v)
                {
                    return;
                }

                this._selectableFirefoxProfiles.Clear();

                if (this.SelectedAutoLoginType.Value == AutoLoginTypeString.Firefox)
                {
                    this._selectableFirefoxProfiles.AddRange(WS.AutoLogin.GetFirefoxProfiles(AutoLoginType.Firefox).Select(p => p.ProfileName));
                }
                else
                {
                    this._selectableFirefoxProfiles.AddRange(WS.AutoLogin.GetFirefoxProfiles(AutoLoginType.StoreFirefox).Select(p => p.ProfileName));
                }
            };

            action(true);
            this.DisplayFirefoxPrifile.Subscribe(action);

            this.SelectedFirefoxProfileName = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.FirefoxProfileName, string.Empty), string.Empty).AddTo(this.Bindables);

            this.IsShowingTasksAsTabEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.ShowDownloadTasksAsTab, true), true).AddTo(this.Bindables);

            var n1 = new SelectBoxItem<int>("1", 1);
            var n2 = new SelectBoxItem<int>("2", 2);
            var n3 = new SelectBoxItem<int>("3", 3);
            var n4 = new SelectBoxItem<int>("4", 4);
            var n5 = new SelectBoxItem<int>("5", 5);

            this.SelectableMaxParallelFetch = new() { n1, n2, n3, n4 };
            this.MaxFetchParallelCount = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.MaxParallelFetchCount, 4), 4).AddTo(this.Bindables);

            this.SelectablefetchSleepInterval = new() { n1, n2, n3, n4, n5 };
            this.FetchSleepInterval = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.FetchSleepInterval, 5), 5).AddTo(this.Bindables);

            this.IsSkippingSSLVerificationEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.SkipSSLVerification, false), false).AddTo(this.Bindables);

            this.IsSavePrevPlaylistExpandedStateEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.SaveTreePrevExpandedState, false), false).AddTo(this.Bindables);

            this.IsExpandallPlaylistsEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.ExpandTreeOnStartUp, false), false).AddTo(this.Bindables);

            this.IsStoreOnlyNiconicoIDEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.StoreOnlyNiconicoIDOnRegister, false), false).AddTo(this.Bindables);

            this.IsAutoRenamingRemotePlaylistEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.AutoRenamingAfterSetNetworkPlaylist, true), true);

            this.IsConfirmngIfDownloadingEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.ConfirmIfDownloading, true), true).AddTo(this.Bindables);

            this.IsSingletonWindowsEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.LimitWindowsToSingleton, true), true).AddTo(this.Bindables);

            this.LocalServerPort = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.LocalServerPort, NetConstant.DefaultServerPort), NetConstant.DefaultServerPort, p => p > 65535 || p < 0 ? NetConstant.DefaultServerPort : p, x => x is int).AddTo(this.Bindables);

            this.SnackbarDuration = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.SnackbarDuration, NetConstant.DefaultServerPort), NetConstant.DefaultServerPort, x => x < 0 ? 4000 : x, x => x is int).AddTo(this.Bindables);

        }

        #region field

        private readonly ObservableCollection<string> _selectableFirefoxProfiles = new();

        #endregion

        #region Props

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// 自動ログイン
        /// </summary>
        public IBindableSettingInfo<bool> IsAutologinEnable { get; init; }

        /// <summary>
        /// 選択可能な自動ログインの種別
        /// </summary>
        public List<SelectBoxItem<string>> SelectableAutoLoginType { get; init; } = new();

        /// <summary>
        /// 自動ログインの種別
        /// </summary>
        public IBindableSettingInfo<string> SelectedAutoLoginType { get; init; }

        /// <summary>
        /// Firefox設定の表示・非表示
        /// </summary>
        public IReadonlyBindablePperty<bool> DisplayFirefoxPrifile { get; init; }

        /// <summary>
        /// 選択可能なFirefoxのプロファイル
        /// </summary>
        public BindableCollection<string, string> SelectableFirefoxProfiles { get; init; }

        /// <summary>
        /// Firefoxのプロファイル
        /// </summary>
        public IBindableSettingInfo<string> SelectedFirefoxProfileName { get; init; }

        /// <summary>
        /// タスク一覧をタブ表示
        /// </summary>
        public IBindableSettingInfo<bool> IsShowingTasksAsTabEnable { get; init; }

        /// <summary>
        /// 動画情報最大並列取得数
        /// </summary>
        public List<SelectBoxItem<int>> SelectableMaxParallelFetch { get; init; } = new();

        /// <summary>
        /// 動画情報最大並列取得数
        /// </summary>
        public IBindableSettingInfo<int> MaxFetchParallelCount { get; init; }

        /// <summary>
        /// スリープ間隔
        /// </summary>
        public List<SelectBoxItem<int>> SelectablefetchSleepInterval { get; init; } = new();

        /// <summary>
        /// スリープ間隔
        /// </summary>
        public IBindableSettingInfo<int> FetchSleepInterval { get; init; }

        /// <summary>
        /// SSL証明書の検証をスキップする
        /// </summary>
        public IBindableSettingInfo<bool> IsSkippingSSLVerificationEnable { get; init; }

        /// <summary>
        /// 展開状況を保存する
        /// </summary>
        public IBindableSettingInfo<bool> IsSavePrevPlaylistExpandedStateEnable { get; init; }

        /// <summary>
        /// すべて展開する
        /// </summary>
        public IBindableSettingInfo<bool> IsExpandallPlaylistsEnable { get; init; }

        /// <summary>
        /// ニコニコ動画のIDのみを保存する
        /// </summary>
        public IBindableSettingInfo<bool> IsStoreOnlyNiconicoIDEnable { get; init; }

        /// <summary>
        /// 自動リネーム
        /// </summary>
        public IBindableSettingInfo<bool> IsAutoRenamingRemotePlaylistEnable { get; init; }

        /// <summary>
        /// DL中は終了時に確認する
        /// </summary>
        public IBindableSettingInfo<bool> IsConfirmngIfDownloadingEnable { get; init; }

        /// <summary>
        /// マルチウィンドウ禁止
        /// </summary>
        public IBindableSettingInfo<bool> IsSingletonWindowsEnable { get; init; }

        /// <summary>
        /// ローカルサーバーのポート
        /// </summary>
        public IBindableSettingInfo<int> LocalServerPort { get; init; }

        /// <summary>
        /// スナックバー表示時間
        /// </summary>
        public IBindableSettingInfo<int> SnackbarDuration { get; init; }

        #endregion

        public void Dispose()
        {
            this.Bindables.Dispose();
        }
    }
}
