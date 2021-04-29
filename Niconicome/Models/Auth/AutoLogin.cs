using System;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Auth
{
    interface IAutoLogin
    {
        bool IsAUtologinEnable { get; }
        bool Canlogin();
        Task<bool> LoginAsync();
    }

    class AutoLogin : IAutoLogin
    {
        public AutoLogin(ISession session, IAccountManager accountManager, ILocalSettingHandler settingHandler, IWebview2SharedLogin webview2SharedLogin)
        {
            this.session = session;
            this.accountManager = accountManager;
            this.settingHandler = settingHandler;
            this.webview2SharedLogin = webview2SharedLogin;
        }

        private readonly ISession session;

        private readonly IAccountManager accountManager;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IWebview2SharedLogin webview2SharedLogin;

        /// <summary>
        /// 自動ログインが可能であるかどうか
        /// </summary>
        /// <returns></returns>
        public bool Canlogin()
        {
            var type = this.GetAutoLoginType();

            return type != AutoLoginType.None;
        }

        /// <summary>
        /// ログインを試行する
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoginAsync()
        {
            if (!this.Canlogin()) throw new InvalidOperationException("自動ログインは不可能です。");

            bool result = false;
            var type = this.GetAutoLoginType();

            if (type == AutoLoginType.Normal)
            {
                var cred = this.accountManager.GetUserCredential();
                result = await this.session.Login(cred);
            }
            else if (type == AutoLoginType.Webview2)
            {
                result = await this.webview2SharedLogin.TryLogin();
            }

            return result;
        }

        /// <summary>
        /// 自動ログインが有効であるかどうか
        /// </summary>
        public bool IsAUtologinEnable
        {
            get => this.settingHandler.GetBoolSetting(SettingsEnum.AutologinEnable);
        }

        /// <summary>
        /// 自動ログインのタイプを取得する
        /// </summary>
        /// <returns></returns>
        private AutoLoginType GetAutoLoginType()
        {
            var mode = this.settingHandler.GetStringSetting(SettingsEnum.AutologinMode) ?? AutoLoginTypeString.Normal;

            if (mode == AutoLoginTypeString.Normal)
            {
                bool isCredencialSaved = this.accountManager.IsPasswordSaved;
                if (isCredencialSaved) return AutoLoginType.Normal;
            }
            else if (mode == AutoLoginTypeString.Webview2)
            {
                bool canLoginWithWebview2 = this.webview2SharedLogin.CanLogin();
                if (canLoginWithWebview2) return AutoLoginType.Webview2;
            }

            return AutoLoginType.None;
        }
    }

    enum AutoLoginType
    {
        None,
        Normal,
        Webview2
    }

    static class AutoLoginTypeString
    {
        public const string Normal = "Normal";

        public const string Webview2 = "Webview2";
    }
}
