using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Core.Utils
{
    public interface IHostPermissionsHandler
    {
        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="allowedHosts">許可されたホスト</param>
        void Initialize(List<string> allowedHosts);

        /// <summary>
        /// アクセスが許可されるかどうか
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        bool CanAccess(string url);
    }

    public class HostPermissionsHandler : IHostPermissionsHandler
    {
        #region field

        List<string> replacedHosts = new();

        #endregion

        #region Methods

        public void Initialize(List<string> allowedHosts)
        {
            this.replacedHosts.Clear();
            this.replacedHosts.AddRange(allowedHosts.Select(host =>
            {
                string replaced = host.Replace("*", ".*");

                replaced = Regex.Replace(replaced, @"\.(?!\*)", @"\.");
                if (replaced.EndsWith("/"))
                {
                    replaced += "?";
                }

                return replaced;
            }));
        }

        public bool CanAccess(string url)
        {
            return this.replacedHosts.Any(host => Regex.IsMatch(url, host));
        }



        #endregion
    }
}
