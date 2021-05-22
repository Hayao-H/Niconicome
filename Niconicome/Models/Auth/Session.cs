using System;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Niconicome.Models.Auth
{

    public interface ISession
    {
        Task<bool> Login(IUserCredential credential);
        Task Logout();
        ReactiveProperty<bool> IsLogin { get; }
        ReactiveProperty<User?> User { get; }
    }

    /// <summary>
    /// ログインセッション
    /// </summary>
    class Session : BindableBase, ISession
    {
        public Session(INiconicoContext context)
        {
            this.context = context;
            this.IsLogin = new ReactiveProperty<bool>(context.IsLogin);
            this.User = context.User.ToReactiveProperty().AddTo(this.disposables);
            context.User.Subscribe(value => this.IsLogin.Value = value is not null);
        }

        #region DI
        private readonly INiconicoContext context;
        #endregion

        /// <summary>
        /// ログインフラグ
        /// </summary>
        public ReactiveProperty<bool> IsLogin { get; }

        /// <summary>
        /// ユーザー
        /// </summary>
        public ReactiveProperty<User?> User { get; }

        /// ログイン
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        public async Task<bool> Login(IUserCredential credential)
        {
            if (this.context.IsLogin) return true;

            bool result = await this.context.Login(credential.Username, credential.Password);

            return result;
        }

        /// <summary>
        /// ログアウト
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            await this.context.Logout();
        }
    }
}
