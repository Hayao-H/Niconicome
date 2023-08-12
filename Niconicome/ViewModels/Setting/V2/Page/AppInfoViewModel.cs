using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS = Niconicome.Workspaces.SettingPageV2;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class AppInfoViewModel
    {
        public AppInfoViewModel()
        {
            this.Version = WS.LocalInfo.ApplicationVersion;
        }

        /// <summary>
        /// アプリケーションバージョン
        /// </summary>
        public string Version { get; init; }

    }
}
