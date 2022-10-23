using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Loader;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Shared
{
    public class FailedResultViewModel
    {
        public FailedResultViewModel(FailedResult result)
        {
            this.DirectoryName = result.DirectoryName;
            this.Message = result.Message;
        }

        /// <summary>
        /// ディレクトリ名
        /// </summary>
        public string DirectoryName { get; init; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; init; }
    }
}
