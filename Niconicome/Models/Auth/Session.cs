using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Auth
{

    public interface ISession
    {
        User? User { get; }
        bool IsLogin { get; }
        Task<bool> Login(IUserCredential credential);
        Task Logout();
    }

    /// <summary>
    /// ログインセッション
    /// </summary>
    class Session:ISession
    {
        public Session(INiconicoContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// ユーザー情報
        /// </summary>
        public User? User { get; private set; }

        /// <summary>
        /// ログインフラグ
        /// </summary>
        public bool IsLogin { get => this.context.IsLogin; }

        /// <summary>
        /// コンテキスト
        /// </summary>
        private readonly INiconicoContext context;

        /// ログイン
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        public async Task<bool> Login(IUserCredential credential)
        {
            if (this.IsLogin) return true;
            bool result = await this.context.Login(credential.Username, credential.Password);

            if (result)
            {
                this.User = NiconicoContext.User;
            }

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
