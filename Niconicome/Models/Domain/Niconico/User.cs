﻿using System;
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
    public class User:IStorable
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
        /// ユーザーページ
        /// </summary>
        public string UserPage
        {
            get
            {
                return $"https://www.nicovideo.jp/user/{this.ID}";
            }
        }

        public User(string nickname, string id)
        {
            this.Nickname = nickname;
            this.ID = id;
            this.UserImage = new Uri($"https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/{Math.Floor(double.Parse(id) / 10000)}/{id}.jpg");
        }
    }

}
