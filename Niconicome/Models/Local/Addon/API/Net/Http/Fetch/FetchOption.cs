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
        string Method { get; }

        /// <summary>
        /// 認証情報フラグ
        /// </summary>
        bool IncludeCredentioals { get; }

        /// <summary>
        /// body
        /// </summary>
        string Body { get; }

        /// <summary>
        /// Header
        /// </summary>
        Dictionary<string, string> Headers { get; }
    }

    public class FetchOption : IFetchOption
    {
        public string Method { get; init; } = string.Empty;

        public bool IncludeCredentioals { get; init; }

        public string Body { get; init; } = string.Empty;

        public Dictionary<string, string> Headers { get; init; } = new();

    }
}
