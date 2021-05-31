using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;

namespace Niconicome.Models.Auth
{
    public class SharedLoginBase
    {
        public SharedLoginBase(INicoHttp http, ICookieManager cookieManager, INiconicoContext context)
        {
            this.cookieManager = cookieManager;
            this.http = http;
            this.context = context;
        }

        #region DIされるクラス
        private readonly INicoHttp http;

        protected readonly ICookieManager cookieManager;

        protected readonly INiconicoContext context;
        #endregion

        /// <summary>
        /// ログインできたかどうか確かめる
        /// ログインしていない場合、ログインページにリダイレクトされるためLocationヘッダーが指定されるハズ
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> CheckIfLoginSucceeded()
        {
            var result = await this.http.GetAsync(new Uri(@"https://www.nicovideo.jp/my"));

            if (result.Headers.Contains("Location"))
            {
                return false;
            }

            return true;
        }
    }
}
