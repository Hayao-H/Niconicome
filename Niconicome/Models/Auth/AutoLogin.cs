using System;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Local;

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
        public AutoLogin(ISession session, IAccountManager accountManager, ILocalSettingHandler settingHandler)
        {
            this.session = session;
            this.accountManager = accountManager;
            this.settingHandler = settingHandler;
        }

        private readonly ISession session;

        private readonly IAccountManager accountManager;

        private readonly ILocalSettingHandler settingHandler;

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

            var cred = this.accountManager.GetUserCredential();

            var result = await this.session.Login(cred);

            return result;
        }

        /// <summary>
        /// 自動ログインが有効であるかどうか
        /// </summary>
        public bool IsAUtologinEnable
        {
            get => this.settingHandler.GetBoolSetting(Settings.AutologinEnable);
        }

        /// <summary>
        /// 自動ログインのタイプを取得する
        /// </summary>
        /// <returns></returns>
        private AutoLoginType GetAutoLoginType()
        {
            bool isCredencialSaved = this.accountManager.IsPasswordSaved;

            if (isCredencialSaved) return AutoLoginType.Normal;

            return AutoLoginType.None;
        }
    }

    enum AutoLoginType
    {
        None,
        Normal
    }
}
