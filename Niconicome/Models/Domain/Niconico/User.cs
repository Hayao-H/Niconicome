using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local;

namespace Niconicome.Models.Domain.Niconico
{

    /// <summary>
    /// ユーザー情報クラス
    /// </summary>
    public class User : IStorable
    {
        public static string TableName { get; } = "users";

        /// <summary>
        /// データベース用ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// ニックネーム
        /// </summary>
        public string Nickname { get; private set; }

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// プロフィール画像
        /// </summary>
        public Uri UserImage { get; private set; }

        /// <summary>
        /// プレミアム会員
        /// </summary>
        public bool IsPremium { get; private set; }

        /// <summary>
        /// ユーザーページ
        /// </summary>
        public string UserPage
        {
            get
            {
                return $"https://www.nicovideo.jp/user/{this.ID}";
            }
        }

        public User(string nickname, string id, bool isPremium, string iconURL)
        {
            this.Nickname = nickname;
            this.ID = id;
            this.IsPremium = isPremium;
            this.UserImage = new Uri(iconURL);
        }
    }

}
