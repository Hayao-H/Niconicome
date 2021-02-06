using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Watch;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class AppinfopageViewmodel
    {

        public AppinfopageViewmodel()
        {
            this.Version = WS::SettingPage.LocalInfo.ApplicationVersion;
        }

        /// <summary>
        /// アプリケーションバージョン
        /// </summary>
        public string Version { get; init; }
    }
}
