using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Addon.API.Net.Http.Fetch
{
    public interface IFetchOption
    {
        /// <summary>
        /// メソッド
        /// </summary>
        string? method { get; set; }

        /// <summary>
        /// 認証情報フラグ
        /// </summary>
        string? credentials { get; set; }

        /// <summary>
        /// body
        /// </summary>
        dynamic? body { get; set; }
    }

    public class FetchOption : IFetchOption
    {
        public string? method { get; set; }

        public string? credentials { get; set; }

        public dynamic? body { get; set; }
    }
}
