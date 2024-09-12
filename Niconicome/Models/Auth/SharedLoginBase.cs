using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Auth
{
    public class SharedLoginBase
    {
        public SharedLoginBase(INiconicoContext context)
        {
            this.context = context;
        }

        #region DIされるクラス
        protected readonly INiconicoContext context;
        #endregion

        /// <summary>
        /// ログインできたかどうか確かめる
        /// ログインしていない場合、ログインページにリダイレクトされるためLocationヘッダーが指定されるハズ
        /// </summary>
        /// <returns></returns>
        protected async Task<IAttemptResult> LoginAndSaveCookieAsync(string userSession,string userSessionSecure)
        {
            return await this.context.LoginAndSaveCookieAsync(userSession, userSessionSecure);
        }
    }
}
