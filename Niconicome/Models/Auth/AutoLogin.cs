using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.Web.WebView2.Core;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Niconico.UserAuth;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Auth
{
    public interface IAutoLogin
    {
        /// <summary>
        /// 自動ログインフラグ
        /// </summary>
        bool IsAUtologinEnable { get; }

        /// <summary>
        /// 自動ログインの可否をチェック
        /// </summary>
        /// <returns></returns>
        bool Canlogin();

        /// <summary>
        /// ログインを思考する
        /// </summary>
        /// <returns></returns>
        Task<bool> LoginAsync();

        /// <summary>
        /// Firefoxのプロファイルを取得する
        /// </summary>
        /// <param name="loginType"></param>
        /// <returns></returns>
        IEnumerable<IFirefoxProfileInfo> GetFirefoxProfiles(AutoLoginType loginType);
    }

    class AutoLogin : IAutoLogin
    {
        public AutoLogin(IWebview2SharedLogin webview2SharedLogin, IFirefoxSharedLogin firefoxSharedLogin, IStoreFirefoxSharedLogin storeFirefoxSharedLogin, ISettingsContainer settingsConainer,IStoredCookieLogin storedCookieLogin)
        {
            this._webview2SharedLogin = webview2SharedLogin;
            this._firefoxSharedLogin = firefoxSharedLogin;
            this._storeFirefoxSharedLogin = storeFirefoxSharedLogin;
            this._settingsConainer = settingsConainer;
            this._storedCookieLogin = storedCookieLogin;
        }

        #region field

        private readonly IStoredCookieLogin _storedCookieLogin;

        private readonly ISettingsContainer _settingsConainer;

        private readonly IWebview2SharedLogin _webview2SharedLogin;

        private readonly IFirefoxSharedLogin _firefoxSharedLogin;

        private readonly IStoreFirefoxSharedLogin _storeFirefoxSharedLogin;

        #endregion

        #region Props

        public bool IsAUtologinEnable
        {
            get => this._settingsConainer.GetSetting(SettingNames.IsAutologinEnable, false).Data?.Value ?? false;
        }

        #endregion

        #region Method

        public bool Canlogin()
        {
            var type = this.GetAutoLoginType();

            return type != AutoLoginType.None;
        }

        public async Task<bool> LoginAsync()
        {
            if (!this.Canlogin()) throw new InvalidOperationException("自動ログインは不可能です。");

            bool result = false;
            var type = this.GetAutoLoginType();

            //保存されているCookieでのログインを優先する
            if (this._storedCookieLogin.CanLogin())
            {
                result = await this._storedCookieLogin.TryLogin();
                if (result) return true;
            }

             if (type == AutoLoginType.Webview2)
            {
                result = await this._webview2SharedLogin.TryLogin();
            }
            else if (type == AutoLoginType.Firefox)
            {
                var ffProfile = this._settingsConainer.GetSetting(SettingNames.FirefoxProfileName, "");
                if (!ffProfile.IsSucceeded || ffProfile.Data is null) return false;
                result = await this._firefoxSharedLogin.TryLogin(ffProfile.Data.Value);
            }
            else if (type == AutoLoginType.StoreFirefox)
            {
                var ffProfile = this._settingsConainer.GetSetting(SettingNames.FirefoxProfileName, "");
                if (!ffProfile.IsSucceeded || ffProfile.Data is null) return false;
                result = await this._storeFirefoxSharedLogin.TryLogin(ffProfile.Data.Value);
            }

            return result;
        }


        private AutoLoginType GetAutoLoginType()
        {
            IAttemptResult<ISettingInfo<string>> mResult = this._settingsConainer.GetSetting(SettingNames.AutoLoginMode, AutoLoginTypeString.Webview2);
            IAttemptResult<ISettingInfo<string>> pResult = this._settingsConainer.GetSetting(SettingNames.FirefoxProfileName, "");

            if (!mResult.IsSucceeded || mResult.Data is null)
            {
                return AutoLoginType.None;

            }
            else if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AutoLoginType.None;
            }

            var mode = mResult.Data.Value.IsNullOrEmpty() ? AutoLoginTypeString.Webview2 : mResult.Data.Value;
            var ffProfile = pResult.Data.Value;

             if (mode == AutoLoginTypeString.Webview2)
            {
                bool canLoginWithWebview2 = this._webview2SharedLogin.CanLogin();
                if (canLoginWithWebview2) return AutoLoginType.Webview2;
            }
            else if (mode == AutoLoginTypeString.Firefox && ffProfile is not null)
            {
                bool canLoginWithFirefox = this._firefoxSharedLogin.CanLogin(ffProfile);
                if (canLoginWithFirefox) return AutoLoginType.Firefox;
            }
            else if (mode == AutoLoginTypeString.StoreFirefox && ffProfile is not null)
            {
                bool canLoginWithStoreFirefox = this._storeFirefoxSharedLogin.CanLogin(ffProfile);
                if (canLoginWithStoreFirefox) return AutoLoginType.StoreFirefox;
            }

            return AutoLoginType.None;
        }

        public IEnumerable<IFirefoxProfileInfo> GetFirefoxProfiles(AutoLoginType loginType)
        {
            return loginType switch
            {
                AutoLoginType.Firefox => this._firefoxSharedLogin.GetFirefoxProfiles(),
                AutoLoginType.StoreFirefox => this._storeFirefoxSharedLogin.GetFirefoxProfiles(),
                _ => throw new InvalidOperationException("ログインの種別がFirefox以外です。"),
            };

        }

        #endregion

    }

    public enum AutoLoginType
    {
        None,
        Webview2,
        Firefox,
        StoreFirefox,
    }

    static class AutoLoginTypeString
    {
        public const string Webview2 = "Webview2";

        public const string Firefox = "Firefox";

        public const string StoreFirefox = "StoreFirefox";
    }
}
