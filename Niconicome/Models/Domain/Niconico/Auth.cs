using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CredentialManagement;

namespace Niconicome.Models.Domain.Niconico
{

    public interface IUserCredential
    {
        string Username { get; }
        string Password { get; }
    }

    public interface IAccountManager
    {
        void Save(string username, string password);
        bool IsPasswordSaved { get; }
        IUserCredential GetUserCredential();
        static IUserCredential GetUserCredential(string username, string password)
        {
            return new UserCredential(username, password);
        }
    }

    /// <summary>
    /// ユーザーアカウントマネージャー
    /// </summary>
    class AccountManager : IAccountManager
    {
        private readonly string siteName = "https://nicovideo.jp";

        /// <summary>
        /// 資格情報を保存する
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Save(string username, string password)
        {
            using var credential = new Credential
            {
                Target = this.siteName,
                Username = username,
                Password = password,
                Type = CredentialType.Generic,
                PersistanceType = PersistanceType.LocalComputer
            };
            credential.Save();
        }

        /// <summary>
        /// パスワードが保存済であるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool IsPasswordSaved
        {
            get
            {
                using var credential = new Credential
                {
                    Target = this.siteName
                };

                credential.Load();

                if (credential.Password == null || credential.Username == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 資格情報を取得する
        /// </summary>
        /// <returns></returns>
        public IUserCredential GetUserCredential()
        {
            if (!this.IsPasswordSaved) throw new InvalidOperationException("資格情報が存在しません。");
            using var credential = new Credential
            {
                Target = this.siteName
            };
            credential.Load();
            return new UserCredential(credential.Username, credential.Password);
        }

        /// <summary>
        /// 資格情報オブジェクトを作成する
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IUserCredential GetUserCredential(string username, string password)
        {
            return new UserCredential(username, password);
        }
    }

    /// <summary>
    /// 認証情報
    /// </summary>
    public record UserCredential : IUserCredential
    {
        public UserCredential(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password { get; private set; }

    }

}
