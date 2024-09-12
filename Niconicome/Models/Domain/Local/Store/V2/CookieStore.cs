using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.UserAuth;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface ICookieStore
    {
        /// <summary>
        /// Cookieを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<ICookieInfo> GetCookieInfo();

        /// <summary>
        /// Cookieを削除
        /// </summary>
        /// <returns></returns>
        IAttemptResult DeleteCookieInfo();

        /// <summary>
        /// Cookie情報を上書き
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        IAttemptResult Update(ICookieInfo cookie);

        /// <summary>
        /// Cookieが存在するかどうか
        /// </summary>
        /// <returns></returns>
        bool Exists();
    }
}
